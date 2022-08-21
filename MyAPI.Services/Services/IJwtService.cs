using MyAPI.Entities;

namespace MyAPI.Services.Services
{
    public interface IJwtService
    {
        string Generate(User user);
    }
}