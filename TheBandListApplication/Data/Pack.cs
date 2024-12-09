using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheBandListApplication.Data
{
    public class Pack
    {
        [Key]
        public int Id { get; set; }
        public string Nom { get; set; }
        public int PointsBonus { get; set; }
    }
}
