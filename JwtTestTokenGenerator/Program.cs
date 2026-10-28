
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

var key = Environment.GetEnvironmentVariable("JWT_KEY")
    ?? throw new InvalidOperationException(
        "Set the JWT_KEY environment variable.");

if (Encoding.UTF8.GetByteCount(key) < 32)
    throw new InvalidOperationException(
        "JWT key must be at least 32 bytes.");

var signingKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(key));

var credentials = new SigningCredentials(
    signingKey,
    SecurityAlgorithms.HmacSha256);

var token = new JwtSecurityToken(
    issuer: "UserManagementAPI",
    audience: "UserManagementAPIClients",
    claims: new[]
    {
        new Claim(ClaimTypes.NameIdentifier, "test-user"),
        new Claim(ClaimTypes.Name, "Test User")
    },
    notBefore: DateTime.UtcNow,
    expires: DateTime.UtcNow.AddMinutes(30),
    signingCredentials: credentials
);

Console.WriteLine(
    new JwtSecurityTokenHandler().WriteToken(token));
