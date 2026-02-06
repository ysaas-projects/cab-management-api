
CREATE TABLE PricingRules2
(
    PricingRuleId INT IDENTITY PRIMARY KEY,

    FirmId INT NOT NULL,

    RuleName VARCHAR(150) NOT NULL,              -- e.g. First 80KM / 8Hrs
    RuleCategory VARCHAR(50) NOT NULL,           -- Base, ExtraKM, ExtraHr, Allowance, Tax
    CalculationType VARCHAR(50) NOT NULL,        -- Fixed, PerKm, PerHour, PerDay, Percentage

    ConditionJson NVARCHAR(MAX) NOT NULL,        -- 🔥 Core logic
    RateValue DECIMAL(10,2) NOT NULL,             -- Amount or Rate

    Priority INT NOT NULL DEFAULT 1,              -- Execution order
    IsActive BIT DEFAULT 1,

    CreatedAt DATETIME DEFAULT GETDATE()
);

-- ✅ STEP 4: INSERT REALISTIC PRICING RULES (INDIA)
-- 1️⃣ BASE PACKAGE – First 80 KM / 8 Hrs

INSERT INTO PricingRules2
(
    FirmId, RuleName, RuleCategory, CalculationType,
    ConditionJson, RateValue, Priority
)
VALUES
(
    1,
    'First 80KM / 8Hrs',
    'Base',
    'Fixed',
    '{
        "when": { "always": true },
        "apply": {
            "base_km": 80,
            "base_hr": 8
        }
    }',
    2500,
    1
);

-- 2️⃣ EXTRA KM CHARGE

INSERT INTO PricingRules2
(
    FirmId, RuleName, RuleCategory, CalculationType,
    ConditionJson, RateValue, Priority
)
VALUES
(
    1,
    'Extra KM Charge',
    'ExtraKM',
    'PerKm',
    '{
        "when": { "total_km_gt": 80 },
        "apply": {
            "after_km": 80
        }
    }',
    12,
    2
);


-- 3️⃣ EXTRA HOUR CHARGE

INSERT INTO PricingRules2
(
    FirmId, RuleName, RuleCategory, CalculationType,
    ConditionJson, RateValue, Priority
)
VALUES
(
    1,
    'Extra Hour Charge',
    'ExtraHr',
    'PerHour',
    '{
        "when": { "total_hr_gt": 8 },
        "apply": {
            "after_hr": 8,
            "grace_min": 30
        }
    }',
    150,
    3
);


-- 4️⃣ DRIVER ALLOWANCE (OUTSTATION)

INSERT INTO PricingRules2
(
    FirmId, RuleName, RuleCategory, CalculationType,
    ConditionJson, RateValue, Priority
)
VALUES
(
    1,
    'Driver Allowance',
    'Allowance',
    'Fixed',
    '{
        "when": { "trip_type": "Outstation" },
        "apply": {
            "per": "day"
        }
    }',
    500,
    4
);


-- 5️⃣ NIGHT CHARGE (10 PM – 6 AM)

INSERT INTO PricingRules2
(
    FirmId, RuleName, RuleCategory, CalculationType,
    ConditionJson, RateValue, Priority
)
VALUES
(
    1,
    'Night Charge',
    'Surcharge',
    'Percentage',
    '{
        "when": {
            "night_between": ["22:00", "06:00"]
        },
        "apply": {
            "on": "subtotal"
        }
    }',
    10,
    5
);


-- 6️⃣ TOLL & PARKING (PASS THROUGH)

INSERT INTO PricingRules2
(
    FirmId, RuleName, RuleCategory, CalculationType,
    ConditionJson, RateValue, Priority
)
VALUES
(
    1,
    'Toll and Parking',
    'PassThrough',
    'Fixed',
    '{
        "when": { "always": true },
        "apply": {
            "manual_entry": true
        }
    }',
    0,
    6
);


-- 7️⃣ CANCELLATION CHARGE

INSERT INTO PricingRules2
(
    FirmId, RuleName, RuleCategory, CalculationType,
    ConditionJson, RateValue, Priority
)
VALUES
(
    1,
    'Cancellation Charges',
    'Cancellation',
    'Percentage',
    '{
        "when": {
            "status": "Cancelled",
            "cancel_before_min": 120
        },
        "apply": {
            "on": "base"
        }
    }',
    50,
    1
);


-- 8️⃣ GST (INDIA)

INSERT INTO PricingRules
(
    FirmId, RuleName, RuleCategory, CalculationType,
    ConditionJson, RateValue, Priority
)
VALUES
(
    1,
    'GST 5%',
    'Tax',
    'Percentage',
    '{
        "when": { "always": true },
        "apply": {
            "on": "subtotal"
        }
    }',
    5,
    99
);





CREATE TABLE DutySlipBills
(
    BillId INT IDENTITY PRIMARY KEY,

    DutySlipId INT NOT NULL,
    FirmId INT NOT NULL,
    CustomerId INT NOT NULL,

    BillNumber VARCHAR(50) NOT NULL,
    BillDate DATETIME NOT NULL DEFAULT GETDATE(),

    SubTotal DECIMAL(10,2) NOT NULL DEFAULT 0,
    GstPercentage DECIMAL(5,2) NOT NULL DEFAULT 0,
    GstAmount DECIMAL(10,2) NOT NULL DEFAULT 0,

    RoundOffAmount DECIMAL(10,2) NOT NULL DEFAULT 0,
    GrandTotal DECIMAL(10,2) NOT NULL DEFAULT 0,

    BillStatus VARCHAR(20) NOT NULL DEFAULT 'Draft',  -- Draft / Final / Cancelled

    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,

    IsDeleted BIT DEFAULT 0
);

ALTER TABLE DutySlips
ADD IsBillingLocked BIT DEFAULT 0;

ALTER TABLE DutySlipBills
ADD
    CancelReason VARCHAR(500) NULL,
    CancelledBy INT NULL,
    CancelledAt DATETIME NULL;


-- prevent duplicate bill
CREATE UNIQUE INDEX UX_DutySlipBills_DutySlip
ON DutySlipBills (DutySlipId)
WHERE IsDeleted = 0;




CREATE TABLE DutySlipBillDetails
(
    BillDetailId INT IDENTITY PRIMARY KEY,

    BillId INT NOT NULL,
    PricingRuleId INT NOT NULL,

    RuleName VARCHAR(150) NOT NULL,
    RuleCategory VARCHAR(50) NOT NULL,

    Quantity DECIMAL(10,2) NOT NULL,
    Rate DECIMAL(10,2) NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,

    CreatedAt DATETIME DEFAULT GETDATE()
);


-- 3️⃣ (Optional but Recommended) Foreign Keys (srd not using this)

ALTER TABLE DutySlipBills
ADD CONSTRAINT FK_DutySlipBills_DutySlips
FOREIGN KEY (DutySlipId) REFERENCES DutySlips(DutySlipId);

ALTER TABLE DutySlipBillDetails
ADD CONSTRAINT FK_BillDetails_Bills
FOREIGN KEY (BillId) REFERENCES DutySlipBills(BillId);


