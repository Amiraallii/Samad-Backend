using System.ComponentModel.DataAnnotations;

namespace Samad.Domain.Enum
{
    public enum CouncilVote : byte
    {
        [Display(Name = "تایید")]
        Approved = 1,
        [Display(Name = "تایید")]
        Rejected = 2,
        [Display(Name = "نیاز به بررسی بیشتر")]
        NeedsRevision = 3
    }
}
