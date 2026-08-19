namespace Samad.Domain.Entity
{
    public class RequestCouncilAssignment : IEntity<int>
    {
        public int Id { get; set; }

        public int RequestId { get; set; }

        public int CouncilMemberId { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        public Request Request { get; set; } = null!;

        public User CouncilMember { get; set; } = null!;
    }
}
