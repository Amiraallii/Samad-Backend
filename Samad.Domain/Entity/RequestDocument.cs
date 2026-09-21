using Samad.Domain.Enum;

namespace Samad.Domain.Entity
{
    public class RequestDocument : IEntity<int>
    {
        public int Id { get; set; }
        public string ContentType { get; set; }
        public string FileUrl { get; set; }
        public DocumentType DocumentType { get; set; }
        public int RequestId { get; set; }
        #region ' Relations '
        public Request Request { get; set; }
        #endregion ' Relations '
    }
}
