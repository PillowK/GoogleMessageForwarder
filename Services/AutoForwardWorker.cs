using GoogleMessage.Windows;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleMessage.Services
{
    public class AutoForwardWorker : BackgroundService
    {        
        private readonly IHtmlParseService _htmlParseService;

        private readonly List<string> messagesBefore;

        public AutoForwardWorker(
            IHtmlParseService htmlParseService)
        {
            _htmlParseService = htmlParseService;

            messagesBefore = new List<string>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
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

                await Task.Delay(1000);
            }            
        }
    }
}
