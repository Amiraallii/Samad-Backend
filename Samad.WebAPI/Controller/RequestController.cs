using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Samad.Application.Dtos;
using Samad.Application.IServices;
using Samad.Domain.Entity;

namespace Samad.WebAPI.Controller
{
    [ApiController]
    [Route("api/requests")]
    public sealed class RequestController(IRequestService requestService) : SamadController
    {
        [HttpPost]
        [Authorize(Roles = "Applicant")]
        public async Task<IActionResult> NewRequest(
    [FromForm] NewRequestModel request,
    CancellationToken cancellationToken)
        {
            var incomingFiles = new List<IncomingFile>();
            var streams = new List<Stream>();

            try
            {
                foreach (var file in request.files ?? [])
                {
                    if (file is null || file.Length == 0)
                        return BadRequest("فایل ارسال نشده است.");

                    var stream = file.OpenReadStream();
                    streams.Add(stream);

                    incomingFiles.Add(new IncomingFile(
                        Stream: stream,
                        OriginalFileName: Path.GetFileName(file.FileName),
                        ClientContentType: file.ContentType,
                        Length: file.Length));
                }

                var newRequest = new NewRequest(
                    request.Title,
                    request.Description,
                    request.Urgency,
                    CurrentUserId,
                    incomingFiles);

                await requestService.AddRequest(newRequest);

                return Ok();
            }
            finally
            {
                foreach (var stream in streams)
                    await stream.DisposeAsync();
            }
        }

        [HttpGet("my")]
        [Authorize(Roles = nameof(UserRole.Applicant))]
        public async Task<IActionResult> GetMyRequests(
    CancellationToken ct)
        {
            var result =
                await requestService.GetMyRequests(
                    CurrentUserId,
                    ct);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = nameof(UserRole.Applicant))]
        public async Task<IActionResult> Get(
    int id,
    CancellationToken ct)
        {
            return Ok(
                await requestService.GetMyRequest(
                    id,
                    CurrentUserId,
                    ct));
        }
        [HttpPut("{id:int}")]
        [Authorize(Roles = nameof(UserRole.Applicant))]
        public async Task<IActionResult> Update(
    int id,
    [FromForm] UpdateRequestModel model,
    CancellationToken ct)
        {
            var incomingFiles = new List<IncomingFile>();
            var streams = new List<Stream>();

            try
            {
                foreach (var file in model.Files ?? [])
                {
                    var stream = file.OpenReadStream();

                    streams.Add(stream);

                    incomingFiles.Add(
                        new IncomingFile(
                            stream,
                            Path.GetFileName(file.FileName),
                            file.ContentType,
                            file.Length));
                }

                var dto = new UpdateRequest(
                    model.Title,
                    model.Description,
                    model.Urgency,
                    model.RemoveDocumentIds ?? [],
                    incomingFiles);

                await requestService.UpdateRequest(
                    id,
                    CurrentUserId,
                    dto,
                    ct);

                return Ok();
            }
            finally
            {
                foreach (var stream in streams)
                    await stream.DisposeAsync();
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = nameof(UserRole.Applicant))]
        public async Task<IActionResult> Delete(
    int id,
    CancellationToken ct)
        {
            await requestService.DeleteRequest(
                id,
                CurrentUserId,
                ct);

            return NoContent();
        }
    }
}
