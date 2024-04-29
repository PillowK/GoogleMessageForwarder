using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using System.Collections.Generic;

namespace GoogleMessage.Services
{
    public interface IHtmlParseService
    {
        List<string> ParseMessages(string html);
    }

    public class HtmlParseService : IHtmlParseService
    {
        public List<string> ParseMessages(string html)
        {
            List<string> returnList = new List<string>();

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var messages = htmlDoc.DocumentNode.QuerySelectorAll("div.text-msg-content > div.msg-content > div.ng-star-inserted");
                          
            foreach(var message in messages)
            {
                returnList.Add(message.InnerText);
            }            

            return returnList;
        }
    }
}
