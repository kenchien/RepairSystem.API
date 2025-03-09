using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepairSystem.API.Data;
using RepairSystem.API.Models;

namespace RepairSystem.API.Services
{
    /// <summary>
    /// 使用Dapper實現的認證服務
    /// </summary>
    public class DapperAuthService : IAuthService
    {
        private readonly IRepairRepository _repository;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// 構建DapperAuthService的新實例
        /// </summary>
        /// <param name="repository">數據訪問存儲庫</param>
        /// <param name="configuration">應用程序配置</param>
        public DapperAuthService(IRepairRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        /// <summary>
        /// 驗證用戶名和密碼
        /// </summary>
        /// <param name="username">用戶名</param>
        /// <param name="password">密碼</param>
        /// <returns>驗證結果包含成功狀態、令牌、用戶信息和消息</returns>
        public async Task<(bool success, string token, User? user, string message)> AuthenticateAsync(string username, string password)
        {
            var user = await _repository.GetUserByUsernameAsync(username);
            
            if (user == null)
                return (false, string.Empty, null, "用戶不存在");
                
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return (false, string.Empty, null, "密碼不正確");
                
            var token = GenerateJwtToken(user);
            
            return (true, token, user, "驗證成功");
        }

        /// <summary>
        /// 註冊新用戶
        /// </summary>
        /// <param name="user">用戶信息</param>
        /// <param name="password">密碼</param>
        /// <returns>註冊結果包含成功狀態、用戶信息和消息</returns>
        public async Task<(bool success, User? user, string message)> RegisterUserAsync(User user, string password)
        {
            var existingUser = await _repository.GetUserByUsernameAsync(user.Username);
            if (existingUser != null)
                return (false, null, "用戶名已存在");
                
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            
            var createdUser = await _repository.CreateUserAsync(user);
            return (true, createdUser, "註冊成功");
        }

        /// <summary>
        /// 更改用戶密碼
        /// </summary>
        /// <param name="userId">用戶ID</param>
        /// <param name="currentPassword">當前密碼</param>
        /// <param name="newPassword">新密碼</param>
        /// <returns>密碼更改是否成功</returns>
        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _repository.GetUserByIdAsync(userId);
            if (user == null)
                return false;
                
            if (!VerifyPasswordHash(currentPassword, user.PasswordHash, user.PasswordSalt))
                return false;
                
            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
            
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            
            await _repository.UpdateUserAsync(user);
            return true;
        }

        /// <summary>
        /// 根據ID獲取用戶信息
        /// </summary>
        /// <param name="id">用戶ID</param>
        /// <returns>用戶信息</returns>
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _repository.GetUserByIdAsync(id);
        }

        /// <summary>
        /// 獲取所有用戶信息
        /// </summary>
        /// <returns>用戶集合</returns>
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _repository.GetAllUsersAsync();
        }

        /// <summary>
        /// 驗證用戶信息
        /// </summary>
        /// <param name="username">用戶名</param>
        /// <param name="password">密碼</param>
        /// <returns>驗證結果包含成功狀態和用戶信息</returns>
        public async Task<(bool success, User? user)> ValidateUserAsync(string username, string password)
        {
            var user = await _repository.GetUserByUsernameAsync(username);
            
            if (user == null)
                return (false, null);
                
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return (false, null);
                
            return (true, user);
        }

        // 輔助方法
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != storedHash[i]) 
                    return false;
            }
            return true;
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("JWT:Key").Value ?? throw new InvalidOperationException("JWT Key not configured")));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds,
                Issuer = _configuration.GetSection("JWT:Issuer").Value,
                Audience = _configuration.GetSection("JWT:Audience").Value
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            return tokenHandler.WriteToken(token);
        }
    }
} 