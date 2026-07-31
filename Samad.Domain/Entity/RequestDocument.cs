using Samad.Domain.Enum;

namespace Samad.Domain.Entity
{
    public class RequestDocument
    {
        public int Id { get; set; }
        public DocumentType Type { get; set; }
        public string FileUrl { get; set; } 

        public int RequestId { get; set; }
        #region ' Relations '
        public Request Request { get; set; }
        #endregion ' Relations '
    }
}
