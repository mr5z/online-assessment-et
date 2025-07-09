using System.Diagnostics.CodeAnalysis;

namespace OnlineAssessmentET.Core.Patterns;

internal enum ErrorCode
{
	None = 0,

	General = 1,

	InvalidParameter = 2,

	Duplicate = 3,

	NotFound = 4,
}

internal interface IResult
{
	public bool IsSuccess { get; }

	public bool IsFailure { get; }

	public ErrorCode ErrorCode { get; }

	string? ErrorMessage { get; }
}

internal readonly struct Result<T> : IResult
{
	public T? Value { get; }

	public bool IsSuccess { get; }

	public bool IsFailure => IsSuccess == false;

	public string? ErrorMessage { get; }

	public ErrorCode ErrorCode { get; }

	internal Result(T value)
	{
		Value = value;
		IsSuccess = true;
		ErrorCode = ErrorCode.None;
		ErrorMessage = null;
	}

	internal Result(ErrorCode errorCode, string errorMessage)
	{
		Value = default;
		IsSuccess = false;
		ErrorMessage = errorMessage;
		ErrorCode = errorCode;
	}

	public bool TryGetValue([NotNullWhen(true)] out T? value)
	{
		if (IsSuccess && Value is T outValue)
		{
			value = outValue;
			return true;
		}
		value = default;
		return false;
	}
}

internal static class Result
{
	private readonly struct Unit
	{
		internal static readonly Unit Instance = new();
	}

	public static IResult Ok()
		=> new Result<Unit>(value: Unit.Instance);

	public static IResult Fail(ErrorCode errorCode, string errorMessage)
		=> new Result<Unit>(errorCode, errorMessage);

	public static Result<TValue> Ok<TValue>(TValue value)
		=> new(value);

	public static Result<TValue> Fail<TValue>(ErrorCode errorCode, string errorMessage)
		=> new(errorCode, errorMessage);
}
