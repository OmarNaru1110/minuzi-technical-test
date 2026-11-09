# Menuzi Technical Test

Delivery slot scheduling API built with .NET 10, split into `API`, `CORE`, and `DATA` projects, with xUnit tests.

## Requirements

- .NET 10 SDK
- Docker (for containerized runs)

## Get the Code

1. Clone the repository:

   ```powershell
   git clone https://github.com/omarnaru1110/minuzi-technical-test.git
   ```

2. Change into the repository root:

   ```powershell
   cd minuzi-technical-test
   ```

Run all commands below from the repository root.

## Run with Docker

1. Build the image from the repository root:

   ```powershell
   docker build -t menuzi-technical-test .
   ```

2. Run the container:

   ```powershell
   docker run --rm -p 8080:8080 menuzi-technical-test
   ```

3. Open the API and Swagger UI:

   - API: `http://localhost:8080`
   - Swagger UI: `http://localhost:8080/swagger`

The container listens on HTTP port `8080`

## Run Locally

1. From the repository root, start the API:

   ```powershell
   dotnet run --project .\API\API.csproj --launch-profile https
   ```

2. Open the API and Swagger UI:

   - API: `https://localhost:7267`
   - Swagger UI: `https://localhost:7267/swagger`

If the development certificate is not trusted yet:

```powershell
dotnet dev-certs https --trust
```

## API Endpoints

```text
GET /api/products
GET /api/delivery-slots?productIds=warehouse-milk&productIds=fresh-sourdough
```

## Tests

From the repository root:

```powershell
dotnet test .\minuzi-technical-test.slnx
```
