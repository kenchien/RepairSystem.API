using System.Threading.Tasks;
using RepairSystem.API.Models;

namespace RepairSystem.API.Services
{
    public interface IAuthService
    {
        Task<(bool success, string token, User? user, string message)> AuthenticateAsync(string username, string password);
        
        // 添加缺失的方法
        Task<(bool success, User? user, string message)> RegisterUserAsync(User user, string password);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        
        // 保留現有的方法
        Task<User?> GetUserByIdAsync(int id);
        Task<IEnumerable<User>> GetAllUsersAsync();
        // 其他方法...

        // 添加這個方法
        Task<(bool success, User? user)> ValidateUserAsync(string username, string password);
    }
} 