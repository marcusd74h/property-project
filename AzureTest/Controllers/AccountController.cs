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

namespace AzureTest.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;
        private IPasswordHasher<UserModel> _passwordHasher;
        private UserManager<UserModel> _userManager;
        private SignInManager<UserModel> _signInManager;
        private IMapper _autoMapper;
        private readonly IAccountRepository _accountRepo;
        private readonly IProjectRepository _projectRepo;
        private readonly BlobService _blobService;
        private readonly AccountService _accountService;

        public AccountController(AppDbContext context, IPasswordHasher<UserModel> passwordHash, UserManager<UserModel> userManager, SignInManager<UserModel> signInManager, IAccountRepository accountRepo, IProjectRepository projectRepo, BlobService blobService, AccountService accountService)
        {
            _context = context;
            _passwordHasher = passwordHash;
            _userManager = userManager;
            _signInManager = signInManager;
            _accountRepo = accountRepo;
            _projectRepo = projectRepo;
            _blobService = blobService;
            _accountService = accountService;

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserModel, OP_UserModel>();
                cfg.CreateMap<ProjectModel, IP_CreateProjectModel>();
                cfg.CreateMap<IP_CreateProjectModel, ProjectModel>();
                cfg.CreateMap<IP_RegisterModel, UserModel>();
            });

            _autoMapper = mapperConfig.CreateMapper();
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _accountRepo.GetUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/[action]")]
        public string ReturnString()
        {
            return "Hello world";
        }

        [HttpPost]
        [Route("api/[controller]/[action]")]
        public async Task<OP_RegisterResult> Register([FromBody] IP_RegisterModel registerData)
        {
            UserModel user = _autoMapper.Map<UserModel>(registerData);

            user.RegisterDate = DateTime.Now;
            user.Description = "";
            user.RegisterYear = DateTime.Now.Year;
            user.FollowersCount = 0;
            user.FollowingCount = 0;
            user.AccountType = 1;

            user.PasswordHash = _passwordHasher.HashPassword(user, registerData.Password);

            var result = await _userManager.CreateAsync(user);

            var registerResult = new OP_RegisterResult
            {
                Success = result.Succeeded
            };

            if(!result.Succeeded)
            {
                registerResult.Error = result.Errors.ToList()[0].Description;
            }

            return registerResult;
        }

        [HttpPost]
        [Route("api/[controller]/[action]")]
        public async Task<bool> Login([FromBody] IP_LoginModel loginData)
        {
            var loginResult = false;

            if (loginData.Email != null && loginData.Password != null)
            {
                var result = await _signInManager.PasswordSignInAsync(loginData.Email, loginData.Password, true, false);

                if (result.Succeeded)
                {
                    loginResult = true;
                }
                else
                {
                    return loginResult;
                }
            }

            return loginResult;
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<bool> LogOut()
        {
            await _signInManager.SignOutAsync();

            return true;
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<string> GetCurrentUserName()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser != null)
            {
                return currentUser.Email;
            }
            else
            {
                return "Empty value here";
            }
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public TestStringModel SubmitTestString(string inputValue)
        {
            var result = new TestStringModel
            {
                Value = inputValue,
                Date = DateTime.Now
            };

            _context.Add(result);
            _context.SaveChanges();

            return result;
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<OP_UserModel> GetNavbar()
        {
            UserModel currentUser = await _userManager.GetUserAsync(User);

            if(currentUser != null)
            {
                OP_UserModel returnUser = _autoMapper.Map<OP_UserModel>(currentUser);

                returnUser.NotificationCount = await _accountRepo.CountUsersNotification(currentUser.Id);

                var notifications = await GetUsersNotifications(0);

                returnUser.Notifications = notifications;

                return returnUser;
            }
            else
            {
                return null;
            }
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<List<OP_NotificationModel>> GetUsersNotifications(int pageNum)
        {
            UserModel currentUser = await _userManager.GetUserAsync(User);

            var notifications = await _accountRepo.GetUsersNotifications(currentUser.Id, pageNum * 5);

            List<OP_NotificationModel> returnValue = await _accountService.GetUsersNotifications(notifications);
            
            return returnValue;
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public bool CheckIfUserAuthenticated()
        {
            if (User.Identity.IsAuthenticated)
            {
                return true;
            }
            return false;
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<OP_UserModel> GetCurrentUser()
        {
            if(!User.Identity.IsAuthenticated)
            {
                return null;
            }

            UserModel currentUser = await _userManager.GetUserAsync(User);

            List<ProjectModel> projects = await _accountRepo.GetCurrentUsersProjects(currentUser.Id);

            OP_UserModel returnValue = await _accountService.GetCurrentUser(currentUser, projects);

            return returnValue;
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<OP_UserModel> GetUserProfile(int userId)
        {
            UserModel currentUser = await _userManager.GetUserAsync(User);

            UserModel user = await _accountRepo.GetUserProfile(userId);

            FollowModel followsUser = await _accountRepo.CheckIfUserFollowsUser(currentUser.Id, userId);

            return await _accountService.GetUserProfile(currentUser, user, followsUser);
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<OP_UserModel> RenderEditProfile()
        {
            if(!User.Identity.IsAuthenticated)
            {
                return null;
            }

            UserModel currentUser = await _userManager.GetUserAsync(User);

            OP_UserModel returnUser = await _accountService.RenderEditProfile(currentUser);

            List<ProjectModel> projects = await _accountRepo.GetCurrentUsersProjects(currentUser.Id);

            List<OP_ProjectModel> returnProjects = await _accountService.GetUsersProjects(projects);

            returnUser.Projects = returnProjects;

            return returnUser;
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<bool> SaveChangedProfileImage(string tempImageId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return false;
            }

            UserModel currentUser = await _userManager.GetUserAsync(User);

            await _blobService.DeleteBlob("1-" + currentUser.Id);

            byte[] profileImage = await _blobService.GetImage(tempImageId);

            await _blobService.UploadImage(profileImage, "1-" + currentUser.Id);

            await _blobService.DeleteBlob(tempImageId);

            return true;
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<bool> SaveChangedDescription(string description)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return false;
            }
            var currentUser = await _userManager.GetUserAsync(User);

            var user = _context.Users.Where(w => w.Id == currentUser.Id).First();

            user.Description = description;

            _context.SaveChanges();

            return true;
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<bool> FollowUser(int userId)
        {
            if(!User.Identity.IsAuthenticated)
            {
                return false;
            }
            UserModel currentUser = await _userManager.GetUserAsync(User);

            await _accountRepo.FollowUser(userId, currentUser, DateTime.Now);

            return true;
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<bool> UnFollowUser(int userId)
        {
            UserModel currentUser = await _userManager.GetUserAsync(User);

            await _accountRepo.UnFollowUser(userId, currentUser.Id);

            return true;
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<OP_HomePage> GetCurrentUserForHomePage()
        {
            if (!User.Identity.IsAuthenticated)
            {
                var result = new OP_HomePage
                {
                    Projects = await GetHomePageRecommendedNotAuthenticated(),
                    User = null
                };

                return result;
            }
            else
            {
                var currentUser = await _userManager.GetUserAsync(User);

                var returnUser = new OP_UserModel
                {
                    Firstname = currentUser.Firstname,
                    Lastname = currentUser.Lastname,
                    Id = currentUser.Id
                };

                var result = new OP_HomePage
                {
                    Projects = await GetHomePageRecommendedAuthenticated(),
                    User = returnUser
                };

                return result;
            }
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<List<OP_ProjectPageModel>> LoadMoreRecommendedProjects()
        {
            List<OP_ProjectPageModel> result = new List<OP_ProjectPageModel>();

            if (!User.Identity.IsAuthenticated)
            {
                result = await GetHomePageRecommendedNotAuthenticated();

                return result;
            }

            result = await GetHomePageRecommendedAuthenticated();

            return result;
        }

        [HttpPost]
        [Route("api/[controller]/[action]")]
        public async Task<List<OP_UserModel>> GetUsersFollowers(IP_LoadUsersFollow loadData)
        {
            List<FollowModel> followers = await _accountRepo.GetUsersFollowers(loadData.UserId, loadData.PageNum * 5);

            return await _accountService.GetUsersFollowers(followers, loadData.UserId);
        }

        [HttpPost]
        [Route("api/[controller]/[action]")]
        public async Task<List<OP_UserModel>> GetUsersFollowing(IP_LoadUsersFollow loadData)
        {
            List<FollowModel> followings = await _accountRepo.GetUsersFollowing(loadData.UserId, loadData.PageNum * 5);

            return await _accountService.GetUsersFollowing(followings, loadData.UserId);
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<List<OP_ProjectPageModel>> GetHomePageRecommendedAuthenticated()
        {
            List<OP_ProjectPageModel> result = new List<OP_ProjectPageModel>();

            List<int> projectPageIds = await _projectRepo.GetListOfProjectPageIds();

            UserModel currentUser = await _userManager.GetUserAsync(User);

            List<int> userProjectIds = await _accountService.GetRandomProjectPageIds(projectPageIds);

            foreach (var projectPageId in userProjectIds)
            {
                ProjectPageModel model = await _projectRepo.GetUserProjectPageAuthenticatedHomePage(projectPageId, currentUser.Id);

                result.Add(await _accountService.GetUserProjectPageAuthenticatedHomePage(model));
            }

            return result;
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<List<OP_ProjectPageModel>> GetHomePageRecommendedNotAuthenticated()
        {
            List<OP_ProjectPageModel> result = new List<OP_ProjectPageModel>();

            List<int> projectPageIds = await _projectRepo.GetListOfProjectPageIds();

            List<int> userProjectIds = await _accountService.GetRandomProjectPageIds(projectPageIds);

            foreach (int userProjectId in userProjectIds)
            {
                ProjectPageModel model = await _projectRepo.GetUserProjectPageNotAuthenticatedHomePage(userProjectId);

                result.Add(await _accountService.GetUserProjectPageNotAuthenticatedHomePage(model));
            }

            return result;
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<bool> DeleteNotification(int notificationId)
        {
            UserModel currentUser = await _userManager.GetUserAsync(User);

            await _accountRepo.DeleteNotification(notificationId, currentUser.Id);

            return true;
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<List<OP_ProjectPageModel>> GetSavedItems(int pageNum)
        {
            UserModel currentUser = await _userManager.GetUserAsync(User);

            List<SaveItemModel> savedItems = await _accountRepo.GetUserSavedItems(currentUser.Id, pageNum * 5);

            List<OP_ProjectPageModel> result = new List<OP_ProjectPageModel>();

            foreach (SaveItemModel savedItem in savedItems)
            {
                result.Add(await _accountService.GetSavedItem(savedItem));
            }

            return result;
        }

        [HttpPost]
        [Route("api/[controller]/[action]")]
        public async Task<List<OP_UserModel>> SearchUsers(IP_SearchModel searchData)
        {
            if(User.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.GetUserAsync(User);

                var searchResult = await _accountRepo.SearchUsers(searchData);

                var result = new List<OP_UserModel>();

                if(searchResult != null)
                {
                    foreach(var searchResultUser in searchResult)
                    {
                        result.Add(await _accountService.GetSearchResultAuthenticated(searchResultUser, currentUser.Id));
                    }

                    return result;
                }
                return null;
            }
            else
            {
                var searchResult = await _accountRepo.SearchUsers(searchData);

                var result = new List<OP_UserModel>();

                if (searchResult != null)
                {
                    foreach (var searchResultUser in searchResult)
                    {
                        result.Add(await _accountService.GetSearchResultNotAuthenticated(searchResultUser));
                    }
                    return result;
                }
                return null;
            }
        }

        [HttpPost]
        [Route("api/[controller]/[action]")]
        public async Task<List<OP_ProjectModel>> SearchProjects(IP_SearchModel searchData)
        {
            var searchResult = await _accountRepo.SearchProjects(searchData);

            if(searchResult != null)
            {
                var result = new List<OP_ProjectModel>();

                foreach(var searchResultProject in searchResult)
                {
                    result.Add(await _accountService.GetSearchResultProjects(searchResultProject));
                }
                return result;
            }
            return null;
        }
    }
}
