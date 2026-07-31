using System.ComponentModel.DataAnnotations;

namespace Samad.Domain.Enum
{
    public enum DocumentType : byte
    {
        [Display(Name = " استشهاد نامه")]
        Affidavit = 1,
        [Display(Name = " سند رسمی")]
        OfficialDeed = 2,
        [Display(Name = " سند دادگاه")]
        CourtDocument = 3,
        [Display(Name = " سایر")]
        Other = 4
    }
}
