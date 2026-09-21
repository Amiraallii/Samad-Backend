using System.ComponentModel.DataAnnotations;

namespace Samad.Domain.Enum
{
    public enum CouncilVote : byte
    {
        [Display(Name = "تایید")]
        Approved = 1,

        [Display(Name = "رد")]
        Rejected = 2,

        [Display(Name = "نیاز به بررسی بیشتر")]
        RequestRevision = 3
    }
}
