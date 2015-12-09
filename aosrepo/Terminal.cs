using System;
using System.Diagnostics;

namespace aosrepo {
    public class Terminal {
        public static string Execute(string command, string dir = "") {
            var error = string.Empty;
            var process = new Process {
                StartInfo = {
                    FileName = "bash",
                    Arguments = $"-c \"{command}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                }
            };
            if (dir.Length > 0) {
                process.StartInfo.WorkingDirectory = dir;
            }
            try {
                process.Start();
                string output;
                using (var streamReader = process.StandardOutput) {
                    output = streamReader.ReadToEnd();
                }
                using (var streamReader = process.StandardError) {
                    error = streamReader.ReadToEnd();
                }
                process.WaitForExit();
                return output;
            }
            catch (Exception) {
                process.Close();
                return error;
            }
        }
    }
}