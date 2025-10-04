using System.ComponentModel.DataAnnotations;

namespace examen_parcial_programacion1.Models
{
    public class Curso
    {
        public int Id { get; set; }
        
        [Required]
        public string Codigo { get; set; } = string.Empty;
        
        [Required]
        public string Nombre { get; set; } = string.Empty;
        
        public string? Descripcion { get; set; }
        
        public int Creditos { get; set; }
        
        public TimeOnly HorarioInicio { get; set; }
        
        public TimeOnly HorarioFin { get; set; }
        
        // Relación con Matriculas
        public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
    }
}
