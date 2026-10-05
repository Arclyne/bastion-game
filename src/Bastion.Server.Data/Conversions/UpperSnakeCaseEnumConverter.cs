using System;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bastion.Server.Data.Conversions;

public sealed class UpperSnakeCaseEnumConverter<TEnum> : ValueConverter<TEnum, string>
    where TEnum : struct, Enum
{
    private const char Separator = '_';

    public UpperSnakeCaseEnumConverter()
        : base(value => ToCode(value), code => FromCode(code))
    {
    }

    public static string ToCode(TEnum value)
    {
        string name = value.ToString();
        var builder = new StringBuilder(name.Length * 2);
        for (int index = 0; index < name.Length; index++)
        {
            if (index > 0 && char.IsUpper(name[index]))
            {
                builder.Append(Separator);
            }

            builder.Append(char.ToUpperInvariant(name[index]));
        }

        return builder.ToString();
    }

    public static TEnum FromCode(string code)
    {
        string name = code.Replace(Separator.ToString(), string.Empty, StringComparison.Ordinal);
        return Enum.Parse<TEnum>(name, ignoreCase: true);
    }
}
