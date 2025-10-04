using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace examen_parcial_programacion1.Models // Asegúrate que el namespace sea el correcto
{
    [Index(nameof(Codigo), IsUnique = true)]
    public class Curso
    {
        public int Id { get; set; }

        [Required]
        public required string Codigo { get; set; } // <-- MODIFICADO

        [Required]
        public required string Nombre { get; set; } // <-- MODIFICADO

        [Range(1, int.MaxValue)]
        public int Creditos { get; set; }

        [Range(1, 100)]
        public int CupoMaximo { get; set; }

        public TimeOnly HorarioInicio { get; set; }
        public TimeOnly HorarioFin { get; set; }

        public bool Activo { get; set; } = true;
    }
}