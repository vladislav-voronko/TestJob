using TestJob.Models;

namespace TestJob.Interfaces
{
    public interface ITestJobService
    {
        public Task<TestJobResponse> Parse(TestJobRequest model, CancellationToken cancellationToken);
    }
}
