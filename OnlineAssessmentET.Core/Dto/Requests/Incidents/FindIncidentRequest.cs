namespace OnlineAssessmentET.Core.Dto.Requests.Incidents;

internal record FindIncidentRequest
{
	public required string? SearchTerm { get; init; }

	public required int Page { get; init; }

	public required int Size { get; init; }
}
