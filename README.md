# YourCarBud

A .NET 5 application to be Your Car Buddy for ordering and delivery of used cars!

## Getting Started

Clone the repository in your local machine.

- Open the `YourCarBud.sln` solution from `src` folder using Visual Studio 2019+
- Make sure you have a valid `ConnectionString` in `appsettings.Development.json`.
- Hit F5:
- [ Or simply navigate to `src\Api\YourCarBud.WebApi` folder in the terminal and type `dotnet run` ]

You should be able to browse the Swagger interface of the application by using the URL http://localhost:5005

See the last section of this file to learn how to test the app using Postman.

## Business Flow

[ To be edited ]

## Postman

To test the controller, open Postman and make a POST request to `http://localhost:5005/api/orders` endpoint while putting your Json args as a `Content-Type: application/json` in request body.

Sample Json:

```
{

}
```

![](/images/img1.jpg)
