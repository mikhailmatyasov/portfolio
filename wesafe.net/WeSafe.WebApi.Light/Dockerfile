ARG NODE_IMAGE=node:10-alpine

FROM ${NODE_IMAGE} as node-build
WORKDIR /src
COPY WeSafe.WebApi.Light/ClientApp/package*.json ./
RUN npm install

COPY WeSafe.WebApi.Light/ClientApp .
RUN npm run build:prod

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-bionic-arm64v8 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["WeSafe.WebApi.Light/WeSafe.Nano.WebApi.csproj", "WeSafe.WebApi.Light/"]
COPY ["WeSafe.DAL/WeSafe.DAL.csproj", "WeSafe.DAL/"]
COPY ["WeSafe.DAL.Entities/WeSafe.DAL.Entities.csproj", "WeSafe.DAL.Entities/"]
COPY ["WeSafe.Services/WeSafe.Services.csproj", "WeSafe.Services/"]
COPY ["WeSafe.Services.Client/WeSafe.Services.Client.csproj", "WeSafe.Services.Client/"]
COPY ["WeSafe.Shared/WeSafe.Shared.csproj", "WeSafe.Shared/"]
COPY ["WeSafe.Web.Core/WeSafe.Web.Core.csproj", "WeSafe.Web.Core/"]
COPY ["WeSafe.Nano.DAL/WeSafe.Nano.DAL.csproj", "WeSafe.Nano.DAL/"]
COPY ["WeSafe.Nano.Services/WeSafe.Nano.Services.csproj", "WeSafe.Nano.Services/"]
COPY ["WeSafe.Nano.Services.Abstraction/WeSafe.Nano.Services.Abstraction.csproj", "WeSafe.Nano.Services.Abstraction/"]
RUN dotnet restore "WeSafe.WebApi.Light/WeSafe.Nano.WebApi.csproj"
COPY . .
WORKDIR "/src/WeSafe.WebApi.Light"
RUN dotnet build "WeSafe.Nano.WebApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "WeSafe.Nano.WebApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY --from=node-build /src/dist/ClientApp ./wwwroot

ENTRYPOINT ["dotnet", "WeSafe.Nano.WebApi.dll"]