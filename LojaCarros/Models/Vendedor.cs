using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LojaCarros.Models
{
    public class Vendedor
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public DateTime DataAdmissao { get; set; }
        public string Matricula { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Salario { get; set; }

        public decimal CalcComissao(decimal totalVendas)
        {
            return totalVendas * 0.05m;
        }

        // Associações
        public ICollection<Nota>? Vendas { get; set; }
    }
}