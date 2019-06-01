FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["GoTo.Service/GoTo.Service.csproj", "GoTo.Service/"]
RUN dotnet restore "GoTo.Service/GoTo.Service.csproj"
COPY . .
WORKDIR "/src/GoTo.Service"
RUN dotnet build "GoTo.Service.csproj" -c Release -o /app

FROM node:10.16.0-alpine AS build_client
WORKDIR /app
COPY GoTo.Client/ /app
RUN npm install
RUN npm run build

FROM build AS publish
RUN dotnet publish "GoTo.Service.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY --from=build_client /app/dist/ wwwroot/
ENTRYPOINT ["dotnet", "GoTo.Service.dll"]