ARG NODE_IMAGE=node:10-alpine

FROM ${NODE_IMAGE} as node-build
WORKDIR /src
COPY WeSafe.Web/ClientApp/package*.json ./
RUN npm install

COPY WeSafe.Web/ClientApp .
RUN npm run build:prod

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["WeSafe.Web/WeSafe.Web.csproj", "WeSafe.Web/"]
COPY ["WeSafe.DAL/WeSafe.DAL.csproj", "WeSafe.DAL/"]
COPY ["WeSafe.DAL.Entities/WeSafe.DAL.Entities.csproj", "WeSafe.DAL.Entities/"]
COPY ["WeSafe.Services/WeSafe.Services.csproj", "WeSafe.Services/"]
COPY ["WeSafe.Services.Client/WeSafe.Services.Client.csproj", "WeSafe.Services.Client/"]
COPY ["WeSafe.Shared/WeSafe.Shared.csproj", "WeSafe.Shared/"]
COPY ["WeSafe.Web.Core/WeSafe.Web.Core.csproj", "WeSafe.Web.Core/"]
RUN dotnet restore "WeSafe.Web/WeSafe.Web.csproj"
COPY . .
WORKDIR "/src/WeSafe.Web"
RUN dotnet build "WeSafe.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "WeSafe.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY --from=node-build /src/dist/ClientApp ./wwwroot

ENTRYPOINT ["dotnet", "WeSafe.Web.dll"]