/*
===============================================================================
 BLOOD BANK MANAGEMENT SYSTEM V2
===============================================================================
 Base de datos diseñada para un sistema moderno de Banco de Sangre.

 Arquitectura objetivo:
    Backend : ASP.NET Core .NET 10 Web API
    Frontend: React 19 + TypeScript
    ORM     : Entity Framework Core
    Database: PostgreSQL 17

 Diseño basado en:
    - Clean Architecture
    - Domain Driven Design (DDD)
    - CQRS
    - Seguridad basada en Roles
    - Auditoría completa
    - Trazabilidad de unidades de sangre

 MÓDULOS DEL SISTEMA
 ------------------------------------------------------------------------------
 1. Seguridad
    - Roles
    - Usuarios
    - Tokens de actualización
    - Auditoría

 2. Gestión de donantes
    - Información personal
    - Historial médico
    - Medicamentos
    - Restricciones de donación

 3. Donaciones
    - Citas
    - Evaluación previa
    - Registro de extracción

 4. Laboratorio
    - Screening de enfermedades
    - Validación médica

 5. Trazabilidad de sangre
    - Unidades individuales
    - Código QR único
    - Estados y movimientos

 6. Inventario
    - Ubicaciones físicas
    - Reservas
    - Transferencias
    - Descarte

 7. Reportes y notificaciones

 Convenciones:
    - UUID como llave primaria.
    - Soft Delete.
    - Fechas en UTC.
    - Auditoría completa en todas las tablas (CreatedAt, UpdatedAt,
      CreatedBy, UpdatedBy, IsDeleted).
    - Índices parciales con WHERE IsDeleted = FALSE para eficiencia.
===============================================================================
*/


-- =============================================================================
-- EXTENSIONES NECESARIAS
-- =============================================================================

CREATE EXTENSION IF NOT EXISTS "pgcrypto";


-- =============================================================================
-- CONVENCIÓN DE AUDITORÍA
-- =============================================================================
/*
Todas las tablas principales incluyen:

    CreatedAt   TIMESTAMPTZ NOT NULL DEFAULT NOW()
    UpdatedAt   TIMESTAMPTZ
    CreatedBy   UUID
    UpdatedBy   UUID
    IsDeleted   BOOLEAN NOT NULL DEFAULT FALSE

Los índices operacionales usan WHERE IsDeleted = FALSE
para excluir registros eliminados lógicamente.
*/


-- =============================================================================
-- PARTE 1/8 — CATÁLOGOS MAESTROS
-- =============================================================================


-- -----------------------------------------------------------------------------
-- CATÁLOGO: TIPOS DE SANGRE
-- -----------------------------------------------------------------------------

CREATE TABLE BloodTypes
(
    Id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    Code            VARCHAR(5)   NOT NULL UNIQUE,
    Description     VARCHAR(50)  NOT NULL,

    CreatedAt       TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    UpdatedAt       TIMESTAMPTZ,

    CreatedBy       UUID,
    UpdatedBy       UUID,

    IsDeleted       BOOLEAN      NOT NULL DEFAULT FALSE
);

COMMENT ON TABLE BloodTypes IS
'Catálogo oficial de grupos sanguíneos y factor Rh';


-- -----------------------------------------------------------------------------
-- CATÁLOGO: GÉNEROS
-- -----------------------------------------------------------------------------

CREATE TABLE Genders
(
    Id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    Name            VARCHAR(50)  NOT NULL UNIQUE,

    CreatedAt       TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    UpdatedAt       TIMESTAMPTZ,

    CreatedBy       UUID,
    UpdatedBy       UUID,

    IsDeleted       BOOLEAN      NOT NULL DEFAULT FALSE
);

COMMENT ON TABLE Genders IS
'Catálogo de géneros para personas';


-- -----------------------------------------------------------------------------
-- CATÁLOGO: ESTADOS DE DONACIÓN
-- -----------------------------------------------------------------------------

CREATE TABLE DonationStatuses
(
    Id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    Code            VARCHAR(30)  NOT NULL UNIQUE,
    Description     VARCHAR(150) NOT NULL,

    CreatedAt       TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    UpdatedAt       TIMESTAMPTZ,

    CreatedBy       UUID,
    UpdatedBy       UUID,

    IsDeleted       BOOLEAN      NOT NULL DEFAULT FALSE
);

COMMENT ON TABLE DonationStatuses IS
'Estados del proceso de una donación';


-- -----------------------------------------------------------------------------
-- CATÁLOGO: ESTADOS DE UNIDADES DE SANGRE
-- -----------------------------------------------------------------------------

CREATE TABLE BloodUnitStatuses
(
    Id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    Code            VARCHAR(30)  NOT NULL UNIQUE,
    Description     VARCHAR(150) NOT NULL,

    CreatedAt       TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    UpdatedAt       TIMESTAMPTZ,

    CreatedBy       UUID,
    UpdatedBy       UUID,

    IsDeleted       BOOLEAN      NOT NULL DEFAULT FALSE
);

COMMENT ON TABLE BloodUnitStatuses IS
'Controla el ciclo de vida de una bolsa de sangre';


-- -----------------------------------------------------------------------------
-- CATÁLOGO: TIPOS DE MOVIMIENTO DE INVENTARIO
-- -----------------------------------------------------------------------------

CREATE TABLE InventoryMovementTypes
(
    Id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    Code            VARCHAR(30)  NOT NULL UNIQUE,
    Description     VARCHAR(150) NOT NULL,

    CreatedAt       TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    UpdatedAt       TIMESTAMPTZ,

    CreatedBy       UUID,
    UpdatedBy       UUID,

    IsDeleted       BOOLEAN      NOT NULL DEFAULT FALSE
);

COMMENT ON TABLE InventoryMovementTypes IS
'Tipos de movimiento de una unidad de sangre';


-- -----------------------------------------------------------------------------
-- CATÁLOGO: TIPOS DE NOTIFICACIÓN
-- -----------------------------------------------------------------------------

CREATE TABLE NotificationTypes
(
    Id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    Code            VARCHAR(50)  NOT NULL UNIQUE,
    Description     VARCHAR(150) NOT NULL,

    CreatedAt       TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    UpdatedAt       TIMESTAMPTZ,

    CreatedBy       UUID,
    UpdatedBy       UUID,

    IsDeleted       BOOLEAN      NOT NULL DEFAULT FALSE
);

COMMENT ON TABLE NotificationTypes IS
'Tipos de mensajes enviados por el sistema';


-- -----------------------------------------------------------------------------
-- CATÁLOGO: COMPONENTES SANGUÍNEOS
-- -----------------------------------------------------------------------------

CREATE TABLE BloodComponents
(
    Id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    Code            VARCHAR(10)  NOT NULL UNIQUE,
    Name            VARCHAR(100) NOT NULL,
    ShelfLifeDays   INTEGER      NOT NULL,

    CreatedAt       TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    UpdatedAt       TIMESTAMPTZ,

    CreatedBy       UUID,
    UpdatedBy       UUID,

    IsDeleted       BOOLEAN      NOT NULL DEFAULT FALSE
);

COMMENT ON TABLE BloodComponents IS
'Componentes derivados de la sangre: sangre completa, glóbulos rojos, plaquetas, plasma';


-- -----------------------------------------------------------------------------
-- ÍNDICES — CATÁLOGOS (parciales: excluyen registros eliminados)
-- -----------------------------------------------------------------------------

CREATE INDEX IX_BloodTypes_Code
ON BloodTypes(Code)
WHERE IsDeleted = FALSE;

CREATE INDEX IX_Genders_Name
ON Genders(Name)
WHERE IsDeleted = FALSE;

CREATE INDEX IX_DonationStatuses_Code
ON DonationStatuses(Code)
WHERE IsDeleted = FALSE;

CREATE INDEX IX_BloodUnitStatuses_Code
ON BloodUnitStatuses(Code)
WHERE IsDeleted = FALSE;

CREATE INDEX IX_InventoryMovementTypes_Code
ON InventoryMovementTypes(Code)
WHERE IsDeleted = FALSE;

CREATE INDEX IX_NotificationTypes_Code
ON NotificationTypes(Code)
WHERE IsDeleted = FALSE;

CREATE INDEX IX_BloodComponents_Code
ON BloodComponents(Code)
WHERE IsDeleted = FALSE;


-- =============================================================================
-- PARTE 2/8 — SEGURIDAD
-- =============================================================================


-- -----------------------------------------------------------------------------
-- TABLA: ROLES
-- -----------------------------------------------------------------------------

CREATE TABLE Roles
(
    Id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    Name            VARCHAR(100) NOT NULL UNIQUE,
    Description     VARCHAR(250),

    CreatedAt       TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    UpdatedAt       TIMESTAMPTZ,

    CreatedBy       UUID,
    UpdatedBy       UUID,

    IsDeleted       BOOLEAN      NOT NULL DEFAULT FALSE
);

COMMENT ON TABLE Roles IS
'Roles del sistema para control de acceso basado en roles (RBAC)';


-- -----------------------------------------------------------------------------
-- TABLA: USUARIOS
-- -----------------------------------------------------------------------------

CREATE TABLE Users
(
    Id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    RoleId          UUID         NOT NULL,

    Username        VARCHAR(100) NOT NULL UNIQUE,
    Email           VARCHAR(150) NOT NULL UNIQUE,

    PasswordHash    VARCHAR(500) NOT NULL,

    IsActive        BOOLEAN      NOT NULL DEFAULT TRUE,

    LastLoginAt     TIMESTAMPTZ,

    CreatedAt       TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    UpdatedAt       TIMESTAMPTZ,

    CreatedBy       UUID,
    UpdatedBy       UUID,

    IsDeleted       BOOLEAN      NOT NULL DEFAULT FALSE,

    CONSTRAINT FK_Users_Role
        FOREIGN KEY (RoleId)
        REFERENCES Roles(Id)
);

COMMENT ON TABLE Users IS
'Usuarios del sistema con credenciales y rol asignado';


-- -----------------------------------------------------------------------------
-- TABLA: REFRESH TOKENS
-- -----------------------------------------------------------------------------

CREATE TABLE RefreshTokens
(
    Id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    UserId          UUID         NOT NULL,

    Token           VARCHAR(500) NOT NULL UNIQUE,

    ExpiresAt       TIMESTAMPTZ  NOT NULL,
    RevokedAt       TIMESTAMPTZ,

    CreatedAt       TIMESTAMPTZ  NOT NULL DEFAULT NOW(),

    CONSTRAINT FK_RefreshTokens_User
        FOREIGN KEY (UserId)
        REFERENCES Users(Id)
);

COMMENT ON TABLE RefreshTokens IS
'Tokens de renovación JWT para autenticación persistente';


-- -----------------------------------------------------------------------------
-- TABLA: AUDITORÍA DEL SISTEMA
-- -----------------------------------------------------------------------------

CREATE TABLE AuditLogs
(
    Id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    UserId          UUID,

    TableName       VARCHAR(100) NOT NULL,
    RecordId        UUID         NOT NULL,

    Action          VARCHAR(20)  NOT NULL,   -- INSERT, UPDATE, DELETE

    OldValues       JSONB,
    NewValues       JSONB,

    IpAddress       VARCHAR(50),
    UserAgent       VARCHAR(500),

    CreatedAt       TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

COMMENT ON TABLE AuditLogs IS
'Registro de cambios en todas las entidades del sistema';


-- -----------------------------------------------------------------------------
-- ÍNDICES — SEGURIDAD
-- -----------------------------------------------------------------------------

CREATE INDEX IX_Users_Username
ON Users(Username)
WHERE IsDeleted = FALSE;

CREATE INDEX IX_Users_Email
ON Users(Email)
WHERE IsDeleted = FALSE;

CREATE INDEX IX_Users_Role
ON Users(RoleId)
WHERE IsDeleted = FALSE;

CREATE INDEX IX_RefreshTokens_Token
ON RefreshTokens(Token);

CREATE INDEX IX_RefreshTokens_User
ON RefreshTokens(UserId);

CREATE INDEX IX_AuditLogs_Table_Record
ON AuditLogs(TableName, RecordId);

CREATE INDEX IX_AuditLogs_User
ON AuditLogs(UserId);

CREATE INDEX IX_AuditLogs_CreatedAt
ON AuditLogs(CreatedAt);


-- =============================================================================
-- PARTE 3/8 — UBICACIÓN Y PERSONAL
-- =============================================================================


-- -----------------------------------------------------------------------------
-- TABLA: DIRECCIONES
-- -----------------------------------------------------------------------------

CREATE TABLE Addresses
(
    Id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    Country         VARCHAR(100) NOT NULL,
    State           VARCHAR(100),
    City            VARCHAR(100) NOT NULL,

    AddressLine1    VARCHAR(250) NOT NULL,
    AddressLine2    VARCHAR(250),

    PostalCode      VARCHAR(20),

    Latitude        NUMERIC(10,7),
    Longitude       NUMERIC(10,7),

    CreatedAt       TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    UpdatedAt       TIMESTAMPTZ,

    CreatedBy       UUID,
    UpdatedBy       UUID,

    IsDeleted       BOOLEAN      NOT NULL DEFAULT FALSE
);

COMMENT ON TABLE Addresses IS
'Direcciones reutilizables para donantes, personal y bancos de sangre';


-- -----------------------------------------------------------------------------
-- TABLA: BANCOS DE SANGRE
-- -----------------------------------------------------------------------------

CREATE TABLE BloodBanks
(
    Id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    Name            VARCHAR(200) NOT NULL,
    Code            VARCHAR(50)  NOT NULL UNIQUE,

    AddressId       UUID         NOT NULL,

    PhoneNumber     VARCHAR(30),
    Email           VARCHAR(150),

    IsActive        BOOLEAN      NOT NULL DEFAULT TRUE,

    CreatedAt       TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    UpdatedAt       TIMESTAMPTZ,

    CreatedBy       UUID,
    UpdatedBy       UUID,

    IsDeleted       BOOLEAN      NOT NULL DEFAULT FALSE,

    CONSTRAINT FK_BloodBanks_Address
        FOREIGN KEY (AddressId)
        REFERENCES Addresses(Id)
);

COMMENT ON TABLE BloodBanks IS
'Centros de almacenamiento y administración de sangre';


-- -----------------------------------------------------------------------------
-- TABLA: CATEGORÍAS DEL PERSONAL
-- -----------------------------------------------------------------------------

CREATE TABLE StaffCategories
(
    Id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    Name            VARCHAR(100) NOT NULL UNIQUE,
    Description     VARCHAR(250),

    CreatedAt       TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    UpdatedAt       TIMESTAMPTZ,

    CreatedBy       UUID,
    UpdatedBy       UUID,

    IsDeleted       BOOLEAN      NOT NULL DEFAULT FALSE
);

COMMENT ON TABLE StaffCategories IS
'Categorías del personal: Médico, Enfermero, Técnico de laboratorio, Recepcionista';


-- -----------------------------------------------------------------------------
-- TABLA: PERSONAL
-- -----------------------------------------------------------------------------

CREATE TABLE Staff
(
    Id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    BloodBankId     UUID         NOT NULL,
    UserId          UUID,
    CategoryId      UUID         NOT NULL,
    AddressId       UUID,

    EmployeeCode    VARCHAR(50)  NOT NULL UNIQUE,

    FirstName       VARCHAR(100) NOT NULL,
    LastName        VARCHAR(100) NOT NULL,

    Identification  VARCHAR(50),
    PhoneNumber     VARCHAR(30),

    HireDate        DATE,

    IsActive        BOOLEAN      NOT NULL DEFAULT TRUE,

    CreatedAt       TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    UpdatedAt       TIMESTAMPTZ,

    CreatedBy       UUID,
    UpdatedBy       UUID,

    IsDeleted       BOOLEAN      NOT NULL DEFAULT FALSE,

    CONSTRAINT FK_Staff_BloodBank
        FOREIGN KEY (BloodBankId)
        REFERENCES BloodBanks(Id),

    CONSTRAINT FK_Staff_User
        FOREIGN KEY (UserId)
        REFERENCES Users(Id),

    CONSTRAINT FK_Staff_Category
        FOREIGN KEY (CategoryId)
        REFERENCES StaffCategories(Id),

    CONSTRAINT FK_Staff_Address
        FOREIGN KEY (AddressId)
        REFERENCES Addresses(Id)
);

COMMENT ON TABLE Staff IS
'Personal médico y administrativo del banco de sangre';


-- -----------------------------------------------------------------------------
-- ÍNDICES — UBICACIÓN Y PERSONAL
-- -----------------------------------------------------------------------------

CREATE INDEX IX_BloodBanks_Code
ON BloodBanks(Code)
WHERE IsDeleted = FALSE;

CREATE INDEX IX_BloodBanks_Name
ON BloodBanks(Name)
WHERE IsDeleted = FALSE;

CREATE INDEX IX_Staff_EmployeeCode
ON Staff(EmployeeCode)
WHERE IsDeleted = FALSE;

CREATE INDEX IX_Staff_BloodBank
ON Staff(BloodBankId)
WHERE IsDeleted = FALSE;

CREATE INDEX IX_Staff_Category
ON Staff(CategoryId)
WHERE IsDeleted = FALSE;


-- =============================================================================
-- PARTE 4/8 — DONANTES
-- =============================================================================


-- -----------------------------------------------------------------------------
-- TABLA: CONDICIONES MÉDICAS (catálogo)
-- -----------------------------------------------------------------------------

CREATE TABLE MedicalConditions
(
    Id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    Code            VARCHAR(50)  NOT NULL UNIQUE,
    Name            VARCHAR(200) NOT NULL,
    IsExclusionary  BOOLEAN      NOT NULL DEFAULT FALSE,

    CreatedAt       TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    UpdatedAt       TIMESTAMPTZ,

    CreatedBy       UUID,
    UpdatedBy       UUID,

    IsDeleted       BOOLEAN      NOT NULL DEFAULT FALSE
);

COMMENT ON TABLE MedicalConditions IS
'Catálogo de condiciones médicas; IsExclusionary indica si impide donar';


-- -----------------------------------------------------------------------------
-- TABLA: DONANTES
-- -----------------------------------------------------------------------------

CREATE TABLE Donors
(
    Id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    DonorCode       VARCHAR(50)  NOT NULL UNIQUE,

    BloodTypeId     UUID         NOT NULL,
    GenderId        UUID         NOT NULL,
    AddressId       UUID,

    FirstName       VARCHAR(100) NOT NULL,
    LastName        VARCHAR(100) NOT NULL,

    Identification  VARCHAR(50)  NOT NULL UNIQUE,
    DateOfBirth     DATE         NOT NULL,

    PhoneNumber     VARCHAR(30),
    Email           VARCHAR(150),

    IsEligible      BOOLEAN      NOT NULL DEFAULT TRUE,

    LastDonationDate DATE,

    CreatedAt       TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    UpdatedAt       TIMESTAMPTZ,

    CreatedBy       UUID,
    UpdatedBy       UUID,

    IsDeleted       BOOLEAN      NOT NULL DEFAULT FALSE,

    CONSTRAINT FK_Donors_BloodType
        FOREIGN KEY (BloodTypeId)
        REFERENCES BloodTypes(Id),

    CONSTRAINT FK_Donors_Gender
        FOREIGN KEY (GenderId)
        REFERENCES Genders(Id),

    CONSTRAINT FK_Donors_Address
        FOREIGN KEY (AddressId)
        REFERENCES Addresses(Id)
);

COMMENT ON TABLE Donors IS
'Registro de donantes de sangre con información personal y elegibilidad';


-- -----------------------------------------------------------------------------
-- TABLA: CONDICIONES MÉDICAS DEL DONANTE
-- -----------------------------------------------------------------------------

CREATE TABLE DonorMedicalConditions
(
    Id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    DonorId             UUID         NOT NULL,
    MedicalConditionId  UUID         NOT NULL,

    DiagnosisDate       DATE,
    Notes               TEXT,

    CreatedAt           TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    UpdatedAt           TIMESTAMPTZ,

    CreatedBy           UUID,
    UpdatedBy           UUID,

    IsDeleted           BOOLEAN      NOT NULL DEFAULT FALSE,

    CONSTRAINT FK_DonorMedical_Donor
        FOREIGN KEY (DonorId)
        REFERENCES Donors(Id),

    CONSTRAINT FK_DonorMedical_Condition
        FOREIGN KEY (MedicalConditionId)
        REFERENCES MedicalConditions(Id),

    CONSTRAINT UQ_DonorMedical
        UNIQUE (DonorId, MedicalConditionId)
);

COMMENT ON TABLE DonorMedicalConditions IS
'Historial de condiciones médicas por donante';


-- -----------------------------------------------------------------------------
-- ÍNDICES — DONANTES
-- -----------------------------------------------------------------------------

CREATE INDEX IX_Donors_Code
ON Donors(DonorCode)
WHERE IsDeleted = FALSE;

CREATE INDEX IX_Donors_Identification
ON Donors(Identification)
WHERE IsDeleted = FALSE;

CREATE INDEX IX_Donors_BloodType
ON Donors(BloodTypeId)
WHERE IsDeleted = FALSE;

CREATE INDEX IX_DonorMedical_Donor
ON DonorMedicalConditions(DonorId)
WHERE IsDeleted = FALSE;


-- =============================================================================
-- PARTE 5/8 — DONACIONES
-- =============================================================================


-- -----------------------------------------------------------------------------
-- TABLA: CITAS DE DONACIÓN
-- -----------------------------------------------------------------------------

CREATE TABLE DonationAppointments
(
    Id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    DonorId         UUID         NOT NULL,
    BloodBankId     UUID         NOT NULL,

    -- FK a catálogo en lugar de texto libre
    StatusId        UUID         NOT NULL,

    AppointmentDate TIMESTAMPTZ  NOT NULL,

    Notes           TEXT,

    CreatedAt       TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    UpdatedAt       TIMESTAMPTZ,

    CreatedBy       UUID,
    UpdatedBy       UUID,

    IsDeleted       BOOLEAN      NOT NULL DEFAULT FALSE,

    CONSTRAINT FK_Appointment_Donor
        FOREIGN KEY (DonorId)
        REFERENCES Donors(Id),

    CONSTRAINT FK_Appointment_BloodBank
        FOREIGN KEY (BloodBankId)
        REFERENCES BloodBanks(Id),

    CONSTRAINT FK_Appointment_Status
        FOREIGN KEY (StatusId)
        REFERENCES DonationStatuses(Id)
);

COMMENT ON TABLE DonationAppointments IS
'Agenda de citas para donaciones de sangre';


-- -----------------------------------------------------------------------------
-- TABLA: EVALUACIÓN MÉDICA PREVIA
-- -----------------------------------------------------------------------------

CREATE TABLE DonationEvaluations
(
    Id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    DonorId         UUID            NOT NULL,
    DoctorId        UUID            NOT NULL,

    EvaluationDate  TIMESTAMPTZ     NOT NULL DEFAULT NOW(),

    Temperature     DECIMAL(4,2),
    BloodPressure   VARCHAR(20),
    HeartRate       INTEGER,
    Hemoglobin      DECIMAL(4,2),
    WeightKg        DECIMAL(5,2),

    IsApproved      BOOLEAN         NOT NULL,
    RejectionReason TEXT,
    Notes           TEXT,

    CreatedAt       TIMESTAMPTZ     NOT NULL DEFAULT NOW(),
    UpdatedAt       TIMESTAMPTZ,

    CreatedBy       UUID,
    UpdatedBy       UUID,

    IsDeleted       BOOLEAN         NOT NULL DEFAULT FALSE,

    CONSTRAINT FK_Evaluation_Donor
        FOREIGN KEY (DonorId)
        REFERENCES Donors(Id),

    CONSTRAINT FK_Evaluation_Doctor
        FOREIGN KEY (DoctorId)
        REFERENCES Staff(Id)
);

COMMENT ON TABLE DonationEvaluations IS
'Chequeo médico obligatorio antes de extraer sangre';


-- -----------------------------------------------------------------------------
-- TABLA: DONACIONES
-- -----------------------------------------------------------------------------

CREATE TABLE Donations
(
    Id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    DonationCode        VARCHAR(50)  NOT NULL UNIQUE,

    DonorId             UUID         NOT NULL,
    BloodBankId         UUID         NOT NULL,
    EvaluationId        UUID,
    StatusId            UUID         NOT NULL,
    PerformedByStaffId  UUID,

    DonationDate        TIMESTAMPTZ  NOT NULL DEFAULT NOW(),

    VolumeML            INTEGER      NOT NULL,
    CollectionBagCode   VARCHAR(100),

    Notes               TEXT,

    CreatedAt           TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    UpdatedAt           TIMESTAMPTZ,

    CreatedBy           UUID,
    UpdatedBy           UUID,

    IsDeleted           BOOLEAN      NOT NULL DEFAULT FALSE,

    CONSTRAINT FK_Donation_Donor
        FOREIGN KEY (DonorId)
        REFERENCES Donors(Id),

    CONSTRAINT FK_Donation_BloodBank
        FOREIGN KEY (BloodBankId)
        REFERENCES BloodBanks(Id),

    CONSTRAINT FK_Donation_Status
        FOREIGN KEY (StatusId)
        REFERENCES DonationStatuses(Id),

    CONSTRAINT FK_Donation_Evaluation
        FOREIGN KEY (EvaluationId)
        REFERENCES DonationEvaluations(Id),

    CONSTRAINT FK_Donation_Staff
        FOREIGN KEY (PerformedByStaffId)
        REFERENCES Staff(Id),

    CONSTRAINT CK_Donation_Volume
        CHECK (VolumeML BETWEEN 250 AND 550)
);

COMMENT ON TABLE Donations IS
'Registro de cada extracción de sangre realizada';


-- -----------------------------------------------------------------------------
-- ÍNDICES — DONACIONES
-- -----------------------------------------------------------------------------

CREATE INDEX IX_Donation_Code
ON Donations(DonationCode)
WHERE IsDeleted = FALSE;

CREATE INDEX IX_Donation_Donor
ON Donations(DonorId)
WHERE IsDeleted = FALSE;

CREATE INDEX IX_Donation_Date
ON Donations(DonationDate)
WHERE IsDeleted = FALSE;

CREATE INDEX IX_Appointment_Donor
ON DonationAppointments(DonorId)
WHERE IsDeleted = FALSE;

CREATE INDEX IX_Appointment_Date
ON DonationAppointments(AppointmentDate)
WHERE IsDeleted = FALSE;

CREATE INDEX IX_Evaluation_Donor
ON DonationEvaluations(DonorId)
WHERE IsDeleted = FALSE;


-- =============================================================================
-- PARTE 6/8 — LABORATORIO Y TRAZABILIDAD
-- =============================================================================


-- -----------------------------------------------------------------------------
-- TABLA: UNIDADES DE SANGRE
-- -----------------------------------------------------------------------------

CREATE TABLE BloodUnits
(
    Id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    UnitCode        VARCHAR(50)  NOT NULL UNIQUE,
    QrCode          VARCHAR(200),

    DonationId      UUID         NOT NULL,
    BloodTypeId     UUID         NOT NULL,
    ComponentId     UUID         NOT NULL,
    StatusId        UUID         NOT NULL,
    StorageLocationId UUID,

    VolumeML        INTEGER      NOT NULL,

    CollectionDate  TIMESTAMPTZ  NOT NULL,
    ExpirationDate  TIMESTAMPTZ  NOT NULL,

    IsReleased      BOOLEAN      NOT NULL DEFAULT FALSE,

    Notes           TEXT,

    CreatedAt       TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    UpdatedAt       TIMESTAMPTZ,

    CreatedBy       UUID,
    UpdatedBy       UUID,

    IsDeleted       BOOLEAN      NOT NULL DEFAULT FALSE,

    CONSTRAINT FK_BloodUnit_Donation
        FOREIGN KEY (DonationId)
        REFERENCES Donations(Id),

    CONSTRAINT FK_BloodUnit_BloodType
        FOREIGN KEY (BloodTypeId)
        REFERENCES BloodTypes(Id),

    CONSTRAINT FK_BloodUnit_Component
        FOREIGN KEY (ComponentId)
        REFERENCES BloodComponents(Id),

    CONSTRAINT FK_BloodUnit_Status
        FOREIGN KEY (StatusId)
        REFERENCES BloodUnitStatuses(Id)
);

COMMENT ON TABLE BloodUnits IS
'Unidades individuales de sangre con trazabilidad completa por QR y código único';


-- -----------------------------------------------------------------------------
-- TABLA: SCREENING DE LABORATORIO
-- -----------------------------------------------------------------------------

CREATE TABLE BloodScreenings
(
    Id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    BloodUnitId     UUID         NOT NULL,
    TechnicianId    UUID         NOT NULL,

    ScreeningDate   TIMESTAMPTZ  NOT NULL DEFAULT NOW(),

    -- Marcadores serológicos
    HivResult       VARCHAR(20)  NOT NULL DEFAULT 'PENDING',
    HbsAgResult     VARCHAR(20)  NOT NULL DEFAULT 'PENDING',  -- Hepatitis B
    HcvResult       VARCHAR(20)  NOT NULL DEFAULT 'PENDING',  -- Hepatitis C
    VdrlResult      VARCHAR(20)  NOT NULL DEFAULT 'PENDING',  -- Sífilis
    ChagasResult    VARCHAR(20)  NOT NULL DEFAULT 'PENDING',

    IsApproved      BOOLEAN,
    RejectionReason TEXT,
    Notes           TEXT,

    CreatedAt       TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    UpdatedAt       TIMESTAMPTZ,

    CreatedBy       UUID,
    UpdatedBy       UUID,

    IsDeleted       BOOLEAN      NOT NULL DEFAULT FALSE,

    CONSTRAINT FK_Screening_BloodUnit
        FOREIGN KEY (BloodUnitId)
        REFERENCES BloodUnits(Id),

    CONSTRAINT FK_Screening_Technician
        FOREIGN KEY (TechnicianId)
        REFERENCES Staff(Id)
);

COMMENT ON TABLE BloodScreenings IS
'Resultados de serología y tamizaje infeccioso por unidad de sangre';


-- -----------------------------------------------------------------------------
-- TABLA: MOVIMIENTOS DE INVENTARIO
-- -----------------------------------------------------------------------------

CREATE TABLE InventoryMovements
(
    Id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    BloodUnitId     UUID         NOT NULL,
    MovementTypeId  UUID         NOT NULL,
    PerformedById   UUID         NOT NULL,

    FromLocationId  UUID,
    ToLocationId    UUID,

    MovementDate    TIMESTAMPTZ  NOT NULL DEFAULT NOW(),

    Notes           TEXT,

    CreatedAt       TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    UpdatedAt       TIMESTAMPTZ,

    CreatedBy       UUID,
    UpdatedBy       UUID,

    IsDeleted       BOOLEAN      NOT NULL DEFAULT FALSE,

    CONSTRAINT FK_Movement_BloodUnit
        FOREIGN KEY (BloodUnitId)
        REFERENCES BloodUnits(Id),

    CONSTRAINT FK_Movement_Type
        FOREIGN KEY (MovementTypeId)
        REFERENCES InventoryMovementTypes(Id),

    CONSTRAINT FK_Movement_Staff
        FOREIGN KEY (PerformedById)
        REFERENCES Staff(Id)
);

COMMENT ON TABLE InventoryMovements IS
'Trazabilidad completa de movimientos de cada unidad de sangre en el inventario';


-- -----------------------------------------------------------------------------
-- ÍNDICES — LABORATORIO Y TRAZABILIDAD
-- -----------------------------------------------------------------------------

CREATE INDEX IX_BloodUnit_Code
ON BloodUnits(UnitCode)
WHERE IsDeleted = FALSE;

CREATE INDEX IX_BloodUnit_Status
ON BloodUnits(StatusId)
WHERE IsDeleted = FALSE;

CREATE INDEX IX_BloodUnit_BloodType
ON BloodUnits(BloodTypeId)
WHERE IsDeleted = FALSE;

CREATE INDEX IX_BloodUnit_Expiration
ON BloodUnits(ExpirationDate)
WHERE IsDeleted = FALSE;

CREATE INDEX IX_Screening_BloodUnit
ON BloodScreenings(BloodUnitId)
WHERE IsDeleted = FALSE;

CREATE INDEX IX_Movement_BloodUnit
ON InventoryMovements(BloodUnitId)
WHERE IsDeleted = FALSE;

CREATE INDEX IX_Movement_Date
ON InventoryMovements(MovementDate)
WHERE IsDeleted = FALSE;


-- =============================================================================
-- PARTE 7/8 — NOTIFICACIONES
-- =============================================================================


-- -----------------------------------------------------------------------------
-- TABLA: NOTIFICACIONES
-- -----------------------------------------------------------------------------

CREATE TABLE Notifications
(
    Id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    TypeId          UUID         NOT NULL,
    UserId          UUID,

    Subject         VARCHAR(250) NOT NULL,
    Body            TEXT         NOT NULL,

    IsRead          BOOLEAN      NOT NULL DEFAULT FALSE,
    ReadAt          TIMESTAMPTZ,

    SentAt          TIMESTAMPTZ,

    CreatedAt       TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    UpdatedAt       TIMESTAMPTZ,

    CreatedBy       UUID,
    UpdatedBy       UUID,

    IsDeleted       BOOLEAN      NOT NULL DEFAULT FALSE,

    CONSTRAINT FK_Notification_Type
        FOREIGN KEY (TypeId)
        REFERENCES NotificationTypes(Id),

    CONSTRAINT FK_Notification_User
        FOREIGN KEY (UserId)
        REFERENCES Users(Id)
);

COMMENT ON TABLE Notifications IS
'Mensajes del sistema hacia usuarios: alertas de vencimiento, stock crítico, resultados';


-- -----------------------------------------------------------------------------
-- ÍNDICES — NOTIFICACIONES
-- -----------------------------------------------------------------------------

CREATE INDEX IX_Notification_User
ON Notifications(UserId)
WHERE IsDeleted = FALSE;

CREATE INDEX IX_Notification_Unread
ON Notifications(UserId, IsRead)
WHERE IsDeleted = FALSE AND IsRead = FALSE;


-- =============================================================================
-- PARTE 8/8 — DATOS SEMILLA
-- =============================================================================


-- Roles
INSERT INTO Roles (Name, Description)
VALUES
    ('Administrator', 'Acceso completo al sistema'),
    ('Doctor',        'Validación médica y aprobaciones'),
    ('Nurse',         'Atención de donantes'),
    ('Technician',    'Laboratorio y análisis'),
    ('Receptionist',  'Citas y atención al público');


-- Géneros
INSERT INTO Genders (Name)
VALUES
    ('Masculino'),
    ('Femenino'),
    ('No binario'),
    ('Prefiero no indicar');


-- Tipos sanguíneos
INSERT INTO BloodTypes (Code, Description)
VALUES
    ('O+',  'O Positivo'),
    ('O-',  'O Negativo'),
    ('A+',  'A Positivo'),
    ('A-',  'A Negativo'),
    ('B+',  'B Positivo'),
    ('B-',  'B Negativo'),
    ('AB+', 'AB Positivo'),
    ('AB-', 'AB Negativo');


-- Estados de donación
INSERT INTO DonationStatuses (Code, Description)
VALUES
    ('SCHEDULED',   'Cita programada'),
    ('IN_PROGRESS', 'En proceso'),
    ('COMPLETED',   'Completada'),
    ('CANCELLED',   'Cancelada'),
    ('REJECTED',    'Rechazada por evaluación médica');


-- Estados de unidades de sangre
INSERT INTO BloodUnitStatuses (Code, Description)
VALUES
    ('QUARANTINE',  'En cuarentena — pendiente de screening'),
    ('AVAILABLE',   'Disponible para uso'),
    ('RESERVED',    'Reservada para paciente específico'),
    ('TRANSFUSED',  'Utilizada en transfusión'),
    ('DISCARDED',   'Descartada por resultado positivo o vencimiento'),
    ('EXPIRED',     'Vencida por fecha de caducidad');


-- Tipos de movimiento
INSERT INTO InventoryMovementTypes (Code, Description)
VALUES
    ('ENTRY',      'Ingreso de unidad al inventario'),
    ('TRANSFER',   'Transferencia entre ubicaciones'),
    ('RESERVE',    'Reserva para paciente'),
    ('RELEASE',    'Liberación de reserva'),
    ('TRANSFUSE',  'Entrega para transfusión'),
    ('DISCARD',    'Descarte por screening positivo o vencimiento');


-- Tipos de notificación
INSERT INTO NotificationTypes (Code, Description)
VALUES
    ('LOW_STOCK',       'Stock crítico de un grupo sanguíneo'),
    ('UNIT_EXPIRING',   'Unidad próxima a vencer'),
    ('SCREENING_READY', 'Resultado de screening disponible'),
    ('APPOINTMENT',     'Recordatorio de cita de donación');


-- Componentes sanguíneos
INSERT INTO BloodComponents (Code, Name, ShelfLifeDays)
VALUES
    ('WB',  'Sangre completa',  35),
    ('RBC', 'Glóbulos rojos',   42),
    ('PLT', 'Plaquetas',         5),
    ('PLS', 'Plasma',          365);


-- Condiciones médicas base (exclusionarias)
INSERT INTO MedicalConditions (Code, Name, IsExclusionary)
VALUES
    ('HIV',      'VIH / SIDA',                                TRUE),
    ('HEP_B',    'Hepatitis B',                               TRUE),
    ('HEP_C',    'Hepatitis C',                               TRUE),
    ('CHAGAS',   'Enfermedad de Chagas',                      TRUE),
    ('DIABETES', 'Diabetes mellitus',                         FALSE),
    ('HYPERT',   'Hipertensión arterial',                     FALSE),
    ('ANEMIA',   'Anemia',                                    TRUE),
    ('PREGNANCY','Embarazo',                                  TRUE);


-- Categorías de personal
INSERT INTO StaffCategories (Name, Description)
VALUES
    ('Médico',                 'Médico general o especialista hematólogo'),
    ('Enfermero',              'Profesional de enfermería'),
    ('Técnico de laboratorio', 'Técnico de pruebas serológicas y hematológicas'),
    ('Recepcionista',          'Atención al público y gestión de citas');


-- =============================================================================
-- VISTAS OPERACIONALES
-- =============================================================================


-- Inventario disponible por tipo y componente
CREATE VIEW AvailableBloodInventory AS
SELECT
    bu.UnitCode,
    bt.Code         AS BloodType,
    bc.Name         AS Component,
    bu.VolumeML,
    bu.ExpirationDate,
    bu.CollectionDate
FROM BloodUnits bu
JOIN BloodTypes         bt ON bu.BloodTypeId = bt.Id
JOIN BloodComponents    bc ON bu.ComponentId  = bc.Id
JOIN BloodUnitStatuses  bs ON bu.StatusId     = bs.Id
WHERE
    bs.Code          = 'AVAILABLE'
    AND bu.IsReleased = TRUE
    AND bu.ExpirationDate > NOW()
    AND bu.IsDeleted  = FALSE;

COMMENT ON VIEW AvailableBloodInventory IS
'Inventario de unidades disponibles, liberadas y vigentes';


-- Historial médico del donante
CREATE VIEW DonorMedicalHistory AS
SELECT
    d.DonorCode,
    d.FirstName,
    d.LastName,
    mc.Code         AS ConditionCode,
    mc.Name         AS Condition,
    mc.IsExclusionary,
    dm.DiagnosisDate,
    dm.Notes
FROM Donors                 d
JOIN DonorMedicalConditions dm ON d.Id  = dm.DonorId
JOIN MedicalConditions      mc ON mc.Id = dm.MedicalConditionId
WHERE
    d.IsDeleted  = FALSE
    AND dm.IsDeleted = FALSE;

COMMENT ON VIEW DonorMedicalHistory IS
'Historial médico activo por donante con indicador de exclusión';


-- Resumen de stock crítico por tipo de sangre
CREATE VIEW BloodStockSummary AS
SELECT
    bt.Code         AS BloodType,
    bc.Name         AS Component,
    COUNT(bu.Id)    AS UnitsAvailable,
    SUM(bu.VolumeML) AS TotalVolumeML
FROM BloodUnits         bu
JOIN BloodTypes         bt ON bu.BloodTypeId = bt.Id
JOIN BloodComponents    bc ON bu.ComponentId  = bc.Id
JOIN BloodUnitStatuses  bs ON bu.StatusId     = bs.Id
WHERE
    bs.Code          = 'AVAILABLE'
    AND bu.IsReleased = TRUE
    AND bu.ExpirationDate > NOW()
    AND bu.IsDeleted  = FALSE
GROUP BY bt.Code, bc.Name
ORDER BY bt.Code, bc.Name;

COMMENT ON VIEW BloodStockSummary IS
'Resumen de unidades disponibles agrupado por tipo y componente para alertas de stock';


-- =============================================================================
-- RECOMENDACIONES ENTITY FRAMEWORK CORE
-- =============================================================================
/*
1. Agregados DDD:
   - UserAggregate      → Users, Roles, RefreshTokens
   - DonorAggregate     → Donors, DonorMedicalConditions
   - DonationAggregate  → DonationAppointments, DonationEvaluations, Donations
   - BloodUnitAggregate → BloodUnits, BloodScreenings, InventoryMovements

2. Configuración EF Core:
   - Fluent API para todas las entidades (sin Data Annotations en el dominio)
   - Value Objects: BloodPressure, Address, PersonName
   - Global Query Filters: modelBuilder.Entity<T>().HasQueryFilter(e => !e.IsDeleted)
   - Interceptores: SaveChangesInterceptor para auditoría automática

3. CQRS con MediatR:
   - Commands: CreateDonorCommand, RegisterDonationCommand, ReleaseBloodUnitCommand
   - Queries:  GetAvailableInventoryQuery, GetDonorHistoryQuery
   - Behaviors: ValidationBehavior, LoggingBehavior, TransactionBehavior

4. Seguridad:
   - JWT + Refresh Token rotativo
   - Authorization Policies por rol
   - Row-level security vía UserId en queries

5. Migrations:
   - Una migration por parte del script
   - Seeds en IEntityTypeConfiguration<T> o DataSeeder dedicado
*/


-- =============================================================================
-- FIN DEL SCRIPT
-- BloodBankManagementSystem_v2 — Script completo (8/8 partes)
-- =============================================================================
