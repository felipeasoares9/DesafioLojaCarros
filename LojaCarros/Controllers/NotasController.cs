using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LojaCarros.Data;
using LojaCarros.Models;
using System.Text.Json;

namespace LojaCarros.Controllers
{
    public class NotasController : Controller
    {
        private readonly LojaContext _context;
        // Taxa de comissão fixa em 5%
        private const decimal TaxaComissao = 0.05m;

        public NotasController(LojaContext context)
        {
            _context = context;
        }

        private string GenerateUniqueNoteNumber()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            Random rnd = new Random();
            int randomPart = rnd.Next(100, 999);
            return $"{timestamp}-{randomPart}";
        }

        // 1. GET: Notas (Listar)
        public async Task<IActionResult> Index()
        {
            var lojaContext = _context.Notas
                .Include(n => n.Carro)
                .Include(n => n.Comprador)
                .Include(n => n.Vendedor);

            return View(await lojaContext.ToListAsync());
        }

        // 2. GET: Notas/Details/5 (Detalhes)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nota = await _context.Notas
                .Include(n => n.Carro)
                .Include(n => n.Comprador)
                .Include(n => n.Vendedor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (nota == null)
            {
                return NotFound();
            }

            return View(nota);
        }

        // 3. GET: Notas/Create (Exibir formulário de criação)
        public IActionResult Create()
        {
            // 1. GERAÇÃO DO NÚMERO DA NOTA E DATA PADRÃO
            var numeroNota = GenerateUniqueNoteNumber();

            // 2. Cria um objeto Nota com a data de hoje e o número gerado
            var nota = new Nota
            {
                DataEmissao = DateTime.Today,
                Numero = numeroNota
            };

            // 3. FILTRO DE ESTOQUE E PREPARAÇÃO DOS PREÇOS PARA JAVASCRIPT
            var carrosDisponiveis = _context.Carros
                .Where(c => c.Vendido == false)
                .Select(c => new { c.Id, c.Modelo, c.Preco }) // Inclui o Preco
                .ToList();

            // Mapeamento para o SelectList na View
            ViewData["CarroId"] = new SelectList(carrosDisponiveis.Select(c => new {
                c.Id,
                // Exibição mais detalhada para o dropdown
                ModeloComPreco = $"{c.Modelo} (R$ {c.Preco:N2})"
            }), "Id", "ModeloComPreco");

            // Envia o mapeamento de ID do carro para PREÇO para o JavaScript
            ViewData["CarroPrecosJson"] = JsonSerializer.Serialize(carrosDisponiveis.ToDictionary(c => c.Id, c => c.Preco));

            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nome");
            ViewData["VendedorId"] = new SelectList(_context.Vendedores, "Id", "Nome");

            return View(nota); // Passa a nota com DataEmissao e Numero pré-preenchidos
        }

        // 4. POST: Notas/Create (Salvar nova nota)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Numero,DataEmissao,Garantia,ValorVenda,ClienteId,VendedorId,CarroId")] Nota nota)
        {
            // Remoção da Validação de Propriedades de Navegação
            ModelState.Remove("Comprador");
            ModelState.Remove("Vendedor");
            ModelState.Remove("Carro");

            // IMPLEMENTAÇÃO DA LÓGICA DE COMISSÃO
            nota.Comissao = nota.ValorVenda * TaxaComissao;

            if (ModelState.IsValid)
            {
                _context.Add(nota);
                await _context.SaveChangesAsync();

                // MARCAR CARRO COMO VENDIDO (CONTROLE DE ESTOQUE)
                var carroVendido = await _context.Carros.FindAsync(nota.CarroId);
                if (carroVendido != null)
                {
                    carroVendido.Vendido = true;
                    _context.Update(carroVendido);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            // Se falhar (ModelState.IsValid é falso): Recarrega Dropdowns e JSON de preços
            var carrosDisponiveis = _context.Carros
                .Where(c => c.Vendido == false || c.Id == nota.CarroId)
                .Select(c => new { c.Id, c.Modelo, c.Preco })
                .ToList();

            ViewData["CarroId"] = new SelectList(carrosDisponiveis.Select(c => new {
                c.Id,
                ModeloComPreco = $"{c.Modelo} (R$ {c.Preco:N2})"
            }), "Id", "ModeloComPreco", nota.CarroId);

            ViewData["CarroPrecosJson"] = JsonSerializer.Serialize(carrosDisponiveis.ToDictionary(c => c.Id, c => c.Preco));

            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nome", nota.ClienteId);
            ViewData["VendedorId"] = new SelectList(_context.Vendedores, "Id", "Nome", nota.VendedorId);

            return View(nota);
        }

        // 5. GET: Notas/Edit/5 (Exibir formulário de edição)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // CORREÇÃO: Inclui Carro, Cliente e Vendedor para que Model.Carro.Modelo funcione no Edit.cshtml
            var nota = await _context.Notas
                .Include(n => n.Carro)
                .Include(n => n.Comprador)
                .Include(n => n.Vendedor)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (nota == null)
            {
                return NotFound();
            }

            // Não carregamos o CarroId na ViewData, pois ele é somente leitura na View.
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nome", nota.ClienteId);
            ViewData["VendedorId"] = new SelectList(_context.Vendedores, "Id", "Nome", nota.VendedorId);

            return View(nota);
        }

        // 6. POST: Notas/Edit/5 (Salvar edição)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Numero,DataEmissao,Garantia,ValorVenda,ClienteId,VendedorId,CarroId,Comissao")] Nota novaNota)
        {
            if (id != novaNota.Id)
            {
                return NotFound();
            }

            // Pega a nota antiga (sem rastreamento) para fins de comparação de estoque
            var notaAntiga = await _context.Notas.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);

            ModelState.Remove("Comprador");
            ModelState.Remove("Vendedor");
            ModelState.Remove("Carro");

            novaNota.Comissao = novaNota.ValorVenda * TaxaComissao;

            if (ModelState.IsValid)
            {
                if (notaAntiga != null && notaAntiga.CarroId != novaNota.CarroId)
                {
                    var carroAntigo = await _context.Carros.FindAsync(notaAntiga.CarroId);
                    if (carroAntigo != null)
                    {
                        carroAntigo.Vendido = false;
                        _context.Update(carroAntigo);
                    }

                    // 2. Marca o carro NOVO como VENDIDO
                    var carroNovo = await _context.Carros.FindAsync(novaNota.CarroId);
                    if (carroNovo != null)
                    {
                        carroNovo.Vendido = true;
                        _context.Update(carroNovo);
                    }
                    await _context.SaveChangesAsync(); // Salva as mudanças de estoque
                }

                try
                {
                    _context.Update(novaNota);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotaExists(novaNota.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            var notaComCarro = await _context.Notas.Include(n => n.Carro).AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);
            if (notaComCarro != null)
            {
                novaNota.Carro = notaComCarro.Carro;
            }

            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nome", novaNota.ClienteId);
            ViewData["VendedorId"] = new SelectList(_context.Vendedores, "Id", "Nome", novaNota.VendedorId);

            return View(novaNota);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nota = await _context.Notas
                .Include(n => n.Carro)
                .Include(n => n.Comprador)
                .Include(n => n.Vendedor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (nota == null)
            {
                return NotFound();
            }

            return View(nota);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nota = await _context.Notas.FindAsync(id);
            if (nota != null)
            {
                var carroDisponivel = await _context.Carros.FindAsync(nota.CarroId);
                if (carroDisponivel != null)
                {
                    carroDisponivel.Vendido = false;
                    _context.Update(carroDisponivel);
                    await _context.SaveChangesAsync();
                }

                _context.Notas.Remove(nota);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotaExists(int id)
        {
            return _context.Notas.Any(e => e.Id == id);
        }
    }
}
