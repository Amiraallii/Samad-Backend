using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Samad.Application.Dtos;
using Samad.Application.IServices;
using Samad.Domain.Entity;

namespace Samad.WebAPI.Controller
{
    [ApiController]
    [Route("api/council/requests")]
    [Authorize(Roles = nameof(UserRole.CouncilMember))]
    public sealed class CouncilRequestController(
        ICouncilReviewService councilReviewService)
        : SamadController
    {
        [HttpGet]
        public async Task<IActionResult> GetRequests(
            CancellationToken cancellationToken)
        {
            var result =
                await councilReviewService
                    .GetAssignedRequests(
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
                await councilReviewService
                    .GetRequest(
                        requestId,
                        CurrentUserId,
                        cancellationToken);

            return Ok(result);
        }

        [HttpPost("{requestId:int}/review")]
        public async Task<IActionResult> SubmitReview(
            int requestId,
            [FromBody] SubmitCouncilReviewDto dto,
            CancellationToken cancellationToken)
        {
            await councilReviewService
                .SubmitReview(
                    requestId,
                    CurrentUserId,
                    dto,
                    cancellationToken);

            return Ok(new
            {
                Message = "نظر شما با موفقیت ثبت شد."
            });
        }
    }
}
