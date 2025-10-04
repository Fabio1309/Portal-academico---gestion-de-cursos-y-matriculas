using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace examen_parcial_programacion1.Models
{
    public class Matricula
    {
        public int Id { get; set; }
        
        // Relación con Curso
        public int CursoId { get; set; }
        public Curso Curso { get; set; } = null!;
        
        // Relación con Usuario (IdentityUser)
        public string UsuarioId { get; set; } = string.Empty;
        public IdentityUser Usuario { get; set; } = null!;
        
        public DateTime FechaMatricula { get; set; } = DateTime.Now;
        
        public bool Activa { get; set; } = true;
    }
}
