using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AlquimiaEsencial.Data;
using AlquimiaEsencial.Models;
using Microsoft.AspNetCore.Authorization;

namespace AlquimiaEsencial.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Todos pueden ver productos
        public async Task<IActionResult> Index(int? categoriaId)
        {
            var categorias = await _context.Categorias.ToListAsync();
            ViewBag.Categorias = new SelectList(categorias, "Id", "Nombre");
            ViewBag.CategoriaSeleccionada = categoriaId;

            var productos = _context.Productos.Include(p => p.Categoria).AsQueryable();

            if (categoriaId.HasValue && categoriaId.Value != 0)
            {
                productos = productos.Where(p => p.CategoriaId == categoriaId.Value);
            }

            return View(await productos.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
                return NotFound();

            return View(producto);
        }

        // Solo Admins
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Categorias = new SelectList(_context.Categorias, "Id", "Nombre");
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto producto)
        {
            if (ModelState.IsValid)
            {
                if (producto.ImagenArchivo != null)
                {
                    var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                    var extension = Path.GetExtension(producto.ImagenArchivo.FileName).ToLowerInvariant();

                    if (!extensionesPermitidas.Contains(extension))
                    {
                        ModelState.AddModelError("ImagenArchivo", "Formato no permitido.");
                        ViewBag.Categorias = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaId);
                        return View(producto);
                    }

                    var nombreArchivo = Path.GetFileName(producto.ImagenArchivo.FileName);
                    var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/productos", nombreArchivo);

                    using (var stream = new FileStream(ruta, FileMode.Create))
                    {
                        await producto.ImagenArchivo.CopyToAsync(stream);
                    }

                    producto.ImagenUrl = "/img/productos/" + nombreArchivo;
                }

                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categorias = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        // Solo Admins
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return NotFound();

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion,Precio,ImagenUrl,Stock,FechaCreacion,CategoriaId")] Producto producto)
        {
            if (id != producto.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Productos.Any(e => e.Id == producto.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        // Solo Admins
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
                return NotFound();

            return View(producto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
                _context.Productos.Remove(producto);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
