using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using examen_parcial_programacion1.Data; // Asegúrate que el namespace sea correcto
using examen_parcial_programacion1.Models; // Asegúrate que el namespace sea correcto
using System.Security.Claims;

namespace examen_parcial_programacion1.Controllers
{
    // [Authorize] asegura que solo usuarios logueados pueden acceder a cualquier método de este controlador.
    [Authorize]
    public class MatriculasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MatriculasController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // [HttpPost] significa que este método solo se puede llamar a través de una petición POST (desde un formulario).
        // [ValidateAntiForgeryToken] es una medida de seguridad para prevenir ataques CSRF.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Inscribir(int cursoId)
        {
            // --- PASO 1: Obtener los datos necesarios ---
            var cursoAInscribir = await _context.Cursos.FindAsync(cursoId);
            if (cursoAInscribir == null)
            {
                TempData["ErrorMessage"] = "El curso seleccionado no existe.";
                return RedirectToAction("Index", "Cursos");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                // Esto no debería pasar si [Authorize] funciona, pero es una buena práctica.
                return Unauthorized();
            }

            // --- PASO 2: Realizar todas las validaciones ---

            // VALIDACIÓN 1: El usuario no puede estar ya matriculado en el mismo curso (a menos que esté cancelado).
            bool yaMatriculado = await _context.Matriculas
                .AnyAsync(m => m.CursoId == cursoId && m.UsuarioId == userId && m.Estado != EstadoMatricula.Cancelada);
            if (yaMatriculado)
            {
                TempData["ErrorMessage"] = "Ya estás matriculado o tienes una inscripción pendiente en este curso.";
                return RedirectToAction("Details", "Cursos", new { id = cursoId });
            }

            // VALIDACIÓN 2: No se puede superar el cupo máximo del curso.
            int matriculadosActuales = await _context.Matriculas
                .CountAsync(m => m.CursoId == cursoId && m.Estado != EstadoMatricula.Cancelada);
            if (matriculadosActuales >= cursoAInscribir.CupoMaximo)
            {
                TempData["ErrorMessage"] = "Lo sentimos, no hay cupos disponibles para este curso.";
                return RedirectToAction("Details", "Cursos", new { id = cursoId });
            }

            // VALIDACIÓN 3: El horario del nuevo curso no debe solaparse con otros cursos ya confirmados.
            var misCursosConfirmados = await _context.Matriculas
                .Where(m => m.UsuarioId == userId && m.Estado == EstadoMatricula.Confirmada)
                .Select(m => m.Curso)
                .ToListAsync();

            foreach (var cursoExistente in misCursosConfirmados)
            {
                // Fórmula de solapamiento de tiempo: (InicioA < FinB) AND (InicioB < FinA)
                if (cursoAInscribir.HorarioInicio < cursoExistente.HorarioFin && cursoExistente.HorarioInicio < cursoAInscribir.HorarioFin)
                {
                    TempData["ErrorMessage"] = $"El horario de este curso se solapa con tu curso ya inscrito: '{cursoExistente.Nombre}'.";
                    return RedirectToAction("Details", "Cursos", new { id = cursoId });
                }
            }

            // --- PASO 3: Si todas las validaciones pasan, crear la matrícula ---
            var nuevaMatricula = new Matricula
            {
                CursoId = cursoId,
                UsuarioId = userId,
                FechaRegistro = DateTime.UtcNow,
                Estado = EstadoMatricula.Pendiente // La matrícula se crea como "Pendiente"
            };

            _context.Matriculas.Add(nuevaMatricula);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "¡Inscripción registrada con éxito! Tu matrícula está pendiente de confirmación por un coordinador.";
            return RedirectToAction("Details", "Cursos", new { id = cursoId });
        }
    }
}