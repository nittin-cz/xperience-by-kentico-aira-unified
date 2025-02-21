# Xperience by Kentico Aira Unified

[![Kentico Labs](https://img.shields.io/badge/Kentico_Labs-grey?labelColor=orange&logo=data:image/svg+xml;base64,PHN2ZyBjbGFzcz0ic3ZnLWljb24iIHN0eWxlPSJ3aWR0aDogMWVtOyBoZWlnaHQ6IDFlbTt2ZXJ0aWNhbC1hbGlnbjogbWlkZGxlO2ZpbGw6IGN1cnJlbnRDb2xvcjtvdmVyZmxvdzogaGlkZGVuOyIgdmlld0JveD0iMCAwIDEwMjQgMTAyNCIgdmVyc2lvbj0iMS4xIiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciPjxwYXRoIGQ9Ik05NTYuMjg4IDgwNC40OEw2NDAgMjc3LjQ0VjY0aDMyYzE3LjYgMCAzMi0xNC40IDMyLTMycy0xNC40LTMyLTMyLTMyaC0zMjBjLTE3LjYgMC0zMiAxNC40LTMyIDMyczE0LjQgMzIgMzIgMzJIMzg0djIxMy40NEw2Ny43MTIgODA0LjQ4Qy00LjczNiA5MjUuMTg0IDUxLjIgMTAyNCAxOTIgMTAyNGg2NDBjMTQwLjggMCAxOTYuNzM2LTk4Ljc1MiAxMjQuMjg4LTIxOS41MnpNMjQxLjAyNCA2NDBMNDQ4IDI5NS4wNFY2NGgxMjh2MjMxLjA0TDc4Mi45NzYgNjQwSDI0MS4wMjR6IiAgLz48L3N2Zz4=)](https://github.com/Kentico/.github/blob/main/SUPPORT.md#labs-limited-support)
[![CI: Build and Test](https://github.com/Kentico/aira-unified/actions/workflows/ci.yml/badge.svg)](https://github.com/Kentico/aira-unified/actions/workflows/ci.yml)
[![NuGet Package](https://img.shields.io/nuget/v/Kentico.Xperience.AiraUnified.svg)](https://www.nuget.org/packages/Kentico.Xperience.AiraUnified)

## Description

Aira Unified integration provides an alternative administration UI and chatbot that can be easily added to an Xperience by Kentico (XbyK) project. It is designed for content editors and marketers who need essential functionality on mobile devices.

### Main Features

- Ask questions about stored data in XbyK from anywhere.
- Get product-related help and advice.
- Carry out tasks directly through the chatbot.

## Screenshots

![UI Application](/images/screenshots/ui_application.png)

## Library Version Matrix

| Xperience Version |    Library Version   |
| ----------------- | -------------------- |
| >= 30.0.0         | >= 0.1.0-prerelase-1 |

## Dependencies

- [ASP.NET Core 8.0](https://dotnet.microsoft.com/en-us/download)
- [Xperience by Kentico](https://docs.xperience.io/xp/changelog)

## Package Installation

Install the package using the .NET CLI:

```powershell
dotnet add package Kentico.Xperience.AiraUnified
```

## Quick Start

1. Add the Aira Unified API subscription key to your `appsettings.json`:

```json
"AiraUnifiedOptions": {
  "AiraUnifiedApiSubscriptionKey": "<your aira unified API key>"
}
```

2. Include the `Kentico.Xperience.AiraUnified` package in your project:

   ```powershell
   dotnet add package Kentico.Xperience.AiraUnified
   ```

3. Register the required services in `Program.cs`:

   ```csharp
   var builder = WebApplication.CreateBuilder(args);
   
   builder.Services.AddKenticoAiraUnified(builder.Configuration);
   
   var app = builder.Build();
   
   app.UseAiraUnifiedEndpoints();
   
   app.Run();
   ```

4. Configure the Aira Unified settings in the administration UI:
   - **Relative Path Base**: Defines where Aira Unified is available.
   - **Logo**: Select an asset from the Media Library.
   - **Chat Title**: Title for the chat page.
   - **Smart Upload Title**: Title for the smart upload page.
   
   ![Admin Configuration](/images/AiraUnifiedAdminConfiguration.png)

5. Set up a `Content Type` for [Mass Asset Upload](https://docs.kentico.com/developers-and-admins/development/content-types#mass-asset-upload-configuration).
6. Configure role-based permissions in the `Role Management` application:
   - **View**: Access the Aira Unified chat.
   - **Create/Update**: Access and upload content via Smart Upload.

   ![Role Configuration](/images/ConfigureAiraUnifiedPermissions.png)

7. Users can now access Aira Unified at the specified path.

## Full Instructions

Refer to the [Usage Guide](./docs/Usage-Guide.md) for detailed instructions.

You can also check out the **Dancing Goat** example project.

## Contributing

Follow the [Kentico Contributing Guidelines](https://github.com/Kentico/.github/blob/main/CONTRIBUTING.md) and adhere to the [Code of Conduct](https://github.com/Kentico/.github/blob/main/CODE_OF_CONDUCT.md).

Find project-specific contribution details in [Contributing Setup](./docs/Contributing-Setup.md).

## License

Distributed under the MIT License. See [`LICENSE.md`](./LICENSE.md) for details.

## Support

[![Kentico Labs](https://img.shields.io/badge/Kentico_Labs-grey?labelColor=orange&logo=data:image/svg+xml;base64,PHN2ZyBjbGFzcz0ic3ZnLWljb24iIHN0eWxlPSJ3aWR0aDogMWVtOyBoZWlnaHQ6IDFlbTt2ZXJ0aWNhbC1hbGlnbjogbWlkZGxlO2ZpbGw6IGN1cnJlbnRDb2xvcjtvdmVyZmxvdzogaGlkZGVuOyIgdmlld0JveD0iMCAwIDEwMjQgMTAyNCIgdmVyc2lvbj0iMS4xIiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciPjxwYXRoIGQ9Ik05NTYuMjg4IDgwNC40OEw2NDAgMjc3LjQ0VjY0aDMyYzE3LjYgMCAzMi0xNC40IDMyLTMycy0xNC40LTMyLTMyLTMyaC0zMjBjLTE3LjYgMC0zMiAxNC40LTMyIDMyczE0LjQgMzIgMzIgMzJIMzg0djIxMy40NEw2Ny43MTIgODA0LjQ4Qy00LjczNiA5MjUuMTg0IDUxLjIgMTAyNCAxOTIgMTAyNGg2NDBjMTQwLjggMCAxOTYuNzM2LTk4Ljc1MiAxMjQuMjg4LTIxOS41MnpNMjQxLjAyNCA2NDBMNDQ4IDI5NS4wNFY2NGgxMjh2MjMxLjA0TDc4Mi45NzYgNjQwSDI0MS4wMjR6IiAgLz48L3N2Zz4=)](https://github.com/Kentico/.github/blob/main/SUPPORT.md#labs-limited-support)

This project has **Kentico Labs limited support**.

For more information, see [`SUPPORT.md`](https://github.com/Kentico/.github/blob/main/SUPPORT.md#full-support).

For security issues, refer to [`SECURITY.md`](https://github.com/Kentico/.github/blob/main/SECURITY.md).
