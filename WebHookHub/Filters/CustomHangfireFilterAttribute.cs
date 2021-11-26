using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;
using System;

namespace WebHookHub.Filters
{
    /// <summary>
    /// Custom Hangfire attribute to Preserve Queue and Job Expiration
    /// </summary>
    public class CustomHangfireFilterAttribute : JobFilterAttribute, IApplyStateFilter
    {
        /// <summary>
        /// On State Applied
        /// </summary>
        /// <param name="context"></param>
        /// <param name="transaction"></param>
        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            context.JobExpirationTimeout = TimeSpan.FromDays(7);
            // Activating only when enqueueing a background job
            if (!(context.NewState is EnqueuedState enqueuedState)) return;

            // Checking if an original queue is already set
            var originalQueue = SerializationHelper.Deserialize<string>(
                context.Connection.GetJobParameter(
                    context.BackgroundJob.Id,
                    "OriginalQueue")
            );

            if (originalQueue != null)
            {
                // Override any other queue value that is currently set (by other filters, for example)
                enqueuedState.Queue = originalQueue;
            }
            else
            {
                // Queueing for the first time, we should set the original queue
                context.Connection.SetJobParameter(
                    context.BackgroundJob.Id,
                    "OriginalQueue",
                    SerializationHelper.Serialize(enqueuedState.Queue));
            }
        }
        /// <summary>
        /// On State unapplied
        /// </summary>
        /// <param name="context"></param>
        /// <param name="transaction"></param>
        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            context.JobExpirationTimeout = TimeSpan.FromDays(7);
        }
    }
}
