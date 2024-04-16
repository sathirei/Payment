using Microsoft.EntityFrameworkCore;
using Payment.Domain.Constants;
using Payment.Infrastructure.Persistence.Configuration;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleToAttribute("Payment.Infrastructure.Tests")]
namespace Payment.Infrastructure
{
    public class PaymentContext : DbContext
    {
        protected internal DbSet<Domain.Payment>? Payments { get; set; }
        public PaymentContext(DbContextOptions<PaymentContext> options)
            : base(options)
        {
            Encryption.SetEncryptionKey("12345678901234567890123456789012");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());
            ConfigurePaymentSource(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private static void ConfigurePaymentSource(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Domain.Payment>().Navigation(e => e.Source).AutoInclude();

            modelBuilder.Entity<Domain.Source.PaymentSource>()
                .Property(x => x.Id)
                .UseIdentityColumn()
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Domain.Source.PaymentSource>()
                .Property(x => x.Type)
                .HasConversion(
                s => s.ToString(),
                v => (PaymentSourceType)Enum.Parse(typeof(PaymentSourceType), v));

            modelBuilder.Entity<Domain.Source.PaymentSource>().UseTpcMappingStrategy();

            modelBuilder.Entity<Domain.Source.CreditCardPaymentSource>()
                .ToTable("Cards");
            modelBuilder.Entity<Domain.Source.CreditCardPaymentSource>()
                .Property(x => x.Number)
                .HasConversion(
                s => Encryption.Encrypt(s),
                v => Encryption.Decrypt(v));
        }
    }
}
