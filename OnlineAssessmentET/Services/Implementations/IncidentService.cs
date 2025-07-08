using Microsoft.EntityFrameworkCore;
using OnlineAssessmentET.Core.Dto.Requests.Incidents;
using OnlineAssessmentET.Core.Dto.Responses.Incidents;
using OnlineAssessmentET.Core.Patterns;
using OnlineAssessmentET.Persistence;
using IResult = OnlineAssessmentET.Core.Patterns.IResult;

namespace OnlineAssessmentET.Services.Implementations;

internal class IncidentService(AppDbContext dbContext) : IIncidentService
{
	async Task<IResult> IIncidentService.ReportIncident(ReportIncidentRequest request)
	{
		if (Enum.IsDefined(request.Severity) == false)
		{
			return Result.Fail(ErrorCode.InvalidParameter, "Invalid severity value.");
		}

		if (string.IsNullOrEmpty(request.Title))
		{
			return Result.Fail(ErrorCode.InvalidParameter, "No title provided.");
		}

		if (request.Title.Length > Incident.TitleMaxLength)
		{
			return Result.Fail(ErrorCode.InvalidParameter, "Title exceeded max characters.");
		}

		if (request.Description?.Length > Incident.DescriptionMaxLength)
		{
			return Result.Fail(ErrorCode.InvalidParameter, "Description exceeded max characters.");
		}

		var now = DateTimeOffset.Now;
		var threshold = now.AddHours(-24);

		var hasDuplicates = await dbContext.Incidents
			.Where(i => i.Title == request.Title)
			.Where(i => i.Description == request.Description)
			.Where(i => i.CreatedAt >= threshold)
			.AnyAsync()
			.ConfigureAwait(false);

		if (hasDuplicates)
		{
			return Result.Fail(ErrorCode.Duplicate, "Duplicate request found.");
		}

		await dbContext.Incidents.AddAsync(new Incident
		{
			Severity = request.Severity,
			Title = request.Title,
			Description = request.Description,
			CreatedAt = now
		}).ConfigureAwait(false);

		await dbContext.SaveChangesAsync().ConfigureAwait(false);

		return Result.Ok();
	}

	async Task<Result<FindIncidentResponse[]>> IIncidentService.FindIncidents(FindIncidentRequest request)
	{
		if (string.IsNullOrEmpty(request.SearchTerm))
		{
			return Result.Fail<FindIncidentResponse[]>(ErrorCode.InvalidParameter, "Invalid search term.");
		}

		if (request.Page <= 0)
		{
			return Result.Fail<FindIncidentResponse[]>(ErrorCode.InvalidParameter, "Invalid page value.");
		}

		if (request.Size <= 0)
		{
			return Result.Fail<FindIncidentResponse[]>(ErrorCode.InvalidParameter, "Invalid size value.");
		}

		if (request.SearchTerm.Length > 100)
		{
			return Result.Fail<FindIncidentResponse[]>(ErrorCode.InvalidParameter, "Search term exceeded max characters.");
		}

		var incidents = await dbContext.Incidents
			.Where(i =>
				(i.Title != null && i.Title.Contains(request.SearchTerm, StringComparison.InvariantCultureIgnoreCase)) ||
				(i.Description != null && i.Description.Contains(request.SearchTerm, StringComparison.InvariantCultureIgnoreCase)))
			.Skip((request.Page - 1) * request.Size)
			.Take(request.Size)
			.OrderByDescending(i => i.CreatedAt)
			.Select(i => new FindIncidentResponse
			{
				Severity = i.Severity,
				Title = i.Title,
				Description = i.Description,
				CreatedAt = i.CreatedAt
			})
			.ToArrayAsync()
			.ConfigureAwait(false);

		if (incidents.Length == 0)
		{
			return Result.Fail<FindIncidentResponse[]>(ErrorCode.NotFound, "No result found.");
		}

		return Result.Ok(incidents);
	}
}
