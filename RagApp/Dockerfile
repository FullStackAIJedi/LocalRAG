FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY RagApp.Domain/RagApp.Domain.csproj RagApp.Domain/
COPY RagApp.Application/RagApp.Application.csproj RagApp.Application/
COPY RagApp.Infrastructure/RagApp.Infrastructure.csproj RagApp.Infrastructure/
COPY RagApp.Api/RagApp.Api.csproj RagApp.Api/

RUN dotnet restore RagApp.Api/RagApp.Api.csproj

COPY . .

WORKDIR /src/RagApp.Api
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

RUN mkdir -p /app/storage

EXPOSE 5000

ENTRYPOINT ["dotnet", "RagApp.Api.dll"]