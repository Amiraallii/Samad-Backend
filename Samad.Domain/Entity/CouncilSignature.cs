namespace Samad.Domain.Entity
{
    public class CouncilSignature : IEntity<int>
    {
        public int Id { get; set; }

        public int RequestId { get; set; }

        public int CouncilMemberId { get; set; }

        public DateTime? SignedAt { get; set; }

        public bool IsSigned { get; set; }

        public Request Request { get; set; } = null!;

        public User CouncilMember { get; set; } = null!;
    }
}
