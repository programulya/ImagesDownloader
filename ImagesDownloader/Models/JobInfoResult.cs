using System.Collections.Generic;

namespace ImagesDownloader.Models
{
    /// <summary>
    /// Job information
    /// </summary>
    public class JobInfoResult
    {
        /// <summary>
        /// Current status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Downloaded images
        /// </summary>
        public IList<ImageInfoResult> Result { get; set; }
    }
}