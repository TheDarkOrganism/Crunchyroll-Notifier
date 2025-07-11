namespace WinUIApp.Options
{
	public interface ISavableJsonOptions<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] out TOptions> : IOptions<TOptions>
		where TOptions : ModelBase
	{
		void Save();

		Task SaveAsync();
	}
}
