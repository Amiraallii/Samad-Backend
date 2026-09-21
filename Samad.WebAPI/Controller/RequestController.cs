using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Samad.Application.Dtos;
using Samad.Application.IServices;
using Samad.Domain.Entity;
using Samad.Domain.Enum;

namespace Samad.WebAPI.Controller
{
    [ApiController]
    [Route("api/requests")]
    public sealed class RequestController(
        IRequestService requestService)
        : SamadController
    {
        [HttpPost]
        [Authorize(Roles = nameof(UserRole.Applicant))]
        public async Task<IActionResult> NewRequest(
            [FromForm] NewRequestModel request,
            CancellationToken cancellationToken)
        {
            var files = request.files ?? [];
            var documentTypes = request.documentTypes ?? [];

            if (files.Count == 0)
            {
                return BadRequest(
                    "حداقل یک فایل برای ثبت درخواست الزامی است.");
            }

            if (files.Count != documentTypes.Count)
            {
                return BadRequest(
                    "تعداد فایل‌ها و نوع مدارک باید برابر باشد.");
            }

            var incomingDocuments = new List<IncomingDocument>();
            var streams = new List<Stream>();

            try
            {
                for (var i = 0; i < files.Count; i++)
                {
                    var file = files[i];

                    if (file is null || file.Length == 0)
                    {
                        return BadRequest(
                            "یکی از فایل‌های ارسال‌شده خالی است.");
                    }

                    var documentType = documentTypes[i];

                    if (!Enum.IsDefined(
                            typeof(DocumentType),
                            documentType))
                    {
                        return BadRequest(
                            $"نوع مدرک برای فایل شماره {i + 1} معتبر نیست.");
                    }

                    var stream = file.OpenReadStream();
                    streams.Add(stream);

                    var incomingFile = new IncomingFile(
                        Stream: stream,
                        OriginalFileName: Path.GetFileName(file.FileName),
                        ClientContentType: file.ContentType,
                        Length: file.Length);

                    incomingDocuments.Add(
                        new IncomingDocument(
                            incomingFile,
                            documentType));
                }

                var newRequest = new NewRequest(
                    request.Title,
                    request.Description,
                    request.Urgency,
                    CurrentUserId,
                    incomingDocuments);

                await requestService.AddRequest(newRequest);

                return Ok();
            }
            finally
            {
                foreach (var stream in streams)
                {
                    await stream.DisposeAsync();
                }
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
            var result =
                await requestService.GetMyRequest(
                    id,
                    CurrentUserId,
                    ct);

            return Ok(result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = nameof(UserRole.Applicant))]
        public async Task<IActionResult> Update(
            int id,
            [FromForm] UpdateRequestModel model,
            CancellationToken ct)
        {
            var files = model.Files ?? [];
            var documentTypes = model.DocumentTypes ?? [];

            if (files.Count != documentTypes.Count)
            {
                return BadRequest(
                    "تعداد فایل‌ها و نوع مدارک باید برابر باشد.");
            }

            var incomingDocuments = new List<IncomingDocument>();
            var streams = new List<Stream>();

            try
            {
                for (var i = 0; i < files.Count; i++)
                {
                    var file = files[i];

                    if (file is null || file.Length == 0)
                    {
                        return BadRequest(
                            "یکی از فایل‌های ارسال‌شده خالی است.");
                    }

                    var documentType = documentTypes[i];

                    if (!Enum.IsDefined(
                            typeof(DocumentType),
                            documentType))
                    {
                        return BadRequest(
                            $"نوع مدرک برای فایل شماره {i + 1} معتبر نیست.");
                    }

                    var stream = file.OpenReadStream();
                    streams.Add(stream);

                    var incomingFile = new IncomingFile(
                        Stream: stream,
                        OriginalFileName: Path.GetFileName(file.FileName),
                        ClientContentType: file.ContentType,
                        Length: file.Length);

                    incomingDocuments.Add(
                        new IncomingDocument(
                            incomingFile,
                            documentType));
                }

                var dto = new UpdateRequest(
                    model.Title,
                    model.Description,
                    model.Urgency,
                    model.RemoveDocumentIds ?? [],
                    incomingDocuments);

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
                {
                    await stream.DisposeAsync();
                }
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