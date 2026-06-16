using Domain.Entities.BloodUnits;
using Domain.Entities.Donors;
using Domain.Entities.Inventory;
using Domain.Entities.Notifications;
using Domain.Entities.Tenancy;
using Domain.Entities.Donations;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seeds;

public static class CatalogSeed
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.BloodTypes.AnyAsync()) return;

        var bloodTypes = new List<BloodType>
        {
            new() { Code = "O+", Description = "O Positivo" },
            new() { Code = "O-", Description = "O Negativo" },
            new() { Code = "A+", Description = "A Positivo" },
            new() { Code = "A-", Description = "A Negativo" },
            new() { Code = "B+", Description = "B Positivo" },
            new() { Code = "B-", Description = "B Negativo" },
            new() { Code = "AB+", Description = "AB Positivo" },
            new() { Code = "AB-", Description = "AB Negativo" },
        };

        var genders = new List<Gender>
        {
            new() { Name = "Masculino" },
            new() { Name = "Femenino" },
            new() { Name = "No binario" },
            new() { Name = "Prefiero no indicar" },
        };

        var donationStatuses = new List<DonationStatus>
        {
            new() { Code = "SCHEDULED", Description = "Cita programada" },
            new() { Code = "IN_PROGRESS", Description = "En proceso" },
            new() { Code = "COMPLETED", Description = "Completada" },
            new() { Code = "CANCELLED", Description = "Cancelada" },
            new() { Code = "REJECTED", Description = "Rechazada por evaluación médica" },
        };

        var bloodUnitStatuses = new List<BloodUnitStatus>
        {
            new() { Code = "QUARANTINE", Description = "En cuarentena — pendiente de screening" },
            new() { Code = "AVAILABLE", Description = "Disponible para uso" },
            new() { Code = "RESERVED", Description = "Reservada para paciente específico" },
            new() { Code = "TRANSFUSED", Description = "Utilizada en transfusión" },
            new() { Code = "DISCARDED", Description = "Descartada por resultado positivo o vencimiento" },
            new() { Code = "EXPIRED", Description = "Vencida por fecha de caducidad" },
        };

        var movementTypes = new List<InventoryMovementType>
        {
            new() { Code = "ENTRY", Description = "Ingreso de unidad al inventario" },
            new() { Code = "TRANSFER", Description = "Transferencia entre ubicaciones" },
            new() { Code = "RESERVE", Description = "Reserva para paciente" },
            new() { Code = "RELEASE", Description = "Liberación de reserva" },
            new() { Code = "TRANSFUSE", Description = "Entrega para transfusión" },
            new() { Code = "DISCARD", Description = "Descarte por screening positivo o vencimiento" },
        };

        var notificationTypes = new List<NotificationType>
        {
            new() { Code = "LOW_STOCK", Description = "Stock crítico de un grupo sanguíneo" },
            new() { Code = "UNIT_EXPIRING", Description = "Unidad próxima a vencer" },
            new() { Code = "SCREENING_READY", Description = "Resultado de screening disponible" },
            new() { Code = "APPOINTMENT", Description = "Recordatorio de cita de donación" },
        };

        var bloodComponents = new List<BloodComponent>
        {
            new() { Code = "WB", Name = "Sangre completa", ShelfLifeDays = 35 },
            new() { Code = "RBC", Name = "Glóbulos rojos", ShelfLifeDays = 42 },
            new() { Code = "PLT", Name = "Plaquetas", ShelfLifeDays = 5 },
            new() { Code = "PLS", Name = "Plasma", ShelfLifeDays = 365 },
        };

        var medicalConditions = new List<MedicalCondition>
        {
            new() { Code = "HIV", Name = "VIH / SIDA", IsExclusionary = true },
            new() { Code = "HEP_B", Name = "Hepatitis B", IsExclusionary = true },
            new() { Code = "HEP_C", Name = "Hepatitis C", IsExclusionary = true },
            new() { Code = "CHAGAS", Name = "Enfermedad de Chagas", IsExclusionary = true },
            new() { Code = "DIABETES", Name = "Diabetes mellitus", IsExclusionary = false },
            new() { Code = "HYPERT", Name = "Hipertensión arterial", IsExclusionary = false },
            new() { Code = "ANEMIA", Name = "Anemia", IsExclusionary = true },
            new() { Code = "PREGNANCY", Name = "Embarazo", IsExclusionary = true },
        };

        var staffCategories = new List<StaffCategory>
        {
            new() { Name = "Médico", Description = "Médico general o especialista hematólogo" },
            new() { Name = "Enfermero", Description = "Profesional de enfermería" },
            new() { Name = "Técnico de laboratorio", Description = "Técnico de pruebas serológicas y hematológicas" },
            new() { Name = "Recepcionista", Description = "Atención al público y gestión de citas" },
        };

        context.BloodTypes.AddRange(bloodTypes);
        context.Genders.AddRange(genders);
        context.DonationStatuses.AddRange(donationStatuses);
        context.BloodUnitStatuses.AddRange(bloodUnitStatuses);
        context.InventoryMovementTypes.AddRange(movementTypes);
        context.NotificationTypes.AddRange(notificationTypes);
        context.BloodComponents.AddRange(bloodComponents);
        context.MedicalConditions.AddRange(medicalConditions);
        context.StaffCategories.AddRange(staffCategories);

        await context.SaveChangesAsync();
    }
}
