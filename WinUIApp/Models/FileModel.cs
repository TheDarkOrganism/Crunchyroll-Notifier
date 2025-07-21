namespace WinUIApp.Models
{
	internal sealed class FileModel<TModel, TIModel> : IFileModel<TIModel>
		where TModel : notnull, ModelBase, TIModel
		where TIModel : class, IModelBase
	{
		public string File { get; }

		public string ConfigurationSection { get; }

		public Lock Lock { get; }

		public FileModel(string file)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(file, nameof(file));

			File = file;

			ConfigurationSection = Path.GetFileNameWithoutExtension(file);

			Lock = new();
		}
	}
}
