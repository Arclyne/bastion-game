using System;

namespace Bastion.Client.Controls;

public static class EmailMask
{
    private const int MaxHidden = 6;

    private const int VisibleCharacters = 2;
    private const char AtSign = '@';
    private const char MaskCharacter = '*';

    public static string Apply(string email)
    {
        ArgumentNullException.ThrowIfNull(email);

        int atIndex = email.IndexOf(AtSign);

        if (atIndex < VisibleCharacters)
        {
            return email;
        }

        string local = email[..atIndex];
        int hidden = Math.Min(local.Length - VisibleCharacters, MaxHidden);

        return local[0] + new string(MaskCharacter, hidden) + local[^1] + email[atIndex..];
    }
}
