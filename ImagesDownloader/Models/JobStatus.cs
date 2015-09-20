namespace ImagesDownloader.Models
{
    /// <summary>
    /// All posible job statuses
    /// </summary>
    public enum JobStatus
    {
        Succeeded,
        Processing,
        Failed,
        Deleted,
        Scheduled,
        Enqueued
    }
}