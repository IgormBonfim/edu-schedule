FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine

WORKDIR /app

COPY . .

ENTRYPOINT ["dotnet", "test", "backend/EduSchedule.UnitTests/EduSchedule.UnitTests.csproj", "--logger:console;verbosity=normal"]