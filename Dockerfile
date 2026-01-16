FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ARG GITHUB_USERNAME
ARG GITHUB_TOKEN

WORKDIR /src

# Adicionar fonte do GitHub Packages
RUN dotnet nuget add source "https://nuget.pkg.github.com/TC-FIAP-Grupo-11/index.json" --name "TC-FIAP-Grupo-11" --username $GITHUB_USERNAME --password $GITHUB_TOKEN --store-password-in-clear-text

# Copiar projeto do Payments
COPY ["FCG.Api.Payments/src/FCG.Api.Payments/FCG.Api.Payments.csproj", "FCG.Api.Payments/src/FCG.Api.Payments/"]

RUN dotnet restore "FCG.Api.Payments/src/FCG.Api.Payments/FCG.Api.Payments.csproj"

COPY . .
WORKDIR "/src/FCG.Api.Payments/src/FCG.Api.Payments"
RUN dotnet build "FCG.Api.Payments.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FCG.Api.Payments.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FCG.Api.Payments.dll"]
