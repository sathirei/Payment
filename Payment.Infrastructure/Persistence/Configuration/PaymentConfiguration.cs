using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payment.Domain;
using Payment.Domain.Constants;
using Payment.Domain.Source;

namespace Payment.Infrastructure.Persistence.Configuration
{
    internal class PaymentConfiguration : IEntityTypeConfiguration<Domain.Payment>
    {
        public void Configure(EntityTypeBuilder<Domain.Payment> builder)
        {
            builder.ToTable("Payments");
            builder.HasKey(x => x.Id).IsClustered();
            builder.Property(x => x.Id)
                .ValueGeneratedNever();
            builder.Property(x => x.Status)
                .HasConversion(
                s => s.ToString(),
                v => (PaymentStatus)Enum.Parse(typeof(PaymentStatus), v));
            builder.Property(x => x.Type)
                .HasConversion(
                s => s.ToString(),
                v => (PaymentType)Enum.Parse(typeof(PaymentType), v));

            builder
              .Property(s => s.CreatedDateTime)
              .ValueGeneratedOnAdd()
              .IsRequired()
              .HasDefaultValue(SystemTime.OffsetNow());

            builder
               .Property(s => s.LastChangedDateTime)
               .HasColumnName("LastChangedDateTime")
               .ValueGeneratedOnAddOrUpdate()
               .HasDefaultValue(SystemTime.OffsetNow())
               .IsRequired()
               .IsConcurrencyToken();

            builder.OwnsOne(p => p.Plan, ConfigurePaymentPlan);
            builder.HasOne(p => p.Source)
                .WithOne(s => s.Payment)
                .HasForeignKey<PaymentSource>(s => s.PaymentId);
        }

        private void ConfigurePaymentPlan(
            OwnedNavigationBuilder<Domain.Payment, PaymentPlan> paymentPlanConfiguration)
        {
            paymentPlanConfiguration.ToTable("PaymentPlans");
            paymentPlanConfiguration.Property(x => x.Id)
                .UseIdentityColumn()
                .ValueGeneratedOnAdd();
        }
    }
}
