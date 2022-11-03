using AzureTest.Models;
using AzureTest.Models.OutPutModels;
using AzureTest.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AzureTest.Controllers
{
    [ApiController]
    public class ReUseController : ControllerBase
    {
        private readonly AppDbContext _context;
        private UserManager<UserModel> _userManager;
        private readonly BlobService _blobService;

        public ReUseController(AppDbContext context, UserManager<UserModel> userManager, BlobService blobService)
        {
            _context = context;
            _userManager = userManager;
            _blobService = blobService;
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("api/[controller]/[action]")]
        public async Task<OP_TempImageModel> UploadTempImage([FromQuery] int imageType)
        {
            var currentUser = _userManager.GetUserAsync(User);

            var validContentTypes = new List<string>
            {
                "image/bmp",
                "image/png",
                "image/jpeg",
                "image/gif",
                "image/tiff",
                "image/vnd.microsoft.icon"
            };

            var file = Request.Form.Files[0];
            if (!validContentTypes.Contains(file.ContentType))
            {
                throw new ApplicationException("Could not resolve file");
            }

            var tempImage = new TemporaryImageModel();
            tempImage.ImageType = imageType;
            tempImage.Date = DateTime.Now;

            string imageId;

            using (var content = file.OpenReadStream())
            {
                tempImage.Image = new byte[content.Length];
                content.Read(tempImage.Image, 0, (int)content.Length);

                imageId = "6-" + currentUser.Id + "-" + DateTime.Now;

                await _blobService.UploadImage(tempImage.Image, imageId);
            }

            var result = new OP_TempImageModel
            {
                Image = tempImage.Image,
                Id = imageId
            };

            return result;
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<bool> RemoveTempImage(string tempImageId)
        {
            await _blobService.DeleteBlob("6-" + tempImageId);

            if(await _blobService.GetImage("6-" + tempImageId) == null)
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
