using Microsoft.UI.Dispatching;
using WinRT;
using WinUIApp;

string? processPath = Environment.ProcessPath;

if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(processPath)).Any(process => process.Id != Environment.ProcessId && process.MainModule?.FileName == processPath))
{
	Debug.WriteLine("The application was already running.");

	Environment.Exit(0);

	return;
}

ComWrappersSupport.InitializeComWrappers();

Application.Start(static delegate
{
	DispatcherQueueSynchronizationContext context = new(DispatcherQueue.GetForCurrentThread());
	SynchronizationContext.SetSynchronizationContext(context);
	_ = new App();
});
