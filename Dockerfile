# Устанавливаем базовый образ для сборки
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Копируем файлы проекта и восстанавливаем зависимости
COPY *.csproj ./
RUN dotnet restore

# Копируем остальные файлы и создаем выпуск
COPY . ./
RUN dotnet publish -c Release -o out

# Создаем финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/out .

# Указываем команду для запуска приложения
ENTRYPOINT ["dotnet", "Telegramm.dll"]
