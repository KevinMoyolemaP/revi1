using AlquimiaEsencial.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlquimiaEsencial.Controllers
{
    public class RecomendacionesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecomendacionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Recomendaciones
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string sintomas)
        {
            if (string.IsNullOrWhiteSpace(sintomas))
            {
                ViewBag.Error = "Por favor ingresa uno o más síntomas.";
                return View();
            }

            var recomendaciones = await _context.Recomendaciones
                .Include(r => r.Producto)
                .Where(r => sintomas.ToLower().Contains(r.Sintoma.ToLower()))
                .Select(r => r.Producto)
                .Distinct()
                .ToListAsync();

            ViewBag.SintomasIngresados = sintomas;
            return View(recomendaciones);
        }
    }
}
