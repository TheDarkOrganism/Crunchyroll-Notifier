namespace App.Attributes
{
	internal sealed class TimeSpanValidationAttribute(double seconds) : RangeAttribute(seconds, double.MaxValue)
	{
		public override bool IsValid(object? value)
		{
			return Minimum is double minimum && value is TimeSpan timeSpan && minimum >= timeSpan.TotalSeconds;
		}
	}
}