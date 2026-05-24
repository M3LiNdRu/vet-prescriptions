FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY VetPrescription.slnx ./
COPY VetPrescription.Domain/VetPrescription.Domain.csproj VetPrescription.Domain/
COPY VetPrescription.Api/VetPrescription.Api.csproj VetPrescription.Api/
COPY VetPrescription.UnitTests/VetPrescription.UnitTests.csproj VetPrescription.UnitTests/
COPY VetPrescription.IntegrationTests/VetPrescription.IntegrationTests.csproj VetPrescription.IntegrationTests/
RUN dotnet restore VetPrescription.slnx

COPY . .
RUN dotnet publish VetPrescription.Api/VetPrescription.Api.csproj \
    -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "VetPrescription.Api.dll"]
