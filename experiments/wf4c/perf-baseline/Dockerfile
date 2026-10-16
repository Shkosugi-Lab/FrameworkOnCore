FROM mcr.microsoft.com/dotnet/aspnet:10.0
COPY out/ /app/
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "baseline.dll"]
