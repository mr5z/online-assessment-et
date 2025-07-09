using OnlineAssessmentET.Core.Dto.Requests.Incidents;
using Refit;

namespace OnlineAssessmentET.Mobile.Services;

internal interface IIncidentService
{
	[Post("/api/v1/Incident")]
	Task ReportIncident(ReportIncidentRequest request);
}
