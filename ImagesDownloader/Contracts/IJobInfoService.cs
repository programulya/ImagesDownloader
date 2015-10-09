using ImagesDownloader.Models;

namespace ImagesDownloader.Contracts
{
    /// <summary>
    /// Job information service.
    /// </summary>
    internal interface IJobInfoService
    {
        /// <summary>
        /// Get job information.
        /// </summary>
        /// <param name="id">Job Id.</param>
        /// <returns>Information about job.</returns>
        JobInfoResult GetJobInfoById(int id);
    }
}