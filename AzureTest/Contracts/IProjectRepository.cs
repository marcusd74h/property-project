using AzureTest.Models;
using AzureTest.Models.Entities;
using AzureTest.Models.InputModels;
using AzureTest.Models.OutPutModels;
using Microsoft.AspNetCore.Mvc;

namespace AzureTest.Contracts
{
    public interface IProjectRepository
    {
        public Task<ProjectModel> CreateProject(ProjectModel projectData, UserModel currentUser);
        public Task<ProjectModel> GetProject(int projectId);
        public Task<NotificationModel> CreateProjectPage(ProjectPageModel projectPageData, UserModel currentUser);
        public Task<ProjectModel> RenderEditProjectPage(int projectId);
        public Task UpdateProject(IP_UpdateProjectModel projectData);
        public Task<ProjectPageModel> GetProjectPage(int projectPageId);
        public Task SaveProjectPage(SaveItemModel model);
        public Task UnSaveProjectPage(int projectPageId, int currentUserId);
        public Task CommentProjectPage(IP_CommentProjectPage commentData, UserModel currentUser);
        public Task<List<ProjectPageCommentModel>> GetProjectPageComments(IP_GetCommentsProjectPage commentsData);
        public Task<bool> CanActivateProject(int projectId, int userId);
        public Task<ProjectModel> GetUserProject(int projectId, int currentUserId);
        public Task<ProjectModel> GetUserProjectNotAuthenticated(int projectId);
        public Task<ProjectPageModel> GetUserProjectPage(int projectPageId, int currentUserId);
        //public Task VoteOnProjectPage(int projectPageId, int currentUserId, int starsCount, int projectId);
        //public Task<int> CreatedProjectPageNotification(int projectId, UserModel currentUser);
        public Task CreateNotificationConnection(int userId, int notificationId);
        public Task<List<int>> GetListOfProjectIds();
        public Task<List<int>> GetListOfProjectPageIds();
        public Task LikeProjectPage(int projectPageId, int currentUserId);
        public Task UnLikeProjectPage(int projectPageId, int currentUserId);    
        public Task<ProjectPageModel> GetUserProjectPageNotAuthenticatedHomePage(int projectPageId);
        public Task<ProjectPageModel> GetUserProjectPageAuthenticatedHomePage(int projectPageId, int currentUserId);
    }
}
