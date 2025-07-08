using Microsoft.EntityFrameworkCore;
using OnlineAssessmentET.Controllers;
using OnlineAssessmentET.Persistence;
using OnlineAssessmentET.Services;
using OnlineAssessmentET.Services.Implementations;

namespace OnlineAssessmentET;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddDbContext<AppDbContext>(options => 
		{
			options.UseInMemoryDatabase("ET");
			using var context = new AppDbContext(options.Options);
			SeedData(context);
		});

		// First/third party services
		builder.Services
			.AddControllers()
			.EnableInternalControllers();

		builder.Services.AddOpenApi();
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();

		// Custom services
		builder.Services.AddScoped<IIncidentService, IncidentService>();

		var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
			app.MapOpenApi();
			app.UseSwagger();
			app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
			{
				options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
				options.RoutePrefix = string.Empty;
			});
		}

        app.MapControllers();

        app.Run();
    }

	private static void SeedData(AppDbContext context)
	{
		if (context.Incidents.Any())
			return;

		var now = DateTimeOffset.UtcNow;

		List<Incident> incidents =
		[
			new()
			{
				Id = 1,
				Severity = Severity.High,
				Title = "Database Connection Failure",
				Description = "Production database unreachable since 02:00 AM. Services dependent on it are failing.",
				CreatedAt = now.AddMinutes(-5)
			},
			new()
			{
				Id = 2,
				Severity = Severity.Medium,
				Title = "Slow Response from API",
				Description = "Users reporting slow response times when accessing the /orders endpoint.",
				CreatedAt = now.AddMinutes(-15)
			},
			new()
			{
				Id = 3,
				Severity = Severity.Low,
				Title = "Typo in Welcome Email",
				Description = "Subject line misspells 'Welcome' as 'Welocme'. Not affecting functionality.",
				CreatedAt = now.AddHours(-1)
			},
			new()
			{
				Id = 4,
				Severity = Severity.Medium,
				Title = "Payment Gateway Timeout",
				Description = "Transactions intermittently failing during checkout via Stripe integration.",
				CreatedAt = now.AddMinutes(-25)
			},
			new()
			{
				Id = 5,
				Severity = Severity.Low,
				Title = "UI Misalignment in Safari",
				Description = "Some buttons are misaligned on the dashboard when viewed in Safari 16.",
				CreatedAt = now.AddHours(-2)
			},
			new()
			{
				Id = 6,
				Severity = Severity.High,
				Title = "Security Breach Detected",
				Description = "Unauthorized access attempt on admin panel. Account locked and IP blacklisted.",
				CreatedAt = now.AddMinutes(-2)
			},
			new()
			{
				Id = 7,
				Severity = Severity.Low,
				Title = "Broken Link in FAQ Page",
				Description = "The 'Shipping Policy' link returns a 404 error.",
				CreatedAt = now.AddDays(-1)
			},
			new()
			{
				Id = 8,
				Severity = Severity.Medium,
				Title = "Email Notifications Delayed",
				Description = "Outlook integration experiencing delays in dispatching automated emails.",
				CreatedAt = now.AddMinutes(-35)
			},
			new()
			{
				Id = 9,
				Severity = Severity.High,
				Title = "Memory Leak in Background Service",
				Description = "ServiceWorker is consuming 95% of system memory every 30 minutes. Restart required.",
				CreatedAt = now.AddMinutes(-10)
			},
			new()
			{
				Id = 10,
				Severity = Severity.Low,
				Title = "Incorrect Product Image",
				Description = "The thumbnail for product ID 324 displays the wrong item.",
				CreatedAt = now.AddHours(-3)
			},
		];

		context.Incidents.AddRange(incidents);
		context.SaveChanges();
	}
}
