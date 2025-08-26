namespace WinUIApp
{
	internal static class ResourceHelper
	{
		private static readonly EmbeddedFileProvider _embeddedFileProvider = new(Assembly.GetExecutingAssembly(), nameof(WinUIApp));

		public static void RestoreFile<TIModel, T>(IFileModel<TIModel> fileModel, ILogger<T> logger)
			where TIModel : class, IModelBase
		{
			ArgumentNullException.ThrowIfNull(fileModel, nameof(fileModel));

			string file = fileModel.File;

			IFileInfo fileInfo = _embeddedFileProvider.GetFileInfo(Path.GetFileName(file));

			if (!fileInfo.Exists)
			{
				throw new FileNotFoundException("Unable to find the file as a embedded resource.", file);
			}

			fileModel.Wait(() =>
			{
				try
				{
					using Stream stream = fileInfo.CreateReadStream();

					using FileStream fileStream = File.Create(file);

					fileStream.SetLength(0);

					stream.CopyTo(fileStream);
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex);

					Application.Current.Exit();
				}
			}, logger);
		}
	}
}
