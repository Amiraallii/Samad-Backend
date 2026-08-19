using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Samad.Application.Dtos;
using Samad.Application.IServices;
using Samad.Domain.Entity;

namespace Samad.WebAPI.Controller
{
    [ApiController]
    [Route("api/secretary/requests")]
    [Authorize(Roles = nameof(UserRole.Secretary))]
    public sealed class SecretaryRequestController(
        ISecretaryRequestService secretaryRequestService)
        : SamadController
    {
        [HttpGet]
        public async Task<IActionResult> GetRequests(
            CancellationToken cancellationToken)
        {
            var result =
                await secretaryRequestService
                    .GetPendingRequests(
                        cancellationToken);

            return Ok(result);
        }

        [HttpGet("{requestId:int}")]
        public async Task<IActionResult> GetRequest(
            int requestId,
            CancellationToken cancellationToken)
        {
            var result =
                await secretaryRequestService
                    .GetRequest(
                        requestId,
                        cancellationToken);

            return Ok(result);
        }

        [HttpPost("{requestId:int}/decision")]
        public async Task<IActionResult> SubmitDecision(
            int requestId,
            [FromBody] SecretaryDecisionDto dto,
            CancellationToken cancellationToken)
        {
            await secretaryRequestService
                .SubmitDecision(
                    requestId,
                    CurrentUserId,
                    dto,
                    cancellationToken);

            return Ok(new
            {
                Message =
                    "تصمیم نهایی با موفقیت ثبت شد."
            });
        }
    }
}