# MetaExchange

Solution for the BSDigital/CML Team coding task.

The application builds a best execution plan for buying or selling a requested BTC amount across multiple crypto exchanges.

## Current status

Implemented:

- Core execution planning logic
- Console application
- Web API endpoint
- Swagger UI
- Reading order books from the provided data file
- Exchange balances from configuration

Planned next:

- Unit tests for the core execution planning logic
- Local Docker setup for the Web API

## Approach

For a BUY request, the algorithm uses asks from all exchanges and fills the order from the lowest available price to the highest available price.

For a SELL request, the algorithm uses bids from all exchanges and fills the order from the highest available price to the lowest available price.

Each exchange has its own EUR and BTC balance. These balances are treated as funds available on that specific exchange. The algorithm does not transfer EUR or BTC between exchanges.

## Requirements

- .NET 8 SDK

Check installed version:

```bash
dotnet --version
```

Expected output:

```bash
8.0.x
```

## Setup

```bash
git clone <repository-url>
cd MetaExchange
dotnet restore
dotnet build
```

## Run console application

Buy BTC:

```bash
dotnet run --project MetaExchange.ConsoleApp -- buy 9
```

Sell BTC:

```bash
dotnet run --project MetaExchange.ConsoleApp -- sell 5
```

Arguments:

```text
<buy|sell>   Order side
<amount>     BTC amount, for example 9 or 0.5
```

## Run Web API

```bash
dotnet run --project MetaExchange.Api
```

Swagger UI is available at `/swagger` on the running API host.

For local development, it is usually available at:

```text
http://localhost:5243/swagger
```

Example request — `POST /api/execution-plans`:

```json
{
  "side": "Buy",
  "amount": 1.5
}
```

`side` accepts `"Buy"` or `"Sell"`.

## Configuration

Exchange balances are configured in `appsettings.json` of the respective project:

```text
MetaExchange.ConsoleApp/appsettings.json
MetaExchange.Api/appsettings.json
```

Example:

```json
{
  "DataFile": "order_books_data",
  "Exchanges": [
    { "Id": "Exchange_1", "EurBalance": 10000, "BtcBalance": 5 },
    { "Id": "Exchange_2", "EurBalance": 5000,  "BtcBalance": 3 },
    { "Id": "Exchange_3", "EurBalance": 15000, "BtcBalance": 8 }
  ]
}
```

The provided data file contains order books only, so exchange balances are provided separately through configuration.

Each line in the data file is treated as one exchange order book.

## Solution structure

```text
MetaExchange.sln
├── MetaExchange.Core          # Models and execution planning logic
│   ├── Models                 # Order, OrderBook, Exchange, ExecutionPlan, etc.
│   └── Services
│       ├── IExecutionPlanService
│       ├── ExecutionPlanService
│       └── OrderBookReader
├── MetaExchange.ConsoleApp    # Console application
└── MetaExchange.Api           # Web API with Swagger
```
