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


        //Configration the Data Base
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ================== Customers ==================
            modelBuilder.Entity<Customers>(entity =>
            {
                entity.HasKey(c => c.CUID);
                entity.Property(c => c.CUID).ValueGeneratedOnAdd().UseIdentityColumn();
                entity.Property(c => c.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(c => c.SecondName).IsRequired().HasMaxLength(50);
                entity.Property(c => c.LastName).IsRequired().HasMaxLength(50);
                entity.Property(c => c.Gender).IsRequired().HasMaxLength(50);
                entity.Property(c => c.BirthDate).IsRequired().HasColumnType("date");
                entity.Property(c => c.PhoneNumber).IsRequired();
                entity.Property(c => c.Address).IsRequired().HasMaxLength(25);
                entity.Property(c => c.Salary).IsRequired();
                entity.Property(c => c.Email).IsRequired().HasMaxLength(70);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Customer_Phone",
                        "PhoneNumber LIKE '07[0-9]%' AND LEN(PhoneNumber) = 10");
                });

                // Navigation property
                entity.HasMany(c => c.Accounts)
                      .WithOne(a => a.Customers)
                      .HasForeignKey(a => a.CUID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.Loans)
                      .WithOne(l => l.Customer)
                      .HasForeignKey(l => l.CUID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ================== Branches ==================
            modelBuilder.Entity<Branches>(entity =>
            {
                entity.HasKey(b => b.BranchID);
                entity.Property(b => b.BranchID).ValueGeneratedOnAdd();
                entity.Property(b => b.BranchName).IsRequired().HasMaxLength(25);
                entity.Property(b => b.BranchAddress).IsRequired().HasMaxLength(50);

                entity.HasMany(b => b.Accounts)
                      .WithOne(a => a.Branches)
                      .HasForeignKey(a => a.BranchID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(b => b.Employees)
                      .WithOne(e => e.Branches)
                      .HasForeignKey(e => e.BranchID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ================== Accounts ==================
            modelBuilder.Entity<Accounts>(entity =>
            {
                entity.HasKey(a => a.AccountID);
                entity.Property(a => a.AccountID).ValueGeneratedOnAdd();
                entity.Property(a => a.AccountType).IsRequired().HasMaxLength(50);
                entity.Property(a => a.Balance).IsRequired();

                entity.HasOne(a => a.Customers)
                      .WithMany(c => c.Accounts)
                      .HasForeignKey(a => a.CUID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ================== Cards ==================
            modelBuilder.Entity<Cards>(entity =>
            {
                entity.HasKey(c => c.CardID);
                entity.Property(c => c.CardNumber).IsRequired().HasMaxLength(16);
                entity.Property(c => c.CardType).IsRequired().HasMaxLength(25);
                entity.Property(c => c.CVV).IsRequired().HasMaxLength(3);
                entity.Property(c => c.PasswordHash).IsRequired().HasMaxLength(99);

                entity.HasOne(c => c.Account)
                      .WithOne(a => a.Cards)
                      .HasForeignKey<Cards>(c => c.AccountID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ================== Loans ==================
            modelBuilder.Entity<Loans>(entity =>
            {
                entity.HasKey(l => l.LoanID);
                entity.Property(l => l.LoanID).ValueGeneratedOnAdd();
                entity.Property(l => l.LoanAmount).IsRequired();
                entity.Property(l => l.PaymentAmount).IsRequired();
                entity.Property(l => l.StartDate).IsRequired().HasColumnType("date");
                entity.Property(l => l.EndDate).IsRequired().HasColumnType("date");
                entity.Property(l => l.InterestRate).HasPrecision(5, 2);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Loans_LoanAmount_Positive", "LoanAmount >= 1");
                    t.HasCheckConstraint("CK_Payments_PaymentAmount_Positive", "PaymentAmount >= 1");
                });

                entity.HasOne(l => l.Customer)
                      .WithMany(c => c.Loans)
                      .HasForeignKey(l => l.CUID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ================== Employees ==================
            modelBuilder.Entity<Employees>(entity =>
            {
                entity.HasKey(e => e.EmpID);
                entity.Property(e => e.EmpID).ValueGeneratedOnAdd();
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.SecondName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Gender).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Salary).IsRequired();

                entity.HasOne(e => e.Branches)
                      .WithMany(b => b.Employees)
                      .HasForeignKey(e => e.BranchID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Supervisor)
                      .WithMany(m => m.Subordinate)
                      .HasForeignKey(e => e.SupervisorID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Salary_Up_FiveHundred", "Salary > 1");
                });
            });

            // ================== Grants ==================
            modelBuilder.Entity<Grants>(entity =>
            {
                entity.HasKey(g => new { g.LoanID, g.EmpID });

                entity.HasOne(g => g.Employee)
                      .WithMany(e => e.Grants)
                      .HasForeignKey(g => g.EmpID)
                      .OnDelete(DeleteBehavior.NoAction); // prevent cascade

                entity.HasOne(g => g.Loan)
                      .WithMany(l => l.Grants)
                      .HasForeignKey(g => g.LoanID)
                      .OnDelete(DeleteBehavior.NoAction); // prevent cascade
            });

        }
    }
}


