using System.ComponentModel;

namespace GemTracker.Shared.Domain.Enums
{
    public enum TokenActionType
    {
        [Description("Added")]
        ADDED = 0,

        [Description("Deleted")]
        DELETED = 1,

        [Description("Added to ACTIVE")]
        KYBER_ADDED_TO_ACTIVE = 2,

        [Description("Deleted from ACTIVE")]
        KYBER_DELETED_FROM_ACTIVE = 3,

        [Description("Added to NOT ACTIVE")]
        KYBER_ADDED_TO_NOT_ACTIVE = 4,

        [Description("Deleted from NOT ACTIVE")]
        KYBER_DELETED_FROM_NOT_ACTIVE = 5
    }
}