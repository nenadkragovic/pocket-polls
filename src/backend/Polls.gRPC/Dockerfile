#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Polls.gRPC/Polls.gRPC.csproj", "Polls.gRPC/"]
RUN dotnet restore "Polls.gRPC/Polls.gRPC.csproj"
COPY . .
WORKDIR "/src/Polls.gRPC"
RUN dotnet build "Polls.gRPC.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Polls.gRPC.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Polls.gRPC.dll"]