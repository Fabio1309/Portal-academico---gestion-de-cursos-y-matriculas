# --- Fase 1: Compilación (Build Stage) ---
# Usamos la imagen oficial de Microsoft que contiene el SDK completo de .NET 8
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos los archivos de proyecto (.csproj y .sln) primero para aprovechar la caché de Docker
COPY ["examen_parcial_programacion1.csproj", "."]
RUN dotnet restore "./examen_parcial_programacion1.csproj"

# Copiamos el resto del código fuente
COPY . .

# Publicamos la aplicación en modo Release en una carpeta llamada /app/publish
RUN dotnet publish "examen_parcial_programacion1.csproj" -c Release -o /app/publish

# --- Fase 2: Ejecución (Final Stage) ---
# Usamos la imagen mucho más ligera que solo contiene el runtime de ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Exponemos el puerto 80, que es el que Render usará internamente
EXPOSE 80

# El comando final para ejecutar la aplicación cuando el contenedor se inicie
ENTRYPOINT ["dotnet", "examen_parcial_programacion1.dll"]