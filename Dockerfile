FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copiar projeto do Payments
COPY ["src/FCG.Api.Payments/FCG.Api.Payments.csproj", "src/FCG.Api.Payments/"]

RUN dotnet restore "src/FCG.Api.Payments/FCG.Api.Payments.csproj"

COPY . .
WORKDIR "/src/src/FCG.Api.Payments"
RUN dotnet build "FCG.Api.Payments.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FCG.Api.Payments.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Install New Relic .NET agent
RUN apt-get update \
    && apt-get install -y wget ca-certificates gnupg \
    && echo 'deb http://apt.newrelic.com/debian/ newrelic non-free' | tee /etc/apt/sources.list.d/newrelic.list \
    && wget https://download.newrelic.com/548C16BF.gpg \
    && apt-key add 548C16BF.gpg \
    && apt-get update \
    && apt-get install -y newrelic-dotnet-agent \
    && rm -rf /var/lib/apt/lists/*

ENV CORECLR_ENABLE_PROFILING=1 \
    CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
    CORECLR_NEWRELIC_HOME=/usr/local/newrelic-dotnet-agent \
    CORECLR_PROFILER_PATH=/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so \
    NEW_RELIC_HOME=/usr/local/newrelic-dotnet-agent

ENTRYPOINT ["dotnet", "FCG.Api.Payments.dll"]
