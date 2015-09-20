using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using Hangfire;
using ImagesDownloader.Models;
using ImagesDownloader.Services;
using Newtonsoft.Json;

namespace ImagesDownloader.Controllers
{
    /// <summary>
    /// API for creating jobs that download images.
    /// </summary>
    public class JobsController : ApiController
    {
        private readonly ImagesService _imagesService = new ImagesService();
        private readonly JobInfoService _jobInfoService = new JobInfoService();

        /// <summary>
        /// Get job by id.
        /// </summary>
        /// <param name="id">Job id.</param>
        /// <returns>Returns job information (status and result) in HttpResponseMessage.</returns>
        public HttpResponseMessage GetById([FromUri] int id)
        {
            try
            {
                var jobInfo = _jobInfoService.GetJobInfoById(id);

                return Request.CreateResponse(HttpStatusCode.OK, jobInfo, JsonMediaTypeFormatter.DefaultMediaType);
            }
            // TODO: Catch not found exception
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return Request.CreateResponse(HttpStatusCode.NotFound, "Job with current identifier was not found", JsonMediaTypeFormatter.DefaultMediaType);
            }
        }

        /// <summary>
        /// Run job for downloading images.
        /// </summary>
        /// <param name="value">Page URL.</param>
        /// <returns>Returns job id in HttpResponseMessage.</returns>
        public HttpResponseMessage Post([FromBody] dynamic value)
        {
            try
            {
                UrlInfo urlInfo = JsonConvert.DeserializeObject<UrlInfo>(value.ToString());
                Uri urlResult;
                var isValidUrl = Uri.TryCreate(urlInfo.Value, UriKind.Absolute, out urlResult) && urlResult.Scheme == Uri.UriSchemeHttp;

                if (isValidUrl)
                {
                    var jobId = BackgroundJob.Enqueue(() => _imagesService.GetImagesByUrl(urlInfo.Value));

                    return Request.CreateResponse(HttpStatusCode.OK, jobId, JsonMediaTypeFormatter.DefaultMediaType);
                }

                return Request.CreateResponse(HttpStatusCode.BadRequest, "Not correct request", JsonMediaTypeFormatter.DefaultMediaType);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Ooops, something went wrong", JsonMediaTypeFormatter.DefaultMediaType);
            }
        }
    }
}