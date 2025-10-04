This project follows the tutorial described [here](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-9.0&tabs=visual-studio-code).

To run the project: 

```ps
cd Api/;
dotnet run --launch-profile https
```

To see the resultant OpenAPI specs:

```
https://localhost:5001/api/swagger
https://localhost:5001/api/scalar
https://localhost:5001/api/redoc
```

To run the test suites:

```ps
dotnet test
```