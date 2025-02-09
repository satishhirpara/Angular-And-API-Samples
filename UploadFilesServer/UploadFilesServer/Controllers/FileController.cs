using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Net.Http.Headers;

namespace UploadFilesServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        [HttpPost, DisableRequestSizeLimit]
        [Route("upload")]
        public async Task<IActionResult> Upload()
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    return Ok(new { dbPath });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet("download")]
		public IActionResult download() //[FromQuery] string fileUrl
		{
            var fileUrl="Resources\\Images\\profile.jpg";

			var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileUrl);
			//var fileStream = new FileStream(filePath, FileMode.Open); // convert it to a stream
            var fileBytes = System.IO.File.ReadAllBytes(filePath); // convert it to a bytes
            var fileName = "profile.jpg";

            var contentDisposition = new System.Net.Mime.ContentDisposition
            {
                FileName = fileName,
                Inline = false
            };

            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

			// Return the file. A byte array can also be used instead of a stream
			return File(fileBytes, "application/octet-stream", fileName);
		}	

        [HttpGet, DisableRequestSizeLimit]
        [Route("getPhotos")]
        public IActionResult GetPhotos()
        {
            try
            {
                var folderName = Path.Combine("Resources", "Images");
                var pathToRead = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var photos = Directory.EnumerateFiles(pathToRead)
                    .Where(IsAPhotoFile)
                    .Select(fullPath => Path.Combine(folderName, Path.GetFileName(fullPath)));

                return Ok(new { photos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        private bool IsAPhotoFile(string fileName)
        {
            return fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                   || fileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                   || fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase);
        }

        private string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;

            if (!provider.TryGetContentType(path, out contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }
    }
}
