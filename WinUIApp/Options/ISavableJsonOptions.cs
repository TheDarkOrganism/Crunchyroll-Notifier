namespace WinUIApp.Options
{
	public interface ISavableJsonOptions<out TOptions> : IOptions<TOptions>
		where TOptions : ModelBase
	{
		void Save();

		Task SaveAsync();
	}
}
