
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

#CAMBIAR AQUI EL NOMBRE DEL APLICATIVO
#nombre de tu app busca en bin\Release**\netcore5.0\plantitas.exe
ENV APP_NET_CORE examen_parcial_programacion1.dll

CMD ASPNETCORE_URLS=http://*:$PORT dotnet $APP_NET_CORE