using MetaExchange.Core.Models;

namespace MetaExchange.Core.Services
{
    /// <summary>
    /// Defines the contract for reading order books from a data source.
    /// </summary>
    public interface IOrderBookReader
    {
        /// <summary>
        /// Reads up to <paramref name="count"/> order books from the specified file.
        /// Each line in the file is expected to be a JSON-serialized <see cref="OrderBook"/>.
        /// </summary>
        /// <param name="filePath">Path to the order books data file.</param>
        /// <param name="count">Maximum number of order books to read.</param>
        /// <returns>A list of deserialized <see cref="OrderBook"/> instances.</returns>
        List<OrderBook> ReadFromFile(string filePath, int count);

        /// <summary>
        /// Asynchronously reads up to <paramref name="count"/> order books from the specified file.
        /// Each line in the file is expected to be a JSON-serialized <see cref="OrderBook"/>.
        /// </summary>
        /// <param name="filePath">Path to the order books data file.</param>
        /// <param name="count">Maximum number of order books to read.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>A list of deserialized <see cref="OrderBook"/> instances.</returns>
        Task<List<OrderBook>> ReadFromFileAsync(string filePath, int count, CancellationToken cancellationToken = default);
    }
}
