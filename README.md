# ViVaKR.AuthLibrary

A lightweight library of the `ApplicationUser`, `ApplicationRole`, and `ApplicationUserRole`
entities you end up writing again and again when setting up **JWT-based authentication**
in ASP.NET Core APIs.

Stop copy-pasting the same classes into every project.

[![NuGet](https://img.shields.io/nuget/v/ViVaKR.AuthLibrary.svg)](https://www.nuget.org/packages/ViVaKR.AuthLibrary)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

[한국어](README.md)

## Features

- Built on `ASP.NET Core Identity` with **`Guid`** keys
- `IAuditable` for audit timestamps (`CreatedAt`, `UpdatedAt`)
- `ISoftDeletable` for soft delete (`DeletedAt`, `IsDeleted`)
- User ↔ Role join entity with navigation properties
- Targets .NET 10

## Installation

```bash
dotnet add package ViVaKR.AuthLibrary
```

## What's inside

All types live under the `AuthLibrary.*` namespaces.

| Kind | Name | Namespace | Description |
|------|------|-----------|-------------|
| Entity | `ApplicationUser` | `AuthLibrary.Entities` | Extends `IdentityUser<Guid>` |
| Entity | `ApplicationRole` | `AuthLibrary.Entities` | Extends `IdentityRole<Guid>` |
| Entity | `ApplicationUserRole` | `AuthLibrary.Entities` | Extends `IdentityUserRole<Guid>` |
| Interface | `IAuditable` | `AuthLibrary.Interfaces` | Created/updated timestamps |
| Interface | `ISoftDeletable` | `AuthLibrary.Interfaces` | Soft-delete timestamp and flag |
| Settings | `JwtSettings` | `AuthLibrary` | JWT options (coming soon) |

### ApplicationUser

| Property | Type | Description |
|----------|------|-------------|
| `FullName` | `string` (required) | Real name |
| `Pseudonym` | `string` (required) | Display alias |
| `Avatar` | `string?` | Profile image |
| `Introduction` | `string?` | Bio |
| `IsActive` | `bool` | Active flag (default `true`) |
| `RefreshToken` / `RefreshTokenExpiryTime` | `string?` / `DateTime?` | Refresh token and expiry |
| `TrustedDeviceKey` / `TrustedDeviceExpiry` | `string?` / `DateTime?` | Trusted device |
| `LastLoginAt` / `LastLoginIp` | `DateTime?` / `string?` | Last login info |
| `Provider` | `string?` | External login provider |
| `TimeZone` / `Locale` | `string` | Defaults: `Asia/Seoul` / `ko-KR` |
| `RowVersion` | `uint` | Concurrency token |
| `CreatedAt` / `UpdatedAt` / `DeletedAt` | `DateTime` / `DateTime?` | Audit and soft delete |
| `UserRoles` | `ICollection<ApplicationUserRole>` | Role links |

### ApplicationRole

| Property | Type | Description |
|----------|------|-------------|
| `KoName` | `string?` | Korean display name |
| `SortOrder` | `int` | Sort order |
| `IsActive` / `IsSystem` | `bool` | Active / system role flags |
| `Description` | `string?` | Description |
| `CreatedAt` / `UpdatedAt` / `DeletedAt` | `DateTime` / `DateTime?` | Audit and soft delete |
| `UserRoles` | `ICollection<ApplicationUserRole>` | User links |

## Quick start

### 1. Configure the DbContext

```csharp
using AuthLibrary.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<
    ApplicationUser, ApplicationRole, Guid,
    IdentityUserClaim<Guid>, ApplicationUserRole, IdentityUserLogin<Guid>,
    IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUserRole>(b =>
        {
            b.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            b.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
        });
    }
}
```

> `RowVersion` is a `uint`. On PostgreSQL it can be mapped to the `xmin` system column;
> other databases may need a different concurrency setup.

### 2. Register Identity

```csharp
builder.Services
    .AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
```

### 3. Create a user (note the required members)

```csharp
var user = new ApplicationUser
{
    UserName = "jdoe",
    Email = "jdoe@example.com",
    FullName = "John Doe",
    Pseudonym = "jd"
};

await userManager.CreateAsync(user, "P@ssw0rd!");
```

### 4. Soft-delete query filter (optional)

```csharp
builder.Entity<ApplicationUser>().HasQueryFilter(u => u.DeletedAt == null);
```

## JwtSettings

`JwtSettings` is currently an empty class. JWT options will be added in a future release.

## Requirements

- .NET 10
- ASP.NET Core Identity (`Microsoft.Extensions.Identity.Stores`)

## License

[MIT](LICENSE)

## Author

**ViVaKR** · [GitHub](https://github.com/ViVaKR/authentication.jwt)
