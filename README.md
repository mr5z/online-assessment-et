# Task A
![image](https://github.com/user-attachments/assets/faa43473-5b36-42cc-b2fe-cf0c524ba4f0)

### Duplicate Handling
![image](https://github.com/user-attachments/assets/a45308cc-d084-4609-af94-5b8850afaec9)

# Task B
![image](https://github.com/user-attachments/assets/426840b1-e917-4d69-aee9-b39a6894a766)

#### Describe what could go wrong in rendering and how you'd fix it.
Blazor components rely heavily on state management, especially in Blazor Server projects where rendering depends on consistent server-side state. I used Fluxor, a Redux-style library, to centralize and manage state effectively.

In Blazor Server, state must be configured properly to avoid issues like UI resets or inconsistent rendering. This can be addressed by enabling distributed caching for scalability, but for this project, it only uses in-memory with a scoped lifetime.

Behind a load balancer, sticky sessions are required to keep users on the same server. This should avoid session-related rendering issues and works well with distributed caching.

For cases where component state should persist after a page reload, Fluxor store can easily be extended to save and restore state.

# Task C
![image](https://github.com/user-attachments/assets/75adca89-7c22-4a25-bdde-b9404c3a9453)

# Task D

```
internal enum Severity { Low, Medium, High }

internal class Incident
{
	public required Severity Severity { get; init; }
	public required string? Title { get; init; }
	public required DateTimeOffset DateReported { get; init; }

	float CalculateUrgency()
	{
		var now = DateTimeOffset.Now;
		var datePassed = now - DateReported;

		int severityWeight = Severity switch
		{
			Severity.Low => 1,
			Severity.Medium => 2,
			Severity.High => 3,
			_ => 0,
		};

		var hoursPassed = (float)datePassed.TotalHours;
		return severityWeight * (1 + hoursPassed / 24);
	}
}
```

### Validation
As for the validation, I would move it on a different layer, not within `Incident` class.
E.g., using `FluentValidation` for such tasks.

### Usage sample

<details>

<summary>Click to collapse</summary>
  
```
using System;
					
public class Program
{
	internal enum Severity { Low, Medium, High }
	
	internal class Incident
	{
		public required Severity Severity { get; init; }
		public required string? Title { get; init; }
		public required DateTimeOffset DateReported { get; init; }

		public float CalculateUrgency()
		{
			var now = DateTimeOffset.Now;
			var datePassed = now - DateReported;

			int severityWeight = Severity switch
			{
				Severity.Low => 1,
				Severity.Medium => 2,
				Severity.High => 3,
				_ => 0
			};

			var hoursPassed = (float)datePassed.TotalHours;
			return severityWeight * (1 + hoursPassed / 24);
		}
	}
	
	public static void Main()
	{
		DateTimeOffset fixedNow = new DateTimeOffset(2025, 7, 9, 12, 0, 0, TimeSpan.FromHours(8));
		
		var incident1 = new Incident
		{
			Severity = Severity.Low,
			Title = "Minor display bug",
			DateReported = fixedNow.AddHours(-2) // 2 hours ago
		};
		
		var incident2 = new Incident
		{
			Severity = Severity.Medium,
			Title = "Performance degradation",
			DateReported = fixedNow.AddHours(-12) // 12 hours ago
		};
		
		var incident3 = new Incident
		{
			Severity = Severity.High,
			Title = "System outage over weekend",
			DateReported = fixedNow.AddHours(-72) // 3 days ago
		};
		
		var incident4 = new Incident
		{
			Severity = Severity.High,
			Title = "Immediate crash after update",
			DateReported = fixedNow // Now
		};
		
		Console.WriteLine(incident1.CalculateUrgency());
		Console.WriteLine(incident2.CalculateUrgency());
		Console.WriteLine(incident3.CalculateUrgency());
		Console.WriteLine(incident4.CalculateUrgency());
	}
}
```
 
</details>

### Sample output

| Severity | Hours Passed | Urgency |
|----------|--------------|---------|
| Low      | 2            | 0.99    |
| Medium   | 12           | 2.81    |
| High     | 72           | 11.7    |
| High     | 0            | 2.72    |
