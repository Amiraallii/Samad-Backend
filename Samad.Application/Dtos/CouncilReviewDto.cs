using Samad.Domain.Enum;

namespace Samad.Application.Dtos
{
    public record SetCouncilReviewDto(CouncilVote Vote, string Comment, int RequestId, int CouncilMemberId);
    public record ChangeCouncilReviewDto(int Id, CouncilVote Vote, string Comment, int RequestId, int CouncilMemberId);
    public class CouncilReviewDto
    {
        public int Id { get; set; }
        public CouncilVote Vote { get; set; }
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; } = DateTime.Now;

        public int RequestId { get; set; }
        public string RequestTitle { get; set; }

        public int CouncilMemberId { get; set; }
        public string CouncilMemberTitle { get; set; }
    }

}
