using Microsoft.AspNetCore.Mvc;
using Samad.Domain.Entity;

namespace Samad.WebAPI.Controller
{
    public class SamadController : ControllerBase
    {
        protected int CurrentUserId
        {
            get
            {
                var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdString, out var userId))
                    return userId;

                return new();
            }
        }
    }
}
