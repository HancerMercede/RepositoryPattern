using AutoMapper;
using Dtos.DtoModels;
using Entities.Models;

namespace RepositoryPatternArquitecture.Configurations
{
    public class AutoMapper:Profile
    {
        public AutoMapper()
        {
            // Company mapping
            CreateMap<Company, CompanyDto>()
                .ForMember(c=>c.FullAddress, opt => 
                           opt.MapFrom(x => string.Join(' ',x.Address, x.Country)));

            CreateMap<CompanyCreateDto, Company>();
            CreateMap<CompanyUpdateDto, Company>();

            // Employee mapping
            CreateMap<Employee, EmployeeDto>().ReverseMap();
            CreateMap<EmployeeCreateDto, Employee>();
            CreateMap<EmployeeUpdateDto, Employee>().ReverseMap();

        }
    }
}
