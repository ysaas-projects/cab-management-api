using cab_management.Controllers;
using cab_management.Models;
using DriverDetails.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Reflection;
using System.Security;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace cab_management.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // =========================
        // DB SETS
        // =========================
        public DbSet<User> Users { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Firm> Firms { get; set; }
        public DbSet<FirmDetail> FirmDetails { get; set; }
        public DbSet<FirmTerm>FirmTerms { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<PricingRule> PricingRules { get; set; }
		public DbSet<PricingRules2> PricingRules2 { get; set; }
		public DbSet<CabPrice> CabPrices { get; set; }
        public DbSet<Cab> Cabs { get; set; }
        public DbSet<Customer>Customers { get; set; }
        public DbSet<DriverDetail> DriverDetails { get; set; }
        public DbSet<DutyExpense> DutyExpenses { get; set; }
        public DbSet<DutySlip> DutySlips { get; set; }

		public DbSet<DutySlipBill> DutySlipBills { get; set; }
		public DbSet<DutySlipBillDetail> DutySlipBillDetails { get; set; }

		public DbSet<DutyLocation> DutyLocations { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<CabNumberDirectory> CabNumberDirectory { get; set; }
        public DbSet<CustomerUser> CustomerUsers { get; set; }

        public DbSet<TourPackage> TourPackages { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<PackagePricing> PackagePricings { get; set; }


        // =========================
        // MODEL CONFIGURATION
        // =========================

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }


    }
}
