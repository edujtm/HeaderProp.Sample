
## Simple Header Propagation Sample

This project shows a simple example of authorization header propagation using the [HeaderPropagation](https://www.nuget.org/packages/Microsoft.AspNetCore.HeaderPropagation/6.0.8) library.

## How to run

This project requires .NET 6.0 SDK.

Run the project:

`dotnet run --urls http://localhost:5000/`

Then test it by running this on another terminal window:

`curl -H "Authorization: fake-auth-token" http://localhost:5000/`