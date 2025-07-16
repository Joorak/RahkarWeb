using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Domain.OldEntities
{
    // جدول افراد (Person)
    public class Person
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(10)]
        [DataType(DataType.Text)]
        [DisplayName("کد/شناسه ملی")]
        public string NationalCode { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        [MaxLength(11)]
        public string MobileNumber { get; set; }

        // روابط
        public virtual ICollection<PersonDetail> PersonDetails { get; set; }
        public virtual ICollection<PersonServiceCall> ServiceCalls { get; set; }
        public virtual ICollection<Order> OrdersAsCustomer { get; set; }
        //public virtual ICollection<OrderItem> OrderItemsAsSupplier { get; set; }
        public virtual ICollection<OrderGuarantor> OrderGuarantors { get; set; }
        public virtual ICollection<FinancialDocument> FinancialDocuments { get; set; }
        public virtual ICollection<Installment> Installments { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual UserSetting UserSetting { get; set; }
        public virtual ICollection<CustomerEligibilityResult> EligibilityResults { get; set; }
    }

    // جدول جزئیات افراد (PersonDetail)
    public class PersonDetail
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid PersonId { get; set; }

        [Required]
        [MaxLength(50)]
        public string RoleType { get; set; } // مشتری، ضامن، تامین‌کننده، کاربر و ...

        public string DetailsJson { get; set; } // اطلاعات اضافی به صورت JSON (نام، نام خانوادگی و ...)

        [ForeignKey(nameof(PersonId))]
        public virtual Person Person { get; set; }
    }

    // جدول تعریف فراخوانی سرویس‌ها (ServiceDefinition)
    public class ServiceDefinition
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string ApiUrl { get; set; }

        [Required]
        [MaxLength(50)]
        public string ReturnType { get; set; } // نوع داده بازگشتی (اعتبارسنجی، استعلام و ...)

        public string ParametersJson { get; set; } // پارامترهای مورد نیاز به صورت JSON

        [MaxLength(200)]
        public string Token { get; set; } // توکن مجاز برای فراخوانی
    }

    // جدول فراخوانی‌های انجام‌شده برای افراد (PersonServiceCall)
    public class PersonServiceCall
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid PersonId { get; set; }

        [Required]
        public Guid ServiceDefinitionId { get; set; }

        [Required]
        public DateTime CallDate { get; set; }

        [MaxLength(200)]
        public string CallLocation { get; set; } // محل فراخوانی (اختیاری)

        public string ResultJson { get; set; } // نتیجه فراخوانی به صورت JSON

        [ForeignKey(nameof(PersonId))]
        public virtual Person Person { get; set; }

        [ForeignKey(nameof(ServiceDefinitionId))]
        public virtual ServiceDefinition ServiceDefinition { get; set; }
    }

    // جدول طبقه‌بندی کالاها (ProductCategory)
    public class ProductCategory
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public Guid? ParentCategoryId { get; set; } // برای سلسله‌مراتب

        [ForeignKey(nameof(ParentCategoryId))]
        public virtual ProductCategory ParentCategory { get; set; }

        public virtual ICollection<ProductCategory> SubCategories { get; set; }
        public virtual ICollection<LoanSchema> LoanSchemas { get; set; }
    }

    // جدول طرح‌های تسهیلات (LoanSchema)
    public class LoanSchema
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public Guid? SupplierId { get; set; } // تامین‌کننده مرتبط (اختیاری)

        public Guid? ProductCategoryId { get; set; } // طبقه کالای مرتبط (اختیاری)

        public string EligibilityCriteriaJson { get; set; } // معیارهای واجد شرایط (JSON Schema)

        public decimal MaxLoanAmount { get; set; } // حداکثر مبلغ وام

        public int MaxInstallments { get; set; } // حداکثر تعداد اقساط

        public int GracePeriodDays { get; set; } // دوره تنفس

        public int RepaymentIntervalDays { get; set; } // فاصله بازپرداخت‌ها

        public decimal InterestRate { get; set; } // نرخ سود

        public decimal EffectiveInterestRate { get; set; } // نرخ سود واقعی

        public string InterestDifferenceMethod { get; set; } // روش دریافت مابه‌التفاوت نرخ سود

        public string LatePaymentFormula { get; set; } // فرمول جریمه دیرکرد (JSON)

        public string RequiredGuaranteesJson { get; set; } // وثایق/تضامین/ضامن‌های اجباری

        [ForeignKey(nameof(SupplierId))]
        public virtual Person Supplier { get; set; }

        [ForeignKey(nameof(ProductCategoryId))]
        public virtual ProductCategory ProductCategory { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<CustomerEligibilityResult> EligibilityResults { get; set; }
    }

    // جدول سفارش‌ها (Order)
    public class Order
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string OrderNumber { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        public Guid? LoanSchemaId { get; set; }

        [MaxLength(50)]
        public string ContractNumber { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Person Customer { get; set; }

        [ForeignKey(nameof(LoanSchemaId))]
        public virtual LoanSchema LoanSchema { get; set; }

        //public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ICollection<OrderStatusLog> StatusLogs { get; set; }
        public virtual ICollection<OrderGuarantor> Guarantors { get; set; }
        public virtual ICollection<OrderGuarantee> Guarantees { get; set; }
        public virtual ICollection<Installment> Installments { get; set; }
        public virtual ICollection<FinancialDocument> FinancialDocuments { get; set; }
    }

    // جدول لاگ وضعیت سفارش (OrderStatusLog)
    public class OrderStatusLog
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid OrderId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } // جدید، تکمیل، در حال بررسی و ...

        [Required]
        public DateTime StatusDate { get; set; }

        public string Description { get; set; }

        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; }
    }

    // جدول ضامن‌های سفارش (OrderGuarantor)
    public class OrderGuarantor
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public Guid GuarantorId { get; set; }

        public string DetailsJson { get; set; } // اطلاعات اضافی ضامن

        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; }

        [ForeignKey(nameof(GuarantorId))]
        public virtual Person Guarantor { get; set; }
    }

    // جدول وثایق/تضامین سفارش (OrderGuarantee)
    public class OrderGuarantee
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid OrderId { get; set; }

        public string GuaranteeDetailsJson { get; set; } // جزئیات وثیقه به صورت JSON

        [MaxLength(500)]
        public string DocumentPath { get; set; } // مسیر تصویر آپلودشده

        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; }
    }

    // جدول اسناد مالی (FinancialDocument)
    public class FinancialDocument
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid PersonId { get; set; }

        public Guid? OrderId { get; set; } // اختیاری، برای اسنادی که به سفارش خاص مرتبط هستند

        [Required]
        [MaxLength(50)]
        public string DocumentType { get; set; } // بدهکار، بستانکار

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime DocumentDate { get; set; }

        [MaxLength(100)]
        public string ReferenceNumber { get; set; } // شماره پیگیری (مثل درگاه پرداخت)

        [MaxLength(200)]
        public string Description { get; set; } // توضیحات (مثل معین سرویس‌دهنده)

        [ForeignKey(nameof(PersonId))]
        public virtual Person Person { get; set; }

        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; }
    }

    // جدول اقساط (Installment)
    public class Installment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public Guid PersonId { get; set; }

        [Required]
        public int InstallmentNumber { get; set; } // شماره قسط

        [Required]
        public decimal Amount { get; set; } // مبلغ قسط

        public decimal? PaidAmount { get; set; } // مبلغ پرداخت‌شده

        [Required]
        public DateTime DueDate { get; set; } // تاریخ سررسید

        public DateTime? PaymentDate { get; set; } // تاریخ پرداخت (در صورت پرداخت)

        [MaxLength(50)]
        public string Status { get; set; } // پرداخت‌نشده، پرداخت‌شده، معوق

        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; }

        [ForeignKey(nameof(PersonId))]
        public virtual Person Person { get; set; }

        public virtual ICollection<InstallmentPenalty> Penalties { get; set; }
    }

    // جدول پیام‌ها (Message)
    public class Message
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid PersonId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime SentDate { get; set; }

        [MaxLength(50)]
        public string EventType { get; set; } // نوع رویداد (قبول سفارش، سررسید پرداخت و ...)

        public bool IsRead { get; set; } // خوانده‌شده یا خیر

        [ForeignKey(nameof(PersonId))]
        public virtual Person Person { get; set; }
    }

    // جدول تنظیمات کاربر (UserSetting)
    public class UserSetting
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid PersonId { get; set; }

        public string ProfileJson { get; set; } // اطلاعات پروفایلی قابل تغییر (پسورد، روش ورود و ...)

        public string IdentityJson { get; set; } // اطلاعات هویتی غیرقابل ویرایش (از ثبت احوال)

        [ForeignKey(nameof(PersonId))]
        public virtual Person Person { get; set; }
    }

    // جدول نتایج اعتبارسنجی مشتری (CustomerEligibilityResult)
    public class CustomerEligibilityResult
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid PersonId { get; set; }

        [Required]
        public Guid LoanSchemaId { get; set; }

        [Required]
        public DateTime EvaluationDate { get; set; }

        public bool IsEligible { get; set; } // آیا مشتری واجد شرایط است

        public string ResultDetailsJson { get; set; } // جزئیات نتیجه (مثلاً کدام معیارها برآورده نشده‌اند)

        [ForeignKey(nameof(PersonId))]
        public virtual Person Person { get; set; }

        [ForeignKey(nameof(LoanSchemaId))]
        public virtual LoanSchema LoanSchema { get; set; }
    }

    // جدول جریمه‌های اقساط (InstallmentPenalty)
    public class InstallmentPenalty
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid InstallmentId { get; set; }

        [Required]
        public decimal PenaltyAmount { get; set; } // مبلغ جریمه

        [Required]
        public DateTime CalculationDate { get; set; } // تاریخ محاسبه جریمه

        public string FormulaUsedJson { get; set; } // فرمول استفاده‌شده (برای ردیابی)

        [MaxLength(200)]
        public string Description { get; set; } // توضیحات (مثلاً تعداد روزهای دیرکرد)

        [ForeignKey(nameof(InstallmentId))]
        public virtual Installment Installment { get; set; }
    }

    // جدول پارامترهای فرمول (FormulaParameter)
    public class FormulaParameter
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(50)]
        public string OutputType { get; set; } // نوع خروجی (عدد، تاریخ و ...)

        public string Query { get; set; } // کوئری SQL
        public bool HasAdditionalParameters { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    public class CountriesTurnoverStat11
    {
        //[Key]
        //public int Id { get; set; }
        public string ISO2 { get; set; }
        public string ISO3 { get; set; }
        public string Country { get; set; }
        public string Name { get; set; }
        public double LeasingVolume { get; set; }
        public double MarketShare { get; set; }
    }
}