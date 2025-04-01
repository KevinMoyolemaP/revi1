using AlquimiaEsencial.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlquimiaEsencial.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var totalProductos = await _context.Productos.CountAsync();
            var totalUsuarios = await _userManager.Users.CountAsync();
            var totalVentas = await _context.Ventas.CountAsync();
            var productosBajoStock = await _context.Productos
                .Where(p => p.Stock <= 5)
                .ToListAsync();

            ViewBag.TotalProductos = totalProductos;
            ViewBag.TotalUsuarios = totalUsuarios;
            ViewBag.TotalVentas = totalVentas;
            ViewBag.ProductosBajoStock = productosBajoStock;

            return View();
            var totalCategorias = await _context.Categorias.CountAsync();
            ViewBag.TotalCategorias = totalCategorias;

        }
    }
}
