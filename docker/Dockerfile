# This Dockerfile only works in .NET project root (../src) 
# As `docker compose` can handle this and for reasons of cleanliness, it's located in this subfolder.
# If you want to use docker without compose, you may copy this file into the .NET project root.
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["NanoBlog.csproj", "./"]
RUN dotnet restore "NanoBlog.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "NanoBlog.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NanoBlog.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NanoBlog.dll"]
