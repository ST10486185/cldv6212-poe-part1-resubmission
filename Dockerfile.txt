# Use the official Azure Functions image for .NET isolated runtime
FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated10.0

# Copy the published app into the container so Functions can run it
COPY . /home/site/wwwroot

# Functions inside the container will listen on port 80
EXPOSE 80