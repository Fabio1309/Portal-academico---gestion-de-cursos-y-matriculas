using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using examen_parcial_programacion1.Data;
using examen_parcial_programacion1.Models;

namespace examen_parcial_programacion1.Controllers
{
    // ¡CLAVE! Este atributo protege todo el controlador.
    // Solo los usuarios con el rol "Coordinador" podrán acceder a estas acciones.
    [Authorize(Roles = "Coordinador")]
    public class CoordinadorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDistributedCache _cache;
        private const string CacheKey = "ListaCursosActivos"; // Usamos una constante para evitar errores de tipeo

        public CoordinadorController(ApplicationDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // --- GESTIÓN DE CURSOS (CRUD) ---

        // GET: /Coordinador - Muestra la lista de todos los cursos para administrar
        public async Task<IActionResult> Index()
        {
            var cursos = await _context.Cursos.ToListAsync();
            return View(cursos);
        }

        // GET: /Coordinador/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Coordinador/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Codigo,Nombre,Creditos,CupoMaximo,HorarioInicio,HorarioFin,Activo")] Curso curso)
        {
            if (ModelState.IsValid)
            {
                _context.Add(curso);
                await _context.SaveChangesAsync();
                
                // ¡INVALIDACIÓN DE CACHÉ!
                // Al crear un curso, la lista cacheada queda obsoleta. La eliminamos.
                await _cache.RemoveAsync(CacheKey);
                
                return RedirectToAction(nameof(Index));
            }
            return View(curso);
        }

        // GET: /Coordinador/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null) return NotFound();
            return View(curso);
        }

        // POST: /Coordinador/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Codigo,Nombre,Creditos,CupoMaximo,HorarioInicio,HorarioFin,Activo")] Curso curso)
        {
            if (id != curso.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(curso);
                await _context.SaveChangesAsync();
                
                // ¡INVALIDACIÓN DE CACHÉ!
                // Al editar un curso, la lista también queda obsoleta.
                await _cache.RemoveAsync(CacheKey);
                
                return RedirectToAction(nameof(Index));
            }
            return View(curso);
        }
        
        // GET: /Coordinador/Deactivate/5
        public async Task<IActionResult> Deactivate(int? id)
        {
            if (id == null) return NotFound();
            var curso = await _context.Cursos.FirstOrDefaultAsync(m => m.Id == id);
            if (curso == null) return NotFound();
            return View(curso);
        }

        // POST: /Coordinador/Deactivate/5
        [HttpPost, ActionName("Deactivate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateConfirmed(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso != null)
            {
                curso.Activo = false; // En lugar de borrar, solo desactivamos
                _context.Update(curso);
                await _context.SaveChangesAsync();

                // ¡INVALIDACIÓN DE CACHÉ!
                // Al desactivar, también debe desaparecer del catálogo público.
                await _cache.RemoveAsync(CacheKey);
            }
            return RedirectToAction(nameof(Index));
        }


        // --- GESTIÓN DE MATRÍCULAS ---

        // GET: /Coordinador/Matriculas/5 - Muestra las matrículas de un curso específico
        public async Task<IActionResult> Matriculas(int? id)
        {
            if (id == null) return NotFound();
            
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null) return NotFound();
            ViewBag.CursoNombre = curso.Nombre; // Enviamos el nombre del curso a la vista

            var matriculas = await _context.Matriculas
                .Include(m => m.Usuario) // ¡Importante! Incluimos los datos del usuario
                .Where(m => m.CursoId == id)
                .ToListAsync();
                
            return View(matriculas);
        }

        // POST: /Coordinador/ConfirmarMatricula
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarMatricula(int matriculaId)
        {
            var matricula = await _context.Matriculas.FindAsync(matriculaId);
            if (matricula != null)
            {
                matricula.Estado = EstadoMatricula.Confirmada;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Matriculas), new { id = matricula.CursoId });
            }
            return NotFound();
        }

        // POST: /Coordinador/CancelarMatricula
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarMatricula(int matriculaId)
        {
            var matricula = await _context.Matriculas.FindAsync(matriculaId);
            if (matricula != null)
            {
                matricula.Estado = EstadoMatricula.Cancelada;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Matriculas), new { id = matricula.CursoId });
            }
            return NotFound();
        }
    }
}