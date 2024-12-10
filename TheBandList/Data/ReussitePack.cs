using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheBandListApplication.Data
{
    public class ReussitePack
    {
        [ForeignKey("UtilisateurId")]
        public int UtilisateurId { get; set; }
        public virtual Utilisateur Utilisateur { get; set; }

        [ForeignKey("PackId")]
        public int PackId { get; set; }
        public virtual Pack Pack { get; set; }

        public DateTime DateReussite { get; set; }
    }
}
