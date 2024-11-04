using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Persistence.Context;

public class RepositoryContext(DbContextOptions<RepositoryContext> options) : DbContext(options)
{
	protected DbSet<Company> Companies { get; set; }
	protected DbSet<Employee> Employees { get; set; }


	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());		
	}
}