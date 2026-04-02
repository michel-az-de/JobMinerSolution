using HtmlAgilityPack;
using JobMiner.Core.Entities;

namespace JobMiner.Providers.Apinfo
{
    /// <summary>
    /// Maps an Apinfo <see cref="HtmlNode"/> to a <see cref="JobVacancy"/>.
    /// Full implementation is a future step.
    /// </summary>
    public class ApinfoMapper
    {
      public JobVacancy Map(HtmlNode node)
        {
            // TODO: extract fields from node attributes / inner text
     return new JobVacancy
       {
    Source = "Apinfo"
     };
     }
  }
}
