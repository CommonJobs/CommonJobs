To create passwords for users (replacement for Google Authentication), see http://csharppad.com/gist/dde4a660e7aeef5862364ca7c071cbc7

```csharp
const string ConstantSalt = "oi83baqd15w2#";
var passwordSalt = Guid.NewGuid();
var username = "newuser";
var password = "123456";
using (var sha = System.Security.Cryptography.SHA256.Create())
{
    var userId = "users/" + username;
    var computedHash = sha.ComputeHash(
        passwordSalt.ToByteArray()
        .Concat(Encoding.Unicode.GetBytes(userId + password + ConstantSalt))
        .ToArray());
    var hashedPassword = Convert.ToBase64String(computedHash);
    Console.WriteLine($@"
id: {userId}
{{
    ""UserName"": ""{username}"",
    ""HashedPassword"": ""{hashedPassword}"",
    ""PasswordSalt"": ""{passwordSalt}"",
    ""Roles"": null
}}");
}
```
