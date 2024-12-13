using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheBandListApplication.Data
{
    public class SoumissionNiveau
    {
        [Key]
        public int IdSoumission { get; set; }
        public string NomNiveau { get; set; }
        public string UrlVideo { get; set; }
        public string NomUtilisateur { get; set; }
        public DateTime DateSoumission { get; set; } = DateTime.Now;
    }
}
