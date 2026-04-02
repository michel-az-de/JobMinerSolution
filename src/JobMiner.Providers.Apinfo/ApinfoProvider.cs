using JobMiner.Core.Contracts;
using JobMiner.Core.Entities;

namespace JobMiner.Providers.Apinfo
{
    /// <summary>
    /// Implements <see cref="IJobProvider"/> for the Apinfo portal.
    /// Orchestrates session, crawler, parser and mapper.
  /// </summary>
    public class ApinfoProvider : IJobProvider
    {
     private readonly ApinfoSessionClient _session;
      private readonly ApinfoCrawler _crawler;
        private readonly ApinfoParser _parser;
        private readonly ApinfoMapper _mapper;

  public ApinfoProvider(ApinfoSessionClient session)
        {
     _session = session;
   _crawler  = new ApinfoCrawler(session);
     _parser = new ApinfoParser();
_mapper   = new ApinfoMapper();
  }

  public string ProviderName => "Apinfo";

   public async Task<JobSearchResult> SearchAsync(JobSearchRequest request, CancellationToken cancellationToken = default)
    {
         // TODO: paginate, parse and map results
            var html = await _crawler.FetchListingPageAsync(request.Keywords, cancellationToken: cancellationToken);
         var nodes = _parser.ParseVacancyNodes(html);
        var vacancies = nodes.Select(_mapper.Map).ToList();

   return new JobSearchResult
        {
          Success   = true,
     Vacancies = vacancies
     };
 }
    }
}
