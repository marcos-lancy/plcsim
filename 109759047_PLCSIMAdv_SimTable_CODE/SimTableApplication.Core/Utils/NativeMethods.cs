using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SimTableApplication.Core.Utils
{
    /// <summary>
    /// Native methods are calls to the operating system
    /// </summary>
    public static class NativeMethods
    {
        private const string WindowsExplorerExecutable = "explorer.exe";

        /// <summary>
        /// Retrieves a handle to the top-level window whose class name and window name match the specified strings.
        /// https://msdn.microsoft.com/de-de/library/windows/desktop/ms633499(v=vs.85).aspx
        /// </summary>
        /// <param name="className">window class name</param>
        /// <param name="windowName">window name</param>
        /// <returns>handle to the window</returns>
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindWindow(string className, string windowName);

        /// <summary>
        /// Brings the thread that created the specified window into the foreground and activates the window.
        /// https://msdn.microsoft.com/de-de/library/windows/desktop/ms633539(v=vs.85).aspx
        /// </summary>
        /// <param name="handle">window handle</param>
        /// <returns>success</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr handle);

        /// <summary>
        /// Path to the Microsoft Windows Explorer
        /// </summary>
        public static string WindowsExplorer
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), WindowsExplorerExecutable);
            }
        }
    }
}
