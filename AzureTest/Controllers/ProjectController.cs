using AutoMapper;
using AzureTest.Contracts;
using AzureTest.Models;
using AzureTest.Models.Entities;
using AzureTest.Models.InputModels;
using AzureTest.Models.OutPutModels;
using AzureTest.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzureTest.Controllers
{
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly AppDbContext _context;
        private UserManager<UserModel> _userManager;
        private IMapper _autoMapper;
        private readonly IProjectRepository _projectRepo;
        private readonly BlobService _blobService;
        private readonly RoutesController _routesController;
        private readonly IAccountRepository _accountRepository;
        private readonly ProjectService _projectService;

        public ProjectController(AppDbContext context, UserManager<UserModel> userManager, IProjectRepository projectRepo, BlobService blobService, RoutesController routesController, IAccountRepository accountRepo, ProjectService projectService)
        {
            _context = context;
            _userManager = userManager;
            _projectRepo = projectRepo;
            _blobService = blobService;
            _accountRepository = accountRepo;
            _projectService = projectService;

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IP_CreateProjectModel, ProjectModel>();
                cfg.CreateMap<ProjectPageModel, OP_ProjectPageModel>();
                cfg.CreateMap<IP_CreateProjectPageModel, ProjectPageModel>();
            });

            _autoMapper = mapperConfig.CreateMapper();
            _routesController = routesController;
        }

        [HttpPost]
        [Route("api/[controller]/[action]")]
        public async Task<int> CreateProject([FromBody] IP_CreateProjectModel projectData)
        {
            UserModel currentUser = await _userManager.GetUserAsync(User);

            ProjectModel projectData2 = _autoMapper.Map<ProjectModel>(projectData);

            projectData2.CreationDate = DateTime.Now;
            projectData2.Status = 1;
            projectData2.UpdatesCount = 0;
            projectData2.CommentsCount = 0;
            projectData2.VotesCount = 0;
            projectData2.UserId = currentUser.Id;

            var project = await _projectRepo.CreateProject(projectData2, currentUser);

            return project.Id;
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<OP_ProjectModel> GetProject(int projectId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if(await _routesController.CheckIfCanActivateProjectPage(projectId, currentUser.Id))
            {
                var project = await _projectRepo.GetProject(projectId);

                return await _projectService.GetProject(project, projectId);
            }
            else
            {
                return null;
            }
        }


        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<OP_ProjectModel> RenderEditProjectPage(int projectId)
        {
            UserModel currentUser = await _userManager.GetUserAsync(User);

            if (!await _routesController.CheckIfCanActivateProjectPage(projectId, currentUser.Id))
            {
                return null;
            }

            ProjectModel projectModel = await _projectRepo.RenderEditProjectPage(projectId);

            return await _projectService.RenderEditProjectPage(projectModel, projectId);
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<OP_ProjectPageModel> GetProjectPage(int projectPageId)
        {
            ProjectPageModel projectPage = await _projectRepo.GetProjectPage(projectPageId);

            return await _projectService.GetProjectPage(projectPage, projectPageId);
        }

        [HttpPost]
        [Route("api/[controller]/[action]")]
        public async Task<OP_RenderDatesModel> CreateProjectPage([FromBody] IP_CreateProjectPageModel projectPageData)
        {
            UserModel currentUser = await _userManager.GetUserAsync(User);

            if(!await _routesController.CheckIfCanActivateProjectPage(projectPageData.ProjectId, currentUser.Id))
            {
                return null;
            }

            ProjectPageModel result = _autoMapper.Map<ProjectPageModel>(projectPageData);

            result.Date = DateTime.Now;
            result.TotalLikes = 0;
            result.CommentsCount = 0;
            result.ImagesCount = projectPageData.TempImageIds.Count();

            var dbConn = await _projectRepo.CreateProjectPage(result, currentUser);

            await _projectService.UploadProjectPageTempImages(projectPageData.TempImageIds, dbConn.ItemId1);

            List<FollowModel> usersFollowers = await _accountRepository.GetUsersFollowersId(currentUser.Id);

            foreach (var userFollowers in usersFollowers)
            {
                await _projectRepo.CreateNotificationConnection(userFollowers.CurrentUserId, dbConn.Id);
            }

            var returnValue = new OP_RenderDatesModel
            {
                Date = result.Date,
                ProjectPageId = dbConn.ItemId1
            };

            return returnValue;
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<OP_ProjectPageModel> GetUserProjectPage(int projectPageId)
        {
            UserModel currentUser = await _userManager.GetUserAsync(User);

            if(currentUser != null)
            {
                ProjectPageModel projectPage = await _projectRepo.GetUserProjectPage(projectPageId, currentUser.Id);

                return await _projectService.GetUserProjectPage(projectPage, projectPageId);
            }
            else
            {
                ProjectPageModel projectPage = await _projectRepo.GetUserProjectPage(projectPageId, 0);

                return await _projectService.GetUserProjectPageNotAuthenticated(projectPage, projectPageId);
            }
        }

        [HttpPost]
        [Route("api/[controller]/[action]")]
        public async Task<bool> EditProject(IP_UpdateProjectModel model)
        {
            UserModel currentUser = await _userManager.GetUserAsync(User);

            if (!await _routesController.CheckIfCanActivateProjectPage(model.ProjectId, currentUser.Id))
            {
                return false;
            }

            await _projectRepo.UpdateProject(model);

            return true;
        }

        [HttpPost]
        [Route("api/[controller]/[action]")]
        public async Task<bool> EditProjectProfileImage(IP_UpdateProjectProfileImage model)
        {
            UserModel currentUser = await _userManager.GetUserAsync(User);

            if (!await _routesController.CheckIfCanActivateProjectPage(model.ProjectId, currentUser.Id))
            {
                return false;
            }

            await _blobService.DeleteBlob("5-" + model.ProjectId);

            byte[] profileImage = await _blobService.GetImage(model.TempImageId);

            await _blobService.UploadImage(profileImage, "5-" + model.ProjectId);

            await _blobService.DeleteBlob(model.TempImageId);

            return true;
        }

        [HttpPost]
        [Route("api/[controller]/[action]")]
        public async Task<byte[]> RenderNextImageProjectPage(IP_NextImageProjectPage model)
        {
            return await _blobService.GetImage("3-" + model.StackNumber + "-" + model.ProjectPageId);
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<bool> SaveProjectPage(int projectPageId)
        {
            if(User.Identity.IsAuthenticated)
            {
                UserModel currentUser = await _userManager.GetUserAsync(User);

                SaveItemModel result = new SaveItemModel
                {
                    ItemId = projectPageId,
                    UserId = currentUser.Id,
                    Date = DateTime.Now,
                };

                await _projectRepo.SaveProjectPage(result);

                return true;
            }
            return false;
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<bool> UnSaveProjectPage(int projectPageId)
        {
            if (User.Identity.IsAuthenticated)
            {
                UserModel currentUser = await _userManager.GetUserAsync(User);

                await _projectRepo.UnSaveProjectPage(projectPageId, currentUser.Id);

                return true;
            }
            return false;
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<OP_UserModel> GetUserForCommentsDialog()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return null;
            }

            UserModel currentUser = await _userManager.GetUserAsync(User);

            OP_UserModel result = new OP_UserModel
            {
                Firstname = currentUser.Firstname,
                Lastname = currentUser.Lastname,
                Id = currentUser.Id,
                AccountType = currentUser.AccountType,
            };

            byte[] profileImage = await _blobService.GetImage("1-" + currentUser.Id);

            if(profileImage != null)
            {
                result.ProfileImage = profileImage;
            }
            return result;
        }

        [HttpPost]
        [Route("api/[controller]/[action]")]
        public async Task<List<OP_ProjectPageCommentModel>> GetProjectPageComments(IP_GetCommentsProjectPage commentsData)
        {
            commentsData.PageNum = commentsData.PageNum * 5;

            var comments = await _projectRepo.GetProjectPageComments(commentsData);

            var result = await _projectService.GetProjectPageComments(comments);

            return result;
        }

        [HttpPost]
        [Route("api/[controller]/[action]")]
        public async Task<OP_ProjectPageCommentModel> CommentProjectPage(IP_CommentProjectPage commentData)
        {
            if(!User.Identity.IsAuthenticated)
            {
                return null;
            }

            UserModel currentUser = await _userManager.GetUserAsync(User);

            commentData.UserId = currentUser.Id;
            commentData.Date = DateTime.Now;

            await _projectRepo.CommentProjectPage(commentData, currentUser);

            return await _projectService.MapCommentUser(commentData, currentUser);
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<OP_ProjectModel> GetUserProject(int projectId)
        {
            if(User.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.GetUserAsync(User);

                var project = await _projectRepo.GetUserProject(projectId, currentUser.Id);

                return await _projectService.MapUserProject(project, currentUser, projectId);
            }
            else
            {
                var project = await _projectRepo.GetUserProjectNotAuthenticated(projectId);

                return await _projectService.MapUserProjectNotAuthenticated(project, projectId);
            }
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<bool> LikeProjectPage(int projectPageId)
        {
            if(User.Identity.IsAuthenticated)
            {
                UserModel currentUser = await _userManager.GetUserAsync (User);

                await _projectRepo.LikeProjectPage(projectPageId, currentUser.Id);

                return true;
            }
            return false;
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<bool> UnLikeProjectPage(int projectPageId)
        {
            if (User.Identity.IsAuthenticated)
            {
                UserModel currentUser = await _userManager.GetUserAsync(User);

                await _projectRepo.UnLikeProjectPage(projectPageId, currentUser.Id);

                return true;
            }
            return false;
        }
    }
}
