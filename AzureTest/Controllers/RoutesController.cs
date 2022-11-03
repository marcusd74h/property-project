using AzureTest.Contracts;
using AzureTest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AzureTest.Controllers
{
    [ApiController]
    public class RoutesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private UserManager<UserModel> _userManager;
        private readonly IProjectRepository _projectRepo;

        public RoutesController(AppDbContext context, UserManager<UserModel> userManager, IProjectRepository projectRepository)
        {
            _context = context;
            _userManager = userManager;
            _projectRepo = projectRepository;
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<bool> CheckIfCanActivateProjectPage(int inputProjectId, int currentUserId)
        {
            if (currentUserId == null)
            {
                return false;
            }

            if (await _projectRepo.CanActivateProject(inputProjectId, currentUserId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
