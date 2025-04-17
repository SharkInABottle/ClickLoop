using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ClickLoop.Services
{
    public class LoopService
    {
        private CancellationTokenSource _cts;
        // DLL Imports for interacting with the Windows API
        [DllImport("User32.dll")]
        private static extern int SetForegroundWindow(IntPtr point);

        [DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public async Task StartLoopAsync(int interval, int speed, string text, Process process, Action onCancel, Action<string> onError)
        {
            _cts = new CancellationTokenSource();

            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(interval, _cts.Token);

                    if (process == null || process.HasExited)
                    {
                        onError($"Process {process.ProcessName} was terminated, loop aborted!");
                        onCancel();
                        break;
                    }
                    ShowWindow(process.MainWindowHandle, 1);
                    SetForegroundWindow(process.MainWindowHandle);
                    // Simulate sending keys
                    foreach (var word in text.Split(' '))
                    {
                        SendKeys.Send(word);
                        await Task.Delay(speed, _cts.Token);
                    }
                }
                catch (OperationCanceledException)
                {
                    onCancel();
                    break;
                }
            }
        }

        public void CancelLoop()
        {
            _cts?.Cancel();
        }
    }

}
