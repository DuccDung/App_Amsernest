using WebSearchLink.Models;

namespace WebSearchLink.Models
{
    public interface IYouTubeService
    {
        Task<List<string>> UploadAllVideosInFolderAsync();
    }
}
