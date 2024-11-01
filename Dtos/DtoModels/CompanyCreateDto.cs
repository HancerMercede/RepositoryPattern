
using System.ComponentModel.DataAnnotations;

namespace Dtos.DtoModels;

public class CompanyCreateDto
{
    public CompanyCreateDto()
    {
        Employees = new HashSet<EmployeeCreateDto>();
    }

    [Required(ErrorMessage="The Name is required field.")]
    public string? Name { get; set; }
    [Required(ErrorMessage = "Company address is a required field.")]
    [MaxLength(60, ErrorMessage = "Maximum length for the Address is 60 characters")]
    public string? Address { get; set; }
    public string? Country { get; set; }
    public IEnumerable<EmployeeCreateDto> Employees { get; set; }
}
