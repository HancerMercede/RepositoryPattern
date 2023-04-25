using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Persistence.Context;

public class RepositoryContext:DbContext
{
	public RepositoryContext(DbContextOptions<RepositoryContext> options)
		:base(options)
	{

		
    }
    public DbSet<Company> Companies { get; set; }
	public DbSet<Employee> Employees { get; set; }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());		
	}
}