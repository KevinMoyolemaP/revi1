using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Rotativa.AspNetCore;
using AlquimiaEsencial.Data;
using AlquimiaEsencial.Models;

namespace AlquimiaEsencial.Controllers
{
    public class PedidosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PedidosController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Admin()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Usuario)
                .Include(p => p.Producto)
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();

            return View(pedidos);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var pedido = await _context.Pedidos
                .Include(p => p.Usuario)
                .Include(p => p.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);

            return pedido == null ? NotFound() : View(pedido);
        }

        public IActionResult Create(int? productoId)
        {
            if (productoId.HasValue)
            {
                var producto = _context.Productos.FirstOrDefault(p => p.Id == productoId.Value);
                if (producto != null)
                {
                    ViewBag.ProductoNombre = producto.Nombre;
                    ViewBag.ProductoId = producto.Id;
                }
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NombreCliente,Telefono,Observaciones,ProductoId")] Pedido pedido)
        {
            if (User.Identity.IsAuthenticated)
            {
                pedido.UsuarioId = _userManager.GetUserId(User);
                pedido.NombreCliente = User.Identity.Name;
                if (string.IsNullOrWhiteSpace(pedido.Telefono))
                {
                    pedido.Telefono = "Sin número";
                }
            }

            if (!ModelState.IsValid)
            {
                return View(pedido);
            }

            _context.Add(pedido);
            await _context.SaveChangesAsync();
            TempData["Exito"] = "? Tu pedido ha sido enviado correctamente.";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Aprobar(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return NotFound();

            pedido.Aprobado = true;
            await _context.SaveChangesAsync();
            TempData["Exito"] = "? Pedido aprobado con éxito.";
            return RedirectToAction(nameof(Admin));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Rechazar(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return NotFound();

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();
            TempData["Error"] = "? Pedido rechazado.";
            return RedirectToAction(nameof(Admin));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Comprobante(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Producto)
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null) return NotFound();

            return new ViewAsPdf("Comprobante", pedido)
            {
                FileName = $"Pedido_{pedido.Id}.pdf"
            };
        }
    }
}
