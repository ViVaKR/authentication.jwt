# ViVaKR.AuthLibrary

ASP.NET Core API에서 **JWT 기반 인증**을 구성할 때 거의 매번 똑같이 만들게 되는
`ApplicationUser`, `ApplicationRole`, `ApplicationUserRole` 엔티티를 모아 둔 가벼운 라이브러리입니다.

프로젝트마다 복사해 붙이던 코드를 NuGet 패키지 하나로 해결하세요.

[![NuGet](https://img.shields.io/nuget/v/ViVaKR.AuthLibrary.svg)](https://www.nuget.org/packages/ViVaKR.AuthLibrary)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

[English](README.md)

## 특징

- `ASP.NET Core Identity` 기반, 키 타입은 **`Guid`**
- 감사(Audit) 정보를 위한 `IAuditable` (`CreatedAt`, `UpdatedAt`)
- 논리 삭제(Soft Delete)를 위한 `ISoftDeletable` (`DeletedAt`, `IsDeleted`)
- 사용자 ↔ 역할 연결 엔티티와 탐색 속성(Navigation) 제공
- .NET 10 지원

## 설치

```bash
dotnet add package ViVaKR.AuthLibrary
```

## 구성 요소

네임스페이스는 `AuthLibrary.*` 입니다.

| 구분 | 이름 | 네임스페이스 | 설명 |
|------|------|-------------|------|
| Entity | `ApplicationUser` | `AuthLibrary.Entities` | `IdentityUser<Guid>` 확장 사용자 |
| Entity | `ApplicationRole` | `AuthLibrary.Entities` | `IdentityRole<Guid>` 확장 역할 |
| Entity | `ApplicationUserRole` | `AuthLibrary.Entities` | `IdentityUserRole<Guid>` 확장 연결 엔티티 |
| Interface | `IAuditable` | `AuthLibrary.Interfaces` | 생성/수정 시각 |
| Interface | `ISoftDeletable` | `AuthLibrary.Interfaces` | 논리 삭제 시각 및 삭제 여부 |
| Settings | `JwtSettings` | `AuthLibrary` | JWT 옵션 (준비 중) |

### ApplicationUser 주요 속성

| 속성 | 타입 | 설명 |
|------|------|------|
| `FullName` | `string` (required) | 실명 |
| `Pseudonym` | `string` (required) | 필명/별칭 |
| `Avatar` | `string?` | 프로필 이미지 |
| `Introduction` | `string?` | 자기소개 |
| `IsActive` | `bool` | 활성 여부 (기본 `true`) |
| `RefreshToken` / `RefreshTokenExpiryTime` | `string?` / `DateTime?` | 리프레시 토큰과 만료 시각 |
| `TrustedDeviceKey` / `TrustedDeviceExpiry` | `string?` / `DateTime?` | 신뢰 기기 |
| `LastLoginAt` / `LastLoginIp` | `DateTime?` / `string?` | 마지막 로그인 정보 |
| `Provider` | `string?` | 외부 로그인 제공자 |
| `TimeZone` / `Locale` | `string` | 기본값 `Asia/Seoul` / `ko-KR` |
| `RowVersion` | `uint` | 동시성 토큰 |
| `CreatedAt` / `UpdatedAt` / `DeletedAt` | `DateTime` / `DateTime?` | 감사·논리 삭제 |
| `UserRoles` | `ICollection<ApplicationUserRole>` | 역할 연결 |

### ApplicationRole 주요 속성

| 속성 | 타입 | 설명 |
|------|------|------|
| `KoName` | `string?` | 한글 역할명 |
| `SortOrder` | `int` | 정렬 순서 |
| `IsActive` / `IsSystem` | `bool` | 활성 여부 / 시스템 역할 여부 |
| `Description` | `string?` | 설명 |
| `CreatedAt` / `UpdatedAt` / `DeletedAt` | `DateTime` / `DateTime?` | 감사·논리 삭제 |
| `UserRoles` | `ICollection<ApplicationUserRole>` | 사용자 연결 |

## 빠른 시작

### 1. DbContext 구성

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

> `RowVersion`은 `uint`입니다. PostgreSQL이라면 `xmin` 시스템 컬럼 매핑에 쓸 수 있고,
> 다른 DB에서는 별도 동시성 설정이 필요할 수 있습니다.

### 2. Identity 등록

```csharp
builder.Services
    .AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
```

### 3. 사용자 생성 (required 속성 주의)

```csharp
var user = new ApplicationUser
{
    UserName = "hong",
    Email = "hong@example.com",
    FullName = "홍길동",
    Pseudonym = "의적"
};

await userManager.CreateAsync(user, "P@ssw0rd!");
```

### 4. 논리 삭제 조회 필터 (선택)

```csharp
builder.Entity<ApplicationUser>().HasQueryFilter(u => u.DeletedAt == null);
```

## JwtSettings

`JwtSettings`는 현재 빈 클래스이며, JWT 옵션은 이후 버전에서 제공될 예정입니다.

## 요구 사항

- .NET 10
- ASP.NET Core Identity (`Microsoft.Extensions.Identity.Stores`)

## 라이선스

[MIT](LICENSE)

## 만든 사람

**ViVaKR** · [GitHub](https://github.com/ViVaKR/authentication.jwt)
