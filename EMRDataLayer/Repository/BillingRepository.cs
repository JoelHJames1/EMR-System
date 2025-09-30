using EMRDataLayer.DataContext;
using EMRDataLayer.Model;
using EMRDataLayer.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRDataLayer.Repository
{
    public class BillingRepository : Repository<Billing>, IBillingRepository
    {
        public BillingRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<Billing>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Billings
                .Include(b => b.BillingItems)
                .Where(b => b.PatientId == patientId)
                .OrderByDescending(b => b.InvoiceDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Billing>> GetUnpaidInvoicesAsync()
        {
            return await _context.Billings
                .Where(b => b.Status != "Paid" && b.BalanceAmount > 0)
                .OrderBy(b => b.DueDate)
                .ToListAsync();
        }

        public async Task<decimal> GetPatientBalanceAsync(int patientId)
        {
            return await _context.Billings
                .Where(b => b.PatientId == patientId && b.Status != "Paid")
                .SumAsync(b => b.BalanceAmount);
        }
    }
}