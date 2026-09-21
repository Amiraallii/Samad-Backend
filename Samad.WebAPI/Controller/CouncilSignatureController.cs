using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Samad.Application.Dtos;
using Samad.Application.IServices;
using Samad.Domain.Entity;
using Samad.Domain.Enum;

namespace Samad.WebAPI.Controller
{
    [ApiController]
    [Route("api/council/signatures")]
    [Authorize(Roles = nameof(UserRole.CouncilMember))]
    public sealed class CouncilSignatureController(
        ICouncilSignatureService councilSignatureService)
        : SamadController
    {
        [HttpGet("my")]
        public async Task<IActionResult> GetMyRequests(
            CancellationToken cancellationToken)
        {
            var result =
                await councilSignatureService.GetAssignedRequests(
                    CurrentUserId,
                    cancellationToken);

            return Ok(result);
        }

        [HttpGet("{requestId:int}")]
        public async Task<IActionResult> GetRequest(
            int requestId,
            CancellationToken cancellationToken)
        {
            var result =
                await councilSignatureService.GetRequest(
                    requestId,
                    CurrentUserId,
                    cancellationToken);

            return Ok(result);
        }

        [HttpPost("{requestId:int}/sign")]
        public async Task<IActionResult> Sign(
            int requestId,
            [FromBody] CouncilSignatureRequestDto dto,
            CancellationToken cancellationToken)
        {
            await councilSignatureService.Sign(
                requestId,
                CurrentUserId,
                dto,
                cancellationToken);

            return Ok(new
            {
                Message = "امضای شما با موفقیت ثبت شد."
            });
        }
    }
}