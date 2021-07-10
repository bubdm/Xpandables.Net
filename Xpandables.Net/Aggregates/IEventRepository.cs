using System.Text.Json;

namespace Xpandables.Net.Aggregates
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEventRepository
    {
        /// <summary>
        /// Gets or sets the current <see cref="JsonSerializerOptions"/> to be used for serialization.
        /// </summary>
        JsonSerializerOptions? SerializerOptions { get; set; }

        /// <summary>
        /// Gets or sets the current <see cref="JsonDocumentOptions"/> to be used for <see cref="JsonDocument"/> parsing.
        /// </summary>
        JsonDocumentOptions DocumentOptions { get; set; }
    }
}
