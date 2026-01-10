using Bank_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace Bank_Project.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

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
            base.OnModelCreating(modelBuilder);

            // =========================
            // Customers -> Accounts
            // =========================
            modelBuilder.Entity<Accounts>()
                .HasOne(a => a.Customer)
                .WithMany(c => c.Accounts)
                .HasForeignKey(a => a.CUID)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // Accounts -> Cards (ONE TO ONE)
            // =========================
            modelBuilder.Entity<Cards>()
                .HasOne(c => c.Account)
                .WithOne(a => a.Cards)
                .HasForeignKey<Cards>(c => c.AccountID)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // Accounts -> Transactions
            // =========================
            modelBuilder.Entity<Transactions>()
                .HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountID)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Customers -> Loans
            // =========================
            modelBuilder.Entity<Loans>()
                .HasOne(l => l.Customer)
                .WithMany(c => c.Loans)
                .HasForeignKey(l => l.CUID)
                .OnDelete(DeleteBehavior.Cascade);

            // ==========================
            // Employees (Self Reference)
            // ==========================
            modelBuilder.Entity<Employees>()
                .HasOne(e => e.Supervisor)
                .WithMany(e => e.Subordinate)
                .HasForeignKey(e => e.SupervisorID)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Grants (Many-to-Many) 
            // =========================
            modelBuilder.Entity<Grants>()
                .HasKey(g => new { g.LoanID, g.EmpID });

            modelBuilder.Entity<Grants>()
                .HasOne(g => g.Loan)
                .WithMany(l => l.Grants)
                .HasForeignKey(g => g.LoanID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Grants>()
                .HasOne(g => g.Employee)
                .WithMany(e => e.Grants)
                .HasForeignKey(g => g.EmpID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}