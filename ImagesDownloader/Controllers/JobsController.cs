using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using Hangfire;
using ImagesDownloader.Services;

namespace ImagesDownloader.Controllers
{
    /// <summary>
    /// API for creating jobs that downloads images
    /// </summary>
    public class JobsController : ApiController
    {
        private readonly ImagesService _imagesService = new ImagesService();
        private readonly JobInfoService _jobInfoService = new JobInfoService();

        /// <summary>
        /// Get job by id (GET api/jobs/5)
        /// </summary>
        /// <param name="id">Job Id</param>
        /// <returns>Job information</returns>
        public HttpResponseMessage GetById([FromUri] int id)
        {
            try
            {
                var jobInfo = _jobInfoService.GetJobInfoById(id);

                return Request.CreateResponse(HttpStatusCode.OK, jobInfo, JsonMediaTypeFormatter.DefaultMediaType);
            }
            // TODO: Catch not found exception
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Job with current identifier was not found", JsonMediaTypeFormatter.DefaultMediaType);
            }
        }

        /// <summary>
        /// Run job for downloading images (POST api/jobs)
        /// </summary>
        /// <param name="url">Page URL</param>
        /// <returns>Job Id</returns>
        public HttpResponseMessage Post([FromBody] string url)
        {
            Uri urlResult;
            var isUrl = Uri.TryCreate(url, UriKind.Absolute, out urlResult) && urlResult.Scheme == Uri.UriSchemeHttp;

            try
            {
                if (isUrl)
                {
                    var jobId = BackgroundJob.Enqueue(() => _imagesService.GetImagesByUrl(url));

                    return Request.CreateResponse(HttpStatusCode.OK, jobId, JsonMediaTypeFormatter.DefaultMediaType);
                }

                return Request.CreateResponse(HttpStatusCode.BadRequest, "Not correct request", JsonMediaTypeFormatter.DefaultMediaType);
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Ooops, something went wrong", JsonMediaTypeFormatter.DefaultMediaType);
            }
        }
    }
}