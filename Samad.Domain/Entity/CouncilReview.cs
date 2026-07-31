using Samad.Domain.Enum;

namespace Samad.Domain.Entity
{
    public class CouncilReview
    {
        public int Id { get; set; }
        public CouncilVote Vote { get; set; } 
        public string Comment { get; set; } 
        public DateTime ReviewDate { get; set; } = DateTime.Now;

        public int RequestId { get; set; }

        public int CouncilMemberId { get; set; }

        #region ' Relations '
        public User CouncilMember { get; set; }
        public Request Request { get; set; }
        #endregion ' Relations '
    }
}
