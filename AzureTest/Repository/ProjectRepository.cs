using AzureTest.Context;
using AzureTest.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using AzureTest.Contracts;
using AzureTest.Models.OutPutModels;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using AzureTest.Models.InputModels;
using AzureTest.Models;

namespace AzureTest.Repository
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly DapperContext _context;

        public ProjectRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<bool> CanActivateProject(int projectId, int userId)
        {
            var query = "SELECT UserId From ProjectTable2 WHERE Id = @projectId";

            using (var connection = _context.CreateConnection())
            {
                var userId2 = await connection.QuerySingleOrDefaultAsync<int>(query, new { projectId });

                if(userId2 == userId)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task LikeProjectPage(int projectPageId, int currentUserId)
        {
            var query = "INSERT INTO dbo.LikeProjectPageTable2 (" +
                "ProjectPageId, " +
                "UserId) VALUES " +
                "(@ProjectPageId, " +
                "@UserId); UPDATE dbo.ProjectPageTable SET TotalLikes = TotalLikes + 1 WHERE Id = @ProjectPageId;";

            var parameters = new DynamicParameters();
            parameters.Add("ProjectPageId", projectPageId, System.Data.DbType.Int32);
            parameters.Add("UserId", currentUserId, System.Data.DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task UnLikeProjectPage(int projectPageId, int currentUserId)
        {
            var query = "DELETE FROM LikeProjectPageTable2 WHERE USERID = @currentUserId AND ProjectPageId = @projectPageId; " +
                "UPDATE dbo.ProjectPageTable SET TotalLikes = TotalLikes - 1 WHERE Id = @projectPageId";

            var parameters = new DynamicParameters();
            parameters.Add("ProjectPageId", projectPageId, System.Data.DbType.Int32);
            parameters.Add("CurrentUserId", currentUserId, System.Data.DbType.Int32);

            using(var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task<ProjectModel> CreateProject(ProjectModel projectData, UserModel currentUser)
        {
            DateTime date = DateTime.Now;

            var query = "BEGIN TRANSACTION Declare @ProjectId int; INSERT INTO dbo.ProjectTable2 (" +
                "UserId, " +
                "ProjectName, " +
                "ProjectDescription, " +
                "CreationDate, " +
                "Status, " +
                "UpdatesCount, " +
                "CommentsCount, " +
                "VotesCount, " +
                "address_line1, " +
                "address_line2, " +
                "city, " +
                "country, " +
                "county, " +
                "state, " +
                "country_code, " +
                "formatted, " +
                "lat, " +
                "lon, " +
                "postcode, " +
                "street) " +
                "VALUES" +
                "(@UserId, " +
                "@ProjectName," +
                "@ProjectDescription," +
                "@CreationDate," +
                "1," +
                "0," +
                "0," +
                "0," +
                "@address_line1," +
                "@address_line2," +
                "@city," +
                "@country," +
                "@county," +
                "@state," +
                "@country_code," +
                "@formatted," +
                "@lat," +
                "@lon," +
                "@postcode," +
                "@street);" +
                " SELECT @ProjectId=SCOPE_IDENTITY(); " +
                "INSERT INTO NotificationTable (NotificationType, ItemId1, ItemId2, ItemText1, ItemText2, Date) VALUES (3, @Projectid, @currentuserid, @projectname, @currentusername, @date); COMMIT select @ProjectId";

            var parameters = new DynamicParameters();
            parameters.Add("UserId", projectData.UserId, System.Data.DbType.Int32);
            parameters.Add("ProjectName", projectData.ProjectName, System.Data.DbType.String);
            parameters.Add("ProjectDescription", projectData.ProjectDescription, System.Data.DbType.String);
            parameters.Add("CreationDate", projectData.CreationDate, System.Data.DbType.DateTime);
            parameters.Add("Status", projectData.Status, System.Data.DbType.Int32);
            parameters.Add("UpdatesCount", projectData.Status, System.Data.DbType.Int32);
            parameters.Add("CommentsCount", projectData.UpdatesCount, System.Data.DbType.Int32);
            parameters.Add("VotesCount", projectData.VotesCount, System.Data.DbType.Int32);
            parameters.Add("address_line1", projectData.address_line1, System.Data.DbType.String);
            parameters.Add("address_line2", projectData.address_line2, System.Data.DbType.String);
            parameters.Add("city", projectData.city, System.Data.DbType.String);
            parameters.Add("country", projectData.country, System.Data.DbType.String);
            parameters.Add("county", projectData.county, System.Data.DbType.String);
            parameters.Add("state", projectData.state, System.Data.DbType.String);
            parameters.Add("country_code", projectData.country_code, System.Data.DbType.String);
            parameters.Add("formatted", projectData.formatted, System.Data.DbType.String);
            parameters.Add("lat", projectData.lat, System.Data.DbType.String);
            parameters.Add("lon", projectData.lon, System.Data.DbType.String);
            parameters.Add("postcode", projectData.postcode, System.Data.DbType.String);
            parameters.Add("street", projectData.street, System.Data.DbType.String);
            parameters.Add("currentUserName", currentUser.Firstname + " " + currentUser.Lastname, System.Data.DbType.String);
            parameters.Add("currentUserId", currentUser.Id, System.Data.DbType.Int32);
            parameters.Add("date", date, System.Data.DbType.DateTime);

            using(var connection = _context.CreateConnection())
            {
                var id = await connection.QuerySingleAsync<int>(query, parameters);

                var project = new ProjectModel
                {
                    Id = id
                };

                return project;
            }
        }

        public async Task<NotificationModel> CreateProjectPage(ProjectPageModel projectPageData, UserModel currentUser)
        {
            NotificationModel returnNotification;

            var query = "BEGIN TRANSACTION DECLARE @ProjectPageId int, @NotificationId int; INSERT INTO dbo.ProjectPageTable (" +
                "ProjectId, " +
                "Date," +
                "Description, " +
                "TotalLikes, " +
                "CommentsCount, " +
                "ImagesCount)" +
                "VALUES" +
                "(@ProjectId, " +
                "@Date, " +
                "@Description, " +
                "@TotalLikes, " +
                "@CommentsCount," +
                "@ImagesCount)" +
                "SELECT @ProjectPageId = SCOPE_IDENTITY() " +
                "INSERT INTO NotificationTable (NotificationType, ItemId1, ItemId2, ItemText1, ItemText2, Date) VALUES (1, @ProjectPageId, @currentUserId, @ProjectName, @currentUsername, @date) " +
                "SELECT @NotificationId = SCOPE_IDENTITY(); COMMIT SELECT Id, ItemId1 FROM NotificationTable WHERE Id = @NotificationId " +
                "UPDATE NotificationTable SET ItemId1 = @ProjectId WHERE Id = @NotificationId; " +
                "UPDATE ProjectTable2 SET UpdatesCount = UpdatesCount + 1 WHERE Id = @ProjectId";

            var parameters = new DynamicParameters();

            parameters.Add("ProjectId", projectPageData.ProjectId, DbType.Int32);
            parameters.Add("Date", projectPageData.Date, DbType.DateTime);
            parameters.Add("Description", projectPageData.Description, DbType.String);
            parameters.Add("TotalLikes", projectPageData.TotalLikes, DbType.Int32);
            parameters.Add("CommentsCount", projectPageData.CommentsCount, DbType.Int32);
            parameters.Add("ImagesCount", projectPageData.ImagesCount, DbType.Int32);
            parameters.Add("ProjectName", projectPageData.ProjectName, System.Data.DbType.String);
            parameters.Add("currentUserId", currentUser.Id, DbType.Int32);
            parameters.Add("currentUsername", currentUser.Firstname + " " + currentUser.Lastname, System.Data.DbType.String);

            using(var connection = _context.CreateConnection())
            {
                var notification = await connection.QuerySingleAsync<NotificationModel>(query, parameters);

                returnNotification = notification;

                return returnNotification;
            }
        }

        public async Task<ProjectModel> RenderEditProjectPage(int projectId)
        {
            var query = "SELECT " +
                "ProjectName, " +
                "ProjectDescription, " +
                "Id, " +
                "Status FROM ProjectTable2 WHERE Id = @projectId";

            using(var connection = _context.CreateConnection())
            {
                var project = await connection.QuerySingleOrDefaultAsync<ProjectModel>(query, new { projectId });

                return project;
            }
        }

        public async Task UpdateProject(IP_UpdateProjectModel projectData)
        {
            var query = "UPDATE " +
                "dbo.ProjectTable2 SET " +
                "ProjectName = @ProjectName, " +
                "ProjectDescription = @ProjectDescription, " +
                "Status = @Status " +
                "WHERE Id = @projectId";

            var parameters = new DynamicParameters();
            parameters.Add("ProjectName", projectData.ProjectName, DbType.String);
            parameters.Add("ProjectDescription", projectData.ProjectDescription, DbType.String);
            parameters.Add("Status", projectData.Status, DbType.Int32);
            parameters.Add("ProjectId", projectData.ProjectId, DbType.Int32);

            using(var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task<ProjectPageModel> GetProjectPage(int id)
        {
            var query = "SELECT Date, " +
                "Description, " +
                "TotalLikes, " +
                "CommentsCount, " +
                "ImagesCount " +
                "FROM dbo.ProjectPageTable WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                var projectPage = await connection.QuerySingleOrDefaultAsync<ProjectPageModel>(query, new { id });

                return projectPage;
            }
        }

        public async Task<ProjectPageModel> GetUserProjectPageNotAuthenticatedHomePage(int projectPageId)
        {
            var query = "SELECT " +
                "pp.id, " +
                "pp.projectid, " +
                "pp.date, " +
                "pp.description, " +
                "pp.totallikes, " +
                "pp.commentscount, " +
                "pp.imagescount, " +
                "pt.projectname," +
                "pt.id, " +
                "pt.userid, " +
                "pt.city," +
                "pt.country, " +
                "u.id," +
                "u.firstname, " +
                "u.lastname," +
                "u.accounttype " +
                "FROM projectpagetable pp " +
                "JOIN projecttable2 pt on pt.id = pp.projectid " +
                "JOIN aspnetusers u on u.id = pt.userId WHERE pp.id = @projectPageId ";

            var parameters = new DynamicParameters();
            parameters.Add("projectPageId", projectPageId, System.Data.DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                var projectPageDict = new Dictionary<int, ProjectPageModel>();
                var projectPages = await connection.QueryAsync<ProjectPageModel, ProjectModel, UserModel, ProjectPageModel>(query, (projectPage, projectModel, user) =>
                {
                    if (!projectPageDict.TryGetValue(projectPage.Id, out var currentProjectPage))
                    {
                        currentProjectPage = projectPage;
                        currentProjectPage.Project = projectModel;
                        currentProjectPage.User = user;
                    }

                    return currentProjectPage;
                },
                splitOn: "projectname, id",
                param: parameters);

                return projectPages.FirstOrDefault();
            }
        }

        public async Task<ProjectPageModel> GetUserProjectPageAuthenticatedHomePage(int projectPageId, int currentUserId)
        {
            var query = "SELECT " +
                "pp.id, " +
                "pp.projectid, " +
                "pp.date, " +
                "pp.description, " +
                "pp.totallikes, " +
                "pp.commentscount, " +
                "pp.imagescount, " +
                "lt.projectpageid, " +
                "st.itemid, " +
                "pt.projectname," +
                "pt.id, " +
                "pt.userid, " +
                "pt.city," +
                "pt.country, " +
                "u.id," +
                "u.firstname, " +
                "u.lastname, " +
                "u.accounttype, " +
                "ft.followingtargetid " +
                "FROM projectpagetable pp " +
                "LEFT JOIN likeprojectpagetable2 lt on lt.projectPageId = @projectpageid AND lt.userid = @currentuserid " +
                "LEFT JOIN saveitemtable st on st.itemid = @projectpageid AND st.userid = @currentUserId " +
                "JOIN projecttable2 pt on pt.id = pp.projectid " +
                "JOIN aspnetusers u on u.id = pt.userId " +
                "LEFT JOIN followtable ft on ft.currentuserId = @currentUserId AND ft.followingtargetid = pt.userid WHERE pp.id = @projectPageId ";

            var parameters = new DynamicParameters();
            parameters.Add("projectPageId", projectPageId, System.Data.DbType.Int32);
            parameters.Add("currentUserId", currentUserId, System.Data.DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                var projectPageDict = new Dictionary<int, ProjectPageModel>();
                var projectPages = await connection.QueryAsync<ProjectPageModel, LikeProjectPageModel, SaveItemModel, ProjectModel, UserModel, FollowModel, ProjectPageModel>(query, (projectPage, like, save, projectModel, user, follow) =>
                {
                    if (!projectPageDict.TryGetValue(projectPage.Id, out var currentProjectPage))
                    {
                        currentProjectPage = projectPage;
                        currentProjectPage.Project = projectModel;
                        currentProjectPage.User = user;
                        currentProjectPage.UserHasLiked = like;
                        currentProjectPage.UserHasSaved = save;
                        currentProjectPage.UserFollowsUser = follow;
                    }

                    return currentProjectPage;
                },
                splitOn: "projectpageid, itemid, projectname, id, followingtargetid",
                param: parameters);

                return projectPages.FirstOrDefault();
            }
        }

        public async Task<ProjectPageModel> GetUserProjectPage(int projectPageId, int currentUserId)
        {
            var query = "SELECT " +
                "pp.id, " +
                "pp.projectid, " +
                "pp.date, " +
                "pp.description, " +
                "pp.totallikes, " +
                "pp.commentscount, " +
                "pp.imagescount, " +
                "st.itemId, " +
                "lt.projectPageId," +
                "pt.projectname," +
                "pt.id " +
                "FROM projectpagetable pp " +
                "LEFT JOIN saveitemtable st on st.ItemId = @projectPageId AND st.userid = @currentUserId " +
                "LEFT JOIN likeprojectpagetable2 lt on lt.projectPageId = @projectPageId AND lt.userid = @currentUserId " +
                "JOIN projecttable2 pt on pt.id = pp.projectid WHERE pp.id = @projectPageId ";

            var parameters = new DynamicParameters();
            parameters.Add("projectPageId", projectPageId, System.Data.DbType.Int32);
            parameters.Add("currentUserId", currentUserId, System.Data.DbType.Int32);

            using(var connection = _context.CreateConnection())
            {
                var projectPageDict = new Dictionary<int, ProjectPageModel>();
                var projectPages = await connection.QueryAsync<ProjectPageModel, SaveItemModel, LikeProjectPageModel, ProjectModel, ProjectPageModel>(query, (projectPage, saveItem, vote, projectModel) =>
                {
                    if (!projectPageDict.TryGetValue(projectPage.Id, out var currentProjectPage))
                    {
                        currentProjectPage = projectPage;
                        projectPageDict.Add(projectPage.Id, currentProjectPage);
                    }
                    currentProjectPage.UserHasSaved = saveItem;
                    currentProjectPage.UserHasLiked = vote;
                    currentProjectPage.Project = projectModel;

                    return currentProjectPage;
                }, 
                splitOn: "itemid, projectpageid, projectname",
                param: parameters);

                return projectPages.FirstOrDefault();
            }
        }

        public async Task SaveProjectPage(SaveItemModel model)
        {
            var query = "INSERT INTO dbo.SaveItemTable (" +
                "ItemId, " +
                "UserId, " +
                "Date) " +
                "VALUES " +
                "(@ItemId, " +
                "@UserId, " +
                "@Date)";

            var parameters = new DynamicParameters();
            parameters.Add("ItemId", model.ItemId, System.Data.DbType.Int32);
            parameters.Add("UserId", model.UserId, System.Data.DbType.Int32);
            parameters.Add("Date", model.Date, System.Data.DbType.DateTime);

            using(var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task UnSaveProjectPage(int projectPageId, int currentUserId)
        {
            var query = "DELETE FROM dbo.SaveItemTable WHERE ItemId = @projectPageId AND UserId = @currentUserId";

            var parameters = new DynamicParameters();
            parameters.Add("projectPageId", projectPageId, System.Data.DbType.Int32);
            parameters.Add("currentUserId", currentUserId, System.Data.DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { projectPageId, currentUserId });
            }
        }

        public async Task CommentProjectPage(IP_CommentProjectPage commentData, UserModel currentUser)
        {
            var query = "INSERT INTO dbo.ProjectPageCommentTable " +
                "(UserId, " +
                "ProjectPageId, " +
                "Comment, " +
                "Date) " +
                "VALUES " +
                "(@UserId, " +
                "@ProjectPageId, " +
                "@Comment, " +
                "@Date); " +
                "UPDATE dbo.ProjectPageTable SET CommentsCount = CommentsCount + 1 WHERE Id = @ProjectPageId; BEGIN TRANSACTION DECLARE @NotificationId int; " +
                "INSERT INTO NotificationTable (NotificationType, ItemId1, ItemId2, ItemText1, ItemText2, Date) VALUES (4, @ProjectId, @UserId, @ProjectName, @currentusername, @date)" +
                " SELECT @NotificationId = SCOPE_IDENTITY(); " +
                "INSERT INTO NotificationConnectionTable (UserId, NotificationId, Date) VALUES (@UserId, @NotificationId, @Date); COMMIT;";

            var parameters = new DynamicParameters();
            parameters.Add("UserId", commentData.UserId, System.Data.DbType.Int32);
            parameters.Add("ProjectPageId", commentData.ProjectPageId, System.Data.DbType.Int32);
            parameters.Add("Comment", commentData.Comment, System.Data.DbType.String);
            parameters.Add("Date", commentData.Date, System.Data.DbType.DateTime);
            parameters.Add("ProjectId", commentData.ProjectId, System.Data.DbType.Int32);
            parameters.Add("ProjectName", commentData.ProjectName, System.Data.DbType.String);
            parameters.Add("CurrentUserName", currentUser.Firstname + " " + currentUser.Lastname, System.Data.DbType.String);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task<ProjectModel> GetProject(int projectId)
        {
            var query = "select pt.userid, " +
                        "pt.projectname, " +
                        "pt.projectdescription, " +
                        "pt.creationdate, " +
                        "pt.status, " +
                        "pt.updatescount, " +
                        "pt.commentscount, " +
                        "pt.votescount, " +
                        "pt.country, " +
                        "pt.city, " +
                        "pp.date," +
                        "pp.id, " +
                        "u.firstname, " +
                        "u.lastname, " +
                        "u.id," +
                        "u.accounttype " +
                "from projecttable2 pt " +
                "join aspnetusers u on u.id = pt.userid " +
                "LEFT join projectpagetable pp on pp.projectid = pt.id where pt.id = @projectid;";

            var parameters = new DynamicParameters();
            parameters.Add("projectId", projectId, System.Data.DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                var projects = new List<ProjectModel>();
                var project = await connection.QueryAsync<ProjectModel, ProjectPageModel, UserModel, ProjectModel>(query, (project, projectPage, user) =>
                {
                    var currentproject = projects.FirstOrDefault(w => w.Id == project.Id);
                    if (currentproject == null)
                    {
                        currentproject = project;
                        currentproject.User = user;
                        projects.Add(currentproject);
                    }

                    currentproject.ProjectPages.Add(projectPage);
                    return currentproject;
                }, 
                splitOn: "date, firstname",
                param: parameters);


                return projects.FirstOrDefault();
            }
        }

        public async Task<ProjectModel> GetUserProject(int projectId, int currentUserId)
        {
            var query = "SELECT " +
                "pt.id, " +
                "pt.userid, " +
                "pt.projectname, " +
                "pt.projectdescription, " +
                "pt.creationdate, " +
                "pt.status, " +
                "pt.updatescount, " +
                "pt.commentscount, " +
                "pt.votescount, " +
                "pt.country, " +
                "pt.city, " +
                "pp.date, " +
                "pp.id, " +
                "ft.followingtargetid, " +
                "u.firstname, " +
                "u.lastname," +
                "u.accounttype " +
                "FROM projecttable2 pt " +
                "LEFT JOIN projectpagetable pp on pp.projectid = @projectid " +
                "JOIN aspnetusers u on u.id = pt.userid " +
                "LEFT JOIN followtable ft on pt.userid = ft.followingtargetid AND ft.currentuserid = @currentUserId WHERE pt.id = @projectId";

            var parameters = new DynamicParameters();
            parameters.Add("projectId", projectId, System.Data.DbType.Int32);
            parameters.Add("currentUserId", currentUserId, System.Data.DbType.Int32);

            using(var connection = _context.CreateConnection())
            {
                var projectDict = new Dictionary<int, ProjectModel>();
                var projects = await connection.QueryAsync<ProjectModel, ProjectPageModel, FollowModel, UserModel, ProjectModel>(query, (project, projectPage, follows, user) =>
                {
                    if (!projectDict.TryGetValue(project.Id, out var currentProject))
                    {
                        currentProject = project;
                        currentProject.FollowsUser = follows;
                        currentProject.User = user;
                        projectDict.Add(project.Id, currentProject);
                    }
                    currentProject.ProjectPages.Add(projectPage);

                    return currentProject;
                },
                splitOn: "date, followingtargetid, firstname",
                param: parameters);

                return projects.FirstOrDefault();
            }
        }

        public async Task<ProjectModel> GetUserProjectNotAuthenticated(int projectId)
        {
            var query = "SELECT " +
                "pt.id, " +
                "pt.userid, " +
                "pt.projectname, " +
                "pt.projectdescription, " +
                "pt.creationdate, " +
                "pt.status, " +
                "pt.updatescount, " +
                "pt.commentscount, " +
                "pt.votescount, " +
                "pt.country, " +
                "pt.city, " +
                "pp.date, " +
                "pp.id, " +
                "u.firstname, " +
                "u.lastname," +
                "u.accounttype " +
                "FROM projecttable2 pt " +
                "LEFT JOIN projectpagetable pp on pp.projectid = @projectid " +
                "JOIN aspnetusers u on u.id = pt.userid WHERE pt.id = @projectId";

            var parameters = new DynamicParameters();
            parameters.Add("projectId", projectId, System.Data.DbType.Int32);

            using(var connection = _context.CreateConnection())
            {
                var projectDict = new Dictionary<int, ProjectModel>();
                var projects = await connection.QueryAsync<ProjectModel, ProjectPageModel, UserModel, ProjectModel>(query, (project, projectPage, user) =>
                {
                    if (!projectDict.TryGetValue(project.Id, out var currentProject))
                    {
                        currentProject = project;
                        currentProject.User = user;
                        projectDict.Add(project.Id, currentProject);
                    }
                    currentProject.ProjectPages.Add(projectPage);

                    return currentProject;
                },
                splitOn: "date, firstname",
                param: parameters);

                return projects.FirstOrDefault();
            }
        }

        public async Task<List<int>> GetListOfProjectIds()
        {
            var query = "SELECT Id FROM ProjectTable2;";

            using (var connection = _context.CreateConnection())
            {
                var ids = await connection.QueryAsync<int>(query);
                return ids.Distinct().ToList();
            }
        }

        public async Task<List<int>> GetListOfProjectPageIds()
        {
            var query = "SELECT Id FROM ProjectPageTable";

            using (var connection = _context.CreateConnection())
            {
                var ids = await connection.QueryAsync<int>(query);
                return ids.Distinct().ToList();
            }
        }

        public async Task<List<ProjectPageCommentModel>> GetProjectPageComments(IP_GetCommentsProjectPage commentsData)
        {
            var query = "select " +
                "ct.Id, " +
                "ct.UserId, " +
                "ct.Comment, " +
                "ct.Date, " +
                "u.Firstname, " +
                "u.Lastname, " +
                "u.Id," +
                "u.AccountType " +
                "from ProjectPageCommentTable ct " +
                "join AspNetUsers u on u.Id = ct.UserId WHERE ct.ProjectPageId = @ProjectPageId " +
                "ORDER BY ct.Date OFFSET @PageNum ROWS FETCH NEXT 5 ROWS ONLY";

            var parameters = new DynamicParameters();
            parameters.Add("projectpageid", commentsData.ProjectPageId, System.Data.DbType.Int32);
            parameters.Add("pagenum", commentsData.PageNum, System.Data.DbType.Int32);

            using(var connection = _context.CreateConnection())
            {
                var commentDict = new Dictionary<int, ProjectPageCommentModel>();

                var comments = await connection.QueryAsync<ProjectPageCommentModel, UserModel, ProjectPageCommentModel>(query, (comment, user) =>
                {
                    if (!commentDict.TryGetValue(comment.Id, out var currentComment))
                    {
                        currentComment = comment;
                        commentDict.Add(comment.Id, currentComment);
                    }

                    currentComment.User = user;
                    return currentComment;
                }, 
                splitOn: "Firstname",
                param: parameters);

                return comments.Distinct().ToList();
            }
        }
        
        public async Task CreateNotificationConnection(int userId, int notificationId)
        {
            DateTime date = DateTime.Now;

            var query = "INSERT INTO NotificationConnectionTable (UserId, NotificationId, Date) VALUES (@userId, @notificationId, @date)";

            var parameters = new DynamicParameters();
            parameters.Add("userId", userId, System.Data.DbType.Int32);
            parameters.Add("notificationId", notificationId, System.Data.DbType.Int32);
            parameters.Add("date", date, System.Data.DbType.DateTime);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }
    }
}
