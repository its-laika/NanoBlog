# This Dockerfile only works in .NET project root (../NanoBlog) 
# As `docker compose` can handle this and for reasons of cleanliness, it's located in this subfolder.
# If you want to use docker without compose, you may copy this file into the .NET project root.
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

EXPOSE 8080

RUN adduser --disabled-password --home /app --gecos '' dotnetuser && chown -R dotnetuser /app
USER dotnetuser

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

ARG TARGETARCH

COPY ["NanoBlog.csproj", "./"]
RUN dotnet restore "NanoBlog.csproj"

COPY . .
RUN dotnet build "NanoBlog.csproj" -c Release -o /app/build -a $TARGETARCH --no-self-contained

FROM build AS publish

ARG TARGETARCH

RUN dotnet publish "NanoBlog.csproj" -c Release -o /app/publish -a $TARGETARCH /p:UseAppHost=false --no-self-contained --no-restore

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

RUN mkdir -p data data/posts data/assets data/export
ENV PostDirectory=/app/data/posts AssetDirectory=/app/data/assets ExportDirectory=/app/data/export

ENTRYPOINT ["dotnet", "NanoBlog.dll"]
