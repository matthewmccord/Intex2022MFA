using System;
using System.Collections.Generic;
using System.Linq;

namespace Intex2022.Models.ViewModels
{
    public class CityLoop
    {
        private CrashDbContext _context { get; set; }


        public IQueryable<Crash> Crashes => _context.Crashes;
    }
}
