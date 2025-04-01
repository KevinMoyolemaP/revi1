using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlquimiaEsencial.Data;
using AlquimiaEsencial.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AlquimiaEsencial.Controllers
{
    [Authorize]
    public class VentasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VentasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Ventas
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Ventas.Include(v => v.Usuario);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Ventas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var venta = await _context.Ventas
                .Include(v => v.Usuario)
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (venta == null)
                return NotFound();

            return View(venta);
        }

        // GET: Ventas/Create
        public IActionResult Create()
        {
            ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Ventas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Fecha,UsuarioId")] Venta venta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(venta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Id", venta.UsuarioId);
            return View(venta);
        }

        // GET: Ventas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var venta = await _context.Ventas.FindAsync(id);
            if (venta == null)
                return NotFound();

            ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Id", venta.UsuarioId);
            return View(venta);
        }

        // POST: Ventas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Fecha,UsuarioId")] Venta venta)
        {
            if (id != venta.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VentaExists(venta.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Id", venta.UsuarioId);
            return View(venta);
        }

        // GET: Ventas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var venta = await _context.Ventas
                .Include(v => v.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (venta == null)
                return NotFound();

            return View(venta);
        }

        // POST: Ventas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venta = await _context.Ventas.FindAsync(id);
            if (venta != null)
            {
                _context.Ventas.Remove(venta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VentaExists(int id)
        {
            return _context.Ventas.Any(e => e.Id == id);
        }

        // NUEVO: Mostrar productos para armar el pedido
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CrearPedido()
        {
            var productos = await _context.Productos.ToListAsync();
            return View(productos);
        }

        // NUEVO: Procesar venta
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarVenta(Dictionary<int, int> cantidades)
        {
            var userId = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name)?.Id;

            var venta = new Venta
            {
                UsuarioId = userId,
                Fecha = DateTime.Now,
                Detalles = new List<DetalleVenta>()
            };

            decimal total = 0;

            foreach (var item in cantidades)
            {
                int productoId = item.Key;
                int cantidad = item.Value;

                if (cantidad > 0)
                {
                    var producto = await _context.Productos.FindAsync(productoId);
                    if (producto != null && producto.Stock >= cantidad)
                    {
                        var subtotal = producto.Precio * cantidad;

                        venta.Detalles.Add(new DetalleVenta
                        {
                            ProductoId = productoId,
                            Cantidad = cantidad,
                            Subtotal = subtotal
                        });

                        producto.Stock -= cantidad;
                        total += subtotal;
                    }
                }
            }

            if (!venta.Detalles.Any())
            {
                TempData["Error"] = "Debes seleccionar al menos un producto válido.";
                return RedirectToAction(nameof(CrearPedido));
            }

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            TempData["Exito"] = "Venta realizada con éxito.";
            return RedirectToAction(nameof(Details), new { id = venta.Id });
        }
    }
}
