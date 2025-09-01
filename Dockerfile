# Dockerfile

# Stage 1: Build the application
# Use the official .NET SDK image for building the application.
# It includes all necessary tools for compiling and publishing.
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set the working directory inside the container.
# This is where your application source code will be copied.
WORKDIR /src

# Copy the .csproj file for the API project first and restore dependencies.
# This leverages Docker's layer caching to speed up builds if only source code changes.
# The project file is located in the 'Api' directory within the solution.
COPY ["Api/Api.csproj", "Api/"]
COPY ["Database/Database.csproj", "Database/"]
RUN dotnet restore "Api/Api.csproj"

# Copy the entire solution's source code, including the Api and Tests projects.
# The context of the build assumes the Dockerfile is at the solution root.
COPY . .

# Publish the API application for release.
# The output will be a self-contained application in a folder.
# We're building for a Linux x64 runtime as is common for VPS deployments.
RUN dotnet publish "Api/Api.csproj" -c Release -o /app/publish --no-restore

# Stage 2: Create the runtime image
# Use a smaller, runtime-only image for the final application.
# This significantly reduces the size of the final Docker image.
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

# Set the working directory for the running application.
WORKDIR /app

# Copy the published output from the build stage to the final image.
COPY --from=build /app/publish .

# Expose port 80 (HTTP) within the container.
# ASP.NET Core typically listens on 80/443 inside the container.
# Your Caddy configuration will map external port 5001 to this internal port 80.
EXPOSE 8080

# Define the entry point for the container.
# This specifies the command that will be run when the container starts.
# The entry point is the compiled DLL of your 'Api' project.
ENTRYPOINT ["dotnet", "Api.dll"]