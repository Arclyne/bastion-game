using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Bastion.Client.Networking;

public sealed class ThumbprintCertificateValidator : X509CertificateValidator
{
    private readonly string _expectedThumbprint;

    public ThumbprintCertificateValidator(string expectedThumbprint)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(expectedThumbprint);

        _expectedThumbprint = expectedThumbprint;
    }

    public override void Validate(X509Certificate2 certificate)
    {
        ArgumentNullException.ThrowIfNull(certificate);

        string thumbprint = certificate.GetCertHashString(HashAlgorithmName.SHA256);
        if (!string.Equals(thumbprint, _expectedThumbprint, StringComparison.OrdinalIgnoreCase))
        {
            throw new SecurityTokenValidationException(
                $"The server certificate is not trusted. Thumbprint={thumbprint}");
        }
    }
}
