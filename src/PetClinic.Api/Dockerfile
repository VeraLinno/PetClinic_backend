FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PetClinic.sln", "."]
COPY ["src/PetClinic.Api/PetClinic.Api.csproj", "src/PetClinic.Api/"]
COPY ["src/PetClinic.Application/PetClinic.Application.csproj", "src/PetClinic.Application/"]
COPY ["src/PetClinic.Domain/PetClinic.Domain.csproj", "src/PetClinic.Domain/"]
COPY ["src/PetClinic.Infrastructure/PetClinic.Infrastructure.csproj", "src/PetClinic.Infrastructure/"]
COPY ["tests/PetClinic.Tests/PetClinic.Tests.csproj", "tests/PetClinic.Tests/"]
RUN dotnet restore
COPY . .
WORKDIR "/src/src/PetClinic.Api"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PetClinic.Api.dll"]