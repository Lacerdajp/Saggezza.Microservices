namespace AuthService.Application.Security;

using System.Text.RegularExpressions;

public static class PasswordPolicy
{
    // Minimum 8 chars, at least 1 upper, 1 lower, 1 digit, 1 special.
    private static readonly Regex StrongPasswordRegex = new(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public const int MinimumLength = 8;

    public static void EnsureStrong(string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required");

        if (!StrongPasswordRegex.IsMatch(password))
            throw new ArgumentException(
                "Password must be at least 8 characters and include uppercase, lowercase, number, and special character");
    }
}
