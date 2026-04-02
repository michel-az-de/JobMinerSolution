namespace JobMiner.Providers.Apinfo
{
    /// <summary>
    /// Fetches raw HTML pages from the Apinfo vacancy listing.
    /// Full implementation is a future step.
  /// </summary>
    public class ApinfoCrawler
    {
     private readonly ApinfoSessionClient _session;

   public ApinfoCrawler(ApinfoSessionClient session)
 {
       _session = session;
   }

 public Task<string> FetchListingPageAsync(string keywords, int page = 1, CancellationToken cancellationToken = default)
 {
       // TODO: build URL and fetch HTML via _session
     return Task.FromResult(string.Empty);
     }
    }
}
