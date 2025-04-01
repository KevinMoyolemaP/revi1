using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AlquimiaEsencial.Data;
using AlquimiaEsencial.Models;

namespace AlquimiaEsencial.Controllers
{
    public class DetallesPedidoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DetallesPedidoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DetallesPedido
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.DetallesPedido.Include(d => d.Pedido).Include(d => d.Producto);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: DetallesPedido/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallePedido = await _context.DetallesPedido
                .Include(d => d.Pedido)
                .Include(d => d.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (detallePedido == null)
            {
                return NotFound();
            }

            return View(detallePedido);
        }

        // GET: DetallesPedido/Create
        public IActionResult Create()
        {
            ViewData["PedidoId"] = new SelectList(_context.Pedidos, "Id", "NombreCliente");
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre");
            return View();
        }

        // POST: DetallesPedido/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PedidoId,ProductoId,Cantidad")] DetallePedido detallePedido)
        {
            if (ModelState.IsValid)
            {
                _context.Add(detallePedido);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PedidoId"] = new SelectList(_context.Pedidos, "Id", "NombreCliente", detallePedido.PedidoId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre", detallePedido.ProductoId);
            return View(detallePedido);
        }

        // GET: DetallesPedido/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallePedido = await _context.DetallesPedido.FindAsync(id);
            if (detallePedido == null)
            {
                return NotFound();
            }
            ViewData["PedidoId"] = new SelectList(_context.Pedidos, "Id", "NombreCliente", detallePedido.PedidoId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre", detallePedido.ProductoId);
            return View(detallePedido);
        }

        // POST: DetallesPedido/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PedidoId,ProductoId,Cantidad")] DetallePedido detallePedido)
        {
            if (id != detallePedido.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(detallePedido);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DetallePedidoExists(detallePedido.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PedidoId"] = new SelectList(_context.Pedidos, "Id", "NombreCliente", detallePedido.PedidoId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre", detallePedido.ProductoId);
            return View(detallePedido);
        }

        // GET: DetallesPedido/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallePedido = await _context.DetallesPedido
                .Include(d => d.Pedido)
                .Include(d => d.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (detallePedido == null)
            {
                return NotFound();
            }

            return View(detallePedido);
        }

        // POST: DetallesPedido/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var detallePedido = await _context.DetallesPedido.FindAsync(id);
            if (detallePedido != null)
            {
                _context.DetallesPedido.Remove(detallePedido);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DetallePedidoExists(int id)
        {
            return _context.DetallesPedido.Any(e => e.Id == id);
        }
    }
}
