FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5200

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/Oslofjord.AdminDashboard.Api/Oslofjord.AdminDashboard.Api.csproj", "Oslofjord.AdminDashboard.Api/"]
COPY ["src/Oslofjord.AdminDashboard.Contracts/Oslofjord.AdminDashboard.Contracts.csproj", "Oslofjord.AdminDashboard.Contracts/"]
RUN dotnet restore "Oslofjord.AdminDashboard.Api/Oslofjord.AdminDashboard.Api.csproj"

COPY src/ .
WORKDIR "/src/Oslofjord.AdminDashboard.Api"
RUN dotnet build "Oslofjord.AdminDashboard.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Oslofjord.AdminDashboard.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Oslofjord.AdminDashboard.Api.dll"]
