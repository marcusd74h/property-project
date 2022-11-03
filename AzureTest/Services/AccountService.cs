using AutoMapper;
using AzureTest.Contracts;
using AzureTest.Models;
using AzureTest.Models.Entities;
using AzureTest.Models.OutPutModels;

namespace AzureTest.Services
{
    public class AccountService
    {
        private readonly BlobService _blobService;
        private readonly IAccountRepository _accountRepo;
        private readonly IMapper _autoMapper;

        public AccountService(BlobService blobService, IAccountRepository accountRepo)
        {
            _blobService = blobService;
            _accountRepo = accountRepo;

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<OP_ProjectModel, ProjectModel>();
                cfg.CreateMap<OP_UserModel, UserModel>();
            });

            _autoMapper = mapperConfig.CreateMapper();
        }

        public async Task<List<OP_ProjectModel>> GetUsersProjects(List<ProjectModel> projects)
        {
            List<OP_ProjectModel> returnValue = new List<OP_ProjectModel>();

            foreach (ProjectModel project in projects)
            {
                OP_ProjectModel resultProject = _autoMapper.Map<OP_ProjectModel>(project);

                resultProject.PeopleVoted = project.VotesCount;

                byte[] projectCoverImage = await _blobService.GetImage("5-" + project.Id);

                if (projectCoverImage != null)
                {
                    resultProject.ProjectProfileImage = projectCoverImage;
                }

                returnValue.Add(resultProject);
            }

            return returnValue;
        }

        public async Task<OP_UserModel> GetCurrentUser(UserModel currentUser, List<ProjectModel> projects)
        {
            OP_UserModel returnValue = _autoMapper.Map<OP_UserModel>(currentUser);

            byte[] profileImage = await _blobService.GetImage("1-" + currentUser.Id);

            if (profileImage != null)
            {
                returnValue.ProfileImage = profileImage;
            }

            returnValue.Projects = await GetUsersProjects(projects);

            return returnValue;
        }

        public async Task<List<OP_NotificationModel>> GetUsersNotifications(List<NotificationConnectionModel> notifications)
        {
            List<OP_NotificationModel> returnValue = new List<OP_NotificationModel>();

            foreach (NotificationConnectionModel notification in notifications)
            {
                if (notification.Notification.NotificationType == 1)
                {
                    returnValue.Add(await MapNotificationType(notification, 1));
                }
                if (notification.Notification.NotificationType == 2)
                {
                    returnValue.Add(await MapNotificationType(notification, 2));
                }
                if (notification.Notification.NotificationType == 3)
                {
                    returnValue.Add(await MapNotificationType(notification, 3));
                }
                if (notification.Notification.NotificationType == 4)
                {
                    returnValue.Add(await MapNotificationType(notification, 4));
                }
            }
            return returnValue;
        }

        public async Task<OP_NotificationModel> MapNotificationType(NotificationConnectionModel notification, int notificationType)
        {
            OP_NotificationModel returnValue = new OP_NotificationModel();

            OP_NotificationModel notificationModel = new OP_NotificationModel
            {
                Id = notification.Id,
                NotificationType = notificationType,
                ItemId1 = notification.Notification.ItemId1,
                ItemId2 = notification.Notification.ItemId2,
                ItemText1 = notification.Notification.ItemText1,
                ItemText2 = notification.Notification.ItemText2,
                Date = notification.Notification.Date
            };

            byte[] notificationImage = await _blobService.GetImage("1-" + notification.Notification.ItemId2);

            if (notificationImage != null)
            {
                notificationModel.NotificationCoverImage = notificationImage;
            }
          
            returnValue = notificationModel;

            return returnValue;
        }

        public async Task<OP_UserModel> GetUserProfile(UserModel currentUser, UserModel user, FollowModel followsUser) 
        {
            var result = new OP_UserModel
            {
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                FollowersCount = user.FollowersCount,
                FollowingCount = user.FollowingCount,
                Description = user.Description,
                AccountType = user.AccountType,
            };

            if (followsUser != null)
            {
                result.FollowsUser = true;
            }
            else
            {
                result.FollowsUser = false;
            }

            byte[] userProfileImage = await _blobService.GetImage("1-" + user.Id);

            if (userProfileImage != null)
            {
                result.ProfileImage = userProfileImage;
            }

            if (user.Projects[0] != null)
            {
                foreach (ProjectModel project in user.Projects)
                {
                    result.Projects = await GetUserProfileProjects(user.Projects);
                }
            }
            return result;
        }

        public async Task<List<OP_ProjectModel>> GetUserProfileProjects(List<ProjectModel> projects)
        {
            List<OP_ProjectModel> returnValue = new List<OP_ProjectModel>();

            foreach (ProjectModel project in projects)
            {
                OP_ProjectModel projectModel = new OP_ProjectModel
                {
                    Id = project.Id,
                    ProjectName = project.ProjectName,
                    ProjectDescription = project.ProjectDescription,
                    UpdatesCount = project.UpdatesCount,
                    Status = project.Status,
                    city = project.city,
                    country = project.country,
                };

                byte[] projectProfileImage = await _blobService.GetImage("5-" + project.Id);

                if (projectProfileImage != null)
                {
                    projectModel.ProjectProfileImage = projectProfileImage;
                }
              
                returnValue.Add(projectModel);
            }

            return returnValue;
        }

        public async Task<OP_UserModel> RenderEditProfile(UserModel currentUser)
        {
            OP_UserModel returnUser = new OP_UserModel
            {
                Firstname = currentUser.Firstname,
                Lastname = currentUser.Lastname,
                Description = currentUser.Description,
                AccountType = currentUser.AccountType
            };

            byte[] profileImage = await _blobService.GetImage("1-" + currentUser.Id);

            if (profileImage != null)
            {
                returnUser.ProfileImage = profileImage;
            }
        
            return returnUser;
        }

        public async Task<List<OP_ProjectModel>> RenderEditProfileProjects(List<ProjectModel> projects)
        {
            List<OP_ProjectModel> returnValue = new List<OP_ProjectModel>();

            foreach (var project in projects)
            {
                var resultProject = new OP_ProjectModel
                {
                    Id = project.Id,
                    ProjectName = project.ProjectName,
                    ProjectDescription = project.ProjectDescription,
                    PeopleVoted = project.VotesCount,
                    CommentsCount = project.CommentsCount,
                    UpdatesCount = project.UpdatesCount,
                    Status = project.Status,
                    city = project.city,
                    country = project.country,
                };

                byte[] projectCoverImage = await _blobService.GetImage("5-" + project.Id);

                if (projectCoverImage != null)
                {
                    resultProject.ProjectProfileImage = projectCoverImage;
                }

                returnValue.Add(resultProject);
            }

            return returnValue;
        }

        public async Task<List<OP_UserModel>> GetUsersFollowers(List<FollowModel> followers, int userId)
        {
            List<OP_UserModel> returnValue = new List<OP_UserModel>();

            foreach (var follower in followers)
            {
                OP_UserModel returnUser = new OP_UserModel
                {
                    Firstname = follower.User.Firstname,
                    Lastname = follower.User.Lastname,
                    Id = follower.User.Id,
                    FollowsUser = true
                };

                var following = await _accountRepo.CheckIfUserFollowsUser(userId, returnUser.Id);

                if (following != null)
                {
                    returnUser.FollowsUser = true;
                }
                else
                {
                    returnUser.FollowsUser = false;
                }

                byte[] profileImage = await _blobService.GetImage("1-" + returnUser.Id);

                if (profileImage != null)
                {
                    returnUser.ProfileImage = profileImage;
                }
                returnValue.Add(returnUser);
            }
            return returnValue;
        }

        public async Task<List<OP_UserModel>> GetUsersFollowing(List<FollowModel> followings, int userId)
        {
            List<OP_UserModel> returnValue = new List<OP_UserModel>();

            foreach (var following in followings)
            {
                OP_UserModel returnUser = new OP_UserModel
                {
                    Firstname = following.User.Firstname,
                    Lastname = following.User.Lastname,
                    Id = following.User.Id
                };

                var followsuser = await _accountRepo.CheckIfUserFollowsUser(userId, returnUser.Id);

                if (followsuser != null)
                {
                    returnUser.FollowsUser = true;
                }
                else
                {
                    returnUser.FollowsUser = false;
                }

                byte[] profileImage = await _blobService.GetImage("1-" + following.User.Id);

                if (profileImage != null)
                {
                    returnUser.ProfileImage = profileImage;
                }
              
                returnValue.Add(returnUser);
            }
            return returnValue;
        }

        public async Task<List<int>> GetRandomProjectPageIds(List<int> projectPageIds)
        {
            Random rnd = new Random();
            int randIndex1 = rnd.Next(projectPageIds.Count);
            int randIndex2 = rnd.Next(projectPageIds.Count);
            int randIndex3 = rnd.Next(projectPageIds.Count);
            int randIndex4 = rnd.Next(projectPageIds.Count);
            int randIndex5 = rnd.Next(projectPageIds.Count);

            List<int> userProjectIds = new List<int>();

            userProjectIds.Add(projectPageIds[randIndex1]);
            userProjectIds.Add(projectPageIds[randIndex2]);
            userProjectIds.Add(projectPageIds[randIndex3]);
            userProjectIds.Add(projectPageIds[randIndex4]);
            userProjectIds.Add(projectPageIds[randIndex5]);

            return userProjectIds;
        }

        public async Task<OP_ProjectPageModel> GetUserProjectPageAuthenticatedHomePage(ProjectPageModel model)
        {
            OP_ProjectPageModel returnProjectPage = new OP_ProjectPageModel
            {
                Id = model.Id,
                Date = model.Date,
                Description = model.Description,
                TotalLikes = model.TotalLikes,
                CommentsCount = model.CommentsCount,
                ImagesCount = model.ImagesCount,
                DisplayImage = 0,
                CurrentImageNumber = 1,
                LoadedImagesNumber = 2,
                CurrentStackNumber = 1
            };

            var project = new OP_ProjectModel
            {
                ProjectName = model.Project.ProjectName,
                Id = model.Project.Id,
                city = model.Project.city,
                country = model.Project.country
            };

            var user = new OP_UserModel
            {
                Id = model.User.Id,
                Firstname = model.User.Firstname,
                Lastname = model.User.Lastname,
                AccountType = model.User.AccountType,
            };

            if (model.UserFollowsUser != null)
            {
                user.FollowsUser = true;
            }
            else
            {
                user.FollowsUser = false;
            }

            if (model.UserHasLiked != null)
            {
                returnProjectPage.UserHasLiked = true;
            }
            else
            {
                returnProjectPage.UserHasLiked = false;
            }

            if (model.UserHasSaved != null)
            {
                returnProjectPage.UserHasSaved = true;
            }
            else
            {
                returnProjectPage.UserHasSaved = false;
            }

            byte[] userProfileImage = await _blobService.GetImage("1-" + user.Id);

            if (userProfileImage != null)
            {
                user.ProfileImage = userProfileImage;
            }
          
            returnProjectPage.Project = project;
            returnProjectPage.User = user;

            returnProjectPage.Images.Add(await _blobService.GetImage("3-1-" + model.Id));
            returnProjectPage.Images.Add(await _blobService.GetImage("3-2-" + model.Id));

            return returnProjectPage;
        }

        public async Task<OP_ProjectPageModel> GetUserProjectPageNotAuthenticatedHomePage(ProjectPageModel model)
        {
            OP_ProjectPageModel returnProjectPage = new OP_ProjectPageModel
            {
                Id = model.Id,
                Date = model.Date,
                Description = model.Description,
                TotalLikes = model.TotalLikes,
                CommentsCount = model.CommentsCount,
                ImagesCount = model.ImagesCount,
                DisplayImage = 0,
                CurrentImageNumber = 1,
                LoadedImagesNumber = 2,
                CurrentStackNumber = 1
            };

            var project = new OP_ProjectModel
            {
                ProjectName = model.Project.ProjectName,
                Id = model.Project.Id,
                city = model.Project.city,
                country = model.Project.country,
            };

            var user = new OP_UserModel
            {
                Id = model.User.Id,
                Firstname = model.User.Firstname,
                Lastname = model.User.Lastname
            };

            byte[] userProfileImage = await _blobService.GetImage("1-" + user.Id);

            if (userProfileImage != null)
            {
                user.ProfileImage = userProfileImage;
            }
           
            returnProjectPage.User = user;
            returnProjectPage.Project = project;

            returnProjectPage.Images.Add(await _blobService.GetImage("3-1-" + model.Id));
            returnProjectPage.Images.Add(await _blobService.GetImage("3-2-" + model.Id));

            return returnProjectPage;
        }

        public async Task<OP_ProjectPageModel> GetSavedItem(SaveItemModel savedItem)
        {
            var returnPage = new OP_ProjectPageModel
            {
                Id = savedItem.ItemId,
                Date = savedItem.ProjectPage.Date,
                Description = savedItem.ProjectPage.Description,
                TotalLikes = savedItem.ProjectPage.TotalLikes,
                CommentsCount = savedItem.ProjectPage.CommentsCount,
                ImagesCount = savedItem.ProjectPage.ImagesCount,
                DisplayImage = 0,
                CurrentImageNumber = 1,
                LoadedImagesNumber = 2,
                CurrentStackNumber = 1,
                UserHasSaved = true,
            };

            OP_UserModel returnUser = new OP_UserModel
            {
                Firstname = savedItem.ProjectPage.User.Firstname,
                Lastname = savedItem.ProjectPage.User.Lastname,
                Id = savedItem.ProjectPage.User.Id,
                AccountType = savedItem.ProjectPage.User.AccountType
            };

            OP_ProjectModel returnProject = new OP_ProjectModel
            {
                ProjectName = savedItem.ProjectPage.Project.ProjectName,
                Id = savedItem.ProjectPage.ProjectId,
                city = savedItem.ProjectPage.Project.city,
                country = savedItem.ProjectPage.Project.country
            };

            if (savedItem.Follows != null)
            {
                returnUser.FollowsUser = true;
            }
            else
            {
                returnUser.FollowsUser = false;
            }

            if (savedItem.ProjectPage.UserHasLiked != null)
            {
                returnPage.UserHasLiked = true;
            }
            else
            {
                returnPage.UserHasLiked = false;
            }

            byte[] userProfileImage = await _blobService.GetImage("1-" + savedItem.ProjectPage.Project.UserId);

            if (userProfileImage != null)
            {
                returnUser.ProfileImage = userProfileImage;
            }
           
            returnPage.Images.Add(await _blobService.GetImage("3-1-" + savedItem.ItemId));
            returnPage.Images.Add(await _blobService.GetImage("3-2-" + savedItem.ItemId));

            returnPage.Project = returnProject;
            returnPage.User = returnUser;

            return returnPage;
        }

        public async Task<OP_UserModel> GetSearchResultAuthenticated(UserModel searchResultUser, int currentUserId)
        {
            var returnUser = new OP_UserModel
            {
                Id = searchResultUser.Id,
                Firstname = searchResultUser.Firstname,
                Lastname = searchResultUser.Lastname,
                AccountType = searchResultUser.AccountType
            };

            byte[] profileImage = await _blobService.GetImage("1-" + searchResultUser.Id);

            FollowModel followsUser = await _accountRepo.CheckIfUserFollowsUser(currentUserId, returnUser.Id);

            if (followsUser != null)
            {
                returnUser.FollowsUser = true;
            }
            else
            {
                returnUser.FollowsUser = false;
            }

            returnUser.ProfileImage = profileImage;

            return returnUser;
        }

        public async Task<OP_UserModel> GetSearchResultNotAuthenticated(UserModel searchResultUser)
        {
            var returnUser = new OP_UserModel
            {
                Id = searchResultUser.Id,
                Firstname = searchResultUser.Firstname,
                Lastname = searchResultUser.Lastname,
                FollowsUser = false,
                AccountType = searchResultUser.AccountType
            };

            byte[] profileImage = await _blobService.GetImage("1-" + searchResultUser.Id);

            if (profileImage != null)
            {
                returnUser.ProfileImage = profileImage;
            }

            return returnUser;
        }

        public async Task<OP_ProjectModel> GetSearchResultProjects(ProjectModel searchResultProject)
        {
            var returnProject = new OP_ProjectModel
            {
                ProjectName = searchResultProject.ProjectName,
                Id = searchResultProject.Id,
                city = searchResultProject.city,
                country = searchResultProject.country
            };

            byte[] projectProfileImage = await _blobService.GetImage("5-" + returnProject.Id);

            if (projectProfileImage != null)
            {
                returnProject.ProjectProfileImage = projectProfileImage;
            }
           
            return returnProject;
        } 
    }
}
