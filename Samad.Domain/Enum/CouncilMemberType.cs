using System.ComponentModel.DataAnnotations;

namespace Samad.Domain.Enum
{
    public enum CouncilMemberType : byte
    {
        [Display(Name = "عضو بررسی‌کننده")]
        Reviewer = 1,

        [Display(Name = "عضو اصلی شورا")]
        MainMember = 2
    }
}
