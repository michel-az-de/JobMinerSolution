using JobMiner.Core.Entities;

namespace JobMiner.Core.Contracts
{
    public interface IJobProvider
    {
        string ProviderName { get; }
        Task<JobSearchResult> SearchAsync(JobSearchRequest request, CancellationToken cancellationToken = default);
    }
}
