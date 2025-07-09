namespace OnlineAssessmentET.Mobile.Services;

internal interface IToastService
{
	void Show(string format, params object[] args);
}