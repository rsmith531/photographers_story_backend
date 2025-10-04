This project follows the tutorial described [here](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-9.0&tabs=visual-studio-code).

To run the project: 

```ps
dotnet run --launch-profile https
```

To develop the API using Swagger:

```
https://localhost:7197/swagger
```

To run the test suites:

```ps
dotnet test
```

Adding a service to the API:
<!-- I wrote this while adding MinIO for S3 mocking during local development -->

1. Create a new project in the solution for your service.

2. Update the `docker-compose.yml` if the service needs a container.

3. Add a `IMyService.cs` file to the root of the project and put an empty class in there.

4. Add an implementation of the service with a constructor accepting the client object that will connect to the container.

5. Add an entry to `appsettings.development.json` to describe the connection parameters.

6. Build a `Connection` class if the connection details need strong typing.

7. Register the new service with the builder in `Program.cs`. Remember to add a reference to your service project in the API project.