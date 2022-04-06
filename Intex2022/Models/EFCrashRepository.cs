using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UDOT.Models
{
    public class EFCrashRepository : ICrashRepository
    {
        private CrashDbContext _context { get; set; }
        public EFCrashRepository (CrashDbContext temp)
        {
            _context = temp;
        }

        public IQueryable<Crash> Crashes => _context.Crashes;
        public List<Crash> GetCrashes()
        {
            return Crashes.ToList();
        }

        public void AddCrash(Crash crash)
        {
            _context.Add(crash);
            _context.SaveChanges();
        }

        public void SaveCrash(Crash crash)
        {
            _context.Update(crash);
            _context.SaveChanges();
        }

        public void DeleteCrash(Crash crash)
        {
            _context.Remove(crash);
            _context.SaveChanges();
        }
    }
}
