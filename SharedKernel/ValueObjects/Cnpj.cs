namespace SharedKernel.ValueObjects;

public sealed record Cnpj
{
    public string Value { get; internal set; } = string.Empty;

    private Cnpj() { }

    private Cnpj(string digits) => Value = digits;

    public static Cnpj Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Invalid CNPJ.");

        var digits = OnlyDigits(value);
        if (!IsValidDigits(digits))
            throw new ArgumentException("Invalid CNPJ.");

        return new Cnpj(digits);
    }

    public override string ToString() => Value;

    public static implicit operator string(Cnpj cnpj) => cnpj.Value;

    public static string OnlyDigits(string value)
        => new(value.Where(char.IsDigit).ToArray());

    private static bool IsValidDigits(string digits)
    {
        if (digits.Length != 14)
            return false;

        if (digits.Distinct().Count() == 1)
            return false;

        int[] mult1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] mult2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

        var temp = digits[..12];
        var sum = 0;
        for (var i = 0; i < 12; i++)
            sum += (temp[i] - '0') * mult1[i];

        var mod = sum % 11;
        var d1 = mod < 2 ? 0 : 11 - mod;
        temp += d1;

        sum = 0;
        for (var i = 0; i < 13; i++)
            sum += (temp[i] - '0') * mult2[i];

        mod = sum % 11;
        var d2 = mod < 2 ? 0 : 11 - mod;

        return digits.EndsWith($"{d1}{d2}", StringComparison.Ordinal);
    }
}
