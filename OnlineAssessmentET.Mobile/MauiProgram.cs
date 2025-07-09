using Microsoft.Extensions.Logging;
using OnlineAssessmentET.Mobile.Services;
using OnlineAssessmentET.Mobile.ViewModels;

namespace OnlineAssessmentET.Mobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			//.UseMauiCommunityToolkit()
			//.UseMauiCompatibility()
			.UsePrism(prism =>
			{
				prism.RegisterTypes(registry =>
				{
					// Page-ViewModel
					registry.RegisterForNavigation<MainPage, MainPageViewModel>()
						;

					// Services
					var client = new HttpClient
					{
						BaseAddress = new Uri("https://localhost:7160")
					};
					var incidentService = Refit.RestService.For<IIncidentService>(client);

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