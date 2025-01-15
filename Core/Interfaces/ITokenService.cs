using Core.Entities;

namespace Core.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(SystemUser user);
    }
}
