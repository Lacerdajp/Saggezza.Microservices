using System.Text.RegularExpressions;

namespace SharedKernel.ValueObjects;

public sealed record Email
{
    private static readonly Regex EmailRegex = new(
        @"^[^\s@]+@[^\s@]+\.[^\s@]+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; internal set; } = string.Empty;

    private Email() { }

    private Email(string value) => Value = value;

    public static Email Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || !EmailRegex.IsMatch(value))
            throw new ArgumentException("Invalid email.");

        return new Email(value.Trim());
    }

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;
}
