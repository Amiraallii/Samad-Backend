using Samad.Application.Dtos;

namespace Samad.Application.IServices
{
    public interface ICouncilReviewService
    {
        Task SetCouncilReview(SetCouncilReviewDto dto);
        Task ChangeCouncilReview(ChangeCouncilReviewDto dto);
        Task<IEnumerable<CouncilReviewDto>> GetAllCouncilReviewByRequest(int requestId);
        Task<IEnumerable<CouncilReviewDto>> GetAllCouncilReviewByCouncil(int councilId);
        Task DeleteCouncilReview(int councilReviewId);
    }
}
