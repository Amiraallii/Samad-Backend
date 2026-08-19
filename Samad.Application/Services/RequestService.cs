using Microsoft.EntityFrameworkCore;
using Samad.Application.Dtos;
using Samad.Application.Files;
using Samad.Application.IServices;
using Samad.Domain.Entity;
using Samad.Domain.Enum;
using Samad.Infrastructure.IRepositories;

namespace Samad.Application.Services
{
    public class RequestService(
    IRepository<Request, int> requestRepository,
    IRepository<RequestDocument, int> documentRepository,
    IRepository<RequestCouncilAssignment, int> assignmentRepository,
    IRepository<User, int> userRepository,
    IUnitOfWork unit,
    IFileUploadService uploadService,
    IFileStorage fileStorage,
    AppSettings settings)
    : IRequestService
    {
        public async Task AddRequest(
            NewRequest dto)
        {
            var councilMemberIds =
                await userRepository
                    .Query()
                    .Where(x =>
                        x.RoleId == (int)UserRole.CouncilMember)
                    .Select(x => x.Id)
                    .ToListAsync();

            if (councilMemberIds.Count == 0)
                throw new InvalidOperationException(
                    "هیچ عضو شورایی تعریف نشده است.");

            var request = new Request
            {
                ApplicantId = dto.ApplicantId,
                Description = dto.Description,
                Title = dto.Title,
                Urgency = dto.Urgency,
                Status = RequestStatus.UnderCouncilReview,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var file in dto.files)
            {
                var uploaded =
                    await uploadService.SaveAsync(file);

                request.Documents.Add(
                    new RequestDocument
                    {
                        FileUrl = uploaded.Key,
                        ContentType = uploaded.ContentType
                    });
            }

            foreach (var memberId in councilMemberIds)
            {
                request.CouncilAssignments.Add(
                    new RequestCouncilAssignment
                    {
                        CouncilMemberId = memberId
                    });
            }

            await requestRepository.AddAsync(request);

            await unit.SaveChangesAsync();
        }

        public async Task<List<RequestListDto>> GetMyRequests(
    int applicantId,
    CancellationToken ct = default)
        {
            return await requestRepository
                .Query()
                .Where(x => x.ApplicantId == applicantId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new RequestListDto(
                    x.Id,
                    x.Title,
                    x.Description,
                    x.Urgency,
                    x.Status,
                    x.CreatedAt))
                .ToListAsync(ct);
        }

        public async Task<RequestDetailsDto> GetMyRequest(
    int requestId,
    int applicantId,
    CancellationToken ct = default)
        {
            var request =
                await requestRepository
                    .Query()
                    .Include(x => x.Documents)
                    .SingleOrDefaultAsync(
                        x =>
                            x.Id == requestId &&
                            x.ApplicantId == applicantId,
                        ct);

            if (request is null)
                throw new KeyNotFoundException(
                    "درخواست یافت نشد.");

            return new RequestDetailsDto(
                request.Id,
                request.Title,
                request.Description,
                request.Urgency,
                request.Status,
                request.CreatedAt,

                request.Documents
                    .Select(x => new RequestDocumentDto(
                        x.Id,
                        $"{settings.S3Storage.BaseUrlWithBucket.TrimEnd('/')}/{x.FileUrl.TrimStart('/')}",
                        x.ContentType))
                    .ToList());
        }
        public async Task UpdateRequest(
    int requestId,
    int applicantId,
    UpdateRequest dto,
    CancellationToken ct = default)
        {
            var request =
                await requestRepository
                    .QueryTracking()
                    .Include(x => x.Documents)
                    .SingleOrDefaultAsync(
                        x =>
                            x.Id == requestId &&
                            x.ApplicantId == applicantId,
                        ct);

            if (request is null)
                throw new KeyNotFoundException(
                    "درخواست یافت نشد.");

            if (request.Status is not
                RequestStatus.UnderCouncilReview and not
                RequestStatus.NeedsRevision)
            {
                throw new InvalidOperationException(
                    "این درخواست دیگر قابل ویرایش نیست.");
            }

            request.Title = dto.Title;
            request.Description = dto.Description;
            request.Urgency = dto.Urgency;

            var documentsToDelete =
                request.Documents
                    .Where(x =>
                        dto.RemoveDocumentIds.Contains(x.Id))
                    .ToList();

            foreach (var document in documentsToDelete)
            {
                await fileStorage.DeleteAsync(
                    document.FileUrl,
                    ct);

                documentRepository.Delete(document);
            }

            foreach (var newFile in dto.Files)
            {
                var uploaded =
                    await uploadService.SaveAsync(newFile);

                request.Documents.Add(
                    new RequestDocument
                    {
                        FileUrl = uploaded.Key,
                        ContentType = uploaded.ContentType
                    });
            }

            await unit.SaveChangesAsync();
        }
        public async Task DeleteRequest(
    int requestId,
    int applicantId,
    CancellationToken ct = default)
        {
            var request =
                await requestRepository
                    .QueryTracking()
                    .Include(x => x.Documents)
                    .SingleOrDefaultAsync(
                        x =>
                            x.Id == requestId &&
                            x.ApplicantId == applicantId,
                        ct);

            if (request is null)
                throw new KeyNotFoundException(
                    "درخواست یافت نشد.");

            if (request.Status is not
                RequestStatus.UnderCouncilReview and not
                RequestStatus.NeedsRevision)
            {
                throw new InvalidOperationException(
                    "این درخواست دیگر قابل حذف نیست.");
            }

            foreach (var document in request.Documents)
            {
                await fileStorage.DeleteAsync(
                    document.FileUrl,
                    ct);
            }

            requestRepository.Delete(request);

            await unit.SaveChangesAsync();
        }
    }

    
    }
