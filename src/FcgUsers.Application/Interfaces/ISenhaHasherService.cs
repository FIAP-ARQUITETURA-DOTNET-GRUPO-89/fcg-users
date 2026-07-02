namespace FcgUsers.Application.Interfaces;

public interface ISenhaHasherService
{
    string Hash(string senha);
    bool VerificarSenha(string senha, string senhaHash);
}
