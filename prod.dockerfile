FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /src

COPY ["MiniERP.API/MiniERP.API.csproj", "MiniERP.API/"]
COPY ["MiniERP.Application/MiniERP.Application.csproj", "MiniERP.Application/"]
COPY ["MiniERP.Core/MiniERP.Core.csproj", "MiniERP.Core/"]
COPY ["MiniERP.Infrastructure/MiniERP.Infrastructure.csproj", "MiniERP.Infrastructure/"]

RUN dotnet restore "MiniERP.API/MiniERP.API.csproj"
COPY . .

WORKDIR "/src/MiniERP.API"
RUN dotnet publish "MiniERP.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build-env /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "MiniERP.API.dll"]
