FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS base
WORKDIR /app
COPY . .

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY . .
RUN dotnet restore "backend/EduSchedule.Worker/EduSchedule.Worker.csproj"

WORKDIR "/src/backend/EduSchedule.Worker"
RUN dotnet build "EduSchedule.Worker.csproj" -c Release -o /app/build

FROM build as publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /out
COPY --from=publish /app .
EXPOSE 80
ENTRYPOINT ["dotnet", "EduSchedule.Worker.dll"]