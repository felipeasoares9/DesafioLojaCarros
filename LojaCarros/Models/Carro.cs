using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LojaCarros.Models
{
    public class Carro
    {
        [Key]
        public int Id { get; set; }

        // Propriedades do Carro
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int AnoFabricacao { get; set; }
        public int AnoModelo { get; set; }
        public string Chassi { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Preco { get; set; }

        public bool Vendido { get; set; } = false;

        // Associações
        public ICollection<Nota>? Notas { get; set; }
    }
}
