using JobMiner.Core.Entities;

namespace JobMiner.Data.Repositories
{
    /// <summary>
    /// Persists and retrieves <see cref="JobVacancy"/> records from SQLite.
    /// Full implementation is a future step.
    /// </summary>
    public class VacancyRepository
    {
        private readonly string _connectionString;

  public VacancyRepository(string connectionString)
      {
          _connectionString = connectionString;
    }

public Task<IEnumerable<JobVacancy>> GetAllAsync()
        {
            // TODO: implement SELECT with Microsoft.Data.Sqlite
         return Task.FromResult(Enumerable.Empty<JobVacancy>());
        }

   public Task SaveAsync(JobVacancy vacancy)
    {
            // TODO: implement INSERT / UPDATE with Microsoft.Data.Sqlite
        return Task.CompletedTask;
 }
    }
}
