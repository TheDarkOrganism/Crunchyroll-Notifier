namespace WinUIApp.Models
{
	internal interface ILastUpdateModel : IModelBase
	{
		DateTime LastUpdate { get; set; }
	}
}