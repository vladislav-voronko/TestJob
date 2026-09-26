using TestJob.Models;

namespace TestJob.Interfaces
{
    public interface ITestJobService
    {
        public Task<TestJobResponse> ProcessAsync(TestJobRequest model, CancellationToken cancellationToken);
    }
}
