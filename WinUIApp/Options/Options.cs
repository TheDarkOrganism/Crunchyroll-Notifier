namespace WinUIApp.Options
{
	internal sealed class Options<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TOptions>(TOptions options) : IOptions<TOptions>
		where TOptions : class
	{
		public TOptions Value => options;
	}
}
