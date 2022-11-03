using Azure.Storage.Blobs;
using AzureTest.Models;

namespace AzureTest.Services
{
    public class BlobService 
    {
        public async Task<bool> UploadImage(byte[] image, string inputBlobName)
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=azuretestimages;AccountKey=VgrpgRm3YLNNroLGMvpNdmYn2Vw1utXzxpbUI7s+jX8t3C2s9PXc2i1QYO6WapAmpCsrChGbgKh5+AStpBZQOw==;EndpointSuffix=core.windows.net";
            string containerName = "images";
            string blobName = inputBlobName;

            BlobContainerClient container = new BlobContainerClient(connectionString, containerName);

            BlobClient blob = container.GetBlobClient(blobName);

            Stream stream = new MemoryStream(image);

            await blob.UploadAsync(stream, true);

            return true;
        }

        public async Task<byte[]> GetImage(string filePath)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=azuretestimages;AccountKey=VgrpgRm3YLNNroLGMvpNdmYn2Vw1utXzxpbUI7s+jX8t3C2s9PXc2i1QYO6WapAmpCsrChGbgKh5+AStpBZQOw==;EndpointSuffix=core.windows.net");

            var containerClient = blobServiceClient.GetBlobContainerClient("images");
            var blobClient = containerClient.GetBlobClient(filePath).Exists();
            var blobClient2 = containerClient.GetBlobClient(filePath);

            if (blobClient == true)
            {
                var blobDownloadInfo = await blobClient2.DownloadAsync();
                var result = new BlobInfo2(blobDownloadInfo.Value.Content, blobDownloadInfo.Value.ContentType);

                byte[] returnValue;

                using (var memoryStream = new MemoryStream())
                {
                    result.Content.CopyTo(memoryStream);
                    returnValue = memoryStream.ToArray();
                }

                return returnValue;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> DeleteBlob(string inputBlobName)
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=azuretestimages;AccountKey=VgrpgRm3YLNNroLGMvpNdmYn2Vw1utXzxpbUI7s+jX8t3C2s9PXc2i1QYO6WapAmpCsrChGbgKh5+AStpBZQOw==;EndpointSuffix=core.windows.net";
            string containerName = "images";
            string blobName = inputBlobName;

            BlobContainerClient container = new BlobContainerClient(connectionString, containerName);

            bool blobExists = container.GetBlobClient(blobName).Exists();
            BlobClient blob = container.GetBlobClient(blobName);

            if (blobExists)
            {
                await blob.DeleteAsync();

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
