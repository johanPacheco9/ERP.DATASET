FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ERP.DATASET/ERP.DATASET.csproj", "ERP.DATASET/"]
COPY ["ERP.DATA/ERP.DATA.csproj", "ERP.DATA/"]
COPY ["ERP.TRAN/ERP.TRAN.csproj", "ERP.TRAN/"]
RUN dotnet restore "ERP.DATASET/ERP.DATASET.csproj"
COPY . .
WORKDIR "/src/ERP.DATASET"
RUN dotnet build "./ERP.DATASET.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ERP.DATASET.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ERP.DATASET.dll"]
