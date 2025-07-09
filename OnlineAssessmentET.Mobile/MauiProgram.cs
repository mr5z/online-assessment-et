using Microsoft.Extensions.Logging;
using OnlineAssessmentET.Mobile.Services;
using OnlineAssessmentET.Mobile.ViewModels;
using Refit;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OnlineAssessmentET.Mobile;

public static class MauiProgram
{
	internal class DelegatingHandler : HttpClientHandler
	{
		public DelegatingHandler()
		{
			// Bypass certificate check for debugging purpose only
			ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			try
			{
				if (request.Content is not null)
				{
					var content = await request.Content.ReadAsStringAsync(cancellationToken);
					Console.WriteLine("Request: " + content);
				}

				var response = await base.SendAsync(request, cancellationToken);

				if (response.Content is not null)
				{
					var content = await response.Content.ReadAsStringAsync(cancellationToken);
					Console.WriteLine("Response: " + content);
				}
				return response;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				throw;
			}
		}
	}


	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UsePrism(prism =>
			{
				prism.RegisterTypes(registry =>
				{
					// Page-ViewModel
					registry.RegisterForNavigation<MainPage, MainPageViewModel>()
						;

					// Services
					var client = new HttpClient(new DelegatingHandler())
					{
						BaseAddress = new Uri("https://10.0.2.2:7160")
					};

					var serializer = SystemTextJsonContentSerializer.GetDefaultJsonSerializerOptions();
					serializer.Converters.Remove(serializer.Converters.Single(x => x.GetType().Equals(typeof(JsonStringEnumConverter))));
					var settings = new RefitSettings
					{
						ContentSerializer = new SystemTextJsonContentSerializer(serializer)
					};
					var incidentService = RestService.For<IIncidentService>(client, settings);

					registry.RegisterInstance(incidentService);
				});

				prism.CreateWindow(navigationService =>
				{
					return navigationService.CreateBuilder()
						.UseAbsoluteNavigation()
						.AddNavigationPage()
						.AddSegment<MainPageViewModel>()
						.NavigateAsync(Console.Error.WriteLine);
				});

			})
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});


#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}