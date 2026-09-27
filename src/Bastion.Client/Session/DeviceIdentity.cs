using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Bastion.Client.Session;

// The session stores a device fingerprint and the client version (CU-01 step 5); the fingerprint is a hash so the
// machine and user names never leave the computer.
public static class DeviceIdentity
{
    private const string UnknownVersion = "0.0.0";
    private const char Separator = '|';

    public static string GetFingerprint()
    {
        string source = Environment.MachineName + Separator + Environment.UserName;
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(source)));
    }

    public static string GetClientVersion()
    {
        Version? version = Assembly.GetExecutingAssembly().GetName().Version;
        return version?.ToString() ?? UnknownVersion;
    }
}
