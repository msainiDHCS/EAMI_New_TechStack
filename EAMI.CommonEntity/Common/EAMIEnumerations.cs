using System.Runtime.Serialization;

namespace EAMI.CommonEntity
{
    public static class EnumUtil
    {
        public static T ToEnum<T>(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException("value is null or empty string");
                //return ToEnum<T>(1);
            }
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static T ToEnum<T>(this int value)
        {
            var name = Enum.GetName(typeof(T), value);
            return name.ToEnum<T>();
        }
    }

    public enum EAMIADUserStatus
    {
        DISABLED,
        LOCKED,
        PASSWORD_NOT_REQUIRED,
        PASSWORD_CAN_NOT_CHANGE,
        ENABLED,
        PASSWORD_WILL_NOT_EXPIRE,
        PASSWORD_EXPIRED,
        PASSWORD_RESET_REQUIRED
    }


    public enum KvpOwnerEntity
    {
        Unknown = 0,
        PaymentRec = 1,
        Funding = 2
    }

    //public enum FundingSource_Title:int
    //{
    //    Title_19 =19,
    //    Title_21 = 21

    //}

    public enum enRefTables
    {
        [EnumMember]
        TB_TRANSACTION_TYPE,
        [EnumMember]
        TB_SYSTEM_OF_RECORD,
        [EnumMember]
        TB_SOR_KVP_KEY,
        [EnumMember]
        TB_RESPONSE_STATUS_TYPE,
        [EnumMember]
        TB_PAYMENT_STATUS_TYPE,
        [EnumMember]
        TB_PAYMENT_EXCHANGE_ENTITY,
        [EnumMember]
        TB_PAYDATE_CALENDAR,
        [EnumMember]
        TB_DRAWDATE_CALENDAR,
        [EnumMember]
        TB_CLAIM_SCHEDULE_STATUS_TYPE,
        [EnumMember]
        TB_PAYMENT_STATUS_TYPE_EXTERNAL,
        [EnumMember]
        TB_ECS_STATUS_TYPE,
        [EnumMember]
        TB_EXCLUSIVE_PAYMENT_TYPE,
        [EnumMember]
        TB_USER,
        [EnumMember]
        TB_PAYMENT_METHOD_TYPE,
        [EnumMember]
        TB_FUNDING_SOURCE,
        [EnumMember]
        TB_SCO_PROPERTY,
        [EnumMember]
        TB_SCO_FILE_PROPERTY,
        [EnumMember]
        TB_System
    }

    public enum EAMIPermission
    {
        [EnumMember]
        ASSIGN_INVOICE,

        [EnumMember]
        SET_PAYDATE,

        [EnumMember]
        SEND_TO_SCO,

        [EnumMember]
        SEND_TO_CALSTARS,

        [EnumMember]
        CREATE_CLAIM_SCHEDULE,

        [EnumMember]
        APPROVE_CLAIM_SCHEDULE
    }

    public enum EAMIDateType
    {
        [EnumMember]
        PayDate,
        [EnumMember]
        DrawDate
        //    ,
        //[EnumMember]
        //PayAndDrawDate
    }

    public enum EAMICalendarAction
    {
        [EnumMember]
        Add,
        [EnumMember]
        Delete,
        [EnumMember]
        Update
    }

    public enum EAMIPaymentRecordInsertionModeToClaimSchedule
    {
        [EnumMember]
        New,

        [EnumMember]
        Existing
    }

    public enum SCOPRopertiesEnvironments
    {
        TEST = 1,
        STAGE = 2,
        PROD = 3,
    }
    public enum SCOPRopertiesPaymentTypes
    {
        WARRANT = 1,
        EFT = 2
    }
}
