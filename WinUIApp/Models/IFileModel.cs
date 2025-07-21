namespace WinUIApp.Models
{
	internal interface IFileModel<TIModel>
		where TIModel : class, IModelBase
	{
		string File { get; }

		string ConfigurationSection { get; }

		Lock Lock { get; }
	}
}
