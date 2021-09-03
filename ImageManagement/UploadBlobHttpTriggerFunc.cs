using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.Azure.Storage.Blob;
using Azure.Storage.Blobs;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using System.Linq;
using SixLabors.ImageSharp.Processing;
using System.Configuration;
using Azure.Storage;

namespace ImageManagement
{
    public static class UploadBlobHttpTriggerFunc
    {
        [FunctionName("UploadBlobHttpTriggerFunc")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest httpRequest,
        ILogger log, ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Get Application settings
            var config = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

            var acctParameter = config["AccountName"];
            var imgContainerParameter = config["ImageContainer"];
            var imgAccountKeyParameter = config["AccountKey"];
            
            //rename the image with a guid
            var blobExt = config["BlobExtension"];
            var blobName = Guid.NewGuid().ToString();

            bool isUploaded = false;
            //Grab the file attachment from the body of the request
            var imageFile = httpRequest.Form.Files["File"];

            
                if (IsImage(imageFile))
                {
                    if (imageFile.Length > 0)
                    {
                        //resize
                        using (var img = Image.Load(imageFile.OpenReadStream(), out IImageFormat format))
                        {

                        ////resize the image as appropriate
                        //string newSize = ResizeImage(img, 2500, 1406);
                        //string[] aSize = newSize.Split(',');
                        //img.Mutate(h => h.Resize(Convert.ToInt32(aSize[1]), Convert.ToInt32(aSize[0])));

                        //This section save the image to database using blob
                        using (var ms = new MemoryStream())
                            {
                                img.Save(ms, format);
                                isUploaded = await UploadFileToStorage(ms, blobName + blobExt, acctParameter, imgContainerParameter, imgAccountKeyParameter);

                            }


                        }
                        //resize end 

                    }
                }
                else
                {
                    return new UnsupportedMediaTypeResult();
                }

            return new OkObjectResult(blobName);
        }

        public static string ResizeImage(Image img, int maxWidth, int maxHeight)
        {
            if (img.Width > maxWidth || img.Height > maxHeight)
            {
                double widthRatio = (double)img.Width / (double)maxWidth;
                double heightRatio = (double)img.Height / (double)maxHeight;
                double ratio = Math.Max(widthRatio, heightRatio);
                int newWidth = (int)(img.Width / ratio);
                int newHeight = (int)(img.Height / ratio);
                return newHeight.ToString() + "," + newWidth.ToString();
            }
            else
            {
                return img.Height.ToString() + "," + img.Width.ToString();
            }
        }

        public static bool IsImage(IFormFile file)
        {
            if (file.ContentType.Contains("image"))
            {
                return true;
            }

            string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" };

            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        public static async Task<bool> UploadFileToStorage(MemoryStream fileStream, string fileName, string acctParameter, string imgContainerParameter, string imgAccountKeyParameter)
        {

            // Create a URI to the blob
            Uri blobUri = new Uri("https://" +
                                  acctParameter +
                                  ".blob.core.windows.net/" +
                                  imgContainerParameter +
                                  "/" + fileName);

            // Create StorageSharedKeyCredentials object by reading
            // the values from the configuration (appsettings.json)
            StorageSharedKeyCredential storageCredentials =
                new StorageSharedKeyCredential(acctParameter, imgAccountKeyParameter);

            // Create the blob client.
            BlobClient blobClient = new BlobClient(blobUri, storageCredentials);

            //reset the filestream position
            fileStream.Position = 0;

            // Upload the file
            await blobClient.UploadAsync(fileStream);

            return await Task.FromResult(true);
        }

    }
}
