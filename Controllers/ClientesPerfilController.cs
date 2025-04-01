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
    public class ClientesPerfilController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientesPerfilController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ClientesPerfil
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ClientesPerfil.Include(c => c.Usuario);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ClientesPerfil/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientePerfil = await _context.ClientesPerfil
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clientePerfil == null)
            {
                return NotFound();
            }

            return View(clientePerfil);
        }

        // GET: ClientesPerfil/Create
        public IActionResult Create()
        {
            ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: ClientesPerfil/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UsuarioId,Direccion,Telefono")] ClientePerfil clientePerfil)
        {
            if (ModelState.IsValid)
            {
                _context.Add(clientePerfil);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Id", clientePerfil.UsuarioId);
            return View(clientePerfil);
        }

        // GET: ClientesPerfil/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientePerfil = await _context.ClientesPerfil.FindAsync(id);
            if (clientePerfil == null)
            {
                return NotFound();
            }
            ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Id", clientePerfil.UsuarioId);
            return View(clientePerfil);
        }

        // POST: ClientesPerfil/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UsuarioId,Direccion,Telefono")] ClientePerfil clientePerfil)
        {
            if (id != clientePerfil.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clientePerfil);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientePerfilExists(clientePerfil.Id))
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
            ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Id", clientePerfil.UsuarioId);
            return View(clientePerfil);
        }

        // GET: ClientesPerfil/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientePerfil = await _context.ClientesPerfil
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clientePerfil == null)
            {
                return NotFound();
            }

            return View(clientePerfil);
        }

        // POST: ClientesPerfil/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clientePerfil = await _context.ClientesPerfil.FindAsync(id);
            if (clientePerfil != null)
            {
                _context.ClientesPerfil.Remove(clientePerfil);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientePerfilExists(int id)
        {
            return _context.ClientesPerfil.Any(e => e.Id == id);
        }
    }
}
