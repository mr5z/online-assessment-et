using Microsoft.Extensions.Logging;
using OnlineAssessmentET.Core.Dto.Requests.Incidents;
using OnlineAssessmentET.Mobile.Services;
using PropertyChanged;
using System.Windows.Input;

namespace OnlineAssessmentET.Mobile.ViewModels;

[AddINotifyPropertyChangedInterface]
internal class MainPageViewModel
{
	private readonly ILogger<MainPageViewModel> _logger;
	private readonly IIncidentService _incidentService;

	public MainPageViewModel(ILogger<MainPageViewModel> logger, IIncidentService incidentService)
	{
		_logger = logger;
		_incidentService = incidentService;

		SubmitCommand = new AsyncDelegateCommand(Submit);
	}

	public Severity[] Severities { get; } = [Severity.Low, Severity.Medium, Severity.High];

	public Severity SelectedSeverity { get; set; } = Severity.Low;

	public string? IncidentTitle { get; set; }

	public string? Description { get; set; }

	public ICommand SubmitCommand { get; private set; }

	private async Task Submit()
	{
		if (string.IsNullOrEmpty(IncidentTitle))
		{
			// TODO prompt error
			return;
		}

		try
		{
			await Task.Run(async () =>
			{
				await _incidentService.ReportIncident(new ReportIncidentRequest
				{ 
					Severity = SelectedSeverity,
					Title = IncidentTitle,
					Description = Description
				});
			});
		}
		catch (Exception ex)
		{
			const string error = "Failed to report incident";
			_logger.LogError(ex, error);
		}
	}
}
