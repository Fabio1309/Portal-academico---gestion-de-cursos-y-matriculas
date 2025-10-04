using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

[Index(nameof(Codigo), IsUnique = true)]
public class Curso
{
    public int Id { get; set; }

    [Required]
    public string Codigo { get; set; }

    [Required]
    public string Nombre { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Los créditos deben ser mayores que 0.")]
    public int Creditos { get; set; }

    [Range(1, 100)]
    public int CupoMaximo { get; set; }

    public TimeOnly HorarioInicio { get; set; }
    public TimeOnly HorarioFin { get; set; }

    public bool Activo { get; set; } = true;
}