namespace JobMiner.Core.Entities
{
    public class JobSearchRequest
    {
 public string Keywords { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
        public int MaxResults { get; set; } = 50;
    }
}
