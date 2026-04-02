namespace JobMiner.Providers.Apinfo
{
    /// <summary>
    /// Manages HTTP session (cookies, auth headers) for the Apinfo portal.
    /// Full implementation is a future step.
    /// </summary>
    public class ApinfoSessionClient : IDisposable
    {
 private readonly HttpClient _http = new();

  public bool IsAuthenticated { get; private set; }

        public Task<bool> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
   {
   // TODO: implement form-based login against Apinfo portal
      return Task.FromResult(false);
        }

        public void Dispose() => _http.Dispose();
    }
}
