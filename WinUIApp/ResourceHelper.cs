namespace WinUIApp
{
	internal static class ResourceHelper
	{
		private static readonly EmbeddedFileProvider _embeddedFileProvider = new(Assembly.GetExecutingAssembly(), nameof(WinUIApp));

		public static void RestoreFile<TIModel>(IFileModel<TIModel> fileModel)
			where TIModel : class, IModelBase
		{
			ArgumentNullException.ThrowIfNull(fileModel, nameof(fileModel));

			lock (fileModel.Lock)
			{
				string file = fileModel.File;

				IFileInfo fileInfo = _embeddedFileProvider.GetFileInfo(Path.GetFileName(file));

				if (!fileInfo.Exists)
				{
					throw new FileNotFoundException("Unable to find the file as a embedded resource.", file);
				}

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
			}
		}
	}
}
