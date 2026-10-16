namespace AuthLibrary.Common;

public class JwtOptions
{
  public const string SectionName = "JwtConfig";
  public string Key { get; init; } = string.Empty;
  public string Issuer { get; set; } = string.Empty;
  public string Audience { get; set; } = string.Empty;
  public int AccessTokenMinuts { get; set; } = 15;
  public int RefreshTokenDays { get; set; } = 7;
}
