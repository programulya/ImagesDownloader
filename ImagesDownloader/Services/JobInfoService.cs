using System;
using System.Collections.Generic;
using System.Linq;
using Hangfire;
using ImagesDownloader.Contracts;
using ImagesDownloader.Models;

namespace ImagesDownloader.Services
{
    public class JobInfoService : IJobInfoService
    {
        public JobInfoResult GetJobInfoById(int id)
        {
            var jobinfo = new JobInfoResult {Result = new List<ImageInfoResult>()};

            var api = JobStorage.Current.GetMonitoringApi();
            var succeededJobDto =
                api.SucceededJobs(0, int.MaxValue).FirstOrDefault(job => job.Key == id.ToString()).Value;

            if (succeededJobDto != null)
            {
                jobinfo.Status = JobStatus.Succeeded.ToString();

                // TODO: Refactor database
                using (var context = new JobsEntities())
                {
                    var result = context.ImageInfoes.Where(i => i.JobId == id).ToList();

                    foreach (var imageResult in result.Select(r => new ImageInfoResult
                    {
                        ContentType = r.ContentType,
                        Width = r.Width,
                        Size = r.Size,
                        Height = r.Height,
                        LocalUrl = r.LocalUrl,
                        RemoteUrl = r.RemoteUrl
                    }))
                    {
                        jobinfo.Result.Add(imageResult);
                    }
                }

                return jobinfo;
            }

            // TODO: Refactor for small methods
            var processingJobDto =
                api.ProcessingJobs(0, int.MaxValue).FirstOrDefault(job => job.Key == id.ToString()).Value;
            if (processingJobDto != null)
            {
                jobinfo.Status = JobStatus.Processing.ToString();
                return jobinfo;
            }

            var failedJobDto = api.FailedJobs(0, int.MaxValue).FirstOrDefault(job => job.Key == id.ToString()).Value;
            if (failedJobDto != null)
            {
                jobinfo.Status = JobStatus.Failed.ToString();
                return jobinfo;
            }

            var scheduledJobDto =
                api.ScheduledJobs(0, int.MaxValue).FirstOrDefault(job => job.Key == id.ToString()).Value;
            if (scheduledJobDto != null)
            {
                jobinfo.Status = JobStatus.Scheduled.ToString();
                return jobinfo;
            }

            var deletedJobDto = api.DeletedJobs(0, int.MaxValue).FirstOrDefault(job => job.Key == id.ToString()).Value;
            if (deletedJobDto != null)
            {
                jobinfo.Status = JobStatus.Deleted.ToString();
                return jobinfo;
            }

            var enqueuedJobDto =
                api.EnqueuedJobs("default", 0, 1000)
                    .FirstOrDefault(job => job.Key == id.ToString())
                    .Value;

            if (enqueuedJobDto != null)
            {
                jobinfo.Status = JobStatus.Enqueued.ToString();
                return jobinfo;
            }

            throw new Exception("Job was not found");
        }
    }
}