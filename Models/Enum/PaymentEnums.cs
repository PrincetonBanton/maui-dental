namespace DentalApp.Models.Enum
{
    public enum PaymentType : short
    {
        Cash = 0,
        GCash = 1,
        BankTransfer = 2,
    }

    public enum PaymentStatus
    {
        Pending = 0,
        Confirmed = 1
    }
}
