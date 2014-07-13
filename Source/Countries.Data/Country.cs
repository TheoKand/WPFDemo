using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Countries.Data.Models
{
    public class Country
    {

        [Key]
        public string Name { get; set; }
        public double GDP { get; set; }
        public int Population { get; set; }

    }

}
