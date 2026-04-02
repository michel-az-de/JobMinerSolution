namespace JobMiner.Core.Entities
{
    public class JobSearchResult
    {
  public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
  public List<JobVacancy> Vacancies { get; set; } = new();
    }
}
