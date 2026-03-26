using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;

namespace HQSOFT.Order.SaleOrders;

public class RecordingBackgroundJobManager : IBackgroundJobManager
{
    public ConcurrentBag<(object Args, BackgroundJobPriority Priority, int? DelaySeconds, TimeSpan? DelayTimeSpan)> EnqueuedJobs { get; } = [];

    public Task<string> EnqueueAsync<TArgs>(TArgs args, BackgroundJobPriority priority = BackgroundJobPriority.Normal, int? delay = null)
    {
        EnqueuedJobs.Add((args!, priority, delay, null));
        return Task.FromResult("job-id");
    }

    public Task<string> EnqueueAsync<TArgs>(TArgs args, BackgroundJobPriority priority = BackgroundJobPriority.Normal, TimeSpan? delay = null)
    {
        EnqueuedJobs.Add((args!, priority, null, delay));
        return Task.FromResult("job-id");
    }
}
