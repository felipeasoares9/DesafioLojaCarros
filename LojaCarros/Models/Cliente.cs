using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LojaCarros.Models
{
    public class Cliente
    {
        // Padrão PascalCase (Id minúsculo)
        public int Id { get; set; }

        [Display(Name = "Nome do Cliente")]
        public string Nome { get; set; }

        [Display(Name = "Data de Nascimento")]
        [DataType(DataType.Date)]
        public DateTime DataNasc { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Telefone")]
        [DataType(DataType.PhoneNumber)]
        public string Telefone { get; set; }

        [Display(Name = "Endereço")]
        public string Endereco { get; set; }

        // Padrão PascalCase (Cpf minúsculo)
        [Display(Name = "CPF")]
        public string Cpf { get; set; }

        // Relação de Navegação
        public ICollection<Nota> Notas { get; set; } = new List<Nota>();
    }
}