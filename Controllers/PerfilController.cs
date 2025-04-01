using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AlquimiaEsencial.Models;

namespace AlquimiaEsencial.Controllers
{
    [Authorize]
    public class PerfilController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public PerfilController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
                return NotFound();

            return View(usuario);
        }
    }
}
