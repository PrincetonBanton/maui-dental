using DentalApp.Models;

namespace DentalApp.Services.Validations
{
    public static class AppointmentValidationService
    {
        public static (bool IsValid, string ErrorMessage) ValidateAppointment(Appointment appointment)
        {
            if (string.IsNullOrWhiteSpace(appointment.Title))
                return (false, "Appointment title is required.");

            if (string.IsNullOrWhiteSpace(appointment.Description))
                return (false, "Appointment description is required.");

            if (appointment.PatientId <= 0)
                return (false, "Please select a valid patient.");

            if (appointment.DentistId <= 0)
                return (false, "Please select a valid dentist.");

            if (appointment.StartDate == default)
                return (false, "Please select a valid start date and time.");

            if (appointment.EndDate == default)
                return (false, "Please select a valid end date and time.");

            if (appointment.EndDate <= appointment.StartDate)
                return (false, "End time must be later than start time.");

            return (true, string.Empty);
        }

        public static bool HasTimeConflict(Appointment newAppointment, IEnumerable<Appointment> existingAppointments)
        {
            return existingAppointments.Any(existing =>
                existing.DentistId == newAppointment.DentistId &&
                existing.Id != newAppointment.Id &&                            
                existing.StartDate.Date == newAppointment.StartDate.Date &&    
                (
                    newAppointment.StartDate < existing.EndDate &&
                    newAppointment.EndDate > existing.StartDate
                )
            );
        }
    }
}
