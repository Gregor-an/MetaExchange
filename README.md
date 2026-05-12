# MetaExchange

Work-in-progress solution for the BSDigital/CML Team coding task.

The goal is to build a meta-exchange that creates the best execution plan for buying or selling a requested BTC amount across multiple crypto exchanges.

## Current status

Implemented:

- Core best execution algorithm
- Console application for Part 1
- Reading order books from the provided data file
- Exchange balances from `appsettings.json`
- Buy and sell execution plans with full/partial fill status

Not implemented yet:

- Web API endpoint
- Swagger UI
- Docker setup
- Unit tests

## Approach

For a BUY request, the algorithm uses asks from all exchanges and fills the order from the lowest price to the highest price.

For a SELL request, the algorithm uses bids from all exchanges and fills the order from the highest price to the lowest price.

Each exchange has its own EUR and BTC balance. Balances are treated as funds available on that specific exchange. The algorithm does not transfer EUR or BTC between exchanges.

## Configuration

Exchange balances are configured in:

```bash
MetaExchange.ConsoleAppApp/appsettings.json
```

Example:

```json
{
  "DataFile": "order_books_data",
  "Exchanges": [
    { "Id": "Exchange_1", "EurBalance": 10000, "BtcBalance": 5 },
    { "Id": "Exchange_2", "EurBalance": 5000, "BtcBalance": 3 },
    { "Id": "Exchange_3", "EurBalance": 15000, "BtcBalance": 8 }
  ]
}
```

The provided data file contains order books only, so exchange balances are provided separately through configuration.

Each line in the data file is currently treated as one exchange order book.

## Run

```bash
dotnet run --project MetaExchange.ConsoleApp -- buy 9
dotnet run --project MetaExchange.ConsoleApp -- sell 5
```

## Solution structure

```text
MetaExchange.sln
├── MetaExchange.Core
└── MetaExchange.ConsoleApp
```
