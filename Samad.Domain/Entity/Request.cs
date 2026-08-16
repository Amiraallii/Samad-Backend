using Samad.Domain.Enum;

namespace Samad.Domain.Entity
{
    public class Request : IEntity<int>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public UrgencyLevel Urgency { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int ApplicantId { get; set; }

        public int? SecretaryId { get; set; }
        public string? FinalSecretaryComment { get; set; }
        public DateTime? FinalDecisionDate { get; set; }
        #region ' Relations '
        public User Applicant { get; set; }
        public User Secretary { get; set; }
        public ICollection<RequestDocument> Documents { get; set; }
        public ICollection<CouncilReview> CouncilReviews { get; set; }
        #endregion ' Relations '

    }
}
