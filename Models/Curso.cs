using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace examen_parcial_programacion1.Models
{
    [Index(nameof(Codigo), IsUnique = true)]
    public class Curso
    {
        public int Id { get; set; }

        [Required]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Los créditos deben ser mayores que 0.")]
        public int Creditos { get; set; }

        [Range(1, 100)]
        public int CupoMaximo { get; set; }

        public TimeOnly HorarioInicio { get; set; }
        public TimeOnly HorarioFin { get; set; }

        public bool Activo { get; set; } = true;

        // Relación con Matriculas
        public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
    }
}
