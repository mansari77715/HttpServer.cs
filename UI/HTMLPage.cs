//my
using HtmlAgilityPack;
using System.Linq;

namespace HttpUI.UI
{
    public class HTMLPage
    {



        //Variables



        public HtmlDocument duc = new HtmlDocument();
        public HtmlNode head;
        public HtmlNode body;



        //Constructor



        public HTMLPage()
        {

            duc.LoadHtml("<!DOCTYPE html><html dir=\"rtl\"><head><meta charset=\"UTF-8\"><meta name =\"viewport\" content=\"width = device-width, initial-scale = 1\"></head><body></body></html>");

            head = duc.DocumentNode.ChildNodes[1].ChildNodes[0];
            body = duc.DocumentNode.ChildNodes[1].ChildNodes[1];

        }

        public void AddJS(string js) => head.AppendChild(HtmlNode.CreateNode("<script>" + js + "</script>"));
        public void AddJSLink(string link) => head.AppendChild(HtmlNode.CreateNode("<script rel=\"text/javascript\" src=\"" + link + "\"></script>"));

        public void AddCSS(string css) => head.AppendChild(HtmlNode.CreateNode("<style>" + css + "</style>"));
        public void AddCSSLink(string link) => head.AppendChild(HtmlNode.CreateNode("<link rel=\"stylesheet\" href=\"" + link + "\">"));

        public void SetTitle(string title)
        {

            var ts = head.ChildNodes.Descendants("title").ToList();
            if (ts.Count > 0)
                ts[0].InnerHtml = title;
            else
                head.ChildNodes.Add(CreateElement("title", title));

        }

        /// <summary>
        /// without #
        /// </summary>
        public void SetTitleColor(string color)
        {

            head.InnerHtml += string.Format(@"
<meta name=""theme-color"" content=""#{0}"">
<meta name=""msapplication-navbutton-color"" content=""#{0}"" >
<meta name=""apple-mobile-web-app-status-bar-style"" content=""#{0}"" >
            ", color);

        }

        public override string ToString() => duc.DocumentNode.OuterHtml;



        //Statics



        public static HtmlNode CreateElement(string tag, HtmlNode content = null)
        {

            var node = HtmlNode.CreateNode("<" + tag + ">" + "</" + tag + ">");
            if (content != null)
                node.ChildNodes.Add(content);
            return node;

        }

        public static HtmlNode CreateElement(string tag, string content = null) => HtmlNode.CreateNode("<" + tag + ">" + (string.IsNullOrEmpty(content) ? "" : content) + "</" + tag + ">");

        public static string CreateCSS(string css) => "<style>" + css + "</style>";
        public static string CreateJS(string js) => "<script>" + js + "</script>";

    }

}