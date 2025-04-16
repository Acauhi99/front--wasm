using System;

namespace FrontWasm.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public int Star { get; set; } // Pode ser do tipo Enum para definir estrelas como 1 a 5?
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }

        // Relação com serviço
        public int ServiceId { get; set; }
        public Service? Service { get; set; }
    }
}
