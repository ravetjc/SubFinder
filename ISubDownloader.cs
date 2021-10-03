using System.Threading.Tasks;

namespace SubFinder
{
    /// <summary>
    /// Interface to implement website api
    /// </summary>
    public interface ISubDownloader
    {

        /// <summary>
        /// Task that will download the best matching subtitle(s) for the specified parameters if any
        /// </summary>
        /// <param name="show">TV show</param>
        /// <param name="season">Season number</param>
        /// <param name="number">Episode number</param>
        /// <param name="team">detected team (for synced subs)</param>
        /// <param name="filename">Filename (to name sub file accordingly to the episode file)</param>
        /// <returns>The task that will handle the seek and download</returns>
        Task<string> FindSubtitle(string serie, string saison, string numero, string team, string filenameWithoutExtension);
    }
}
