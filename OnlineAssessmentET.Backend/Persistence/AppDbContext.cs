using Microsoft.EntityFrameworkCore;

namespace OnlineAssessmentET.Persistence;

internal class AppDbContext(DbContextOptions options) : DbContext(options)
{
	public DbSet<Incident> Incidents { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Incident>()
			.HasKey(i => i.Id);
	}
}
