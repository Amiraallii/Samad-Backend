using System.ComponentModel.DataAnnotations;

namespace Samad.Domain.Enum
{
    public enum UrgencyLevel : byte
    {
        [Display(Name = "عادی")]
        Normal = 1,      
        [Display(Name = "فوری")]
        Urgent = 2,
        [Display(Name = "خیلی فوری")]
        VeryUrgent = 3
    }
}
