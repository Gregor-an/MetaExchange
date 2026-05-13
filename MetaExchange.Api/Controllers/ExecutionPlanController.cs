using MetaExchange.Api.Configuration;
using MetaExchange.Api.Models;
using MetaExchange.Core.Models;
using MetaExchange.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MetaExchange.Api.Controllers
{
    /// <summary>
    /// Provides endpoints for computing best BTC execution plans across multiple exchanges.
    /// </summary>
    [ApiController]
    [Route("api/execution-plans")]
    public sealed class ExecutionPlanController : ControllerBase
    {
        private readonly IExecutionPlanService _service;
        private readonly IOrderBookReader _orderBookReader;
        private readonly ExchangeSettings _settings;

        /// <summary>
        /// Initializes a new instance of <see cref="ExecutionPlanController"/>.
        /// </summary>
        public ExecutionPlanController(IExecutionPlanService service, IOrderBookReader orderBookReader, IOptions<ExchangeSettings> settings)
        {
            _service = service;
            _orderBookReader = orderBookReader;
            _settings = settings.Value;
        }

        /// <summary>
        /// Returns the best execution plan for buying or selling a specified amount of BTC.
        /// </summary>
        /// <remarks>
        /// The algorithm collects all eligible orders across exchanges, sorts them by best price,
        /// and greedily fills the requested amount while respecting each exchange's balance constraints.
        /// </remarks>
        /// <param name="request">Order side (Buy or Sell) and the BTC amount to execute.</param>
        /// <param name="cancellationToken">Token used to cancel the request.</param>
        /// <returns>An execution plan containing the list of orders and summary statistics.</returns>
        /// <response code="200">The execution plan was computed successfully.</response>
        /// <response code="400">The request is invalid.</response>
        /// <response code="500">The server is not configured correctly or the data file cannot be read.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ExecutionPlan), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ExecutionPlan>> CreateExecutionPlan([FromBody] ExecutionPlanRequest request, CancellationToken cancellationToken)
        {
            if (request.Amount <= 0)
            {
                return BadRequest("Amount must be greater than zero.");
            }

            string dataFile = Path.IsPathRooted(_settings.DataFile)
                ? _settings.DataFile
                : Path.Combine(AppContext.BaseDirectory, _settings.DataFile);

            if (!System.IO.File.Exists(dataFile))
            {
                return Problem(detail: $"Data file not found: {dataFile}", statusCode: StatusCodes.Status500InternalServerError);
            }

            List<OrderBook> orderBooks = await _orderBookReader.ReadFromFileAsync(dataFile, _settings.Exchanges.Count, cancellationToken);

            if (orderBooks.Count == 0)
            {
                return Problem(detail: "No exchanges are configured.", statusCode: StatusCodes.Status500InternalServerError);
            }

            List<Exchange> exchanges = _settings.Exchanges
                .Take(orderBooks.Count)
                .Select((config, i) => new Exchange
                {
                    Id = config.Id,
                    EurBalance = config.EurBalance,
                    BtcBalance = config.BtcBalance,
                    OrderBook = orderBooks[i],
                })
                .ToList();

            ExecutionPlan plan = _service.BuildExecutionPlan(exchanges, request.Side, request.Amount);
            return Ok(plan);
        }
    }
}
