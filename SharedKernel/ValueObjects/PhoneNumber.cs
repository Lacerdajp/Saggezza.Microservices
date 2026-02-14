namespace SharedKernel.ValueObjects;

public sealed record PhoneNumber
{
    public string Value { get; internal set; } = string.Empty;
    private PhoneNumber() { }

    private PhoneNumber(string digits) => Value = digits;

    public static PhoneNumber Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Invalid phone number.");

        var digits = OnlyDigits(value);

        if (digits.Length is < 10 or > 11)
            throw new ArgumentException("Invalid phone number.");

        return new PhoneNumber(digits);
    }

    public override string ToString() => Value;

    public static implicit operator string(PhoneNumber phone) => phone.Value;

    public static string OnlyDigits(string value)
        => new(value.Where(char.IsDigit).ToArray());
}
