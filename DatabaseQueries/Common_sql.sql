
CREATE DATABASE cab_management;

use cab_management;


CREATE TABLE Firms
(
    FirmId INT IDENTITY PRIMARY KEY,
    FirmName VARCHAR(100) NOT NULL,
    FirmCode VARCHAR(20) UNIQUE,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    IsDeleted BIT DEFAULT 0
);

CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    FirmId INT NULL,
    FirmType NVARCHAR(20) NULL,		-- Admin, Mill, Company
    UserName VARCHAR(30) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,  -- Store only hashed passwords (updated length)
    Email VARCHAR(255) NULL,             -- Increased length for modern emails
    EmailConfirmed BIT DEFAULT 0,
    MobileNumber VARCHAR(20) NULL,       -- Standardized length
    MobileNumberConfirmed BIT DEFAULT 0,
    AccessFailedCount TINYINT DEFAULT 0,
    IsActive BIT DEFAULT 1,
    LastLoginAt DATETIME NULL,
    SecurityStamp UNIQUEIDENTIFIER DEFAULT NEWID(),  -- Forces logout on critical changes
    LockoutEnd DATETIME NULL,            -- Account lockout expiration
    LockoutEnabled BIT DEFAULT 1,        -- Enable account lockout
    TwoFactorEnabled BIT DEFAULT 0,      -- For 2FA support
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    IsDeleted BIT DEFAULT 0,
    
    -- Constraints

    CONSTRAINT CHK_UserName_Format CHECK (UserName LIKE '[a-zA-Z0-9@_\-.]%'),
    CONSTRAINT CHK_Email_Format CHECK (Email IS NULL OR Email LIKE '%_@__%.__%'),
    CONSTRAINT CHK_MobileNumber_Format CHECK (
        MobileNumber IS NULL OR 
        (MobileNumber LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]' 
        AND LEN(MobileNumber) = 10))
);


CREATE TABLE UserSessions (
    SessionId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId INT NOT NULL,
    Jti NVARCHAR(450) NULL,
    AccessToken VARCHAR(MAX) NULL,           -- Can store JWT if needed (usually not recommended)
    RefreshToken VARCHAR(255) NOT NULL,      -- Hashed refresh token
    RefreshTokenExpiry DATETIME NOT NULL,
    DeviceId VARCHAR(255) NULL,              -- For device fingerprinting
    IPAddress VARCHAR(50) NULL,
    UserAgent VARCHAR(512) NULL,             -- Full browser/device info
    CreatedAt DATETIME DEFAULT GETDATE(),
    LastRefreshedAt DATETIME NULL,
    IsRevoked BIT DEFAULT 0,
    RevokedAt DATETIME NULL,
    RevokedReason VARCHAR(100) NULL,
    
    -- Foreign key with no cascade to preserve sessions on user deletion
    -- CONSTRAINT FK_UserSessions_Users FOREIGN KEY (UserId) 
    --    REFERENCES Users(UserId),
    
    -- Indexes for performance
    INDEX IX_UserSessions_UserId NONCLUSTERED (UserId),
    INDEX IX_UserSessions_RefreshToken NONCLUSTERED (RefreshToken)
);


CREATE TABLE Roles
(
    RoleId SMALLINT IDENTITY PRIMARY KEY,
    RoleName VARCHAR(50),
    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0
);



CREATE TABLE UserRoles
(
    UserRoleId INT IDENTITY(1,1) PRIMARY KEY,
	FirmId INT NULL,
    UserId INT NOT NULL,
    RoleId SMALLINT NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0
   
    -- Foreign key constraints
  --  CONSTRAINT FK_UserRoles_Users FOREIGN KEY (UserId)  REFERENCES Users(UserId),
  --  CONSTRAINT FK_UserRoles_Roles FOREIGN KEY (RoleId)  REFERENCES Roles(RoleId),
        
    -- Composite unique constraint
    CONSTRAINT UQ_User_Role UNIQUE (UserId, RoleId)
);

----------- CABS --------------
CREATE TABLE Cabs(
CabId INT IDENTITY PRIMARY KEY,
OrganizationId INT, 
CabType VARCHAR(50),
IsActive BIT,
IsDeleted BIT,
CreatedAt DATETIME,
UpdatedAt DATETIME
--  CONSTRAINT FK_Cabs_Organizations FOREIGN KEY (OrganizationId)  REFERENCES Organizations(OrganizationId ),
);
------------------CUSTOMER -------------------
CREATE TABLE Customer
(
    CustomerId INT IDENTITY PRIMARY KEY,
    FirmId INT NOT NULL,
    CustomerName VARCHAR(100),
    LogoImagePath VARCHAR(max),
    Address VARCHAR(max),
    GstNumber VARCHAR(20),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    IsDeleted BIT DEFAULT 0
    --CONSTRAINT FK_Customer_Firms FOREIGN KEY (FirmId) REFERENCES Firms(FirmId)
);

------------------DRIVER DETAILS-------------------
CREATE TABLE DriverDetails
(
    DriverDetailId INT IDENTITY PRIMARY KEY,
    FirmId INT NULL,
    UserId INT NULL,
    DriverName VARCHAR(30) NOT NULL,
    MobileNumber VARCHAR(13) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);

------------------DUTY LOCATIONS-------------------
CREATE TABLE DutyLocations
(
    DutyLocationId INT IDENTITY PRIMARY KEY,
    FirmId INT NOT NULL,
    DutyId INT NOT NULL,
    Address VARCHAR(250) NOT NULL,
    GeoLocation VARCHAR(100) NULL,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    IsDeleted BIT DEFAULT 0
);

--CONSTRAINT FK_DutyLocations_Duties FOREIGN KEY (DutyId) REFERENCES Duties(DutyId)
--CONSTRAINT FK_DutyLocations_Firms FOREIGN KEY (FirmId) REFERENCES Firms(FirmId)

------------------CAB NUMBER DIRECTORY-------------------
	CREATE TABLE CabNumberDirectory
(
    CabNumberDirectoryId INT IDENTITY PRIMARY KEY,
    
    FirmId INT NOT NULL,
    
    CabId INT NOT NULL,
    
    CabNumber VARCHAR(20) NOT NULL,
    
    IsActive BIT NOT NULL DEFAULT 1,
    
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    
    UpdatedAt DATETIME NULL,
    
    IsDeleted BIT NOT NULL DEFAULT 0
);
------------------INVOICE ITEMS-------------------
CREATE TABLE InvoiceItems
(
    InvoiceItemId INT IDENTITY(1,1) PRIMARY KEY,
 
    FirmId INT NOT NULL,
 
    InvoiceId INT NOT NULL,
 
    Particulars NVARCHAR(255) NOT NULL,
 
    Quantity INT NOT NULL,
 
    Price DECIMAL(12,2) NOT NULL,
 
    TotalPrice DECIMAL(10,2) NOT NULL,
 
    IsActive BIT NOT NULL DEFAULT 1,
 
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
 
    IsDeleted BIT NOT NULL DEFAULT 0,
);

------------------INVOICE--------------------
CREATE TABLE Invoices
(
    InvoiceId INT IDENTITY(1,1) PRIMARY KEY,
 
    FirmId INT NOT NULL,
    CustomerId INT NOT NULL,
 
    InvoiceDate DATETIME NOT NULL,
    InvoiceNumber NVARCHAR(50) NOT NULL UNIQUE,
 
    IterneryCode NVARCHAR(50),
 
    DutySlipId INT NOT NULL,
 
    TotalAmount DECIMAL(12,2) NOT NULL,
 
    IsActive BIT NOT NULL DEFAULT 1,
 
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
 
    IsDeleted BIT NOT NULL DEFAULT 0,
 
    
);


CREATE TABLE FirmTerms
(
FirmTermId INT IDENTITY PRIMARY KEY,
FirmId INT NOT NULL,
Description VARCHAR(200),
IsActive BIT DEFAULT 1,
CreatedAt DATETIME DEFAULT GETDATE(),
UpdatedAt DATETIME NULL,
IsDeleted BIT DEFAULT 0
-- CONSTRAINT FK_FirmTerms_Firms FOREIGN KEY (FirmId) REFERENCES Firms(FirmId)
);


CREATE TABLE CabPrices
(
    CabPriceId INT IDENTITY(1,1) PRIMARY KEY,
    FirmId INT NOT NULL,
    CabId INT NOT NULL,
    PricingRuleId INT NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);

--------------------DutySlip-----------------------
CREATE TABLE DutySlips
(
    DutySlipId INT IDENTITY PRIMARY KEY,

    BookedDate DATETIME NOT NULL,
    BookedBy INT NOT NULL,

    FirmId INT NOT NULL,
    CustomerId INT NOT NULL,

    ReportingDateTime DATETIME NULL,
    ReportingAddress VARCHAR(500) NULL,
    ReportingGeoLocation VARCHAR(255) NULL,

    RequestedCab INT NULL,
    SentCab INT NULL,

    CabNumber VARCHAR(50) NULL,
    DriverDetailId INT NULL,

    PaymentMode VARCHAR(50) NULL,

    StartKms DECIMAL(10,2) NULL,
    StartKmsImagePath VARCHAR(255) NULL,
    StartDateTime DATETIME NULL,

    CloseKms DECIMAL(10,2) NULL,
    CloseKmsImagePath VARCHAR(255) NULL,
    CloseDateTime DATETIME NULL,

    TotalKms DECIMAL(10,2) NULL,
    TotalTimeInMin INT NULL,

    NextDayInstruction VARCHAR(500) NULL,
    Destination VARCHAR(255) NULL,

    Status VARCHAR(50) NULL,

    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,

    IsDeleted BIT DEFAULT 0
);


CREATE TABLE CustomerUsers
(
CustomerUserId INT IDENTITY PRIMARY KEY,
CustomerId INT NOT NULL,
FirmId INT NULL,
UserName VARCHAR(100),
MobileNumber VARCHAR(20) NULL, --Standardized length     
IsActive BIT DEFAULT 1,
CreatedAt DATETIME DEFAULT GETDATE(),
UpdatedAt DATETIME NULL,
IsDeleted BIT DEFAULT 0
--CONSTRAINT FK_CustomerUsers_Customer FOREIGN KEY (CustomerId) REFERENCES Customer(CustomerId)
)
