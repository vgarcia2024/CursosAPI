FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Backend/Tp_Programacion/*.csproj ./Backend/Tp_Programacion/
RUN dotnet restore ./Backend/Tp_Programacion/Tp_Programacion.csproj

COPY Backend/Tp_Programacion/. ./Backend/Tp_Programacion/
RUN dotnet publish ./Backend/Tp_Programacion/Tp_Programacion.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Tp_Programacion.dll"]