# TestJob

ASP.NET Core Web API for processing HTML pages and related data.

## Contents

- [Features](#features)
- [Requirements](#requirements)
- [Installation and usage](#installation-and-usage)
- [API](#api)
- [Validation and errors](#validation-and-errors)
- [PostgreSQL](#postgresql)
- [Testing](#testing)
- [Project structure](#project-structure)

## Features

The application:

- decodes the URL and HTML page from Base64;
- parses HTML using AngleSharp;
- selects elements using a CSS selector;
- extracts the value of the specified HTML attribute;
- stores the discovered elements in PostgreSQL using Dapper;
- extracts email addresses using a regular expression;
- decrypts text using AES-256 in ECB mode with `PaddingMode.None`;
- returns the result as formatted JSON.

## Requirements

- .NET SDK 10;
- PostgreSQL;
- Docker;

## Libraries

- [AngleSharp](https://anglesharp.github.io/) — HTML parsing;
- [Dapper](https://github.com/DapperLib/Dapper) — database access;
- [Npgsql](https://www.npgsql.org/) — PostgreSQL provider for .NET;
- [FluentValidation](https://docs.fluentvalidation.net/) — request validation;
- Swagger/OpenAPI — API documentation and testing.

## Installation and usage

Run the Docker Compose file from the repository root:

```powershell
docker compose -f .\compose.yml up --build
```

After startup, the services are available at:

```text
http://localhost:8090/swagger
http://localhost:8080
```

To stop the containers, press `Ctrl+C` or run:

```powershell
docker compose -f .\compose.yml down
```

## API

### `POST /api/testJob`

The endpoint accepts a JSON object in the request body.

#### Request body

| Field | Type | Description |
|---|---|---|
| `selector` | `string` | CSS selector used to find HTML elements |
| `attribute` | `string` | HTML attribute name, for example `href` |
| `url_b64` | `string` | URL encoded as Base64 |
| `encrypted_text_bytes_b64` | `string` | Encrypted text encoded as Base64 |
| `key_bytes_b64` | `string` | AES key encoded as Base64; it must decode to exactly 32 bytes |
| `page_b64` | `string` | HTML page encoded as Base64 |

Example request:

```json
{
  "selector": "a",
  "attribute": "href",
  "url_b64": "aHR0cHM6Ly9leGFtcGxlLmNvbQ==",
  "encrypted_text_bytes_b64": "<base64-encoded-ciphertext>",
  "key_bytes_b64": "<base64-encoded-32-byte-key>",
  "page_b64": "PGh0bWw+PGJvZHk+PGFzc29jaWF0ZUBleGFtcGxlLmNvbTwvYm9keT48L2h0bWw+"
}
```

`encrypted_text_bytes_b64` must decode to a non-empty byte array whose length is a multiple of 16 bytes. This is required by `PaddingMode.None`.

#### Successful response — HTTP 200

```json
{
  "is_error": 0,
  "error_code": "",
  "error_message": "",
  "elements_count": 2,
  "emails_count": 1,
  "url": "https://example.com",
  "decrypted_plain_text": "Decrypted text",
  "elements_attr_list": [
    "/first",
    "/second"
  ],
  "emails_list": [
    "associate@example.com"
  ]
}
```

Response fields:

| Field | Type | Description |
|---|---|---|
| `is_error` | `int` | `0` for success, `1` for an error |
| `error_code` | `string` | Error code; empty on success |
| `error_message` | `string` | Error message; empty on success |
| `elements_count` | `int` | Number of elements found by the CSS selector |
| `emails_count` | `int` | Number of email addresses found |
| `url` | `string` | Decoded URL |
| `decrypted_plain_text` | `string` | Decrypted text |
| `elements_attr_list` | `List<string>` | Values of the specified attribute from the discovered elements |
| `emails_list` | `List<string>` | Discovered email addresses |

## Validation and errors

FluentValidation checks that the following request parameters exist and are not empty:

- `selector`;
- `attribute`;
- `url_b64`;
- `page_b64`;
- `key_bytes_b64`;
- `encrypted_text_bytes_b64`;

Base64 format, AES key size, and encrypted text length are checked in the service using `Try` methods.

### Invalid input — HTTP 400

| Error code | Description |
|---|---|
| `REQUEST_BODY_REQUIRED` | Request body is missing |
| `VALIDATION_ERROR` | FluentValidation failed |
| `INVALID_URL_BASE64` | Invalid Base64 in `url_b64` |
| `INVALID_PAGE_BASE64` | Invalid Base64 in `page_b64` |
| `INVALID_KEY_BASE64` | Invalid Base64 in `key_bytes_b64` |
| `INVALID_ENCRYPTED_TEXT_BASE64` | Invalid Base64 in `encrypted_text_bytes_b64` |
| `INVALID_AES_KEY` | Key does not decode to 32 bytes |
| `INVALID_ENCRYPTED_TEXT_LENGTH` | Encrypted text length is not a multiple of 16 bytes |
| `DECRYPTION_ERROR` | Text could not be decrypted |

Example:

```json
{
  "is_error": 1,
  "error_code": "INVALID_PAGE_BASE64",
  "error_message": "Parameter 'page_b64' contains invalid Base64 data.",
  "elements_count": 0,
  "emails_count": 0,
  "url": "",
  "decrypted_plain_text": "",
  "elements_attr_list": [],
  "emails_list": []
}
```

### Unexpected errors — HTTP 500

PostgreSQL errors, configuration errors, and other unhandled exceptions are processed by the global exception handler and returned with the following error code:

```text
INTERNAL_ERROR
```

According to the current response contract, the exception message is returned in `error_message`.

## PostgreSQL

During request processing, the application creates the `elements` table if it does not exist:

```sql
CREATE TABLE IF NOT EXISTS elements
(
    id BIGINT GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
    attribute TEXT NOT NULL,
    html_content TEXT NOT NULL
);
```

For every discovered element that contains the requested attribute, the application stores:

- the attribute value in `attribute`;
- the complete HTML code of the element in `html_content`.

## Testing

The solution contains a separate `TestJob.Tests` project with unit tests for:

- `TestJobValidator`;
- `TestJobResponseFactory`;

Run the tests with:

```powershell
dotnet test .\TestJob.Tests\TestJob.Tests.csproj
```

## Project structure

```text
TestJob/
├── Controllers/       # HTTP controllers
├── Factories/         # API response factory
├── Interfaces/        # Service contracts
├── Models/            # Request and response models
├── Services/          # Core business logic
├── Validators/        # FluentValidation validators
└── Program.cs         # Application configuration

TestJob.Tests/
├── ResponseFactoryTests.cs
└── TestJobValidatorTests.cs        # Validator tests
```
