using System;
using Hangfire.Server;

namespace ImagesDownloader
{
    public class JobContext : IServerFilter
    {
        [ThreadStatic]
        private static string _jobId;

        public static string JobId
        {
            get { return _jobId; }
            private set { _jobId = value; }
        }

        public void OnPerforming(PerformingContext context)
        {
            JobId = context.JobId;
        }

        public void OnPerformed(PerformedContext filterContext)
        {
        }
    }
}