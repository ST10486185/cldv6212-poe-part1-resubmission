# Build the Azure Functions application
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy the project file
COPY CoffeeNChill_Part1.csproj .

# Restore NuGet packages
RUN dotnet restore

# Copy the rest of the project
COPY . .

# Publish the Functions application
RUN dotnet publish CoffeeNChill_Part1.csproj -c Release -o /home/site/wwwroot


# Azure Functions runtime
FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated10.0

# Tell Azure Functions where the application is
ENV AzureWebJobsScriptRoot=/home/site/wwwroot

# Enable console logging
ENV AzureFunctionsJobHost__Logging__Console__IsEnabled=true

# Copy the published application
COPY --from=build /home/site/wwwroot /home/site/wwwroot

# Functions uses port 80 inside the container
EXPOSE 80