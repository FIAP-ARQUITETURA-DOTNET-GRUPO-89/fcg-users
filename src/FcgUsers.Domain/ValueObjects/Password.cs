namespace FcgUsers.Domain.ValueObjects;

public record Password
{
    public string Hash { get; }

    private Password(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new ArgumentException("O hash da senha não pode ser vazio.");
        }

        Hash = hash;
    }

    public static Password FromHash(string hash) => new(hash);

    public static void ValidarTextoPuro(string senhaPura)
    {
        if (senhaPura.Length < 8 || senhaPura.Length > 12)
        {
            throw new ArgumentException("A senha deve ter entre 8 e 12 caracteres.");
        }
    }
}
