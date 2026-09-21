using System.ComponentModel.DataAnnotations;

namespace Samad.Domain.Enum
{
    public enum SecretaryDecisionStage : byte
    {
        [Display(Name = "بررسی اولیه")]
        InitialReview = 1,

        [Display(Name = "بررسی نهایی")]
        FinalReview = 2
    }
}
