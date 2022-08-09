FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

# copy csproj and restore as distinct layers
COPY ["WeSafe.DAL.Entities/*.csproj", "WeSafe.DAL.Entities/"]
COPY ["WeSafe.Services.Client/*.csproj", "WeSafe.Services.Client/"]
COPY ["WeSafe.Monitoring.Services/*.csproj", "WeSafe.Monitoring.Services/"]
COPY ["WeSafe.Monitoring.Console/*.csproj", "WeSafe.Monitoring.Console/"]
COPY ["WeSafe.Shared/*.csproj", "WeSafe.Shared/"]
RUN dotnet restore "WeSafe.Monitoring.Console/WeSafe.Monitoring.Console.csproj"

# copy and publish app and libraries
COPY ["WeSafe.DAL.Entities/", "WeSafe.DAL.Entities/"]
COPY ["WeSafe.Services.Client/", "WeSafe.Services.Client/"]
COPY ["WeSafe.Monitoring.Services/", "WeSafe.Monitoring.Services/"]
COPY ["WeSafe.Monitoring.Console/", "WeSafe.Monitoring.Console/"]
COPY ["WeSafe.Shared/", "WeSafe.Shared/"]
WORKDIR "/src/WeSafe.Monitoring.Console"
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/core/runtime:3.1
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["./WeSafe.Monitoring.Console"]