using Bank_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace Bank_Project.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Accounts> Accounts { get; set; }
        public DbSet<Branches> Branches { get; set; }
        public DbSet<Cards> Cards { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<Loans> Loans { get; set; }
        public DbSet<Employees> Employees { get; set; }
        public DbSet<Grants> Grants { get; set; }
        public DbSet<Transactions> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ================== 1. Customers ==================
            modelBuilder.Entity<Customers>(entity =>
            {
                entity.HasKey(c => c.CUID);
                entity.Property(c => c.CUID).ValueGeneratedOnAdd().UseIdentityColumn();
                entity.Property(c => c.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(c => c.Email).IsRequired().HasMaxLength(70);
                entity.Property(c => c.Salary).HasPrecision(18, 2); // دقة مالية

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Customer_Phone", "PhoneNumber LIKE '07[0-9]%' AND LEN(PhoneNumber) = 10");
                });

                entity.HasMany(c => c.Accounts).WithOne(a => a.Customer).HasForeignKey(a => a.CUID).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(c => c.Loans).WithOne(l => l.Customer).HasForeignKey(l => l.CUID).OnDelete(DeleteBehavior.Cascade);
            });

            // ================== 2. Employees (Fixed Global Filter & Precision) ==================
            modelBuilder.Entity<Employees>(entity =>
            {
                entity.HasKey(e => e.EmpID);
                entity.Property(e => e.Salary).HasPrecision(18, 2);

                // حل مشكلة التحذير: جعلنا العلاقة مع القروض اختيارية برمجياً لتجنب تعارض الفلتر
                entity.HasQueryFilter(e => !e.IsDeleted);

                entity.HasOne(e => e.Supervisor)
                      .WithMany(m => m.Subordinate)
                      .HasForeignKey(e => e.SupervisorID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ================== 3. Accounts ==================
            modelBuilder.Entity<Accounts>(entity =>
            {
                 entity.HasOne(a => a.Customer)
                .WithMany(c => c.Accounts)
                .HasForeignKey(a => a.CUID)
                .HasConstraintName("FK_Accounts_Customers");
            });

            // ================== 4. Loans ==================
            modelBuilder.Entity<Loans>(entity =>
            {
                entity.HasKey(l => l.LoanID);
                entity.Property(l => l.LoanAmount).HasPrecision(18, 2);
                entity.Property(l => l.PaymentAmount).HasPrecision(18, 2);
                entity.Property(l => l.InterestRate).HasPrecision(5, 2);

                // حل مشكلة التحذير: تحديد سلوك الحذف لمنع التعارض مع فلتر الموظفين
                entity.HasOne(l => l.Customer).WithMany(c => c.Loans).HasForeignKey(l => l.CUID);
            });

            // ================== 5. Grants (Join Table) ==================
            modelBuilder.Entity<Grants>(entity =>
            {
                entity.HasKey(g => new { g.LoanID, g.EmpID });

                // منع الحذف المتسلسل لتجنب مشاكل الـ Global Filter
                entity.HasOne(g => g.Employee).WithMany(e => e.Grants).HasForeignKey(g => g.EmpID).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(g => g.Loan).WithMany(l => l.Grants).HasForeignKey(g => g.LoanID).OnDelete(DeleteBehavior.Restrict);
            });

            // ================== 6. Transactions (Fixed Performance) ==================
            modelBuilder.Entity<Transactions>(entity =>
            {
                entity.HasKey(t => t.TransactID);
                entity.Property(t => t.Amount).HasPrecision(18, 2);

                entity.HasOne(t => t.Account)
                      .WithMany(a => a.Transactions)
                      .HasForeignKey(t => t.AccountID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // بقية الكيانات (Branches, Cards)
            modelBuilder.Entity<Branches>().HasKey(b => b.BranchID);
            modelBuilder.Entity<Cards>().HasKey(c => c.CardID);
        }
    }
}