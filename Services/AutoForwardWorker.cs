using GoogleMessage.Windows;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleMessage.Services
{
    public class AutoForwardWorker : BackgroundService
    {
        [DllImportAttribute("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minimumWorkingSetSize, int maximumWorkingSetSize);       

        private readonly ILogger<AutoForwardWorker> _logger;
        private readonly IHtmlParseService _htmlParseService;

        private readonly List<string> messagesBefore;

        public AutoForwardWorker(
            ILogger<AutoForwardWorker> logger,
            IHtmlParseService htmlParseService)
        {
            _logger = logger;
            _htmlParseService = htmlParseService;

            messagesBefore = new List<string>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"AutoForwardWorker Start");
            while(!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var mainWindow = App.ServiceProvider.GetService(typeof(MainWindow)) as MainWindow;
                    var webView = mainWindow.webView;

                    var scriptResult = await webView.CoreWebView2.ExecuteScriptAsync("document.getElementsByClassName('content')[0].outerHTML");

                    if (scriptResult != null)
                    {
                        var jObject = JsonObject.Parse(scriptResult);
                        string html = jObject.GetValue<string>();

                        List<string> messages = _htmlParseService.ParseMessages(html);
                        List<string> newMessages = messages.Except(messagesBefore).ToList();

                        if (newMessages.Count > 0)
                        {
                            //SendMail
                        }

                        messagesBefore.AddRange(messages);
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }

                CollectGarbage();
                await Task.Delay(1000);
            }
            _logger.LogInformation($"AutoForwardWorker Stop");
        }

        protected void CollectGarbage()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
        }
    }
}
