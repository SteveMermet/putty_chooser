using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CVLog_serial_tool
{
    public static class ExecutableFinder
    {
        /// <summary>
        /// Finds the path to an executable by searching common locations and PATH
        /// </summary>
        /// <param name="executableName">Name of the executable (e.g., "putty.exe", "realterm.exe")</param>
        /// <param name="commonPaths">Additional common installation paths to search</param>
        /// <returns>Full path to executable if found, null otherwise</returns>
        public static string FindExecutable(string executableName, params string[] commonPaths)
        {
            // 1. Check if it's in PATH (most reliable)
            string pathResult = FindInPath(executableName);
            if (pathResult != null)
                return pathResult;

            // 2. Check current application directory
            string currentDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string currentDirPath = Path.Combine(currentDir, executableName);
            if (File.Exists(currentDirPath))
                return currentDirPath;

            // 3. Search common installation paths
            var searchPaths = new List<string>();
            
            // Add provided common paths
            if (commonPaths != null)
                searchPaths.AddRange(commonPaths);

            // Add standard Program Files locations
            searchPaths.AddRange(GetStandardProgramPaths());

            // Search recursively in these paths
            foreach (string basePath in searchPaths)
            {
                if (Directory.Exists(basePath))
                {
                    try
                    {
                        string found = Directory.GetFiles(basePath, executableName, SearchOption.AllDirectories)
                                              .FirstOrDefault();
                        if (!string.IsNullOrEmpty(found))
                            return found;
                    }
                    catch
                    {
                        // Skip directories we can't access
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Searches for executable in system PATH
        /// </summary>
        private static string FindInPath(string executableName)
        {
            try
            {
                string pathEnv = Environment.GetEnvironmentVariable("PATH");
                if (string.IsNullOrEmpty(pathEnv))
                    return null;

                var paths = pathEnv.Split(Path.PathSeparator)
                                  .Where(p => !string.IsNullOrWhiteSpace(p))
                                  .Select(p => p.Trim('"'));

                foreach (string path in paths)
                {
                    try
                    {
                        string fullPath = Path.Combine(path, executableName);
                        if (File.Exists(fullPath))
                            return fullPath;
                    }
                    catch
                    {
                        // Skip invalid paths
                    }
                }
            }
            catch
            {
                // PATH parsing failed
            }

            return null;
        }

        /// <summary>
        /// Gets list of standard program installation directories
        /// </summary>
        private static IEnumerable<string> GetStandardProgramPaths()
        {
            var paths = new List<string>();

            // Program Files directories
            string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            
            if (!string.IsNullOrEmpty(programFiles))
                paths.Add(programFiles);
            
            if (!string.IsNullOrEmpty(programFilesX86) && programFilesX86 != programFiles)
                paths.Add(programFilesX86);

            // Local application data (for portable apps)
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (!string.IsNullOrEmpty(localAppData))
                paths.Add(localAppData);

            // User profile desktop (common for portable apps)
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (!string.IsNullOrEmpty(desktop))
                paths.Add(desktop);

            // Chocolatey directory
            paths.Add(@"C:\ProgramData\chocolatey\bin");
            paths.Add(@"C:\tools");

            return paths;
        }

        /// <summary>
        /// Finds PuTTY executable in common locations
        /// </summary>
        public static string FindPutty()
        {
            return FindExecutable("putty.exe",
                @"C:\Program Files\PuTTY",
                @"C:\Program Files (x86)\PuTTY",
                @"C:\PuTTY",
                @"C:\tools\PuTTY"
            );
        }

        /// <summary>
        /// Finds RealTerm executable in common locations
        /// </summary>
        public static string FindRealTerm()
        {
            return FindExecutable("realterm.exe",
                @"C:\Program Files\BEL\Realterm",
                @"C:\Program Files (x86)\BEL\Realterm",
                @"C:\Program Files\RealTerm",
                @"C:\Program Files (x86)\RealTerm",
                @"C:\RealTerm",
                @"C:\tools\RealTerm"
            );
        }
    }
}
