using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheBandListApplication.Data
{
    public class PackNiveau
    {
        [ForeignKey("PackId")]
        public int PackId { get; set; }
        public virtual Pack Pack { get; set; }

        [ForeignKey("NiveauId")]
        public int NiveauId { get; set; }
        public virtual Niveau Niveau { get; set; }
    }
}
