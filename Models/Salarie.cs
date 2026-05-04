namespace AnnuaireEntreprise.Models
{
    public class Salarie
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        
        // --- LIGNE À AJOUTER ---
        public string NomComplet => $"{Prenom} {Nom}";

        public string TelephoneFixe { get; set; }
        public string TelephonePortable { get; set; }
        public string Email { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }

        public int SiteId { get; set; }
        public Site Site { get; set; }
    }
}