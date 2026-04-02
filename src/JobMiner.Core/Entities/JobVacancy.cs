using JobMiner.Core.Enums;

namespace JobMiner.Core.Entities
{
    public class JobVacancy
    {
    public int Id { get; set; }
        public string Source { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
 public string ContactEmail { get; set; } = string.Empty;
  public string ContactPhone { get; set; } = string.Empty;
        public VacancyStatus Status { get; set; }
        public string PostedDate { get; set; } = string.Empty;
        public int MatchScore { get; set; }
    }
}
