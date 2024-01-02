using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TamakenService.Log
{
    public class NlogService
    {
        private ILogger _logger;
        public NlogService(string logFilePath)
        {
            var config = new NLog.Config.LoggingConfiguration();

            var fileTarget = new NLog.Targets.FileTarget("fileTarget")
            {
                FileName = logFilePath,
                Layout = "${longdate} ${level} ${message} ${exception:format=tostring}"
            };

            config.AddRuleForAllLevels(fileTarget);

            LogManager.Configuration = config;
            _logger = LogManager.GetCurrentClassLogger();
        }
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
            _logger.Info(message);
        }
        public string? ReadLine()
        {
            string? line = Console.ReadLine();
            return line;
        }
        public void LogInfo(string message)
        {
            _logger.Info(message);
        }

        public void LogError(string message, System.Exception ex = null)
        {
            Console.WriteLine(message);
            _logger.Error(ex, message);
        }
    }
}
