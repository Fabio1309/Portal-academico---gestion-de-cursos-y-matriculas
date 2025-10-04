# Portal Académico - Gestión de Cursos y Matrículas

Proyecto desarrollado como parte de un examen parcial de programación, implementando un portal web completo con ASP.NET Core 8, Entity Framework Core, Identity, Redis y desplegado en Render.com.

**URL de la aplicación en producción:** [PENDIENTE - SE AÑADIRÁ AL FINALIZAR EL DESPLIEGUE]

## Stack Tecnológico

-   **Backend:** ASP.NET Core MVC (.NET 8)
-   **Base de Datos:** Entity Framework Core con SQLite
-   **Autenticación:** ASP.NET Core Identity con Roles
-   **Caché y Sesiones:** Redis (gestionado en Redis Cloud)
-   **Plataforma de Despliegue (PaaS):** Render.com

## Flujo de Trabajo y Versionado

El proyecto sigue un flujo de trabajo basado en Git Flow, utilizando ramas `feature/*` para cada nueva funcionalidad, las cuales se integran en `develop` a través de Pull Requests. La rama `main` contiene las versiones estables y desplegadas.

## Pasos para la Configuración Local

1.  **Clonar el repositorio:**
    ```bash
    git clone <URL_DE_TU_REPOSITORIO_EN_GITHUB>
    cd examen_parcial_programacion1
    ```

2.  **Restaurar dependencias de .NET:**
    ```bash
    dotnet restore
    ```

3.  **Configurar `appsettings.Development.json`:**
    Asegúrate de tener un archivo `appsettings.Development.json` con la cadena de conexión a Redis (si lo usas localmente). Si no, la aplicación usará caché en memoria.
    ```json
    {
      "Redis_ConnectionString": "localhost:6379"
    }
    ```

4.  **Aplicar migraciones de la base de datos:**
    Este comando creará el archivo `app.db` con la estructura de tablas y los datos iniciales (seed).
    ```bash
    dotnet ef database update
    ```

5.  **Ejecutar la aplicación:**
    ```bash
    dotnet run
    ```
    La aplicación estará disponible en `https://localhost:XXXX`.

## Credenciales de Prueba

-   **Usuario Coordinador:**
    -   **Email:** `coordinador@test.com`
    -   **Password:** `Password123!`
-   **Usuarios Estudiantes:** Se pueden registrar nuevos usuarios desde la interfaz de la aplicación.

## Configuración para el Despliegue en Render

-   **Runtime:** `.NET`
-   **Build Command:** `dotnet publish -c Release -o out`
-   **Start Command:** `dotnet out/examen_parcial_programacion1.dll` (¡Ajusta el nombre del .dll al de tu proyecto!)

-   **Variables de Entorno:**
    -   `ASPNETCORE_ENVIRONMENT`: `Production`
    -   `ASPNETCORE_URLS`: `http://0.0.0.0:${PORT}`
    -   `ConnectionStrings__DefaultConnection`: `Data Source=/var/data/prod.db` (Render persiste el directorio `/var/data`)
    -   `Redis__ConnectionString`: `{TU_CONNECTION_STRING_DE_REDIS_CLOUD}`