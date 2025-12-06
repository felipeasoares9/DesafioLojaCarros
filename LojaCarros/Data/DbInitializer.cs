using LojaCarros.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace LojaCarros.Data
{
    public class DbInitializer
    {
        // Método principal de Seeding (estático para ser chamado no Program.cs)
        public static void Initialize(LojaContext context)
        {
            // Garante que o banco de dados e o esquema estão criados
            context.Database.EnsureCreated();

            // Se o banco já tiver carros, não faz o Seeding novamente.
            if (context.Carros.Any())
            {
                return;
            }

            // --- 1. Dados de Carros ---
            var carros = new Carro[]
            {
                new Carro {
                    Marca = "Fiat",
                    Modelo = "Uno",
                    AnoFabricacao = 2020,
                    AnoModelo = 2021,
                    Chassi = "FIAT12345ABCD",
                    Preco = 45000.00m,
                    Vendido = false
                },
                new Carro {
                    Marca = "Volkswagen",
                    Modelo = "Gol",
                    AnoFabricacao = 2022,
                    AnoModelo = 2022,
                    Chassi = "VWXYZ9876EFGH",
                    Preco = 97999.00m,
                    Vendido = false
                },
            };
            context.Carros.AddRange(carros);
            context.SaveChanges();

            var clientes = new Cliente[]
            {
                new Cliente {
                    Nome = "Maria Silva",
                    DataNasc = DateTime.Parse("1990-01-01"),
                    Email = "maria@example.com",
                    Telefone = "51999998888",
                    Endereco = "Rua das Flores, 100",
                    Cpf = "12345678900"
                },
                new Cliente {
                    Nome = "João Pereira",
                    DataNasc = DateTime.Parse("1985-05-15"),
                    Email = "joao@example.com",
                    Telefone = "51988887777",
                    Endereco = "Avenida Principal, 500",
                    Cpf = "98765432100"
                },
            };
            context.Clientes.AddRange(clientes);
            context.SaveChanges();
        }
    }
}
