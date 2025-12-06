using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LojaCarros.Models
{
    public class Nota
    {
        [Key]
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public DateTime DataEmissao { get; set; }
        public bool Garantia { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorVenda { get; set; }

        // NOVO CAMPO: Para armazenar o valor da comissão calculada (implementado no Controller)
        [Column(TypeName = "decimal(18,2)")]
        public decimal Comissao { get; set; }

        // --- Chaves Estrangeiras e Propriedades de Navegação ---

        // Referência ao Comprador (Cliente)
        public int ClienteId { get; set; }
        [ForeignKey("ClienteId")]
        // O " = null! " é usado para silenciar o warning CS8618 no .NET 6/7/8
        public Cliente Comprador { get; set; } = null!;

        // Referência ao Vendedor
        public int VendedorId { get; set; }
        [ForeignKey("VendedorId")]
        public Vendedor Vendedor { get; set; } = null!;

        // Referência ao Carro
        public int CarroId { get; set; }
        [ForeignKey("CarroId")]
        public Carro Carro { get; set; } = null!;
    }
}