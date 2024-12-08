using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheBandListApplication.Data
{
    public class DifficulteFeature
    {
        [Key]
        public int Id { get; set; }
        public string NomDuFeature { get; set; }
        public string Image { get; set; }

        [ForeignKey("DifficulteRateId")]
        public int DifficulteRateId { get; set; }
        public virtual NiveauDifficulteRate DifficulteRate { get; set; }
    }
}
