using OnlineAssessmentET.Core.Dto.Requests.Incidents;
using OnlineAssessmentET.Core.Dto.Responses.Incidents;
using Refit;

namespace OnlineAssessmentET.Browser.Services;

internal interface IIncidentService
{
	[Post("/api/v1/Incident")]
	Task ReportIncident(ReportIncidentRequest request);

	[Get("/api/v1/Incident")]
	Task<FindIncidentResponse[]> FindIncidents([Query] FindIncidentRequest request);
}
