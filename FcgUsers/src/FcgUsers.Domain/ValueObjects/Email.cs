namespace FcgUsers.Domain.ValueObjects;

public record Email(string Address)
{
    protected Email() : this(string.Empty) { }

    public static Email Create(string address)
    {
        if (string.IsNullOrWhiteSpace(address) || !address.Contains("@"))
            throw new ArgumentException("E-mail inválido.");

        return new Email(address.ToLower().Trim());
    }
}
