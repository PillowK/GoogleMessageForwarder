using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace GoogleMessage.Services
{
    public interface IHtmlParseService
    {
        List<string> ParseMessages(string html);
    }

    public class HtmlParseService : IHtmlParseService
    {
        private readonly ILogger<HtmlParseService> _logger;

        public HtmlParseService(
            ILogger<HtmlParseService> logger)
        {
            _logger = logger;
        }

        public List<string> ParseMessages(string html)
        {
            List<string> returnList = new List<string>();

            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);
                var messages = htmlDoc.DocumentNode.QuerySelectorAll("div.text-msg-content > div.msg-content > div.ng-star-inserted");

                foreach (var message in messages)
                {
                    returnList.Add(message.InnerText);
                }               
            }
            catch(Exception ex)
            {                
                _logger.LogError(ex.ToString());
            }

            if(returnList.Count == 0)
            {
                _logger.LogDebug($"Count 0 Debug: {html}");
            }

            return returnList;
        }
    }
}
