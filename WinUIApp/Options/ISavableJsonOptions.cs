namespace WinUIApp.Options
{
	public interface ISavableJsonOptions<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] out TIOptions> : IOptions<TIOptions>
		where TIOptions : class, IModelBase
	{
		void Save();

		Task SaveAsync(CancellationToken cancellationToken);
	}
}
