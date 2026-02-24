FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/CRM.Corporativo.Api/CRM.Corporativo.Api.csproj", "src/CRM.Corporativo.Api/"]
COPY ["src/CRM.Corporativo.Infra.IoC/CRM.Corporativo.Infra.IoC.csproj", "src/CRM.Corporativo.Infra.IoC/"]
COPY ["src/CRM.Corporativo.Application/CRM.Corporativo.Application.csproj", "src/CRM.Corporativo.Application/"]
COPY ["src/CRM.Corporativo.Domain/CRM.Corporativo.Domain.csproj", "src/CRM.Corporativo.Domain/"]
COPY ["src/CRM.Corporativo.Infra.CrossCutting/CRM.Corporativo.Infra.CrossCutting.csproj", "src/CRM.Corporativo.Infra.CrossCutting/"]
COPY ["src/CRM.Corporativo.Infra.Data/CRM.Corporativo.Infra.Data.csproj", "src/CRM.Corporativo.Infra.Data/"]

RUN dotnet restore "./src/CRM.Corporativo.Api/CRM.Corporativo.Api.csproj"
COPY src/ src/

WORKDIR "/src/src/CRM.Corporativo.Api"
RUN dotnet build "./CRM.Corporativo.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./CRM.Corporativo.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "CRM.Corporativo.Api.dll"]
