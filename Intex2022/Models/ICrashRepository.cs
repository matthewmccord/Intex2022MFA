using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Intex2022.Models
{
    public interface ICrashRepository
    {
        IQueryable<Crash> Crashes { get; }
        public void AddCrash(Crash crash);
        public void SaveCrash(Crash crash);
        public void DeleteCrash(Crash crash);
    }
}
