using Microsoft.AspNetCore.Mvc;
using Upload.Api.Data;
using Upload.Api.Models;

namespace Upload.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ApplicationDbContext _context;
        public FilesController(IWebHostEnvironment hostingEnvironment, ApplicationDbContext context)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetFiles()
        {
            var files = _context.Files.ToList();
            return Ok(files);
        }

        [HttpPost]
        public IActionResult UploadFiles([FromForm] UploadFilesRequest uploadFilesRequest)
        {
            if (uploadFilesRequest.Files == null || !uploadFilesRequest.Files.Any())
            {
                return BadRequest("No files were uploaded");
            }

            var uploadedFiles = new List<UploadedFile>();

            foreach (var file in uploadFilesRequest.Files)
            {
                var storedFileName = Path.GetRandomFileName();
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads",storedFileName);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                file.CopyTo(fileStream);
                

                uploadedFiles.Add(new UploadedFile
                {
                    FileName = file.FileName,
                    StoredFileName = storedFileName,
                    ContentType = file.ContentType
                });
            }

            _context.Files.AddRange(uploadedFiles);
            _context.SaveChanges();

            return Ok(uploadedFiles);
        }

        [HttpGet("{fileName}")]
        public IActionResult DownloadFile(string fileName)
        {
            var uploadedFile = _context.Files.FirstOrDefault(f => f.StoredFileName == fileName);

            if (uploadedFile == null)
            {
                return NotFound();
            }

            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", fileName);

            using var fileStream = new FileStream(filePath, FileMode.Open);
            var memoryStream = new MemoryStream();
            fileStream.CopyTo(memoryStream);
            memoryStream.Position = 0;

            return File(memoryStream, uploadedFile.ContentType, uploadedFile.FileName);
        }
    }
}
