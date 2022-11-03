using AzureTest.Models;
using AzureTest.Models.Entities;
using AzureTest.Models.InputModels;

namespace AzureTest.Contracts
{
    public interface IAccountRepository
    {
        public Task<IEnumerable<UserModel>> GetUsers();
        public Task<List<ProjectModel>> GetCurrentUsersProjects(int userId);
        public Task FollowUser(int followingTargetId, UserModel currentUser, DateTime date);
        public Task UnFollowUser(int followingTargetId, int currentUserId);
        public Task<UserModel> GetUserProfile(int userId);
        public Task<List<NotificationConnectionModel>> GetUsersNotifications(int userId, int pageNum);
        public Task<List<FollowModel>> GetUsersFollowersId(int userId);
        public Task<int> CountUsersNotification(int userId);
        public Task DeleteNotification(int notificationId, int currentUserId);
        public Task<List<FollowModel>> GetUsersFollowers(int userId, int pageNum);
        public Task<List<FollowModel>> GetUsersFollowing(int userId, int pageNum);
        public Task<FollowModel> CheckIfUserFollowsUser(int currentUserId, int followingTargetId);
        public Task<List<SaveItemModel>> GetUserSavedItems(int currentUserId, int pageNum);
        public Task<List<UserModel>> SearchUsers(IP_SearchModel searchData);
        public Task<List<ProjectModel>> SearchProjects(IP_SearchModel searchData);
    }
}
