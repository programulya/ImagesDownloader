using System;

namespace ImagesDownloader.Exceptions
{
    /// <summary>
    /// Exception if job was not found.
    /// </summary>
    public class JobNotFoundException : Exception
    {
        /// <summary>
        /// Create exception if job was not found.
        /// </summary>
        /// <param name="message">Message.</param>
        public JobNotFoundException(string message)
            : base(message)
        {
        }
    }
}