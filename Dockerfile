FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

COPY ["src/Ludero.Web/Ludero.Web.csproj", "src/Ludero.Web/"]
RUN dotnet restore "src/Ludero.Web/Ludero.Web.csproj"
COPY . .
RUN dotnet publish "src/Ludero.Web/Ludero.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Ludero.Web.dll"]
