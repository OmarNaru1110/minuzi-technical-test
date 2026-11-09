FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["API/API.csproj", "API/"]
COPY ["CORE/CORE.csproj", "CORE/"]
COPY ["DATA/DATA.csproj", "DATA/"]
RUN dotnet restore "API/API.csproj"

COPY ["API/", "API/"]
COPY ["CORE/", "CORE/"]
COPY ["DATA/", "DATA/"]

WORKDIR /src/API
RUN dotnet publish "API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
COPY --from=build /app/publish .
USER app
ENTRYPOINT ["dotnet", "API.dll"]
