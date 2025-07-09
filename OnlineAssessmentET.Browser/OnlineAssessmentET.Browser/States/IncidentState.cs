using Fluxor;
using OnlineAssessmentET.Browser.Models;
using OnlineAssessmentET.Browser.Services;
using OnlineAssessmentET.Core.Dto.Requests.Incidents;
using static OnlineAssessmentET.Browser.States.IncidentState;

namespace OnlineAssessmentET.Browser.States;

[FeatureState]
internal record IncidentState
{
	public bool IsBusy { get; init; }
	public Incident[] Data { get; init; } = [];
	public string? ErrorMessage { get; init; }

	public record GetIncidents;
	public record GetIncidentsOk(Incident[] Data);
	public record GetIncidentsFail(string ErrorMessage);
}

internal static class Reducer
{
	[ReducerMethod]
	public static IncidentState OnGetIncidents(IncidentState state, GetIncidents action)
	{
		return state with
		{
			IsBusy = true
		};
	}

	[ReducerMethod]
	public static IncidentState OnGetIncidents(IncidentState state, GetIncidentsOk action)
	{
		return state with
		{
			IsBusy = false,
			Data = action.Data
		};
	}

	[ReducerMethod]
	public static IncidentState OnGetIncidents(IncidentState state, GetIncidentsFail action)
	{
		return state with
		{
			IsBusy = false,
			ErrorMessage = action.ErrorMessage
		};
	}
}

internal class Effect(
	ILogger<Effect> logger,
	IIncidentService incidentService)
{
	[EffectMethod(typeof(GetIncidents))]
	public async Task HandleAsync(IDispatcher dispatcher)
	{
		try
		{
			// Search endpoint simplified as GET all
			var result = await incidentService.FindIncidents(new FindIncidentRequest
			{
				SearchTerm = "*",
				Page = 1,
				Size = 100
			});

			var incidents = result.Select(i => new Incident
			{
				Id = i.Id,
				Severity = i.Severity,
				Title = i.Title,
				Description = i.Description
			}).ToArray();

			dispatcher.Dispatch(new GetIncidentsOk(incidents));
		}
		catch (Exception ex)
		{
			const string error = "Failed to fetch incidents.";
			logger.LogError(ex, error);
			dispatcher.Dispatch(new GetIncidentsFail(ex.Message));
		}
	}
}