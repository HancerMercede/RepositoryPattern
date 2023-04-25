#nullable disable
using System.ComponentModel.DataAnnotations;

namespace Dtos.DtoModels;

public abstract class EmployeeForManipulationDto
{
    [Required(ErrorMessage = "Employee name is a required field.")]
    [MaxLength(30, ErrorMessage = "Max length for name is 30 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Age is a required field.")]
    [Range(18, 80, ErrorMessage = "The age field can be lower than 18.")]
    public int Age { get; set; }

    [Required(ErrorMessage = "Position is a required field.")]
    [MaxLength(20, ErrorMessage = "Maximum length for the Position is 20 characters.")]
    public string Position { get; set; }
}
