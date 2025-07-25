

using Contracts.Interfaces;

namespace Entities.Models;
public class Company : IEntity
{
    public Company()
    {
        Employees = new HashSet<Employee>();
    }

    [Column("CompanyId")] 
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Company name is a required field.")]
    [MaxLength(60, ErrorMessage = "Maximum length for the Name is 60 characters.")] 
    public string? Name { get; set; }
    [Required(ErrorMessage = "Company address is a required field.")]
    [MaxLength(60, ErrorMessage = "Maximum length for the Address is 60 characters")] 
    public string? Address { get; set; }
    public string? Country { get; set; }
    public ICollection<Employee> Employees { get; set; }
}

