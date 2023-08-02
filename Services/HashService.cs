using System.Security.Cryptography;
using API.DTOs;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace API.Services;

public class HashService
{
    public ResultHash Hash(string planeText)
    {
        var sal = new byte[16];
        using (var random = RandomNumberGenerator.Create())
        {
            random.GetBytes(sal);
        }

        return HashResult(planeText, sal);
    }

    private ResultHash HashResult(string planeText, byte[] sal)
    {
        var key = KeyDerivation.Pbkdf2(
            password: planeText,
            salt: sal,
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 32);

        var hash = Convert.ToBase64String(key);

        return new ResultHash()
        {
            Hash = hash,
            Sal = sal
        };
    }
}