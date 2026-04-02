using HtmlAgilityPack;

namespace JobMiner.Providers.Apinfo
{
    /// <summary>
    /// Parses raw HTML from Apinfo into intermediate DTOs.
 /// Full implementation is a future step.
  /// </summary>
    public class ApinfoParser
    {
/// <summary>Returns parsed vacancy nodes from a listing HTML page.</summary>
        public IEnumerable<HtmlNode> ParseVacancyNodes(string html)
 {
 var doc = new HtmlDocument();
       doc.LoadHtml(html);
   // TODO: select correct XPath/CSS selectors for Apinfo listing
  return Enumerable.Empty<HtmlNode>();
     }
    }
}
