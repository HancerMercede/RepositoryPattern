#nullable disable
namespace Dtos.DtoModels
{
    public record CompanyUpdateDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public IEnumerable<EmployeeCreateDto> employees { get; set; }
    }
}
