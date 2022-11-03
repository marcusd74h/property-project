using AzureTest.Models;
using AzureTest.Models.Entities;
using AzureTest.Models.InputModels;
using AzureTest.Models.OutPutModels;

namespace AzureTest.Services
{
    public class ProjectService
    {
        private readonly BlobService _blobService;

        public ProjectService(BlobService blobService)
        {
            _blobService = blobService;
        }

        public async Task<OP_ProjectModel> GetProject(ProjectModel project, int projectId)
        {
            OP_ProjectModel result = new OP_ProjectModel
            {
                ProjectName = project.ProjectName,
                ProjectDescription = project.ProjectDescription,
                CreationDate = project.CreationDate,
                Status = project.Status,
                UpdatesCount = project.UpdatesCount,
                CommentsCount = project.CommentsCount,
                PeopleVoted = project.VotesCount,
                country = project.country,
                city = project.city,
            };

            var resultUser = new OP_UserModel
            {
                Firstname = project.User.Firstname,
                Lastname = project.User.Lastname,
                Id = project.User.Id,
                AccountType = project.User.AccountType,
            };

            if (project.ProjectPages[0] != null)
            {
                foreach (var projectPage in project.ProjectPages)
                {
                    var renderDate = new OP_RenderDatesModel
                    {
                        ProjectPageId = projectPage.Id,
                        Date = projectPage.Date,
                    };

                    result.RenderDates.Add(renderDate);
                }
            }

            byte[] projectProfileImage = await _blobService.GetImage("5-" + projectId);

            if (projectProfileImage != null)
            {
                result.ProjectProfileImage = projectProfileImage;
            }

            byte[] userProfileImage = await _blobService.GetImage("1-" + project.User.Id);

            if (userProfileImage != null)
            {
                resultUser.ProfileImage = userProfileImage;
            }

            result.User = resultUser;

            return result;
        }

        public async Task UploadProjectPageTempImages(List<string> tempImageIds, int projectId)
        {
            int highestStackNumber = 0;

            foreach (string tempImageId in tempImageIds)
            {
                highestStackNumber += 1;

                byte[] tempImage = await _blobService.GetImage(tempImageId);

                await _blobService.UploadImage(tempImage, "3-" + highestStackNumber + "-" + projectId);

                await _blobService.DeleteBlob(tempImageId);
            }
        }

        public async Task<OP_ProjectModel> RenderEditProjectPage(ProjectModel projectModel, int projectId)
        {
            OP_ProjectModel returnModel = new OP_ProjectModel
            {
                ProjectDescription = projectModel.ProjectDescription,
                ProjectName = projectModel.ProjectName,
                Status = projectModel.Status,
            };

            byte[] projectProfileImage = await _blobService.GetImage("5-" + projectId);

            if (projectProfileImage != null)
            {
                returnModel.ProjectProfileImage = projectProfileImage;
            }

            return returnModel;
        }

        public async Task<OP_ProjectPageModel> GetProjectPage(ProjectPageModel projectPage, int projectPageId)
        {
            OP_ProjectPageModel returnModel = new OP_ProjectPageModel
            {
                Id = projectPageId,
                Date = projectPage.Date,
                Description = projectPage.Description,
                TotalLikes = projectPage.TotalLikes,
                CommentsCount = projectPage.CommentsCount,
                ImagesCount = projectPage.ImagesCount,
                DisplayImage = 0,
                CurrentImageNumber = 1,
                LoadedImagesNumber = 2,
                CurrentStackNumber = 1
            };

            if (projectPage.UserHasLiked != null)
            {
                returnModel.UserHasLiked = true;
            }
            else
            {
                returnModel.UserHasLiked = false;
            }

            returnModel.Images.Add(await _blobService.GetImage("3-1-" + projectPageId));
            returnModel.Images.Add(await _blobService.GetImage("3-2-" + projectPageId));

            return returnModel;
        }

        public async Task<OP_ProjectPageModel> GetUserProjectPage(ProjectPageModel projectPage, int projectPageId)
        {
            OP_ProjectPageModel returnProjectPage = new OP_ProjectPageModel
            {
                Id = projectPageId,
                Date = projectPage.Date,
                Description = projectPage.Description,
                TotalLikes = projectPage.TotalLikes,
                CommentsCount = projectPage.CommentsCount,
                ImagesCount = projectPage.ImagesCount,
                DisplayImage = 0,
                CurrentImageNumber = 1,
                LoadedImagesNumber = 2,
                CurrentStackNumber = 1
            };

            if (projectPage.UserHasLiked != null)
            {
                returnProjectPage.UserHasLiked = true;
            }
            else
            {
                returnProjectPage.UserHasLiked = false;
            }

            if (projectPage.UserHasSaved != null)
            {
                returnProjectPage.UserHasSaved = true;
            }
            else
            {
                returnProjectPage.UserHasSaved = false;
            }

            if (projectPage.UserHasSaved != null)
            {
                returnProjectPage.UserHasSaved = true;
            }
            else
            {
                returnProjectPage.UserHasSaved = false;
            }

            returnProjectPage.Images.Add(await _blobService.GetImage("3-1-" + projectPageId));
            returnProjectPage.Images.Add(await _blobService.GetImage("3-2-" + projectPageId));

            return returnProjectPage;
        }

        public async Task<OP_ProjectPageModel> GetUserProjectPageNotAuthenticated(ProjectPageModel projectPage, int projectPageId)
        {
            OP_ProjectPageModel returnProjectPage = new OP_ProjectPageModel
            {
                Id = projectPageId,
                Date = projectPage.Date,
                Description = projectPage.Description,
                TotalLikes = projectPage.TotalLikes,
                CommentsCount = projectPage.CommentsCount,
                ImagesCount = projectPage.ImagesCount,
                DisplayImage = 0,
                CurrentImageNumber = 1,
                LoadedImagesNumber = 2,
                CurrentStackNumber = 1,
                UserHasSaved = false,
                UserHasLiked = false
            };

            returnProjectPage.Images.Add(await _blobService.GetImage("3-1-" + projectPageId));
            returnProjectPage.Images.Add(await _blobService.GetImage("3-2-" + projectPageId));

            return returnProjectPage;
        }

        public async Task<List<OP_ProjectPageCommentModel>> GetProjectPageComments(List<ProjectPageCommentModel> comments)
        {
            List<OP_ProjectPageCommentModel> result = new List<OP_ProjectPageCommentModel>();

            foreach (var comment in comments)
            {
                var resultComment = new OP_ProjectPageCommentModel
                {
                    Comment = comment.Comment,
                    Date = comment.Date
                };

                var resultUser = new OP_UserModel
                {
                    Firstname = comment.User.Firstname,
                    Lastname = comment.User.Lastname,
                    Id = comment.User.Id,
                    AccountType = comment.User.AccountType,
                };

                byte[] profileImage = await _blobService.GetImage("1-" + comment.User.Id);

                if (profileImage != null)
                {
                    resultUser.ProfileImage = profileImage;
                }

                resultComment.User = resultUser;

                result.Add(resultComment);
            }

            return result;
        }

        public async Task<OP_ProjectPageCommentModel> MapCommentUser(IP_CommentProjectPage commentData, UserModel currentUser)
        {
            OP_UserModel commentUser = new OP_UserModel
            {
                Firstname = currentUser.Firstname,
                Lastname = currentUser.Lastname,
                Id = currentUser.Id,
                AccountType = currentUser.AccountType
            };

            commentUser.ProfileImage = await _blobService.GetImage("1-" + currentUser.Id);

            OP_ProjectPageCommentModel comment = new OP_ProjectPageCommentModel
            {
                User = commentUser,
                Comment = commentData.Comment,
                Date = DateTime.Now
            };

            return comment;
        }

        public async Task<OP_ProjectModel> MapUserProject(ProjectModel project, UserModel currentUser, int projectId)
        {
            OP_ProjectModel returnProject = new OP_ProjectModel
            {
                ProjectName = project.ProjectName,
                ProjectDescription = project.ProjectDescription,
                CreationDate = project.CreationDate,
                Status = project.Status,
                UpdatesCount = project.UpdatesCount,
                PeopleVoted = project.VotesCount,
                city = project.city,
                country = project.country,
            };

            byte[] projectProfileImage = await _blobService.GetImage("5-" + projectId);

            if (projectProfileImage != null)
            {
                returnProject.ProjectProfileImage = projectProfileImage;
            }

            OP_UserModel returnUser = new OP_UserModel
            {
                Firstname = project.User.Firstname,
                Lastname = project.User.Lastname,
                Id = project.UserId,
                AccountType = project.User.AccountType
            };

            if (project.FollowsUser != null)
            {
                returnUser.FollowsUser = true;
            }
            else
            {
                returnUser.FollowsUser = false;
            }

            byte[] userProfileImage = await _blobService.GetImage("1-" + project.UserId);

            if (userProfileImage != null)
            {
                returnUser.ProfileImage = userProfileImage;
            }

            returnProject.User = returnUser;

            if (project.ProjectPages[0] != null)
            {
                foreach (var projectPage in project.ProjectPages)
                {
                    OP_RenderDatesModel renderDate = new OP_RenderDatesModel
                    {
                        ProjectPageId = projectPage.Id,
                        Date = projectPage.Date
                    };

                    returnProject.RenderDates.Add(renderDate);
                }
            }
            return returnProject;
        }

        public async Task<OP_ProjectModel> MapUserProjectNotAuthenticated(ProjectModel project, int projectId)
        {
            OP_ProjectModel returnProject = new OP_ProjectModel
            {
                ProjectName = project.ProjectName,
                ProjectDescription = project.ProjectDescription,
                CreationDate = project.CreationDate,
                Status = project.Status,
                UpdatesCount = project.UpdatesCount,
                PeopleVoted = project.VotesCount,
                city = project.city,
                country = project.country,
            };

            byte[] projectProfileImage = await _blobService.GetImage("5-" + projectId);

            if (projectProfileImage != null)
            {
                returnProject.ProjectProfileImage = projectProfileImage;
            }

            OP_UserModel returnUser = new OP_UserModel
            {
                Firstname = project.User.Firstname,
                Lastname = project.User.Lastname,
                Id = project.UserId,
                AccountType = project.User.AccountType
            };

            byte[] userProfileImage = await _blobService.GetImage("1-" + project.UserId);

            if (userProfileImage != null)
            {
                returnUser.ProfileImage = userProfileImage;
            }

            returnProject.User = returnUser;

            if (project.ProjectPages[0] != null)
            {
                foreach (var projectPage in project.ProjectPages)
                {
                    OP_RenderDatesModel renderDate = new OP_RenderDatesModel
                    {
                        ProjectPageId = projectPage.Id,
                        Date = projectPage.Date
                    };

                    returnProject.RenderDates.Add(renderDate);
                }
            }
            return returnProject;
        }
    }
}
