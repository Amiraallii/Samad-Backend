using Samad.Domain.Enum;

namespace Samad.WebAPI
{
    public record NewRequestModel(
        string Title,
        string Description,
        UrgencyLevel Urgency,
        List<IFormFile>? files,
        List<DocumentType>? documentTypes);

    public record UpdateRequestModel(
        string Title,
        string Description,
        UrgencyLevel Urgency,
        List<int>? RemoveDocumentIds,
        List<IFormFile>? Files,
        List<DocumentType>? DocumentTypes);
}