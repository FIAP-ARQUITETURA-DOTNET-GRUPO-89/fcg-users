using FcgUsers.Domain.Enums;
using FcgUsers.Domain.Interfaces;
using FcgUsers.Domain.ValueObjects;
//using FcgUsers.SharedKernel.Domain;

namespace FcgUsers.Domain.Entities;

public class User : BaseEntity, IAggregateRoot
{
    public DateTime CreatedAt { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateOnly BirthDate { get; private set; }
    public Email Email { get; private set; }
    public Password Password { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsInactive { get; private set; }

    protected User()
    {
        Email = null!;
        Password = null!;
    }

    public User(string name, DateOnly birthDate, Email email, Password password, UserRole userRole)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        Name = name;
        BirthDate = birthDate;
        Email = email;
        Password = password;
        Role = userRole;
        IsInactive = false;
    }

    public void UpdateProfile(string name, DateOnly birthDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.");

        if (birthDate > DateOnly.FromDateTime(DateTime.Now))
            throw new ArgumentException("Birth date cannot be a future date.");

        Name = name;
        BirthDate = birthDate;
    }

    public void ChangePassword(Password newPassword)
    {
        Password = newPassword;
    }

    public void ChangeRole(UserRole newRole)
    {
        Role = newRole;
    }

    public void Deactivate() => IsInactive = true;

    public bool IsAdmin() => Role == UserRole.Admin;

    public int CalculateAge()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var age = today.Year - BirthDate.Year;
        if (BirthDate > today.AddYears(-age)) age--;
        return age;
    }
}
