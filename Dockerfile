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
ENTRYPOINT ["dotnet", "FCG.Api.Payments.dll"]
