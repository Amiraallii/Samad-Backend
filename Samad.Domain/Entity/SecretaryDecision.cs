using Samad.Domain.Enum;

namespace Samad.Domain.Entity
{


    public class SecretaryDecision : IEntity<int>
    {
        public int Id { get; set; }

        public int RequestId { get; set; }

        public int SecretaryId { get; set; }

        public SecretaryDecisionStage Stage { get; set; }

        public SecretaryDecisionType Decision { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        #region Relations

        public Request Request { get; set; } = null!;

        public User Secretary { get; set; } = null!;

        #endregion
    }
}
