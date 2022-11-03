using AzureTest.Context;
using AzureTest.Contracts;
using AzureTest.Models;
using AzureTest.Models.Entities;
using AzureTest.Models.InputModels;
using Dapper;

namespace AzureTest.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly DapperContext _context;

        public AccountRepository(DapperContext context)
        {
            _context = context;
        }

        public Task<UserModel> GetCurrentUser(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<UserModel>> GetUsers()
        {
            var query = "SELECT * FROM dbo.AspNetUsers";

            using (var connection = _context.CreateConnection())
            {
                var users = await connection.QueryAsync<UserModel>(query);
                return users.ToList();
            }
        }

        public async Task<List<ProjectModel>> GetCurrentUsersProjects(int userId)
        {
            var query = "SELECT " +
                "Id, " +
                "ProjectName, " +
                "ProjectDescription, " +
                "CreationDate, " +
                "Status, " +
                "UpdatesCount, " +
                "CommentsCount, " +
                "VotesCount, " +
                "city, " +
                "country " +
                "FROM dbo.ProjectTable2 WHERE UserId = @userId;";

            using(var connection = _context.CreateConnection())
            using(var multi = await connection.QueryMultipleAsync(query, new {userId}))
            {
                List<ProjectModel> projects = (await multi.ReadAsync<ProjectModel>()).ToList();

                return projects.ToList();
            }
        }

        public async Task FollowUser(int followingTargetId, UserModel currentUser, DateTime date)
        {
            var query = "INSERT INTO dbo.FollowTable (" +
                "followingtargetid, " +
                "currentuserid, " +
                "date) " +
                "VALUES " +
                "(@followingtargetid, " +
                "@currentuserid," +
                "@date ) " +
                "BEGIN TRANSACTION DECLARE @NotificationId int; " +
                "INSERT INTO notificationtable (NotificationType, ItemId1, ItemText1, Date) " +
                "VALUES (2, @currentUserid, @name, @Date) " +
                "SELECT @NotificationId = SCOPE_IDENTITY(); " +
                "INSERT INTO NotificationConnectionTable (UserId, NotificationId, Date) VALUES (@followingtargetId, @notificationId, @date); " +
                "UPDATE AspNetUsers SET FollowersCount = FollowersCount + 1 WHERE Id = @followingTargetId;  " +
                "UPDATE AspNetUsers SET FollowingCount = FollowingCount + 1 WHERE Id = @currentuserId; " +
                "COMMIT";  

            var parameters = new DynamicParameters();
            parameters.Add("followingtargetid", followingTargetId, System.Data.DbType.Int32);
            parameters.Add("currentuserid", currentUser.Id, System.Data.DbType.Int32);
            parameters.Add("date", date, System.Data.DbType.DateTime);
            parameters.Add("name", currentUser.Firstname + " " + currentUser.Lastname, System.Data.DbType.String);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task UnFollowUser(int followingTargetId, int currentUserId)
        {
            var query = "DELETE FROM dbo.FollowTable WHERE followingtargetid = @followingtargetid AND currentuserid = @currentuserid; " +
                "UPDATE aspnetusers SET FollowingCount = FollowingCount - 1 WHERE Id = @currentuserid; " +
                "UPDATE aspnetusers SET FollowersCount = FollowersCount - 1 WHERE Id = @followingTargetId; ";

            var parameters = new DynamicParameters();
            parameters.Add("followingtargetid", followingTargetId, System.Data.DbType.Int32);
            parameters.Add("currentuserid", currentUserId, System.Data.DbType.Int32);

            using(var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { followingTargetId, currentUserId });
            }
        }

        public async Task<UserModel> GetUserProfile(int userId)
        {
            var query = "SELECT " +
                "u.Firstname, " +
                "u.Lastname, " +
                "u.FollowersCount, " +
                "u.FollowingCount, " +
                "u.Description, " +
                "u.AccountType, " +
                "pt.Id, " +
                "pt.ProjectName, " +
                "pt.ProjectDescription, " +
                "pt.CreationDate, " +
                "pt.Status, " +
                "pt.UpdatesCount, " +
                "pt.city, " +
                "pt.country " +
                "FROM aspnetusers u " +
                "LEFT JOIN ProjectTable2 pt on pt.UserId = u.Id " +
                "WHERE u.Id = @userId";

            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId, System.Data.DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                var userDictionary = new Dictionary<int, UserModel>();

                var users = await connection.QueryAsync<UserModel, ProjectModel, UserModel>(query, (user, project) =>
                {
                    if(!userDictionary.TryGetValue(user.Id, out var currentUser))
                    {
                        currentUser = user;
                        userDictionary.Add(currentUser.Id, currentUser);
                    }

                    currentUser.Projects.Add(project);
                    return currentUser;
                },
                splitOn: "Id",
                param: parameters);

                return users.FirstOrDefault();
            }
        }

        public async Task<List<NotificationConnectionModel>> GetUsersNotifications(int userId, int pageNum)
        {
            var query = "SELECT " +
                "nc.Id, " +
                "nc.UserId, " +
                "nc.NotificationId, " +
                "nc.Id, " +
                "nt.NotificationType, " +
                "nt.ItemId1, " +
                "nt.ItemId2, " +
                "nt.ItemText1, " +
                "nt.ItemText2, " +
                "nt.Date " +
                "FROM notificationconnectiontable nc " +
                "JOIN notificationtable nt on nc.notificationid = nt.id WHERE nc.userid = @userid " +
                "ORDER BY nc.Date DESC, nc.Id OFFSET @pageNum ROWS FETCH NEXT 5 ROWS ONLY";

            var parameters = new DynamicParameters();
            parameters.Add("userid", userId, System.Data.DbType.Int32);
            parameters.Add("pageNum", pageNum, System.Data.DbType.Int32);

            using(var connection = _context.CreateConnection())
            {
                var notificationDict = new Dictionary<int, NotificationConnectionModel>();

                var notifications = await connection.QueryAsync<NotificationConnectionModel, NotificationModel, NotificationConnectionModel>(query, (notificConn, notification) =>
                {
                    if(!notificationDict.TryGetValue(notificConn.Id, out var currentNotification))
                    {
                        currentNotification = notificConn;
                        notificationDict.Add(notificConn.Id, currentNotification);
                    }
                    currentNotification.Notification = notification;
                    return currentNotification;
                },
                splitOn: "NotificationType",
                param: parameters);

                return notifications.Distinct().ToList();
            }
        }

        public async Task<List<FollowModel>> GetUsersFollowersId(int userId)
        {
            var query = "SELECT Id, CurrentUserId FROM FollowTable where FollowingTargetId = @userId";

            var parameters = new DynamicParameters();
            parameters.Add("userId", userId, System.Data.DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                var followObjects = await connection.QueryAsync<FollowModel>(query, parameters);

                return followObjects.ToList();
            }
        }

        public async Task<int> CountUsersNotification(int userId)
        {
            var query = "SELECT COUNT(Id) as count_notifications FROM NotificationConnectionTable WHERE UserId = @userId";

            var parameters = new DynamicParameters();
            parameters.Add("userId", userId, System.Data.DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                int notificationCount = await connection.QuerySingleOrDefaultAsync<int>(query, parameters);

                return notificationCount;
            }
        }

        public async Task DeleteNotification(int notificationId, int currentUserId)
        {
            var query = "DELETE FROM NotificationConnectionTable WHERE UserId = @currentUserId AND Id = @notificationId";

            var parameters = new DynamicParameters();
            parameters.Add("currentUserId", currentUserId, System.Data.DbType.Int32);
            parameters.Add("notificationId", notificationId, System.Data.DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task<List<FollowModel>> GetUsersFollowers(int userId, int pageNum)
        {
            var query = "SELECT " +
                "ft.id, " +
                "ft.currentuserid, " +
                "ft.followingtargetid, " +
                "u.Id, " +
                "u.Firstname, " +
                "u.Lastname " +
                "FROM FollowTable ft " +
                "JOIN aspnetusers u on ft.currentuserid = u.id WHERE ft.followingtargetid = @userId " +
                "ORDER BY ft.Id OFFSET @pageNum ROWS FETCH NEXT 5 ROWS ONLY";

            var parameters = new DynamicParameters();
            parameters.Add("userid", userId, System.Data.DbType.Int32);
            parameters.Add("pageNum", pageNum, System.Data.DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                var followersDict = new Dictionary<int, FollowModel>();

                var followers = await connection.QueryAsync<FollowModel, UserModel, FollowModel>(query, (followModel, userModel) =>
                {
                    if (!followersDict.TryGetValue(followModel.Id, out var currentFollower))
                    {
                        currentFollower = followModel;
                        followersDict.Add(followModel.Id, currentFollower);
                    }
                    currentFollower.User = userModel;
                    return currentFollower;
                },
                splitOn: "id",
                param: parameters);

                return followers.Distinct().ToList();
            }
        }

        public async Task<List<FollowModel>> GetUsersFollowing(int userId, int pageNum)
        {
            var query = "SELECT " +
                "ft.id, " +
                "ft.currentuserid, " +
                "ft.followingtargetid, " +
                "u.id, " +
                "u.firstname, " +
                "u.lastname " +
                "FROM Followtable ft " +
                "JOIN aspnetusers u on ft.followingtargetid = u.id WHERE ft.currentuserid = @userId " +
                "ORDER BY ft.id OFFSET @pageNum ROWS FETCH NEXT 5 ROWS ONLY";

            var parameters = new DynamicParameters();
            parameters.Add("userid", userId, System.Data.DbType.Int32);
            parameters.Add("pageNum", pageNum, System.Data.DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                var followingDict = new Dictionary<int, FollowModel>();

                var followers = await connection.QueryAsync<FollowModel, UserModel, FollowModel>(query, (followModel, userModel) =>
                {
                    if (!followingDict.TryGetValue(followModel.Id, out var currentFollower))
                    {
                        currentFollower = followModel;
                        followingDict.Add(followModel.Id, currentFollower);
                    }
                    currentFollower.User = userModel;
                    return currentFollower;
                },
                splitOn: "id",
                param: parameters);

                return followers.Distinct().ToList();
            }
        }

        public async Task<FollowModel> CheckIfUserFollowsUser(int currentUserId, int followingTargetId)
        {
            var query = "SELECT " +
                "id " +
                "FROM FollowTable WHERE CurrentUserId = @currentUserId AND FollowingTargetId = @followingTargetId";

            var parameters = new DynamicParameters();
            parameters.Add("currentUserId", currentUserId, System.Data.DbType.Int32);
            parameters.Add("followingTargetId", followingTargetId, System.Data.DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                var following = await connection.QuerySingleOrDefaultAsync<FollowModel>(query, new { currentUserId, followingTargetId });

                return following;
            }
        }

        public async Task<List<SaveItemModel>> GetUserSavedItems(int currentUserId, int pageNum)
        {
            var query = "SELECT " +
                "st.itemid, st.userid, st.id, st.date, " +
                "pp.date, pp.description, pp.totallikes, pp.commentscount, pp.imagescount, pp.projectid, pp.id, " +
                "lt.id, " +
                "pt.projectname, pt.userid, pt.id, pt.city, pt.country, " +
                "u.firstname, u.lastname, u.id, u.accounttype, " +
                "ft.followingtargetid, ft.currentuserid " +
                "FROM SaveItemTable st " +
                "JOIN projectpagetable pp on st.itemid = pp.id " +
                "LEFT JOIN likeprojectpagetable2 lt on lt.ProjectPageId = st.itemid AND lt.userid = @currentUserId " +
                "JOIN projecttable2 pt on pt.id = pp.projectid " +
                "JOIN aspnetusers u on u.id = pt.userid " +
                "LEFT JOIN followtable ft on ft.currentuserid = @currentUserId AND ft.followingtargetid = pt.userid WHERE st.userId = @currentUserId ORDER BY st.date OFFSET @pageNum ROWS FETCH NEXT 5 ROWS ONLY";

            var parameters = new DynamicParameters();
            parameters.Add("currentUserId", currentUserId, System.Data.DbType.Int32);
            parameters.Add("pageNum", pageNum, System.Data.DbType.Int32);

            using(var connection = _context.CreateConnection())
            {
                var savedItemsDict = new Dictionary<int, SaveItemModel>();
                var savedItems = await connection.QueryAsync<SaveItemModel, ProjectPageModel, LikeProjectPageModel, ProjectModel, UserModel, FollowModel, SaveItemModel>(query, (saveItem, projectPage, vote, project, user, follows) =>
                {
                    if(!savedItemsDict.TryGetValue(saveItem.Id, out var currentSaveItem))
                    {
                        currentSaveItem = saveItem;
                        currentSaveItem.ProjectPage = projectPage;
                        currentSaveItem.ProjectPage.UserHasLiked = vote;
                        currentSaveItem.ProjectPage.Project = project;
                        currentSaveItem.ProjectPage.User = user;
                        currentSaveItem.Follows = follows;

                        savedItemsDict.Add(currentSaveItem.Id, currentSaveItem);
                    }

                    return currentSaveItem;
                }, 
                splitOn: "date, id, projectname, firstname, followingtargetid", 
                param: parameters);

                return savedItems.Distinct().ToList();
            }
        }

        public async Task<List<UserModel>> SearchUsers(IP_SearchModel searchData)
        {
            var query = "SELECT Id, Firstname, Lastname, AccountType FROM aspnetusers WHERE Firstname LIKE(@searchstring) OR Lastname LIKE(@searchstring) OR(Firstname + ' ' + Lastname LIKE(@searchstring)) OR(Lastname + ' ' + Firstname LIKE(@searchstring)) ORDER BY ID OFFSET @pagenum ROWS FETCH NEXT 5 ROWS ONLY";

            var parameters = new DynamicParameters();
            parameters.Add("searchstring", searchData.SearchValue + "%", System.Data.DbType.String);
            parameters.Add("pagenum", searchData.PageNum, System.Data.DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                var searchResult = await connection.QueryAsync<UserModel>(query, parameters);

                return searchResult.Distinct().ToList();
            }
        }

        public async Task<List<ProjectModel>> SearchProjects(IP_SearchModel searchData)
        {
            var query = "SELECT Id, ProjectName, country, city FROM ProjectTable2 WHERE ProjectName LIKE(@searchstring) OR city LIKE(@searchstring) OR country LIKE(@searchstring) OR county LIKE(@searchstring) OR(ProjectName + ' ' + city LIKE(@searchstring)) ORDER BY ID OFFSET @pagenum ROWS FETCH NEXT 5 ROWS ONLY";

            var parameters = new DynamicParameters();
            parameters.Add("searchstring", searchData.SearchValue + "%", System.Data.DbType.String);
            parameters.Add("pagenum", searchData.PageNum, System.Data.DbType.Int32);

            using(var connection = _context.CreateConnection())
            {
                var searchResult = await connection.QueryAsync<ProjectModel>(query, parameters);

                return searchResult.Distinct().ToList();

            }
        }   

    }
}
