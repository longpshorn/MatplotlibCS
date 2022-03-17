using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NLog;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MatplotlibCS
{
    /// <summary>
    /// Обёртка над питоновским скриптом построения графиков
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class MatplotlibCS
    {
        #region Fields

        private readonly ILogger _log;

        /// <summary>
        /// Пусть к интерпрететору питона
        /// </summary>
        private readonly string _pythonExePath;

        /// <summary>
        /// Путь к скрипту dasPlot.py
        /// </summary>
        private readonly string _dasPlotPyPath;

        /// <summary>
        /// Path to build_figure.py
        /// </summary>
        private readonly string _buildFigurePyPath;

        /// <summary>
        /// Путь директории, в которой хранятся временные json-файлы, через которые передаются параметры задачи
        /// </summary>
        private readonly string _jsonTempPath;

        /// <summary>
        /// Python web service URL
        /// </summary>
        private readonly string _serviceUrlCheckAliveMethod = "http://127.0.0.1:57123/";

        /// <summary>
        /// Python web service URL
        /// </summary>
        private readonly string _serviceUrlPlotMethod = "http://127.0.0.1:57123/plot";

        /// <summary>
        /// Kill web service url
        /// </summary>
        //private string _serviceUrlKillMethod = "http://127.0.0.1:57123/kill";

        #endregion

        #region .ctor

        /// <summary>
        /// Обёртка над python скриптом, строящим matplotlib графики
        /// </summary>
        /// <param name="pythonExePath">Путь python.exe</param>
        /// <param name="dasPlotPyPath">Путь dasPlot.py</param>
        /// <param name="buildFigurePyPath">Path to build_figure.py</param>
        /// <param name="jsonTempPath">Опциональный путь директории, в которой хранятся временные json файлы, через которые передаются данные</param>
        public MatplotlibCS(string pythonExePath, string dasPlotPyPath, string buildFigurePyPath, string jsonTempPath = "c:\\MatplotlibCS")
        {
            _pythonExePath = pythonExePath;
            _dasPlotPyPath = dasPlotPyPath;
            _buildFigurePyPath = buildFigurePyPath;
            _jsonTempPath = jsonTempPath;
            _log = LogManager.GetLogger(typeof(MatplotlibCS).Name);

            if (!Directory.Exists(_jsonTempPath))
                Directory.CreateDirectory(_jsonTempPath);
        }

        #endregion

        #region Public methods

        public async Task BuildFigureViaFile(Figure task)
        {
            task.HealthCheck();

            try
            {
                var json = BuildJson(task);
                var fullpath = Path.Combine(AppContext.BaseDirectory, $"matplotlib_cs.input.json");
                await File.WriteAllTextAsync(fullpath, json);
                await RunPython(fullpath);
            }
            catch (Exception ex)
            {
                _log.Fatal($"Error while building figure: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Выполняет задачу построения графиков
        /// </summary>
        /// <param name="task">Описание задачи</param>
        public async Task BuildFigure(Figure task)
        {
            task.HealthCheck();

            try
            {
                await LaunchPythonWebServiceAsync();

                var json = BuildJson(task);

                JsonConvert.DefaultSettings = (() =>
                {
                    var settings = new JsonSerializerSettings();
                    settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
                    return settings;
                });
                using var client = new HttpClient();
                var content = new StringContent(JsonConvert.SerializeObject(json), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(_serviceUrlPlotMethod, content);
                var responseString = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _log.Fatal($"Error while building figure: {ex.Message}\n{ex.StackTrace}");
            }
        }

        #endregion

        #region Private methods

        private string BuildJson(Figure task)
        {
            if (!Path.IsPathRooted(task.FileName))
                task.FileName = Path.Combine(_jsonTempPath, task.FileName);

            var serializer = new JsonSerializer() { StringEscapeHandling = StringEscapeHandling.EscapeHtml };
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, task);
            }

            var json = sb.ToString();
            return json;
        }

        private async Task RunPython(string inputfile)
        {
            var ps = new ProcessStartInfo
            {
                FileName = _pythonExePath,
                Arguments = $"{_buildFigurePyPath} {inputfile}",
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            using Process process = Process.Start(ps);
            await process.WaitForExitAsync();
        }

        /// <summary>
        /// Check if python web service is alive and if no, launches it
        /// </summary>
        private async Task LaunchPythonWebServiceAsync()
        {
            if (!await CheckIfWebServiceIsUpAndRunningAsync())
            {
                var psi = new ProcessStartInfo(_pythonExePath, _dasPlotPyPath)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                _log.Info($"Starting python process {_pythonExePath}, {_dasPlotPyPath}");
                Process.Start(psi);

                // when starting python process, it's better to wait for some time to ensure, that
                // web service started
                Thread.Sleep(2000);
            }
        }

        /// <summary>
        /// Check if python web service is alive
        /// </summary>
        /// <returns>true if service is up and running</returns>
        private async Task<bool> CheckIfWebServiceIsUpAndRunningAsync()
        {
            try
            {
                _log.Info("Check if python web-service is already running");
                var client = new HttpClient();
                using var response = await client.GetAsync(_serviceUrlCheckAliveMethod);

                _log.Info($"Service response status is {response.StatusCode}");

                return (response.StatusCode == HttpStatusCode.OK);
            }
            catch (Exception)
            {
                _log.Info("Python web-service wasn't found");
                //Any exception will returns false.
                return false;
            }
        }

        #endregion
    }
}
