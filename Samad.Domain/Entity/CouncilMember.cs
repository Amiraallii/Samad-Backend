using Samad.Domain.Enum;

namespace Samad.Domain.Entity
{
    public class CouncilMember : IEntity<int>
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public CouncilMemberType Type { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
    }
}