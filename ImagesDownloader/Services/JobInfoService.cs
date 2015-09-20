using System;
using System.Collections.Generic;
using System.Linq;
using Hangfire;
using Hangfire.Storage;
using ImagesDownloader.Contracts;
using ImagesDownloader.Exceptions;
using ImagesDownloader.Models;

namespace ImagesDownloader.Services
{
    /// <summary>
    /// Job information service
    /// </summary>
    public class JobInfoService : IJobInfoService
    {
        /// <summary>
        /// Get job information
        /// </summary>
        /// <param name="id">Job id</param>
        /// <returns>Returns information</returns>
        public JobInfoResult GetJobInfoById(int id)
        {
            var api = JobStorage.Current.GetMonitoringApi();
            var jobInfo = GetJobInfoFromSucceededJobs(id, api);
            if (jobInfo != null)
                return jobInfo;

            jobInfo = GetJobInfoFromPrecessingJobs(id, api);
            if (jobInfo != null)
                return jobInfo;

            jobInfo = GetJobInfoFromFailedJobs(id, api);
            if (jobInfo != null)
                return jobInfo;

            jobInfo = GetJobInfoFromScheduledJobs(id, api);
            if (jobInfo != null)
                return jobInfo;

            jobInfo = GetJobInfoFromDeletedJobs(id, api);
            if (jobInfo != null)
                return jobInfo;

            jobInfo = GetJobInfoFromEnqueuedJobs(id, api);
            if (jobInfo != null)
                return jobInfo;

            throw new JobNotFoundException("Job was not found");
        }

        #region Private methods

        private static JobInfoResult GetJobInfoFromSucceededJobs(int id, IMonitoringApi api)
        {
            var jobinfo = new JobInfoResult {Result = new List<ImageInfoResult>()};
            var succeededJobDto =
                api.SucceededJobs(0, int.MaxValue).FirstOrDefault(job => job.Key == id.ToString()).Value;

            if (succeededJobDto != null)
            {
                jobinfo.Status = JobStatus.Succeeded.ToString();

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

            return null;
        }

        private static JobInfoResult GetJobInfoFromPrecessingJobs(int id, IMonitoringApi api)
        {
            var jobinfo = new JobInfoResult {Result = new List<ImageInfoResult>()};
            var processingJobDto =
                api.ProcessingJobs(0, int.MaxValue).FirstOrDefault(job => job.Key == id.ToString()).Value;
            if (processingJobDto != null)
            {
                jobinfo.Status = JobStatus.Processing.ToString();

                return jobinfo;
            }

            return null;
        }

        private static JobInfoResult GetJobInfoFromFailedJobs(int id, IMonitoringApi api)
        {
            var jobinfo = new JobInfoResult {Result = new List<ImageInfoResult>()};
            var failedJobDto = api.FailedJobs(0, int.MaxValue).FirstOrDefault(job => job.Key == id.ToString()).Value;
            if (failedJobDto != null)
            {
                jobinfo.Status = JobStatus.Failed.ToString();

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

            return null;
        }

        private static JobInfoResult GetJobInfoFromScheduledJobs(int id, IMonitoringApi api)
        {
            var jobinfo = new JobInfoResult {Result = new List<ImageInfoResult>()};
            var scheduledJobDto =
                api.ScheduledJobs(0, int.MaxValue).FirstOrDefault(job => job.Key == id.ToString()).Value;
            if (scheduledJobDto != null)
            {
                jobinfo.Status = JobStatus.Scheduled.ToString();
                return jobinfo;
            }

            return null;
        }

        private static JobInfoResult GetJobInfoFromDeletedJobs(int id, IMonitoringApi api)
        {
            var jobinfo = new JobInfoResult {Result = new List<ImageInfoResult>()};
            var deletedJobDto = api.DeletedJobs(0, int.MaxValue).FirstOrDefault(job => job.Key == id.ToString()).Value;

            if (deletedJobDto != null)
            {
                jobinfo.Status = JobStatus.Deleted.ToString();

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

            return null;
        }

        private static JobInfoResult GetJobInfoFromEnqueuedJobs(int id, IMonitoringApi api)
        {
            var jobinfo = new JobInfoResult {Result = new List<ImageInfoResult>()};
            var enqueuedJobDto =
                api.EnqueuedJobs("default", 0, 1000)
                    .FirstOrDefault(job => job.Key == id.ToString())
                    .Value;

            if (enqueuedJobDto != null)
            {
                jobinfo.Status = JobStatus.Enqueued.ToString();
                return jobinfo;
            }

            return null;
        }

        #endregion
    }
}