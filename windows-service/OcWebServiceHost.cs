using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Web.Script.Serialization;

namespace OcWebServiceHost
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0 && string.Equals(args[0], "--console", StringComparison.OrdinalIgnoreCase))
            {
                using (var service = new OcWebDashboardService())
                {
                    service.StartForConsole();
                    Console.WriteLine("OC Web Dashboard service host running. Press Enter to stop.");
                    Console.ReadLine();
                    service.StopForConsole();
                }
                return;
            }

            ServiceBase.Run(new ServiceBase[] { new OcWebDashboardService() });
        }
    }

    internal sealed class OcWebDashboardService : ServiceBase
    {
        private readonly string _baseDirectory;
        private readonly string _configPath;
        private Process _childProcess;
        private StreamWriter _logWriter;
        private readonly object _sync = new object();

        public OcWebDashboardService()
        {
            ServiceName = "OcWebDashboard";
            CanStop = true;
            CanShutdown = true;
            AutoLog = true;
            _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _configPath = Path.Combine(_baseDirectory, "oc-web-service.json");
        }

        public void StartForConsole()
        {
            OnStart(new string[0]);
        }

        public void StopForConsole()
        {
            OnStop();
        }

        protected override void OnStart(string[] args)
        {
            lock (_sync)
            {
                if (_childProcess != null && !_childProcess.HasExited)
                {
                    return;
                }

                var config = LoadConfig();
                Directory.CreateDirectory(Path.GetDirectoryName(config.LogPath));
                _logWriter = new StreamWriter(new FileStream(config.LogPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    AutoFlush = true
                };
                Log("Starting dashboard process");

                var startInfo = new ProcessStartInfo
                {
                    FileName = config.ExecutablePath,
                    Arguments = config.Arguments,
                    WorkingDirectory = config.WorkingDirectory,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                if (!string.IsNullOrWhiteSpace(config.UserProfile))
                {
                    startInfo.EnvironmentVariables["USERPROFILE"] = config.UserProfile;
                    startInfo.EnvironmentVariables["HOME"] = config.UserProfile;

                    var root = Path.GetPathRoot(config.UserProfile);
                    if (!string.IsNullOrWhiteSpace(root) && root.Length >= 2)
                    {
                        startInfo.EnvironmentVariables["HOMEDRIVE"] = root.Substring(0, 2);
                    }

                    if (config.UserProfile.Length > 2)
                    {
                        startInfo.EnvironmentVariables["HOMEPATH"] = config.UserProfile.Substring(2);
                    }
                }

                if (!string.IsNullOrWhiteSpace(config.AppData))
                {
                    startInfo.EnvironmentVariables["APPDATA"] = config.AppData;
                }

                if (!string.IsNullOrWhiteSpace(config.LocalAppData))
                {
                    startInfo.EnvironmentVariables["LOCALAPPDATA"] = config.LocalAppData;
                }

                _childProcess = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
                _childProcess.OutputDataReceived += (_, e) => { if (!string.IsNullOrWhiteSpace(e.Data)) Log("OUT: " + e.Data); };
                _childProcess.ErrorDataReceived += (_, e) => { if (!string.IsNullOrWhiteSpace(e.Data)) Log("ERR: " + e.Data); };
                _childProcess.Exited += (_, __) => Log("Child process exited with code " + _childProcess.ExitCode);
                _childProcess.Start();
                _childProcess.BeginOutputReadLine();
                _childProcess.BeginErrorReadLine();

                Thread.Sleep(2000);
                if (_childProcess.HasExited)
                {
                    throw new InvalidOperationException("Dashboard process exited during startup. See log for details.");
                }

                Log("Dashboard process started with PID " + _childProcess.Id);
            }
        }

        protected override void OnStop()
        {
            lock (_sync)
            {
                StopChildProcess();
                CloseLog();
            }
        }

        protected override void OnShutdown()
        {
            OnStop();
            base.OnShutdown();
        }

        private ServiceConfig LoadConfig()
        {
            if (!File.Exists(_configPath))
            {
                throw new FileNotFoundException("Service config not found", _configPath);
            }

            var serializer = new JavaScriptSerializer();
            var config = serializer.Deserialize<ServiceConfig>(File.ReadAllText(_configPath));
            if (config == null)
            {
                throw new InvalidOperationException("Service config could not be parsed.");
            }

            if (string.IsNullOrWhiteSpace(config.ExecutablePath) || !File.Exists(config.ExecutablePath))
            {
                throw new FileNotFoundException("Configured executable was not found.", config.ExecutablePath);
            }

            if (string.IsNullOrWhiteSpace(config.WorkingDirectory) || !Directory.Exists(config.WorkingDirectory))
            {
                throw new DirectoryNotFoundException("Configured working directory was not found: " + config.WorkingDirectory);
            }

            if (string.IsNullOrWhiteSpace(config.LogPath))
            {
                config.LogPath = Path.Combine(_baseDirectory, "oc-web-service.log");
            }

            return config;
        }

        private void StopChildProcess()
        {
            if (_childProcess == null)
            {
                return;
            }

            try
            {
                if (!_childProcess.HasExited)
                {
                    Log("Stopping dashboard process PID " + _childProcess.Id);
                    using (var killer = new Process())
                    {
                        killer.StartInfo = new ProcessStartInfo
                        {
                            FileName = Path.Combine(Environment.SystemDirectory, "taskkill.exe"),
                            Arguments = "/PID " + _childProcess.Id + " /T /F",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        };
                        killer.Start();
                        killer.WaitForExit(10000);
                        var stdout = killer.StandardOutput.ReadToEnd();
                        var stderr = killer.StandardError.ReadToEnd();
                        if (!string.IsNullOrWhiteSpace(stdout)) Log("taskkill: " + stdout.Trim());
                        if (!string.IsNullOrWhiteSpace(stderr)) Log("taskkill error: " + stderr.Trim());
                    }
                }
            }
            finally
            {
                _childProcess.Dispose();
                _childProcess = null;
            }
        }

        private void Log(string message)
        {
            var line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + message;
            if (_logWriter != null)
            {
                _logWriter.WriteLine(line);
            }
        }

        private void CloseLog()
        {
            if (_logWriter == null)
            {
                return;
            }

            _logWriter.Flush();
            _logWriter.Dispose();
            _logWriter = null;
        }
    }

    internal sealed class ServiceConfig
    {
        public string ExecutablePath { get; set; }
        public string Arguments { get; set; }
        public string WorkingDirectory { get; set; }
        public string LogPath { get; set; }
        public string UserProfile { get; set; }
        public string AppData { get; set; }
        public string LocalAppData { get; set; }
    }
}
