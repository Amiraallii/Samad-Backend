using System.ComponentModel.DataAnnotations;

namespace Samad.Domain.Enum
{
    public enum RequestStatus : byte
    {
        [Display(Name = "در انتظار بررسی اولیه دبیر")]
        AwaitingSecretaryInitialReview = 1,

        [Display(Name = "نیاز به اصلاح توسط درخواست‌دهنده")]
        NeedsRevision = 2,

        [Display(Name = "در حال بررسی توسط اعضای شورا")]
        UnderCouncilReview = 3,

        [Display(Name = "در انتظار بررسی نهایی دبیر")]
        AwaitingSecretaryFinalReview = 4,

        [Display(Name = "در انتظار امضای اعضای اصلی شورا")]
        AwaitingMainCouncilApproval = 5,

        [Display(Name = "تایید نهایی")]
        Approved = 6,

        [Display(Name = "رد شده")]
        Rejected = 7
    }
}
