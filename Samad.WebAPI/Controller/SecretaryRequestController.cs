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
        [HttpGet("initial-review")]
        public async Task<IActionResult> GetInitialReviewRequests(
            CancellationToken cancellationToken)
        {
            var result =
                await secretaryRequestService
                    .GetInitialReviewRequests(
                        cancellationToken);

            return Ok(result);
        }

        [HttpGet("final-review")]
        public async Task<IActionResult> GetFinalReviewRequests(
            CancellationToken cancellationToken)
        {
            var result =
                await secretaryRequestService
                    .GetFinalReviewRequests(
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
        [HttpGet("{requestId:int}/report")]
        public async Task<IActionResult> GetReport(
    int requestId,
    CancellationToken cancellationToken)
        {
            var result =
                await secretaryRequestService.GetReport(
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
                    "تصمیم دبیر با موفقیت ثبت شد."
            });
        }
    }
}