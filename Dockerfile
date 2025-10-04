
# Usar la imagen oficial de .NET 8.0 SDK para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar el archivo de proyecto y restaurar dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar todo el código fuente
COPY . ./

# Publicar la aplicación
RUN dotnet publish -c Release -o /app/publish

# Usar la imagen runtime de .NET 8.0 para la imagen final
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copiar los archivos publicados
COPY --from=build /app/publish .

# Exponer el puerto
EXPOSE 8080

# Configurar las variables de entorno
ENV ASPNETCORE_URLS=http://*:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Ejecutar la aplicación
ENTRYPOINT ["dotnet", "examen_parcial_programacion1.dll"]