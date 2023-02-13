using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Context;

public class RepositoryContext:DbContext
{
	public RepositoryContext(DbContextOptions<RepositoryContext> options)
		:base(options)
	{

		
    }
    public DbSet<Company> Companies { get; set; }
	public DbSet<Employee> Employees { get; set; }
}