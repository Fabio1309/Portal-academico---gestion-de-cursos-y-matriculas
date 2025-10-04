using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace examen_parcial_programacion1.Models // Asegúrate que el namespace sea correcto
{
    // ESTE ES EL ENUM QUE FALTABA
    public enum EstadoMatricula
    {
        Pendiente,
        Confirmada,
        Cancelada
    }

    public class Matricula
    {
        public int Id { get; set; }

        [Required]
        public int CursoId { get; set; }
        public virtual Curso? Curso { get; set; } // El '?' y 'virtual' ayudan a EF Core

        [Required]
        public required string UsuarioId { get; set; }
        public virtual IdentityUser? Usuario { get; set; } // El '?' y 'virtual' ayudan a EF Core

        // ESTAS ERAN LAS PROPIEDADES FALTANTES
        public DateTime FechaRegistro { get; set; }
        public EstadoMatricula Estado { get; set; }
    }
}