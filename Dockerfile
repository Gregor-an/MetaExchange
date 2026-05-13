FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MetaExchange.Api/MetaExchange.Api.csproj", "MetaExchange.Api/"]
COPY ["MetaExchange.Core/MetaExchange.Core.csproj", "MetaExchange.Core/"]
RUN dotnet restore "MetaExchange.Api/MetaExchange.Api.csproj"
COPY . .
WORKDIR "/src/MetaExchange.Api"
RUN dotnet publish "MetaExchange.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MetaExchange.Api.dll"]