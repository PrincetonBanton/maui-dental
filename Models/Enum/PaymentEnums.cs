using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DentalApp.Models.Enum
{
    public enum PaymentType
    {
        Full = 0,
        Partial = 1
    }

    public enum PaymentStatus
    {
        Pending = 0,
        Confirmed = 1
    }
}
