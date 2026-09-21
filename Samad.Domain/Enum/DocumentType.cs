using System.ComponentModel.DataAnnotations;

namespace Samad.Domain.Enum
{
    public enum DocumentType : byte
    {
        [Display(Name = "مقاله")]
        Article = 1,
        [Display(Name = "پایان‌نامه")]
        Thesis = 2,
        [Display(Name = "پروپوزال")]
        Proposal = 3,
        [Display(Name = "طرح پژوهشی")]
        ResearchPlan = 4,
        [Display(Name = "گزارش پژوهشی")]
        ResearchReport = 5,
        [Display(Name = "کتاب")]
        Book = 6,
        [Display(Name = "سایر")]
        Other = 7,
    }
}
