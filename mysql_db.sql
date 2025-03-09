-- 維修系統資料庫創建腳本
-- 版本: 1.0
-- 創建日期: 2024-03-09

-- 創建數據庫
CREATE DATABASE IF NOT EXISTS RepairSystem CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE RepairSystem;

-- 1. Users 表 (用戶)
CREATE TABLE Users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL,
    Name VARCHAR(100) NOT NULL,
    Email VARCHAR(100),
    Role VARCHAR(20) NOT NULL DEFAULT 'User',
    Phone VARCHAR(20),
    Department VARCHAR(100),
    PasswordHash VARBINARY(64) NOT NULL,
    PasswordSalt VARBINARY(128) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    LastLoginAt DATETIME,
    UNIQUE INDEX IX_Users_Username (Username),
    INDEX IX_Users_Email (Email),
    INDEX IX_Users_Role (Role)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 2. Equipment 表 (設備)
CREATE TABLE Equipment (
    EquipmentId INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    DeviceType VARCHAR(50) NOT NULL,
    SerialNumber VARCHAR(50) NOT NULL,
    Status VARCHAR(20) NOT NULL,
    Department VARCHAR(50) NOT NULL,
    Location VARCHAR(100) NOT NULL,
    PurchaseDate DATETIME NOT NULL,
    LastMaintenanceDate DATETIME,
    Notes TEXT,
    ImageUrl VARCHAR(255),
    CreateTime DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX IX_Equipment_DeviceType (DeviceType),
    INDEX IX_Equipment_Department (Department),
    INDEX IX_Equipment_Status (Status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 3. RepairTickets 表 (報修單)
CREATE TABLE RepairTickets (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(100) NOT NULL,
    Description TEXT NOT NULL,
    DeviceType VARCHAR(50),
    DeviceNumber VARCHAR(50),
    Problem TEXT,
    Solution TEXT,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME ON UPDATE CURRENT_TIMESTAMP,
    CreateTime DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME ON UPDATE CURRENT_TIMESTAMP,
    Status VARCHAR(20) NOT NULL DEFAULT '待處理',
    Priority VARCHAR(20),
    Location VARCHAR(100),
    EquipmentId INT,
    UserId INT NOT NULL,
    HandledBy INT,
    FOREIGN KEY (EquipmentId) REFERENCES Equipment(EquipmentId) ON DELETE SET NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (HandledBy) REFERENCES Users(Id) ON DELETE SET NULL,
    INDEX IX_RepairTickets_Status (Status),
    INDEX IX_RepairTickets_Priority (Priority),
    INDEX IX_RepairTickets_CreatedAt (CreatedAt)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 4. AttachmentFiles 表 (附件文件)
CREATE TABLE AttachmentFiles (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    FileName VARCHAR(255) NOT NULL,
    FilePath VARCHAR(500) NOT NULL,
    FileSize BIGINT NOT NULL,
    FileType VARCHAR(50),
    UploadTime DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    TicketId INT NOT NULL,
    UploadedBy INT,
    FOREIGN KEY (TicketId) REFERENCES RepairTickets(Id) ON DELETE CASCADE,
    FOREIGN KEY (UploadedBy) REFERENCES Users(Id) ON DELETE SET NULL,
    INDEX IX_AttachmentFiles_TicketId (TicketId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 5. MaintenanceRecords 表 (維護記錄)
CREATE TABLE MaintenanceRecords (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    EquipmentId INT NOT NULL,
    MaintenanceDate DATETIME NOT NULL,
    MaintenanceType VARCHAR(50) NOT NULL,
    Description TEXT,
    Technician VARCHAR(100),
    TechnicianId INT,
    Cost DECIMAL(10,2),
    CreateTime DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (EquipmentId) REFERENCES Equipment(EquipmentId) ON DELETE CASCADE,
    FOREIGN KEY (TechnicianId) REFERENCES Users(Id) ON DELETE SET NULL,
    INDEX IX_MaintenanceRecords_EquipmentId (EquipmentId),
    INDEX IX_MaintenanceRecords_MaintenanceDate (MaintenanceDate)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 6. WorkflowDefinitions 表 (工作流定義)
CREATE TABLE WorkflowDefinitions (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Description TEXT,
    Steps JSON NOT NULL,
    IsActive TINYINT(1) NOT NULL DEFAULT 1,
    CreatedBy INT,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (CreatedBy) REFERENCES Users(Id) ON DELETE SET NULL,
    UNIQUE INDEX IX_WorkflowDefinitions_Name (Name)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 初始化默認管理員用戶
-- 注意: 實際部署時請使用正確的密碼雜湊和鹽值
INSERT INTO Users (Username, Name, Email, Role, PasswordHash, PasswordSalt, CreatedAt)
VALUES ('admin', '系統管理員', 'admin@example.com', 'Admin', 
        UNHEX('0000000000000000000000000000000000000000000000000000000000000000'), 
        UNHEX('0000000000000000000000000000000000000000000000000000000000000000'), 
        NOW());

-- 初始化基本設備類型
INSERT INTO Equipment (Name, DeviceType, SerialNumber, Status, Department, Location, PurchaseDate, CreateTime, UpdateTime)
VALUES 
('Dell Latitude 7420', '筆記本電腦', 'SN12345678', '正常使用', '資訊部', '行政大樓 301 室', '2023-01-15', NOW(), NOW()),
('HP LaserJet Pro M402n', '印表機', 'HPSN87654321', '正常使用', '行政部', '行政大樓 201 室', '2022-11-10', NOW(), NOW()),
('Cisco Switch WS-C2960-24TT-L', '網路設備', 'CSSN55667788', '正常使用', '資訊部', '機房 101', '2022-06-20', NOW(), NOW());

-- 初始化基本報修單狀態示例
INSERT INTO RepairTickets (Title, Description, Status, Priority, EquipmentId, UserId, CreatedAt, CreateTime)
VALUES 
('電腦無法開機', '按下電源按鈕後無反應，指示燈不亮', '待處理', '高', 1, 1, NOW(), NOW());

-- 提交事務
COMMIT; 