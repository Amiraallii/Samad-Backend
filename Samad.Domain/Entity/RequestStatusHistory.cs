using Samad.Domain.Enum;

namespace Samad.Domain.Entity
{
    public class RequestStatusHistory : IEntity<int>
    {
        public int Id { get; set; }

        public int RequestId { get; set; }

        public RequestStatus? FromStatus { get; set; }

        public RequestStatus ToStatus { get; set; }

        public int? ChangedByUserId { get; set; }

        public DateTime ChangedAt { get; set; }

        public string? Comment { get; set; }

        #region Relations

        public Request Request { get; set; } = null!;

        public User? ChangedByUser { get; set; }

        #endregion
    }
}