using System.Collections.Generic;

namespace AnnuaireEntreprise.Models
{
    public class Site
    {
        public int Id { get; set; }
        public string Ville { get; set; }

        // Relation de 1 à plusieurs (Un site possède plusieurs salariés)
        public ICollection<Salarie> Salaries { get; set; } = new List<Salarie>();
    }
}