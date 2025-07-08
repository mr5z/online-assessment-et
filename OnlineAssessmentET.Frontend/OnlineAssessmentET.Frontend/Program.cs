using OnlineAssessmentET.Frontend.Components;
using Fluxor;
using Fluxor.Persist.Middleware;
using OnlineAssessmentET.Frontend.Services;
using Fluxor.Persist.Storage;
using OnlineAssessmentET.Frontend.States;

namespace OnlineAssessmentET.Frontend;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// First/third party services
		builder.Services.AddRazorComponents()
			.AddInteractiveServerComponents()
			.AddInteractiveWebAssemblyComponents();

		builder.Services.AddFluxor(options =>
		{
			options.ScanTypes(typeof(IncidentState), [typeof(Effect), typeof(Reducer)]);
			//options.UsePersist();
		});

		builder.Services.AddHttpClient();

		// Custom services

		//builder.Services.AddScoped<IStoreHandler, FluxorStoreHandler>();

		builder.Services.AddScoped<IIncidentService>(serviceProvider =>
		{
			using var scope = serviceProvider.CreateScope();
			var factory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
			var client = factory.CreateClient();
			client.BaseAddress = new Uri("https://localhost:7160");
			return Refit.RestService.For<IIncidentService>(client);
		});

		var app = builder.Build();

		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			app.UseWebAssemblyDebugging();
		}
		else
		{
			app.UseExceptionHandler("/Error");
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}

		app.UseHttpsRedirection();

		app.UseAntiforgery();

		app.MapStaticAssets();
		app.MapRazorComponents<App>()
			.AddInteractiveServerRenderMode()
			.AddInteractiveWebAssemblyRenderMode()
			.AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

		app.Run();
	}
}
