using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;

namespace SimTableApplication.Core.Utils
{
    /// <summary>
    /// Delivers serveral helper methods for global usage
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Adds an AssemblResolve for Siemens.Simatic.Simulation.Runtime.Api.x64.dll 
        /// </summary>
        public static void AssemblyResolve()
        {
            var currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += PLCSIM_Advanced_Resolver;
        }

        /// <summary>
        /// Reads the paths for Siemens.Simatic.Simulation.Runtime.Api.x64.dll from the registry
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="ResolveEventArgs" /> instance containing the event data.</param>
        /// <returns>Assembly</returns>
        private static Assembly PLCSIM_Advanced_Resolver(object sender, ResolveEventArgs args)
        {
            int index = args.Name.IndexOf(',');
            if (index == -1)
            {
                return null;
            }
            string name = args.Name.Substring(0, index) + ".dll";

            // Check for 64bit installation           
            RegistryKey filePathReg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\Siemens\\Shared Tools\\PLCSIMADV_SimRT");
            if (filePathReg == null) return null;

            string filePath = Path.Combine(filePathReg.GetValue("Path").ToString(), "API", "2.0");

            string path = Path.Combine(filePath, name);
            // User must provide the correct path
            string fullPath = Path.GetFullPath(path);
            if (File.Exists(fullPath))
            {
                return Assembly.LoadFrom(fullPath);
            }
            return null;
        }

        /// <summary>
        /// copy complete directory to an other directory
        /// </summary>
        /// <param name="sourceDirectory">source directory</param>
        /// <param name="targetDirectory">target directory></param>
        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }     
        private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                if (!fi.Extension.Equals(".sim"))
                {
                    fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
                }   
                
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        





    }
}
