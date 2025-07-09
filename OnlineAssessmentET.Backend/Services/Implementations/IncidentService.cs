using Microsoft.EntityFrameworkCore;
using OnlineAssessmentET.Core.Dto.Requests.Incidents;
using OnlineAssessmentET.Core.Dto.Responses.Incidents;
using OnlineAssessmentET.Core.Patterns;
using OnlineAssessmentET.Persistence;
using IResult = OnlineAssessmentET.Core.Patterns.IResult;

namespace OnlineAssessmentET.Services.Implementations;

internal class IncidentService(ILogger<IncidentService> logger, AppDbContext dbContext) : IIncidentService
{
	async Task<IResult> IIncidentService.ReportIncident(ReportIncidentRequest request, CancellationToken cancellation)
	{
		try
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
				.AnyAsync(cancellation)
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
			}, cancellation).ConfigureAwait(false);

			await dbContext.SaveChangesAsync(cancellation).ConfigureAwait(false);

			return Result.Ok();
		}
		catch (Exception ex)
		{
			const string error = "An error occurred whilst trying to report an incident (Title: {Title}).";
			logger.LogError(ex, error, request.Title);
			return Result.Fail(ErrorCode.General, error);
		}
	}

	async Task<Result<FindIncidentResponse[]>> IIncidentService.FindIncidents(FindIncidentRequest request, CancellationToken cancellation)
	{
		try
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

			if (request.SearchTerm.Length > Incident.SearchTermMaxLength)
			{
				return Result.Fail<FindIncidentResponse[]>(ErrorCode.InvalidParameter, "Search term exceeded max characters.");
			}

			var searchTerm = request.SearchTerm.Trim();
			var hasSearch = !string.IsNullOrWhiteSpace(searchTerm) && searchTerm != "*";

			IQueryable<Incident> query = dbContext.Incidents;

			if (hasSearch)
			{
				var pattern = EscapeLikePattern(searchTerm);

				query = query.Where(i =>
					(i.Title != null && EF.Functions.Like(i.Title, pattern)) ||
					(i.Description != null && EF.Functions.Like(i.Description, pattern)));
			}

			var incidents = await query
				.Skip((request.Page - 1) * request.Size)
				.Take(request.Size)
				.OrderByDescending(i => i.CreatedAt)
				.Select(i => new FindIncidentResponse
				{
					Id = i.Id,
					Severity = i.Severity,
					Title = i.Title,
					Description = i.Description,
					CreatedAt = i.CreatedAt
				})
				.ToArrayAsync(cancellation)
				.ConfigureAwait(false);

			if (incidents.Length == 0)
			{
				return Result.Fail<FindIncidentResponse[]>(ErrorCode.NotFound, "No result found.");
			}

			return Result.Ok(incidents);
		}
		catch (Exception ex)
		{
			const string error = "An error occurred whilst trying to find an incident (SearchTerm: {SearchTerm}).";
			logger.LogError(ex, error, request.SearchTerm);
			return Result.Fail<FindIncidentResponse[]>(ErrorCode.General, error);
		}
	}

	private static string EscapeLikePattern(string input)
	{
		return input.Replace("[", "[[]")
					.Replace("%", "[%]")
					.Replace("_", "[_]")
					.Replace("*", "%");
	}

}
