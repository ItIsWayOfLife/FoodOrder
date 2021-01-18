using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    public class ControllerBaseGetUserId : ControllerBase
    {
        protected string GetCurrentUserId()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    return User.FindFirst(ClaimTypes.NameIdentifier).Value;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
