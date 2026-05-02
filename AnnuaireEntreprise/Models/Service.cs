using System.Collections.Generic;

namespace AnnuaireEntreprise.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Nom { get; set; }

        // Relation de 1 à plusieurs (Un service possède plusieurs salariés)
        public ICollection<Salarie> Salaries { get; set; } = new List<Salarie>();
    }
}