FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/NanoBlog.csproj", "./"]
RUN dotnet restore "NanoBlog.csproj"
COPY src .
WORKDIR "/src/"
RUN dotnet build "NanoBlog.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NanoBlog.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NanoBlog.dll"]
