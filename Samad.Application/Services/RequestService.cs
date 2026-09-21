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
        IUnitOfWork unit,
        IFileUploadService uploadService,
        IFileStorage fileStorage,
        IRequestWorkflowService workflowService,
        AppSettings settings)
        : IRequestService
    {
        public async Task AddRequest(
            NewRequest dto)
        {
            if (dto.Documents is null || dto.Documents.Count == 0)
            {
                throw new InvalidOperationException(
                    "حداقل یک مدرک برای ثبت درخواست الزامی است.");
            }

            var createdAt = DateTime.UtcNow;

            var request = new Request
            {
                ApplicantId = dto.ApplicantId,
                Description = dto.Description,
                Title = dto.Title,
                Urgency = dto.Urgency,
                Status = RequestStatus.AwaitingSecretaryInitialReview,
                CreatedAt = createdAt,
                DeadlineAt = CalculateDeadline(
                    createdAt,
                    dto.Urgency)
            };

            foreach (var document in dto.Documents)
            {
                ValidateDocumentType(
                    document.DocumentType);

                var uploaded =
                    await uploadService.SaveAsync(
                        document.File);

                request.Documents.Add(
                    new RequestDocument
                    {
                        FileUrl = uploaded.Key,
                        ContentType = uploaded.ContentType,
                        DocumentType = document.DocumentType
                    });
            }

            await requestRepository.AddAsync(request);

            var history =
                new RequestStatusHistory
                {
                    Request = request,
                    FromStatus = null,
                    ToStatus =
                        RequestStatus.AwaitingSecretaryInitialReview,
                    ChangedByUserId = dto.ApplicantId,
                    ChangedAt = DateTime.UtcNow,
                    Comment =
                        "درخواست توسط درخواست‌دهنده ثبت شد."
                };

            await unit
                .SaveChangesAsync();
        }

        public async Task<List<RequestListDto>> GetMyRequests(
            int applicantId,
            CancellationToken ct = default)
        {
            var requests =
                await requestRepository
                    .Query()
                    .Where(x =>
                        x.ApplicantId == applicantId)
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => new
                    {
                        x.Id,
                        x.Title,
                        x.Description,
                        x.Urgency,
                        x.Status,
                        x.CreatedAt,
                        x.DeadlineAt
                    })
                    .ToListAsync(ct);

            return requests
                .Select(x =>
                {
                    var daysRemaining =
                        CalculateDaysRemaining(
                            x.DeadlineAt);

                    return new RequestListDto(
                        x.Id,
                        x.Title,
                        x.Description,
                        x.Urgency,
                        x.Status,
                        x.CreatedAt,
                        x.DeadlineAt,
                        daysRemaining,
                        IsOverdue(x.DeadlineAt));
                })
                .ToList();
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
            {
                throw new KeyNotFoundException(
                    "درخواست یافت نشد.");
            }

            var daysRemaining =
                CalculateDaysRemaining(
                    request.DeadlineAt);

            var isOverdue =
                IsOverdue(request.DeadlineAt);

            return new RequestDetailsDto(
                request.Id,
                request.Title,
                request.Description,
                request.Urgency,
                request.Status,
                request.CreatedAt,
                request.DeadlineAt,
                daysRemaining,
                isOverdue,
                request.Documents
                    .Select(x =>
                        new RequestDocumentDto(
                            x.Id,
                            $"{settings.S3Storage.BaseUrlWithBucket.TrimEnd('/')}/{x.FileUrl.TrimStart('/')}",
                            x.ContentType,
                            x.DocumentType))
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
            {
                throw new KeyNotFoundException(
                    "درخواست یافت نشد.");
            }

            if (request.Status is not
                RequestStatus.AwaitingSecretaryInitialReview and
                not RequestStatus.NeedsRevision)
            {
                throw new InvalidOperationException(
                    "این درخواست دیگر قابل ویرایش نیست.");
            }

            request.Title =
                dto.Title;

            request.Description =
                dto.Description;

            request.Urgency =
                dto.Urgency;

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

                documentRepository.Delete(
                    document);
            }

            foreach (var newDocument in dto.Documents)
            {
                ValidateDocumentType(
                    newDocument.DocumentType);

                var uploaded =
                    await uploadService.SaveAsync(
                        newDocument.File);

                request.Documents.Add(
                    new RequestDocument
                    {
                        FileUrl = uploaded.Key,
                        ContentType = uploaded.ContentType,
                        DocumentType =
                            newDocument.DocumentType
                    });
            }

            if (request.Status ==
                RequestStatus.NeedsRevision)
            {
                await workflowService.ChangeStatus(
                    request,
                    RequestStatus.AwaitingSecretaryInitialReview,
                    applicantId,
                    "درخواست توسط درخواست‌دهنده اصلاح و مجدداً ارسال شد.");
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
            {
                throw new KeyNotFoundException(
                    "درخواست یافت نشد.");
            }

            if (request.Status is not
                RequestStatus.AwaitingSecretaryInitialReview and
                not RequestStatus.NeedsRevision)
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

        private static void ValidateDocumentType(
            DocumentType documentType)
        {
            if (!Enum.IsDefined(
                    typeof(DocumentType),
                    documentType))
            {
                throw new ArgumentException(
                    "نوع مدرک معتبر نیست.",
                    nameof(documentType));
            }
        }

        private static bool IsOverdue(
            DateTime deadlineAt)
        {
            return deadlineAt < DateTime.UtcNow;
        }

        private static int CalculateDaysRemaining(
            DateTime deadlineAt)
        {
            var remaining =
                deadlineAt - DateTime.UtcNow;

            return (int)Math.Ceiling(
                remaining.TotalDays);
        }

        private DateTime CalculateDeadline(
            DateTime createdAt,
            UrgencyLevel urgency)
        {
            var weeks = urgency switch
            {
                UrgencyLevel.Normal =>
                    settings.Deadline.NormalWeeks,

                UrgencyLevel.Urgent =>
                    settings.Deadline.UrgentWeeks,

                UrgencyLevel.VeryUrgent =>
                    settings.Deadline.VeryUrgentWeeks,

                _ => throw new ArgumentOutOfRangeException(
                    nameof(urgency),
                    urgency,
                    "سطح فوریت معتبر نیست.")
            };

            if (weeks <= 0)
            {
                throw new InvalidOperationException(
                    "مدت زمان Deadline باید بیشتر از صفر باشد.");
            }

            return createdAt.AddDays(
                weeks * 7);
        }
    }
}