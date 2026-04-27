FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем csproj файлы из их папок
COPY ["TestTask_14.03/API.csproj", "TestTask_14.03/"]
COPY ["Data/Data.csproj", "Data/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Application/Application.csproj", "Application/"]

# Восстанавливаем зависимости
RUN dotnet restore "TestTask_14.03/API.csproj"

# Копируем все исходники
COPY . .

# Собираем проект
WORKDIR "/src/TestTask_14.03"
RUN dotnet build "API.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "API.csproj" -c Debug -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]