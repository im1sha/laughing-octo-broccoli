FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Broccoli/Broccoli.csproj", "Broccoli/"]
RUN dotnet restore "Broccoli/Broccoli.csproj"
COPY . .
WORKDIR "/src/Broccoli"
RUN dotnet build "Broccoli.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Broccoli.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet Broccoli.dll