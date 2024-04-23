using HtmlAgilityPack;
using System.Collections.Generic;
using System.Windows.Documents;

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

            var messageWrappers = htmlDoc.DocumentNode.SelectNodes(".//div[@class=\"ng-star-inserted\"]");

            if(messageWrappers != null && messageWrappers.Count > 0)
            {
                foreach(var messageWrapper in messageWrappers )
                {
                    returnList.Add(messageWrapper.InnerText);
                }
            }

            return returnList;
        }
    }
}
