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

		// Add services to the container.

		builder.Services.AddScoped<IIncidentService, IncidentService>();

		builder.Services.AddDbContext<AppDbContext>(options => 
		{
			options.UseInMemoryDatabase("ET");
		});

		builder.Services
			.AddControllers()
			.EnableInternalControllers();

		// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
		builder.Services.AddOpenApi();
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();

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
}
