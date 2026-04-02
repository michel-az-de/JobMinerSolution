namespace JobMiner.Core.Entities
{
    public class CandidateProfile
    {
  public string Name { get; set; } = string.Empty;
   public string Email { get; set; } = string.Empty;
 public List<string> Skills { get; set; } = new();
        public List<string> Keywords { get; set; } = new();
        public string ResumeText { get; set; } = string.Empty;
    }
}
