namespace ImagesDownloader.Models
{
    /// <summary>
    /// Image information
    /// </summary>
    public class ImageInfoResult
    {
        /// <summary>
        /// Local URL
        /// </summary>
        public string LocalUrl { get; set; }

        /// <summary>
        /// Remote URL
        /// </summary>
        public string RemoteUrl { get; set; }

        /// <summary>
        /// Width
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Size
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Content-type
        /// </summary>
        public string ContentType { get; set; }
    }
}