namespace ImagesDownloader.Contracts
{
    /// <summary>
    /// Download images service
    /// </summary>
    interface IImagesService
    {
        /// <summary>
        /// Get images by URL
        /// </summary>
        /// <param name="url">URL</param>
        void GetImagesByUrl(string url);
    }
}
