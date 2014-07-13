using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Countries.Data.Models;

namespace Countries.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Country> Countries { get; set; }

        public DatabaseContext()
            : base("CountriesDemoDatabase")
        {
        }

    }

}
