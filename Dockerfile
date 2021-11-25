FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Worms/Worms.csproj", "Worms/"]
RUN dotnet restore "Worms/Worms.csproj"
COPY . .
WORKDIR "/src/Worms"
RUN dotnet build "Worms.csproj" -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["WormsServer/WormsServer.csproj", "WormsServer/"]
RUN dotnet restore "WormsServer/WormsServer.csproj"
COPY . .
WORKDIR "/src/WormsServer"
RUN dotnet build "WormsServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WormsServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WormsServer.dll"]
