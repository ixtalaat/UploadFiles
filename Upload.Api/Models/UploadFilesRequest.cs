namespace Upload.Api.Models
{
    public class UploadFilesRequest
    {
        public IEnumerable<IFormFile>? Files { get; set; }
    }
}
