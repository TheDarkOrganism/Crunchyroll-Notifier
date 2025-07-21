namespace WinUIApp.Options
{
	internal sealed class Options<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TIOptions>(TIOptions options) : IOptions<TIOptions>
		where TIOptions : class
	{
		public TIOptions Value => options;
	}
}
