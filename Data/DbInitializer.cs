using RepairSystem.API.Models;
using BCrypt.Net;

namespace RepairSystem.API.Data
{
    public static class DbInitializer
    {
        public static void Initialize(RepairDbContext context)
        {
            context.Database.EnsureCreated();

            // 如果已有用戶資料，則不需要初始化
            if (context.Users.Any())
                return;

            var users = new User[]
            {
                new User
                {
                    Username = "admin",
                    Name = "系統管理員",
                    Email = "admin@example.com",
                    Role = "Admin",
                    Password = "Admin123", // 這個會在下面被轉換為哈希
                    CreatedAt = DateTime.Now
                },
                new User
                {
                    Username = "user",
                    Password = BCrypt.Net.BCrypt.HashPassword("user123"),
                    Name = "一般用戶",
                    Department = "業務部",
                    Role = "User",
                    CreatedAt = DateTime.Now
                },
                new User
                {
                    Username = "tech",
                    Password = BCrypt.Net.BCrypt.HashPassword("tech123"),
                    Name = "技術人員",
                    Department = "IT部",
                    Role = "Technician",
                    CreatedAt = DateTime.Now
                }
            };

            // 添加在用戶創建後生成密碼哈希的代碼
            foreach (var user in users)
            {
                if (!string.IsNullOrEmpty(user.Password))
                {
                    CreatePasswordHash(user.Password, out byte[] passwordHash, out byte[] passwordSalt);
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                }
            }

            context.Users.AddRange(users);
            context.SaveChanges();

            // 添加一些示例報修單
            var tickets = new RepairTicket[]
            {
                new RepairTicket
                {
                    UserId = 2, // 一般用戶
                    DeviceType = "電腦",
                    DeviceNumber = "PC-001",
                    Problem = "無法開機",
                    Status = "待處理",
                    Priority = "高",
                    CreateTime = DateTime.Now.AddDays(-5),
                    UpdateTime = DateTime.Now.AddDays(-5)
                },
                new RepairTicket
                {
                    UserId = 2, // 一般用戶
                    DeviceType = "印表機",
                    DeviceNumber = "P002",
                    Problem = "卡紙",
                    Status = "處理中",
                    Priority = "中",
                    CreateTime = DateTime.Now.AddDays(-3),
                    UpdateTime = DateTime.Now.AddDays(-2),
                    HandledBy = 3, // 技術人員
                    Solution = "更換電源供應器"
                }
            };

            context.RepairTickets.AddRange(tickets);
            context.SaveChanges();
        }

        // 在 DbInitializer 類中添加密碼哈希生成方法
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
} 