using System.Threading.Tasks;

namespace Shop.Application.Auth
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto request);
        Task<string> LoginAsync(LoginDto request);
        Task<string> GoogleLoginAsync(string credentialToken);
        Task<bool> SoftDeleteUserAsync(int userId);
    }
}