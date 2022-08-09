FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

# copy csproj and restore as distinct layers
COPY ["WeSafe.Services.Client/*.csproj", "WeSafe.Services.Client/"]
COPY ["WeSafe.DAL.Entities/*.csproj", "WeSafe.DAL.Entities/"]
COPY ["WeSafe.TelegramBot.Services/*.csproj", "WeSafe.TelegramBot.Services/"]
COPY ["WeSafe.TelegramBot.Console/*.csproj", "WeSafe.TelegramBot.Console/"]
COPY ["WeSafe.Shared/*.csproj", "WeSafe.Shared/"]
RUN dotnet restore "WeSafe.TelegramBot.Console/WeSafe.TelegramBot.NetCore.csproj"

# copy and publish app and libraries
COPY ["WeSafe.Services.Client/", "WeSafe.Services.Client/"]
COPY ["WeSafe.DAL.Entities/", "WeSafe.DAL.Entities/"]
COPY ["WeSafe.TelegramBot.Services/", "WeSafe.TelegramBot.Services/"]
COPY ["WeSafe.TelegramBot.Console/", "WeSafe.TelegramBot.Console/"]
COPY ["WeSafe.Shared/", "WeSafe.Shared/"]
WORKDIR "/src/WeSafe.TelegramBot.Console"
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/core/runtime:3.1
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["./WeSafe.TelegramBot.NetCore"]