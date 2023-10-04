using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;

class Program
{
    // Define the GetWindowText function from user32.dll to retrieve window titles.
    [DllImport("user32.dll")]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    private static System.Timers.Timer timer = new System.Timers.Timer(5000);

    private static Process[] processes = Process.GetProcesses();
    private static string filePath = Path.Combine(Path.GetTempPath(), "metadata.txt");

    static void Main()
    {
        // Start a timer that triggers every 5 seconds.
        timer.Elapsed += TimerElapsed;
        timer.AutoReset = true;
        timer.Start();

        Console.WriteLine("WARNING: Mixxx must be open first!");
        Console.WriteLine($"Metadata is saving to {filePath}\n");

        Console.WriteLine("Press 'Q' to exit the program.");
        while (Console.ReadKey().Key != ConsoleKey.Q) { }
    }

    private static void TimerElapsed(object sender, ElapsedEventArgs e)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var process in processes)
            {
                // Check if the process name is equal to "Mixxx"
                if (string.Equals(process.ProcessName, "Mixxx", StringComparison.OrdinalIgnoreCase))
                {
                    // Skip processes that do not have a main window title.
                    if (process.MainWindowHandle != IntPtr.Zero)
                    {
                        // Get the window title of the main window.
                        StringBuilder title = new StringBuilder(256);
                        GetWindowText(process.MainWindowHandle, title, title.Capacity);

                        title = title.Replace(" | Mixxx", "");

                        if (title.ToString().Equals("Mixxx"))
                        {
                            // If no music is playing, just write Live Broadcast to the file.
                            writer.WriteLine("Live Broadcast");
                            writer.WriteLine();
                            return;
                        }

                        // Write the process name and window title to the file.
                        writer.WriteLine(title);
                        writer.WriteLine();
                    }
                }
            }
        }
    }
}
