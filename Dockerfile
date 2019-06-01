FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim
WORKDIR /app
EXPOSE 80
COPY /dist .
ENTRYPOINT ["dotnet", "GoTo.Service.dll"]