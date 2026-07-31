using System.ComponentModel.DataAnnotations;

namespace Samad.Domain.Enum
{
    public enum RequestStatus : byte
    {
        [Display(Name = "در انتظار بررسی")]
        Pending = 1,
        [Display(Name = "نقص مدارک")]
        NeedsRevision = 2,
        [Display(Name = "در حال بررسی")]
        UnderReview = 3,
        [Display(Name = "تایید نهایی دبیر")]
        Approved = 4,
        [Display(Name = "رد نهایی دبیر")]
        Rejected = 5
    }
}
