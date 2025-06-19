namespace WinUIApp.Models
{
	public abstract class ModelBase
	{
		[JsonIgnore]
		public bool Modified { get; private set; }

		protected void OnModified()
		{
			Modified = true;
		}

		internal void MarkUnmodified()
		{
			Modified = false;
		}
	}
}
