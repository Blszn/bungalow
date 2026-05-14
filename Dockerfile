# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Proje dosyalarını kopyala ve restore et
COPY ["Bungalov.WebUI/Bungalov.WebUI.csproj", "Bungalov.WebUI/"]
COPY ["Bungalov.Business/Bungalov.Business.csproj", "Bungalov.Business/"]
COPY ["Bungalov.Core/Bungalov.Core.csproj", "Bungalov.Core/"]
COPY ["Bungalov.DataAccess/Bungalov.DataAccess.csproj", "Bungalov.DataAccess/"]

RUN dotnet restore "Bungalov.WebUI/Bungalov.WebUI.csproj"

# Tüm dosyaları kopyala ve build et
COPY . .
WORKDIR "/src/Bungalov.WebUI"
RUN dotnet build "Bungalov.WebUI.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "Bungalov.WebUI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Uygulama 8080 portundan dinlesin
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Bungalov.WebUI.dll"]
