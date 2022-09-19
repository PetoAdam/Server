using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyServer
{
    public class Match1v1
    {
        public Player player1 { get; set; }
        public Player player2 { get; set; }

        public string host { get; set; }

        public Match1v1()
        {
            //new System.Threading.Thread(LaunchContainer).Start();
        }

        public void LaunchContainer()
        {
            var processInfo = new ProcessStartInfo("docker", $"run --rm --net=host gameimage");

            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;

            int exitCode;
            using (var process = new Process())
            {
                process.StartInfo = processInfo;

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit(1200000);
                if (!process.HasExited)
                {
                    process.Kill();
                }

                exitCode = process.ExitCode;
                process.Close();
            }
        }
    }
}
