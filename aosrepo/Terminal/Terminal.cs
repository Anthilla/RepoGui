using System;
using System.Diagnostics;

namespace aosrepo.Terminal {
    public class Terminal {
        private static bool IsUnix => (Environment.OSVersion.Platform == PlatformID.Unix);

        public static string Execute(string command, string dir = "") {
            var output = string.Empty;
            Process process;
            if (IsUnix) {
                process = new Process {
                    StartInfo = {
                        FileName = "bash",
                        Arguments = $"-c \"{command}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false
                    }
                };
            }
            else {
                process = new Process {
                    StartInfo = {
                        FileName = "cmd.exe",
                        Arguments = $"/C {command}",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                };
            }
            if (dir.Length > 0) {
                process.StartInfo.WorkingDirectory = dir;
            }
            try {
                process.Start();
                using (var streamReader = process.StandardOutput) {
                    output += streamReader.ReadToEnd();
                }
                using (var streamReader = process.StandardError) {
                    output += streamReader.ReadToEnd();
                }
                process.WaitForExit();
                Console.WriteLine(output);
                return output;
            }
            catch (Exception) {
                process.Close();
                return output;
            }
        }
    }
}