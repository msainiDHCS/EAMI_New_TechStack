using Microsoft.EntityFrameworkCore;

namespace EAMI.Respository.EAMI_RX_Repository
{
    public partial class EAMI_RX : DbContext
    {
        public EAMI_RX(DbContextOptions<EAMI_RX> options) : base(options)
        {
        }

        public virtual DbSet<TB_AUDIT_FILE> TB_AUDIT_FILE { get; set; }
        public virtual DbSet<TB_CLAIM_SCHEDULE> TB_CLAIM_SCHEDULE { get; set; }
        public virtual DbSet<TB_CLAIM_SCHEDULE_DN_STATUS> TB_CLAIM_SCHEDULE_DN_STATUS { get; set; }
        public virtual DbSet<TB_CLAIM_SCHEDULE_ECS> TB_CLAIM_SCHEDULE_ECS { get; set; }
        public virtual DbSet<TB_CLAIM_SCHEDULE_REMITTANCE_ADVICE_NOTE> TB_CLAIM_SCHEDULE_REMITTANCE_ADVICE_NOTE { get; set; }
        public virtual DbSet<TB_CLAIM_SCHEDULE_STATUS> TB_CLAIM_SCHEDULE_STATUS { get; set; }
        public virtual DbSet<TB_CLAIM_SCHEDULE_STATUS_TYPE> TB_CLAIM_SCHEDULE_STATUS_TYPE { get; set; }
        public virtual DbSet<TB_CLAIM_SCHEDULE_USER_ASSIGNMENT> TB_CLAIM_SCHEDULE_USER_ASSIGNMENT { get; set; }
        public virtual DbSet<TB_DRAWDATE_CALENDAR> TB_DRAWDATE_CALENDAR { get; set; }
        public virtual DbSet<TB_ECS> TB_ECS { get; set; }
        public virtual DbSet<TB_ECS_SEQUENCE> TB_ECS_SEQUENCE { get; set; }
        public virtual DbSet<TB_ECS_STATUS_TYPE> TB_ECS_STATUS_TYPE { get; set; }
        public virtual DbSet<TB_EXCLUSIVE_PAYMENT_TYPE> TB_EXCLUSIVE_PAYMENT_TYPE { get; set; }
        public virtual DbSet<TB_FACESHEET> TB_FACESHEET { get; set; }
        public virtual DbSet<TB_FUND> TB_FUND { get; set; }
        public virtual DbSet<TB_FUNDING_DETAIL> TB_FUNDING_DETAIL { get; set; }
        public virtual DbSet<TB_FUNDING_DETAIL_EXT_CAPMAN> TB_FUNDING_DETAIL_EXT_CAPMAN { get; set; }
        public virtual DbSet<TB_FUNDING_DETAIL_KVP> TB_FUNDING_DETAIL_KVP { get; set; }
        public virtual DbSet<TB_FUNDING_SOURCE> TB_FUNDING_SOURCE { get; set; }
        public virtual DbSet<TB_PAYDATE_CALENDAR> TB_PAYDATE_CALENDAR { get; set; }
        public virtual DbSet<TB_PAYMENT_DN_STATUS> TB_PAYMENT_DN_STATUS { get; set; }
        public virtual DbSet<TB_PAYMENT_EXCHANGE_ENTITY> TB_PAYMENT_EXCHANGE_ENTITY { get; set; }
        public virtual DbSet<TB_PAYMENT_EXCHANGE_ENTITY_INFO> TB_PAYMENT_EXCHANGE_ENTITY_INFO { get; set; }
        public virtual DbSet<TB_PAYMENT_KVP> TB_PAYMENT_KVP { get; set; }
        public virtual DbSet<TB_PAYMENT_METHOD_TYPE> TB_PAYMENT_METHOD_TYPE { get; set; }
        public virtual DbSet<TB_PAYMENT_RECORD> TB_PAYMENT_RECORD { get; set; }
        public virtual DbSet<TB_PAYMENT_RECORD_EXT_CAPMAN> TB_PAYMENT_RECORD_EXT_CAPMAN { get; set; }
        public virtual DbSet<TB_PAYMENT_STATUS> TB_PAYMENT_STATUS { get; set; }
        public virtual DbSet<TB_PAYMENT_STATUS_TYPE> TB_PAYMENT_STATUS_TYPE { get; set; }
        public virtual DbSet<TB_PAYMENT_STATUS_TYPE_EXTERNAL> TB_PAYMENT_STATUS_TYPE_EXTERNAL { get; set; }
        public virtual DbSet<TB_PAYMENT_USER_ASSIGNMENT> TB_PAYMENT_USER_ASSIGNMENT { get; set; }
        public virtual DbSet<TB_PEE_ADDRESS> TB_PEE_ADDRESS { get; set; }
        public virtual DbSet<TB_PEE_EFT_INFO> TB_PEE_EFT_INFO { get; set; }
        public virtual DbSet<TB_PEE_SYSTEM> TB_PEE_SYSTEM { get; set; }
        public virtual DbSet<TB_PERMISSION> TB_PERMISSION { get; set; }
        public virtual DbSet<TB_REQUEST> TB_REQUEST { get; set; }
        public virtual DbSet<TB_RESPONSE> TB_RESPONSE { get; set; }
        public virtual DbSet<TB_RESPONSE_STATUS_TYPE> TB_RESPONSE_STATUS_TYPE { get; set; }
        public virtual DbSet<TB_ROLE> TB_ROLE { get; set; }
        public virtual DbSet<TB_ROLE_PERMISSION> TB_ROLE_PERMISSION { get; set; }
        public virtual DbSet<TB_SCO_FILE_PROPERTY> TB_SCO_FILE_PROPERTY { get; set; }
        public virtual DbSet<TB_SCO_PROPERTY> TB_SCO_PROPERTY { get; set; }
        public virtual DbSet<TB_SCO_PROPERTY_ENUM> TB_SCO_PROPERTY_ENUM { get; set; }
        public virtual DbSet<TB_SCO_PROPERTY_TYPE> TB_SCO_PROPERTY_TYPE { get; set; }
        public virtual DbSet<TB_SOR_KVP_KEY> TB_SOR_KVP_KEY { get; set; }
        public virtual DbSet<TB_SYSTEM> TB_SYSTEM { get; set; }
        public virtual DbSet<TB_SYSTEM_OF_RECORD> TB_SYSTEM_OF_RECORD { get; set; }
        public virtual DbSet<TB_SYSTEM_USER> TB_SYSTEM_USER { get; set; }
        public virtual DbSet<TB_TRACE_PAYMENT> TB_TRACE_PAYMENT { get; set; }
        public virtual DbSet<TB_TRACE_TRANSACTION> TB_TRACE_TRANSACTION { get; set; }
        public virtual DbSet<TB_TRANSACTION> TB_TRANSACTION { get; set; }
        public virtual DbSet<TB_TRANSACTION_PAYER_ENTITY> TB_TRANSACTION_PAYER_ENTITY { get; set; }
        public virtual DbSet<TB_TRANSACTION_TYPE> TB_TRANSACTION_TYPE { get; set; }
        public virtual DbSet<TB_USER> TB_USER { get; set; }
        public virtual DbSet<TB_USER_ROLE> TB_USER_ROLE { get; set; }
        public virtual DbSet<TB_USER_TYPE> TB_USER_TYPE { get; set; }
        public virtual DbSet<TB_WARRANT> TB_WARRANT { get; set; }
        public virtual DbSet<TB_DB_UPDATE_SCRIPT> TB_DB_UPDATE_SCRIPT { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TB_AUDIT_FILE>()
                .Property(e => e.Audit_File_Name)
                .IsUnicode(false);

            modelBuilder.Entity<TB_AUDIT_FILE>()
                .Property(e => e.Audit_File_Size)
                .HasPrecision(18, 0);

            modelBuilder.Entity<TB_AUDIT_FILE>()
                .Property(e => e.TaskNumber)
                .IsUnicode(false);

            modelBuilder.Entity<TB_CLAIM_SCHEDULE>()
                .Property(e => e.Claim_Schedule_Number)
                .IsUnicode(false);

            modelBuilder.Entity<TB_CLAIM_SCHEDULE>()
                .Property(e => e.Amount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<TB_CLAIM_SCHEDULE>()
                .Property(e => e.FiscalYear)
                .IsUnicode(false);

            modelBuilder.Entity<TB_CLAIM_SCHEDULE>()
                .Property(e => e.Payment_Type)
                .IsUnicode(false);

            modelBuilder.Entity<TB_CLAIM_SCHEDULE>()
                .Property(e => e.ContractNumber)
                .IsUnicode(false);

            modelBuilder.Entity<TB_CLAIM_SCHEDULE>()
                .Property(e => e.LinkedByPGNumber)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_CLAIM_SCHEDULE>()
            //    .HasOptional(e => e.TB_CLAIM_SCHEDULE_DN_STATUS)
            //    .WithRequired(e => e.TB_CLAIM_SCHEDULE);

            //modelBuilder.Entity<TB_CLAIM_SCHEDULE>()
            //    .HasMany(e => e.TB_CLAIM_SCHEDULE_ECS)
            //    .WithRequired(e => e.TB_CLAIM_SCHEDULE)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_CLAIM_SCHEDULE>()
            //    .HasOptional(e => e.TB_CLAIM_SCHEDULE_REMITTANCE_ADVICE_NOTE)
            //    .WithRequired(e => e.TB_CLAIM_SCHEDULE);

            //modelBuilder.Entity<TB_CLAIM_SCHEDULE>()
            //    .HasMany(e => e.TB_CLAIM_SCHEDULE_STATUS)
            //    .WithRequired(e => e.TB_CLAIM_SCHEDULE)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_CLAIM_SCHEDULE>()
            //    .HasMany(e => e.TB_CLAIM_SCHEDULE_USER_ASSIGNMENT)
            //    .WithRequired(e => e.TB_CLAIM_SCHEDULE)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_CLAIM_SCHEDULE_REMITTANCE_ADVICE_NOTE>()
                .Property(e => e.Note)
                .IsUnicode(false);

            modelBuilder.Entity<TB_CLAIM_SCHEDULE_REMITTANCE_ADVICE_NOTE>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_CLAIM_SCHEDULE_REMITTANCE_ADVICE_NOTE>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_CLAIM_SCHEDULE_STATUS>()
                .Property(e => e.Status_Note)
                .IsUnicode(false);

            modelBuilder.Entity<TB_CLAIM_SCHEDULE_STATUS>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_CLAIM_SCHEDULE_STATUS>()
            //    .HasMany(e => e.TB_CLAIM_SCHEDULE_DN_STATUS)
            //    .WithRequired(e => e.TB_CLAIM_SCHEDULE_STATUS)
            //    .HasForeignKey(e => e.CurrentCSStatusID)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_CLAIM_SCHEDULE_STATUS>()
            //    .HasMany(e => e.TB_CLAIM_SCHEDULE_DN_STATUS1)
            //    .WithRequired(e => e.TB_CLAIM_SCHEDULE_STATUS1)
            //    .HasForeignKey(e => e.LatestCSStatusID)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_CLAIM_SCHEDULE_STATUS>()
            //    .HasMany(e => e.TB_CLAIM_SCHEDULE_USER_ASSIGNMENT)
            //    .WithRequired(e => e.TB_CLAIM_SCHEDULE_STATUS)
            //    .HasForeignKey(e => e.Assigned_CS_Status_ID)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_CLAIM_SCHEDULE_STATUS_TYPE>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<TB_CLAIM_SCHEDULE_STATUS_TYPE>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<TB_CLAIM_SCHEDULE_STATUS_TYPE>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_CLAIM_SCHEDULE_STATUS_TYPE>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_CLAIM_SCHEDULE_STATUS_TYPE>()
            //    .HasMany(e => e.TB_CLAIM_SCHEDULE_STATUS)
            //    .WithRequired(e => e.TB_CLAIM_SCHEDULE_STATUS_TYPE)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_DRAWDATE_CALENDAR>()
                .Property(e => e.Note)
                .IsUnicode(false);

            modelBuilder.Entity<TB_DRAWDATE_CALENDAR>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_DRAWDATE_CALENDAR>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_ECS>()
                .Property(e => e.ECS_Number)
                .IsUnicode(false);

            modelBuilder.Entity<TB_ECS>()
                .Property(e => e.ECS_File_Name)
                .IsUnicode(false);

            modelBuilder.Entity<TB_ECS>()
                .Property(e => e.Amount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<TB_ECS>()
                .Property(e => e.SentToScoTaskNumber)
                .IsUnicode(false);

            modelBuilder.Entity<TB_ECS>()
                .Property(e => e.WarrantReceivedTaskNumber)
                .IsUnicode(false);

            modelBuilder.Entity<TB_ECS>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_ECS>()
                .Property(e => e.ApprovedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_ECS>()
                .Property(e => e.CurrentStatusNote)
                .IsUnicode(false);

            modelBuilder.Entity<TB_ECS>()
                .Property(e => e.DexFileName)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_ECS>()
            //    .HasMany(e => e.TB_CLAIM_SCHEDULE_ECS)
            //    .WithRequired(e => e.TB_ECS)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_ECS_STATUS_TYPE>()
                .Property(e => e.CODE)
                .IsUnicode(false);

            modelBuilder.Entity<TB_ECS_STATUS_TYPE>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<TB_ECS_STATUS_TYPE>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_ECS_STATUS_TYPE>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_ECS_STATUS_TYPE>()
            //    .HasMany(e => e.TB_ECS)
            //    .WithRequired(e => e.TB_ECS_STATUS_TYPE)
            //    .HasForeignKey(e => e.Current_ECS_Status_Type_ID)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_EXCLUSIVE_PAYMENT_TYPE>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<TB_EXCLUSIVE_PAYMENT_TYPE>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<TB_EXCLUSIVE_PAYMENT_TYPE>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_EXCLUSIVE_PAYMENT_TYPE>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_EXCLUSIVE_PAYMENT_TYPE>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<TB_EXCLUSIVE_PAYMENT_TYPE>()
                .Property(e => e.DeactivatedBy)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_EXCLUSIVE_PAYMENT_TYPE>()
            //    .HasMany(e => e.TB_CLAIM_SCHEDULE)
            //    .WithRequired(e => e.TB_EXCLUSIVE_PAYMENT_TYPE)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_EXCLUSIVE_PAYMENT_TYPE>()
            //    .HasMany(e => e.TB_ECS)
            //    .WithRequired(e => e.TB_EXCLUSIVE_PAYMENT_TYPE)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_EXCLUSIVE_PAYMENT_TYPE>()
            //    .HasMany(e => e.TB_PAYMENT_RECORD_EXT_CAPMAN)
            //    .WithRequired(e => e.TB_EXCLUSIVE_PAYMENT_TYPE)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_FACESHEET>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_FACESHEET>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_FUND>()
                .Property(e => e.Fund_Code)
                .IsUnicode(false);

            modelBuilder.Entity<TB_FUND>()
                .Property(e => e.Fund_Name)
                .IsUnicode(false);

            modelBuilder.Entity<TB_FUND>()
                .Property(e => e.Fund_Description)
                .IsUnicode(false);

            modelBuilder.Entity<TB_FUND>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_FUND>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_FUND>()
                .Property(e => e.DeactivatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_FUND>()
                .Property(e => e.Stat_Year)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_FUND>()
            //    .HasMany(e => e.TB_EXCLUSIVE_PAYMENT_TYPE)
            //    .WithRequired(e => e.TB_FUND)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_FUND>()
            //    .HasMany(e => e.TB_FACESHEET)
            //    .WithRequired(e => e.TB_FUND)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_FUND>()
            //    .HasMany(e => e.TB_SCO_FILE_PROPERTY)
            //    .WithRequired(e => e.TB_FUND)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_FUNDING_DETAIL>()
                .Property(e => e.Funding_Source_Name)
                .IsUnicode(false);

            modelBuilder.Entity<TB_FUNDING_DETAIL>()
                .Property(e => e.FFPAmount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<TB_FUNDING_DETAIL>()
                .Property(e => e.SGFAmount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<TB_FUNDING_DETAIL>()
                .Property(e => e.TotalAmount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<TB_FUNDING_DETAIL>()
                .Property(e => e.FiscalYear)
                .IsUnicode(false);

            modelBuilder.Entity<TB_FUNDING_DETAIL>()
                .Property(e => e.FiscalQuarter)
                .IsUnicode(false);

            modelBuilder.Entity<TB_FUNDING_DETAIL>()
                .Property(e => e.Title)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_FUNDING_DETAIL>()
            //    .HasOptional(e => e.TB_FUNDING_DETAIL_EXT_CAPMAN)
            //    .WithRequired(e => e.TB_FUNDING_DETAIL);

            //modelBuilder.Entity<TB_FUNDING_DETAIL>()
            //    .HasMany(e => e.TB_FUNDING_DETAIL_KVP)
            //    .WithRequired(e => e.TB_FUNDING_DETAIL)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_FUNDING_DETAIL_EXT_CAPMAN>()
                .Property(e => e.FedFundCode)
                .IsUnicode(false);

            modelBuilder.Entity<TB_FUNDING_DETAIL_EXT_CAPMAN>()
                .Property(e => e.StateFundCode)
                .IsUnicode(false);

            modelBuilder.Entity<TB_FUNDING_DETAIL_KVP>()
                .Property(e => e.Kvp_Value)
                .IsUnicode(false);

            modelBuilder.Entity<TB_FUNDING_SOURCE>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<TB_FUNDING_SOURCE>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<TB_FUNDING_SOURCE>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_FUNDING_SOURCE>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_FUNDING_SOURCE>()
                .Property(e => e.DeactivatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYDATE_CALENDAR>()
                .Property(e => e.Note)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYDATE_CALENDAR>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYDATE_CALENDAR>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_PAYDATE_CALENDAR>()
            //    .HasMany(e => e.TB_CLAIM_SCHEDULE)
            //    .WithRequired(e => e.TB_PAYDATE_CALENDAR)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_PAYMENT_EXCHANGE_ENTITY>()
                .Property(e => e.Entity_ID)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_EXCHANGE_ENTITY>()
                .Property(e => e.Entity_ID_Type)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_EXCHANGE_ENTITY>()
                .Property(e => e.Entity_Name)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_EXCHANGE_ENTITY>()
                .Property(e => e.Entity_EIN)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_EXCHANGE_ENTITY>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_EXCHANGE_ENTITY>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_PAYMENT_EXCHANGE_ENTITY>()
            //    .HasMany(e => e.TB_PAYMENT_EXCHANGE_ENTITY_INFO)
            //    .WithRequired(e => e.TB_PAYMENT_EXCHANGE_ENTITY)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_PAYMENT_EXCHANGE_ENTITY>()
            //    .HasMany(e => e.TB_PEE_SYSTEM)
            //    .WithRequired(e => e.TB_PAYMENT_EXCHANGE_ENTITY)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_PAYMENT_EXCHANGE_ENTITY>()
            //    .HasMany(e => e.TB_TRANSACTION_PAYER_ENTITY)
            //    .WithRequired(e => e.TB_PAYMENT_EXCHANGE_ENTITY)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_PAYMENT_EXCHANGE_ENTITY_INFO>()
                .Property(e => e.Entity_Name)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_EXCHANGE_ENTITY_INFO>()
                .Property(e => e.Entity_Code_Suffix)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_EXCHANGE_ENTITY_INFO>()
                .Property(e => e.Entity_Address_Line1)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_EXCHANGE_ENTITY_INFO>()
                .Property(e => e.Entity_Address_Line2)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_EXCHANGE_ENTITY_INFO>()
                .Property(e => e.Entity_Address_Line3)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_EXCHANGE_ENTITY_INFO>()
                .Property(e => e.Entity_City)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_EXCHANGE_ENTITY_INFO>()
                .Property(e => e.Entity_State)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_EXCHANGE_ENTITY_INFO>()
                .Property(e => e.Entity_Zip)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_EXCHANGE_ENTITY_INFO>()
                .Property(e => e.Entity_VendorTypeCode)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_EXCHANGE_ENTITY_INFO>()
                .Property(e => e.Entity_ContractNumber)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_PAYMENT_EXCHANGE_ENTITY_INFO>()
            //    .HasMany(e => e.TB_CLAIM_SCHEDULE)
            //    .WithOptional(e => e.TB_PAYMENT_EXCHANGE_ENTITY_INFO)
            //    .HasForeignKey(e => e.Payee_Entity_Info_ID);

            //modelBuilder.Entity<TB_PAYMENT_EXCHANGE_ENTITY_INFO>()
            //    .HasMany(e => e.TB_PAYMENT_RECORD)
            //    .WithOptional(e => e.TB_PAYMENT_EXCHANGE_ENTITY_INFO)
            //    .HasForeignKey(e => e.Payee_Entity_Info_ID);

            modelBuilder.Entity<TB_PAYMENT_KVP>()
                .Property(e => e.Kvp_Value)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_METHOD_TYPE>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_METHOD_TYPE>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_METHOD_TYPE>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_METHOD_TYPE>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_PAYMENT_METHOD_TYPE>()
            //    .HasMany(e => e.TB_CLAIM_SCHEDULE)
            //    .WithRequired(e => e.TB_PAYMENT_METHOD_TYPE)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_PAYMENT_METHOD_TYPE>()
            //    .HasMany(e => e.TB_ECS)
            //    .WithRequired(e => e.TB_PAYMENT_METHOD_TYPE)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_PAYMENT_METHOD_TYPE>()
            //    .HasMany(e => e.TB_PAYMENT_RECORD)
            //    .WithRequired(e => e.TB_PAYMENT_METHOD_TYPE)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_PAYMENT_RECORD>()
                .Property(e => e.PaymentRec_Number)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_RECORD>()
                .Property(e => e.PaymentRec_NumberExt)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_RECORD>()
                .Property(e => e.Payment_Type)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_RECORD>()
                .Property(e => e.Amount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<TB_PAYMENT_RECORD>()
                .Property(e => e.FiscalYear)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_RECORD>()
                .Property(e => e.IndexCode)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_RECORD>()
                .Property(e => e.ObjectDetailCode)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_RECORD>()
                .Property(e => e.ObjectAgencyCode)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_RECORD>()
                .Property(e => e.PCACode)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_RECORD>()
                .Property(e => e.ApprovedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_RECORD>()
                .Property(e => e.PaymentSet_Number)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_RECORD>()
                .Property(e => e.PaymentSet_NumberExt)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_RECORD>()
                .Property(e => e.RPICode)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_PAYMENT_RECORD>()
            //    .HasMany(e => e.TB_FUNDING_DETAIL)
            //    .WithRequired(e => e.TB_PAYMENT_RECORD)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_PAYMENT_RECORD>()
            //    .HasOptional(e => e.TB_PAYMENT_DN_STATUS)
            //    .WithRequired(e => e.TB_PAYMENT_RECORD);

            //modelBuilder.Entity<TB_PAYMENT_RECORD>()
            //    .HasMany(e => e.TB_PAYMENT_KVP)
            //    .WithRequired(e => e.TB_PAYMENT_RECORD)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_PAYMENT_RECORD>()
            //    .HasOptional(e => e.TB_PAYMENT_RECORD_EXT_CAPMAN)
            //    .WithRequired(e => e.TB_PAYMENT_RECORD);

            //modelBuilder.Entity<TB_PAYMENT_RECORD>()
            //    .HasMany(e => e.TB_PAYMENT_STATUS)
            //    .WithRequired(e => e.TB_PAYMENT_RECORD)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_PAYMENT_RECORD>()
            //    .HasMany(e => e.TB_PAYMENT_USER_ASSIGNMENT)
            //    .WithRequired(e => e.TB_PAYMENT_RECORD)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_PAYMENT_RECORD_EXT_CAPMAN>()
                .Property(e => e.ContractNumber)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_STATUS>()
                .Property(e => e.Status_Note)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_STATUS>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_PAYMENT_STATUS>()
            //    .HasMany(e => e.TB_PAYMENT_DN_STATUS)
            //    .WithRequired(e => e.TB_PAYMENT_STATUS)
            //    .HasForeignKey(e => e.CurrentPaymentStatusID)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_PAYMENT_STATUS>()
            //    .HasMany(e => e.TB_PAYMENT_DN_STATUS1)
            //    .WithOptional(e => e.TB_PAYMENT_STATUS1)
            //    .HasForeignKey(e => e.CurrentHoldStatusID);

            //modelBuilder.Entity<TB_PAYMENT_STATUS>()
            //    .HasMany(e => e.TB_PAYMENT_DN_STATUS2)
            //    .WithOptional(e => e.TB_PAYMENT_STATUS2)
            //    .HasForeignKey(e => e.CurrentUnHoldStatusID);

            //modelBuilder.Entity<TB_PAYMENT_STATUS>()
            //    .HasMany(e => e.TB_PAYMENT_DN_STATUS3)
            //    .WithOptional(e => e.TB_PAYMENT_STATUS3)
            //    .HasForeignKey(e => e.CurrentReleaseFromSupStatusID);

            //modelBuilder.Entity<TB_PAYMENT_STATUS>()
            //    .HasMany(e => e.TB_PAYMENT_USER_ASSIGNMENT)
            //    .WithRequired(e => e.TB_PAYMENT_STATUS)
            //    .HasForeignKey(e => e.Assigned_Payment_Status_ID)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_PAYMENT_STATUS_TYPE>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_STATUS_TYPE>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_STATUS_TYPE>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_STATUS_TYPE>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_PAYMENT_STATUS_TYPE>()
            //    .HasMany(e => e.TB_PAYMENT_STATUS)
            //    .WithRequired(e => e.TB_PAYMENT_STATUS_TYPE)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_PAYMENT_STATUS_TYPE_EXTERNAL>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_STATUS_TYPE_EXTERNAL>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_STATUS_TYPE_EXTERNAL>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PAYMENT_STATUS_TYPE_EXTERNAL>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_PAYMENT_USER_ASSIGNMENT>()
            //    .HasMany(e => e.TB_PAYMENT_DN_STATUS)
            //    .WithOptional(e => e.TB_PAYMENT_USER_ASSIGNMENT)
            //    .HasForeignKey(e => e.CurrentUserAssignmentID);

            modelBuilder.Entity<TB_PEE_ADDRESS>()
                .Property(e => e.Address_Line1)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PEE_ADDRESS>()
                .Property(e => e.Address_Line2)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PEE_ADDRESS>()
                .Property(e => e.Address_Line3)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PEE_ADDRESS>()
                .Property(e => e.City)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PEE_ADDRESS>()
                .Property(e => e.State)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PEE_ADDRESS>()
                .Property(e => e.Zip)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PEE_ADDRESS>()
                .Property(e => e.ContractNumber)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PEE_ADDRESS>()
                .Property(e => e.Entity_Name)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_PEE_ADDRESS>()
            //    .HasMany(e => e.TB_CLAIM_SCHEDULE)
            //    .WithRequired(e => e.TB_PEE_ADDRESS)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_PEE_ADDRESS>()
            //    .HasMany(e => e.TB_PAYMENT_RECORD)
            //    .WithRequired(e => e.TB_PEE_ADDRESS)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_PEE_EFT_INFO>()
                .Property(e => e.FIRoutingNumber)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PEE_EFT_INFO>()
                .Property(e => e.FIAccountType)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PEE_EFT_INFO>()
                .Property(e => e.PrvAccountNo)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_PEE_SYSTEM>()
            //    .HasMany(e => e.TB_PEE_ADDRESS)
            //    .WithRequired(e => e.TB_PEE_SYSTEM)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_PEE_SYSTEM>()
            //    .HasMany(e => e.TB_PEE_EFT_INFO)
            //    .WithRequired(e => e.TB_PEE_SYSTEM)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_PERMISSION>()
                .Property(e => e.Permission_Code)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PERMISSION>()
                .Property(e => e.Permission_Name)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PERMISSION>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_PERMISSION>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_PERMISSION>()
            //    .HasMany(e => e.TB_ROLE_PERMISSION)
            //    .WithRequired(e => e.TB_PERMISSION)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_REQUEST>()
                .Property(e => e.Msg_Sender_ID)
                .IsUnicode(false);

            modelBuilder.Entity<TB_REQUEST>()
                .Property(e => e.Msg_Transaction_ID)
                .IsUnicode(false);

            modelBuilder.Entity<TB_REQUEST>()
                .Property(e => e.Msg_Transaction_Type)
                .IsUnicode(false);

            modelBuilder.Entity<TB_REQUEST>()
                .Property(e => e.Msg_Transaction_Version)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_REQUEST>()
            //    .HasOptional(e => e.TB_RESPONSE)
            //    .WithRequired(e => e.TB_REQUEST);

            //modelBuilder.Entity<TB_REQUEST>()
            //    .HasOptional(e => e.TB_TRACE_TRANSACTION)
            //    .WithRequired(e => e.TB_REQUEST);

            //modelBuilder.Entity<TB_REQUEST>()
            //    .HasOptional(e => e.TB_TRANSACTION)
            //    .WithRequired(e => e.TB_REQUEST);

            modelBuilder.Entity<TB_RESPONSE>()
                .Property(e => e.Msg_Transaction_ID)
                .IsUnicode(false);

            modelBuilder.Entity<TB_RESPONSE>()
                .Property(e => e.Msg_Transaction_Type)
                .IsUnicode(false);

            modelBuilder.Entity<TB_RESPONSE>()
                .Property(e => e.Response_Message)
                .IsUnicode(false);

            modelBuilder.Entity<TB_RESPONSE_STATUS_TYPE>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<TB_RESPONSE_STATUS_TYPE>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<TB_RESPONSE_STATUS_TYPE>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_RESPONSE_STATUS_TYPE>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_RESPONSE_STATUS_TYPE>()
            //    .HasMany(e => e.TB_RESPONSE)
            //    .WithRequired(e => e.TB_RESPONSE_STATUS_TYPE)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_ROLE>()
                .Property(e => e.Role_Code)
                .IsUnicode(false);

            modelBuilder.Entity<TB_ROLE>()
                .Property(e => e.Role_Name)
                .IsUnicode(false);

            modelBuilder.Entity<TB_ROLE>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_ROLE>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_ROLE>()
            //    .HasMany(e => e.TB_ROLE_PERMISSION)
            //    .WithRequired(e => e.TB_ROLE)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_ROLE>()
            //    .HasMany(e => e.TB_USER_ROLE)
            //    .WithRequired(e => e.TB_ROLE)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_ROLE_PERMISSION>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_ROLE_PERMISSION>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SCO_FILE_PROPERTY>()
                .Property(e => e.SCO_File_Property_Name)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SCO_FILE_PROPERTY>()
                .Property(e => e.SCO_File_Property_Value)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SCO_FILE_PROPERTY>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SCO_FILE_PROPERTY>()
                .Property(e => e.Payment_Type)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SCO_FILE_PROPERTY>()
                .Property(e => e.Environment)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SCO_FILE_PROPERTY>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SCO_FILE_PROPERTY>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SCO_PROPERTY>()
                .Property(e => e.SCO_Property_Name)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SCO_PROPERTY>()
                .Property(e => e.SCO_Property_Value)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SCO_PROPERTY>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SCO_PROPERTY>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SCO_PROPERTY>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SCO_PROPERTY_ENUM>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SCO_PROPERTY_ENUM>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SCO_PROPERTY_ENUM>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SCO_PROPERTY_ENUM>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_SCO_PROPERTY_ENUM>()
            //    .HasMany(e => e.TB_SCO_FILE_PROPERTY)
            //    .WithRequired(e => e.TB_SCO_PROPERTY_ENUM)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_SCO_PROPERTY_ENUM>()
            //    .HasMany(e => e.TB_SCO_PROPERTY)
            //    .WithRequired(e => e.TB_SCO_PROPERTY_ENUM)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_SCO_PROPERTY_TYPE>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SCO_PROPERTY_TYPE>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SCO_PROPERTY_TYPE>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SCO_PROPERTY_TYPE>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_SCO_PROPERTY_TYPE>()
            //    .HasMany(e => e.TB_SCO_FILE_PROPERTY)
            //    .WithRequired(e => e.TB_SCO_PROPERTY_TYPE)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_SCO_PROPERTY_TYPE>()
            //    .HasMany(e => e.TB_SCO_PROPERTY)
            //    .WithRequired(e => e.TB_SCO_PROPERTY_TYPE)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_SCO_PROPERTY_TYPE>()
            //    .HasMany(e => e.TB_SCO_PROPERTY_ENUM)
            //    .WithRequired(e => e.TB_SCO_PROPERTY_TYPE)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_SOR_KVP_KEY>()
                .Property(e => e.Kvp_Key_Name)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SOR_KVP_KEY>()
                .Property(e => e.Kvp_Value_DataType)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SOR_KVP_KEY>()
                .Property(e => e.Kvp_Description)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SOR_KVP_KEY>()
                .Property(e => e.OwnerEntity)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SOR_KVP_KEY>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SOR_KVP_KEY>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_SOR_KVP_KEY>()
            //    .HasMany(e => e.TB_FUNDING_DETAIL_KVP)
            //    .WithRequired(e => e.TB_SOR_KVP_KEY)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_SOR_KVP_KEY>()
            //    .HasMany(e => e.TB_PAYMENT_KVP)
            //    .WithRequired(e => e.TB_SOR_KVP_KEY)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_SYSTEM>()
                .Property(e => e.System_Code)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SYSTEM>()
                .Property(e => e.System_Name)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SYSTEM>()
                .Property(e => e.RA_DEPARTMENT_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SYSTEM>()
                .Property(e => e.RA_DEPARTMENT_ADDR_LINE)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SYSTEM>()
                .Property(e => e.RA_DEPARTMENT_ADDR_CSZ)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SYSTEM>()
                .Property(e => e.RA_ORGANIZATION_CODE)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SYSTEM>()
                .Property(e => e.RA_INQUIRIES_PHONE_NUMBER)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SYSTEM>()
                .Property(e => e.FEIN_Number)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SYSTEM>()
                .Property(e => e.MAX_PYMT_REC_AMOUNT)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SYSTEM>()
                .Property(e => e.MAX_PYMT_REC_PER_TRAN)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SYSTEM>()
                .Property(e => e.MAX_FUNDING_DTL_PER_PYMT_REC)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SYSTEM>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SYSTEM>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SYSTEM>()
                .Property(e => e.TITLE_TRANSFER_LETTER)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_SYSTEM>()
            //    .HasMany(e => e.TB_EXCLUSIVE_PAYMENT_TYPE)
            //    .WithRequired(e => e.TB_SYSTEM)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_SYSTEM>()
            //    .HasMany(e => e.TB_FUND)
            //    .WithRequired(e => e.TB_SYSTEM)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_SYSTEM>()
            //    .HasMany(e => e.TB_FUNDING_SOURCE)
            //    .WithRequired(e => e.TB_SYSTEM)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_SYSTEM>()
            //    .HasMany(e => e.TB_SCO_FILE_PROPERTY)
            //    .WithRequired(e => e.TB_SYSTEM)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_SYSTEM>()
            //    .HasMany(e => e.TB_SCO_PROPERTY)
            //    .WithRequired(e => e.TB_SYSTEM)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_SYSTEM>()
            //    .HasMany(e => e.TB_SYSTEM_USER)
            //    .WithRequired(e => e.TB_SYSTEM)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_SYSTEM_OF_RECORD>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SYSTEM_OF_RECORD>()
                .Property(e => e.Description)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_SYSTEM_OF_RECORD>()
            //    .Property(e => e.CreatedBy)
            //    .IsFixedLength();

            //modelBuilder.Entity<TB_SYSTEM_OF_RECORD>()
            //    .Property(e => e.UpdatedBy)
            //    .IsUnicode(false);

            //modelBuilder.Entity<TB_SYSTEM_OF_RECORD>()
            //    .HasMany(e => e.TB_PEE_SYSTEM)
            //    .WithRequired(e => e.TB_SYSTEM_OF_RECORD)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_SYSTEM_OF_RECORD>()
            //    .HasMany(e => e.TB_SOR_KVP_KEY)
            //    .WithRequired(e => e.TB_SYSTEM_OF_RECORD)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_SYSTEM_OF_RECORD>()
            //    .HasMany(e => e.TB_TRANSACTION)
            //    .WithRequired(e => e.TB_SYSTEM_OF_RECORD)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_SYSTEM_USER>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_SYSTEM_USER>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_PAYMENT>()
                .Property(e => e.Payment_Status_Message)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_PAYMENT>()
                .Property(e => e.ClaimScheduleNumber)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_PAYMENT>()
                .Property(e => e.WarrantNumber)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_PAYMENT>()
                .Property(e => e.WarrantAmount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<TB_TRACE_PAYMENT>()
                .Property(e => e.PaymentRec_Number)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_PAYMENT>()
                .Property(e => e.PaymentRec_NumberExt)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_PAYMENT>()
                .Property(e => e.Payment_Type)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_PAYMENT>()
                .Property(e => e.Amount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<TB_TRACE_PAYMENT>()
                .Property(e => e.FiscalYear)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_PAYMENT>()
                .Property(e => e.IndexCode)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_PAYMENT>()
                .Property(e => e.ObjectDetailCode)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_PAYMENT>()
                .Property(e => e.ObjectAgencyCode)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_PAYMENT>()
                .Property(e => e.PCACode)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_PAYMENT>()
                .Property(e => e.ApprovedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_PAYMENT>()
                .Property(e => e.PaymentSet_Number)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_PAYMENT>()
                .Property(e => e.PaymentSet_NumberExt)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_PAYMENT>()
                .Property(e => e.Payee_Entity_ID)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_PAYMENT>()
                .Property(e => e.Payee_Entity_ID_Type)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_PAYMENT>()
                .Property(e => e.Payee_Entity_Name)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_PAYMENT>()
                .Property(e => e.Payee_Entity_ID_Suffix)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_PAYMENT>()
                .Property(e => e.Payee_Address_Line1)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_PAYMENT>()
                .Property(e => e.Payee_Address_Line2)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_PAYMENT>()
                .Property(e => e.Payee_Address_Line3)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_PAYMENT>()
                .Property(e => e.Payee_City)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_PAYMENT>()
                .Property(e => e.Payee_State)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_PAYMENT>()
                .Property(e => e.Payee_Zip)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_PAYMENT>()
                .Property(e => e.Payee_EIN)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_PAYMENT>()
                .Property(e => e.Payee_VendorTypeCode)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_TRANSACTION>()
                .Property(e => e.Payer_Entity_ID)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_TRANSACTION>()
                .Property(e => e.Payer_Entity_ID_Type)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_TRANSACTION>()
                .Property(e => e.Payer_Entity_Name)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_TRANSACTION>()
                .Property(e => e.Payer_Entity_ID_Suffix)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_TRANSACTION>()
                .Property(e => e.Payer_Address_Line1)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_TRANSACTION>()
                .Property(e => e.Payer_Address_Line2)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_TRANSACTION>()
                .Property(e => e.Payer_Address_Line3)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_TRANSACTION>()
                .Property(e => e.Payer_City)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_TRANSACTION>()
                .Property(e => e.Payer_State)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_TRANSACTION>()
                .Property(e => e.Payer_Zip)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_TRANSACTION>()
                .Property(e => e.TotalPymtAmount)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRACE_TRANSACTION>()
                .Property(e => e.TotalPymtRecCount)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_TRACE_TRANSACTION>()
            //    .HasMany(e => e.TB_TRACE_PAYMENT)
            //    .WithRequired(e => e.TB_TRACE_TRANSACTION)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_TRANSACTION>()
                .Property(e => e.Msg_Transaction_ID)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRANSACTION>()
                .Property(e => e.TransactionVersion)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_TRANSACTION>()
            //    .HasMany(e => e.TB_PAYMENT_RECORD)
            //    .WithRequired(e => e.TB_TRANSACTION)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_TRANSACTION_TYPE>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRANSACTION_TYPE>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRANSACTION_TYPE>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_TRANSACTION_TYPE>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_TRANSACTION_TYPE>()
            //    .HasMany(e => e.TB_TRANSACTION)
            //    .WithRequired(e => e.TB_TRANSACTION_TYPE)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_USER>()
                .Property(e => e.User_Name)
                .IsUnicode(false);

            modelBuilder.Entity<TB_USER>()
                .Property(e => e.Display_Name)
                .IsUnicode(false);

            modelBuilder.Entity<TB_USER>()
                .Property(e => e.User_EmailAddr)
                .IsUnicode(false);

            modelBuilder.Entity<TB_USER>()
                .Property(e => e.User_Password)
                .IsUnicode(false);

            modelBuilder.Entity<TB_USER>()
                .Property(e => e.Domain_Name)
                .IsUnicode(false);

            modelBuilder.Entity<TB_USER>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_USER>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_USER>()
            //    .HasMany(e => e.TB_CLAIM_SCHEDULE_USER_ASSIGNMENT)
            //    .WithRequired(e => e.TB_USER)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_USER>()
            //    .HasMany(e => e.TB_PAYMENT_USER_ASSIGNMENT)
            //    .WithRequired(e => e.TB_USER)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_USER>()
            //    .HasMany(e => e.TB_SYSTEM_USER)
            //    .WithRequired(e => e.TB_USER)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TB_USER>()
            //    .HasMany(e => e.TB_USER_ROLE)
            //    .WithRequired(e => e.TB_USER)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_USER_ROLE>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_USER_ROLE>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_USER_TYPE>()
                .Property(e => e.User_Type_Code)
                .IsUnicode(false);

            modelBuilder.Entity<TB_USER_TYPE>()
                .Property(e => e.User_Type_Name)
                .IsUnicode(false);

            modelBuilder.Entity<TB_USER_TYPE>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<TB_USER_TYPE>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            //modelBuilder.Entity<TB_USER_TYPE>()
            //    .HasMany(e => e.TB_USER)
            //    .WithRequired(e => e.TB_USER_TYPE)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_WARRANT>()
                .Property(e => e.Warrant_Number)
                .IsUnicode(false);

            modelBuilder.Entity<TB_WARRANT>()
                .Property(e => e.Amount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<TB_WARRANT>()
                .Property(e => e.Warrant_Date_Short)
                .IsUnicode(false);

            modelBuilder.Entity<TB_DB_UPDATE_SCRIPT>()
                .Property(e => e.Release_Number)
                .IsUnicode(false);

            modelBuilder.Entity<TB_DB_UPDATE_SCRIPT>()
                .Property(e => e.Script_Name)
                .IsUnicode(false);

            modelBuilder.Entity<TB_DB_UPDATE_SCRIPT>()
                .Property(e => e.Script_Description)
                .IsUnicode(false);
        }
    }
}
