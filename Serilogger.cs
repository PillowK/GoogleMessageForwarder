using Serilog;
using System;
using System.IO;

namespace GoogleMessage
{
    public class Serilogger
    {
        public static ILogger ConfigureLogger()
        {
            string logPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "log",
                "log.log");

            return new LoggerConfiguration()                                  
                    .WriteTo.Console()
                    .WriteTo.File(
                        logPath,                      
                        retainedFileCountLimit: 10,
                        rollingInterval: RollingInterval.Day)              
                .CreateLogger();
        }
    }
}
