using Samad.Application.Dtos;
using Samad.Application.IServices;

namespace Samad.Application.Services
{
    public class CouncilReviewService() : ICouncilReviewService
    {
        public Task ChangeCouncilReview(ChangeCouncilReviewDto dto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCouncilReview(int councilReviewId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CouncilReviewDto>> GetAllCouncilReviewByCouncil(int councilId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CouncilReviewDto>> GetAllCouncilReviewByRequest(int requestId)
        {
            throw new NotImplementedException();
        }

        public Task SetCouncilReview(SetCouncilReviewDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
