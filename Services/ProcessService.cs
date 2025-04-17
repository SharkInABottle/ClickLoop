using System.Diagnostics;

namespace ClickLoop.Services
{
    public class ProcessService
    {
        public List<string> GetRunningProcesses()
        {
            return Process.GetProcesses()
                .Where(p => p.Id > 4 && p.MainWindowHandle != IntPtr.Zero)
                .Select(p => p.ProcessName)
                .OrderBy(name => name)
                .ToList();
        }

        public Process GetProcessByName(string processName)
        {
            return Process.GetProcessesByName(processName).FirstOrDefault();
        }
    }

}
