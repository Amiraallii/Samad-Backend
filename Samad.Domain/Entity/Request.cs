using Samad.Domain.Enum;

namespace Samad.Domain.Entity
{
    public class Request : IEntity<int>
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public DateTime DeadlineAt { get; set; }
        public UrgencyLevel Urgency { get; set; }

        public RequestStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int ApplicantId { get; set; }

        #region Relations

        public User Applicant { get; set; } = null!;

        public ICollection<RequestDocument> Documents { get; set; }
            = new List<RequestDocument>();

        public ICollection<CouncilReview> CouncilReviews { get; set; }
            = new List<CouncilReview>();

        public ICollection<RequestCouncilAssignment> CouncilAssignments { get; set; }
            = new List<RequestCouncilAssignment>();

        public ICollection<CouncilSignature> CouncilSignatures { get; set; }
            = new List<CouncilSignature>();

        public ICollection<RequestStatusHistory> StatusHistory { get; set; }
            = new List<RequestStatusHistory>();

        public ICollection<SecretaryDecision> SecretaryDecisions { get; set; }
            = new List<SecretaryDecision>();

        #endregion
    }
}