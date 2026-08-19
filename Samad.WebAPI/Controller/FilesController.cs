using Microsoft.AspNetCore.Mvc;
using Samad.Application.Dtos;
using Samad.Application.Files;

namespace Samad.WebAPI.Controller
{
    [ApiController]
    [Route("api/files")]
    public sealed class FilesController(
    IFileUploadService fileUploadService)
    : ControllerBase
    {
        [HttpPost]
        [RequestSizeLimit(20 * 1024 * 1024)]
        public async Task<ActionResult<StoredFileDto>> Upload(
            IFormFile file,
            CancellationToken cancellationToken)
        {
            if (file is null || file.Length == 0)
                return BadRequest("فایل ارسال نشده است.");

            await using var stream = file.OpenReadStream();

            var input = new IncomingFile(
                Stream: stream,
                OriginalFileName: Path.GetFileName(file.FileName),
                ClientContentType: file.ContentType,
                Length: file.Length);

            var result = await fileUploadService.SaveAsync(
                input,
                cancellationToken);

            return Ok(result);
        }
    }
}