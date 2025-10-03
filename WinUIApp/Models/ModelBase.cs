namespace WinUIApp.Models
{
	internal abstract class ModelBase : IModelBase
	{
		[JsonIgnore]
		public bool Modified { get; private set; }

		protected void OnModified()
		{
			Modified = true;
		}

		public void MarkUnmodified()
		{
			Modified = false;
		}
	}
}
