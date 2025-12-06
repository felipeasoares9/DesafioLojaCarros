using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LojaCarros.Data;
using LojaCarros.Models;
using System.Globalization;

namespace LojaCarros.Controllers
{
    public class CarrosController : Controller
    {
        private readonly LojaContext _context;
        // Caracteres permitidos para a parte randômica do Chassi (VIN Style)
        private const string ChassiChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public CarrosController(LojaContext context)
        {
            _context = context;
        }

        // NOVO MÉTODO: Gera um Chassi no Padrão VIN Básico (BR + Ano + 12 Alfanuméricos)
        private string GenerateUniqueChassi()
        {
            // 1. Prefixo Fixo: BR (Brasil/Marca)
            string prefixo = "BR";

            // 2. Último dígito do Ano Atual (Assumindo que é o ano de entrada/modelo)
            string anoModeloDigito = DateTime.Now.ToString("yy")[1].ToString(); // Ex: 5 (para 2025)

            // 3. Gera 12 caracteres alfanuméricos randômicos para unicidade
            var random = new Random();
            var randomSuffix = new string(Enumerable.Repeat(ChassiChars, 12)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            string chassi = $"{prefixo}{anoModeloDigito}{randomSuffix}";

            // Verifica se o Chassi gerado já existe no banco de dados
            while (_context.Carros.Any(c => c.Chassi == chassi))
            {
                // Se o chassi já existe (chance mínima, mas possível), gera um novo sufixo
                randomSuffix = new string(Enumerable.Repeat(ChassiChars, 12)
                   .Select(s => s[random.Next(s.Length)]).ToArray());
                chassi = $"{prefixo}{anoModeloDigito}{randomSuffix}";
            }

            return chassi;
        }

        // GET: Carros
        public async Task<IActionResult> Index()
        {
            return View(await _context.Carros.ToListAsync());
        }

        // GET: Carros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carro = await _context.Carros
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carro == null)
            {
                return NotFound();
            }

            return View(carro);
        }

        // GET: Carros/Create
        public IActionResult Create()
        {
            // CRUCIAL: Cria uma nova instância de Carro e pré-preenche o Chassi
            var novoCarro = new Carro
            {
                Chassi = GenerateUniqueChassi(),
                Vendido = false // Garante que o padrão é falso ao criar
            };
            return View(novoCarro);
        }

        // POST: Carros/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Marca,Modelo,AnoFabricacao,AnoModelo,Chassi,Preco,Vendido")] Carro carro)
        {
            // Verifica a unicidade do Chassi (segurança extra)
            if (_context.Carros.Any(c => c.Chassi == carro.Chassi))
            {
                ModelState.AddModelError("Chassi", "O número de Chassi gerado já existe. Por favor, tente novamente.");
                // Se falhar na unicidade, regera um novo Chassi para a próxima tentativa
                carro.Chassi = GenerateUniqueChassi();
            }

            // Note: Adicionei Marca, AnoFabricacao e AnoModelo ao Bind.
            if (ModelState.IsValid)
            {
                _context.Add(carro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(carro);
        }

        // GET: Carros/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carro = await _context.Carros.FindAsync(id);
            if (carro == null)
            {
                return NotFound();
            }
            return View(carro);
        }

        // POST: Carros/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Marca,Modelo,AnoFabricacao,AnoModelo,Chassi,Preco,Vendido")] Carro carro)
        {
            if (id != carro.Id)
            {
                return NotFound();
            }

            // Garante que o Chassi não foi alterado (se fosse editável) para um que já existe em outro carro
            var chassiCheck = await _context.Carros.AsNoTracking().FirstOrDefaultAsync(c => c.Chassi == carro.Chassi && c.Id != carro.Id);
            if (chassiCheck != null)
            {
                ModelState.AddModelError("Chassi", "O número de Chassi já pertence a outro carro.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarroExists(carro.Id))
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
            return View(carro);
        }

        // GET: Carros/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carro = await _context.Carros
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carro == null)
            {
                return NotFound();
            }

            return View(carro);
        }

        // POST: Carros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var carro = await _context.Carros.FindAsync(id);
            if (carro != null)
            {
                _context.Carros.Remove(carro);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarroExists(int id)
        {
            return _context.Carros.Any(e => e.Id == id);
        }
    }
}