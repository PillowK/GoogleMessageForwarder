using GoogleMessage.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;
using System;
using System.Collections.Generic;
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

        private readonly GraphServiceClient graphServiceClient;

        private readonly string senderAddress;        
        private readonly List<string> recipients;
        private string messsageBefore;

        public AutoForwardWorker(
            ILogger<AutoForwardWorker> logger,
            IConfiguration configuration,
            IGraphConfidentialClientFactory confidentialClientFactory,
            IHtmlParseService htmlParseService)
        {
            _logger = logger;
            _htmlParseService = htmlParseService;           

            graphServiceClient = confidentialClientFactory.GetConfidentialClient();

            senderAddress = configuration.GetValue("AppSettings:senderAddress", "");
            var recipientsSection = configuration.GetSection("AppSettings:forwardRecipients");
            recipients = recipientsSection.AsEnumerable()
                .Select(d => d.Value?.ToLower())
                .Where(d => !string.IsNullOrEmpty(d))
                .ToList();
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
                        string message = messages.Last();

                        if (!string.IsNullOrEmpty(messsageBefore) && !messsageBefore.Equals(message))
                        {                           
                            foreach(var reicipient in recipients)
                            {
                                SendMailPostRequestBody mailBody = new SendMailPostRequestBody();
                                mailBody.SaveToSentItems = false;
                                mailBody.Message = new Message()
                                {
                                    ToRecipients = new List<Recipient>() {
                                        new Recipient() {
                                            EmailAddress = new EmailAddress()
                                            {
                                                Address = reicipient
                                            }
                                        }
                                    },
                                    Subject = message,
                                    Body = new ItemBody() { Content = message, ContentType = BodyType.Html }
                                };

                                await graphServiceClient.Users[senderAddress].SendMail.PostAsync(mailBody);                               
                            }                                                                            
                        }

                        messsageBefore = message;
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

            SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
        }
    }
}
