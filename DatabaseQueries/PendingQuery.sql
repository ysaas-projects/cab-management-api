

CREATE TABLE FirmDetails
(
    FirmDetailsId INT IDENTITY PRIMARY KEY,
    FirmId INT NOT NULL,  
    Address VARCHAR(255),--null
    ContactNumber VARCHAR(15),--null
    ContactPerson VARCHAR(100),--null
    LogoImagePath VARCHAR(255),--null
    GstNumber VARCHAR(50),--null
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    IsDeleted BIT DEFAULT 0,
     -- Foreign key constraints
        --CONSTRAINT FK_FirmDetails_Firms
        --FOREIGN KEY (FirmId) REFERENCES Firms(FirmId)
);


CREATE TABLE PricingRules
(
    PricingRuleId INT IDENTITY(1,1) PRIMARY KEY,
    FirmId INT NOT NULL,
    RoleDetails NVARCHAR(500) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);


EXEC sp_rename 
    'PricingRules.RoleDetails',
    'RuleDetails',
    'COLUMN';


CREATE TABLE UserRoles
(
    UserRoleId INT IDENTITY(1,1) PRIMARY KEY,
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

ALTER TABLE UserRoles ADD FirmId INT NULL;


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


CREATE TABLE TourPackages (
    PackageId       INT IDENTITY PRIMARY KEY,
    FirmId        INT NOT NULL,
    PackageName     NVARCHAR(200) NOT NULL,
    Description     NVARCHAR(MAX),
    Location        NVARCHAR(200),
    DurationDays    INT,
    DurationNights  INT,
    BasePrice       DECIMAL(10,2),
    IsActive        BIT DEFAULT 1,
    CreatedAt       DATETIME DEFAULT GETDATE(),
    UpdatedAt    DATETIME NULL,
    IsDeleted BIT DEFAULT 0
    -- CONSTRAINT FK_TourPackages_Tenants
   --     FOREIGN KEY (TenantId) REFERENCES Tenants(TenantId)
);

CREATE TABLE Seasons (
    SeasonId        INT IDENTITY PRIMARY KEY,
    FirmId        INT NOT NULL,
    SeasonName      NVARCHAR(100), -- Peak / Off-Season / Monsoon / Festival
    StartDate       DATE,
    EndDate         DATE,
    IsActive        BIT DEFAULT 1,
    CreatedAt       DATETIME DEFAULT GETDATE(),
    UpdatedAt    DATETIME NULL,
    IsDeleted BIT DEFAULT 0
    -- CONSTRAINT FK_Seasons_Tenants
     --   FOREIGN KEY (TenantId) REFERENCES Tenants(TenantId)
);

CREATE TABLE PackagePricings (
    PricingId       INT IDENTITY PRIMARY KEY,
    PackageId       INT NOT NULL,
    DayType         VARCHAR(20) CHECK (DayType IN ('Weekday','Weekend')),
    PricePerPerson  DECIMAL(10,2),
    MinPersons      INT DEFAULT 1,
    CreatedAt       DATETIME DEFAULT GETDATE(),
    IsDeleted BIT DEFAULT 0
    -- CONSTRAINT FK_PackagePricing_Package
     --   FOREIGN KEY (PackageId) REFERENCES TourPackages(PackageId)
);

CREATE TABLE DutySlipCustomerUsers (
    DutySlipCustomerUserId INT IDENTITY PRIMARY KEY,
    DutySlipId INT NOT NULL,
    CustomerUserId INT NOT NULL,

    CreatedAt DATETIME DEFAULT GETDATE(),
    IsDeleted BIT DEFAULT 0,
    
    --CONSTRAINT FK_DSCU_DutySlip 
        --FOREIGN KEY (DutySlipId) REFERENCES DutySlips(DutySlipId),

    --CONSTRAINT FK_DSCU_CustomerUser 
        --FOREIGN KEY (CustomerUserId) REFERENCES CustomerUsers(CustomerUserId)
);