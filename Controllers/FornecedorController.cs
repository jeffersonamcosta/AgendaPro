using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AgendaPro.Models;
using Microsoft.IdentityModel.Tokens;

namespace AgendaPro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FornecedorController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public FornecedorController(ApplicationDbContext db)
        {
            _db = db;
        }

        // POST: api/fornecedor/novo
        [HttpPost("novo")]
        public IActionResult Criar([FromBody] Fornecedor fornecedor)
        {
            try
            {
              
                var servicos = fornecedor.Servicos?.ToList() ?? new List<Servico>(); 
                fornecedor.Servicos = new List<Servico>(); 
                _db.Fornecedores.Add(fornecedor);
                _db.SaveChanges();


                foreach (var s in servicos)
                {
                    var servico = new Servico
                    {
                        FornecedorId = fornecedor.Id,
                        Nome = s.Nome,
                        Preco = s.Preco,
                    };
                    _db.Servicos.Add(servico);
                }

                _db.SaveChanges();

                return CreatedAtAction(nameof(GetPorId), new { id = fornecedor.Id }, fornecedor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao criar fornecedor: {ex.Message}");
            }
        }


        // GET: api/fornecedor/lista
        [HttpGet("lista")]
        public IActionResult GetTodos()
        {
            try
            {
                var fornecedores = _db.Fornecedores
                    
                    .Select(f => new
                    {
                        f.Id,
                        f.Ativo,
                        f.RazaoSocial,
                        f.CNPJ,
                        f.Telefone,
                        f.Email,
                        Servicos = _db.Servicos.Where(s => s.FornecedorId == f.Id && s.Ativo)
                        .Select(s => new
                        {
                            s.Id,
                            s.Nome,
                            s.Preco
                        })
                        .ToList()

                    })
                    .ToList();

                return Ok(fornecedores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar fornecedores: {ex.Message}");
            }
        }

        // GET: api/fornecedor/lista/{id}
        [HttpGet("lista/{id}")]
        public IActionResult GetPorId(int id)
        {
            try
            {
                var fornecedor = _db.Fornecedores
                    .Where(f => f.Id== id)
                    .Select(f => new
                    {
                        f.Id,
                        f.Ativo,
                        f.RazaoSocial,
                        f.CNPJ,
                        f.Telefone,
                        f.Email,                       
                        Servicos = _db.Servicos.Where(s => s.FornecedorId == f.Id && s.Ativo)
                        .Select(s => new
                        {
                            s.Id,
                            s.Nome,
                            s.Preco
                        })
                        .ToList()

                    })
                    .ToList();
                
                return Ok(new { fornecedor });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar fornecedor: {ex.Message}");
            }
        }


        // PUT: api/fornecedor/atualiza/{id}
        [HttpPut("atualiza/{id}")]
        public IActionResult Atualizar(int id, [FromBody] Fornecedor fornecedorAtualizado)
        {
            try
            {
                var fornecedor = _db.Fornecedores.Find(id);              

                fornecedor.Id = fornecedorAtualizado.Id;
                fornecedor.RazaoSocial = fornecedorAtualizado.RazaoSocial;
                fornecedor.CNPJ = fornecedorAtualizado.CNPJ;
                fornecedor.Telefone = fornecedorAtualizado.Telefone;
                fornecedor.Email = fornecedorAtualizado.Email;
                fornecedor.Ativo = fornecedorAtualizado.Ativo;

                _db.SaveChanges();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar fornecedor: {ex.Message}");
            }
        }

        // DELETE: api/fornecedor/del/{id}
        [HttpDelete("del/{id}")]
        public IActionResult Deletar(int id)
        {
            try
            {
                var fornecedor = _db.Fornecedores.Find(id);
                if (fornecedor == null || !fornecedor.Ativo) return NotFound();

                fornecedor.Ativo = false;

                var servicos = _db.Servicos.Where(s => s.FornecedorId == id).ToList();
                foreach (var s in servicos)
                    s.Ativo = false;

                _db.SaveChanges();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir fornecedor: {ex.Message}");
            }
        }


        // Post: api/fornecedor/pesquisar
        [HttpPost("pesquisar")]
        public IActionResult Pesquisar([FromBody] Fornecedor filtro)
        {
            try
            {
                var query = _db.Fornecedores.AsQueryable();

                // Nome
                if (!string.IsNullOrEmpty(filtro.RazaoSocial))
                    query = query.Where(f => f.RazaoSocial.Contains(filtro.RazaoSocial));

                // Documento
                if (!string.IsNullOrEmpty(filtro.CNPJ))
                    query = query.Where(f => f.CNPJ.Contains(filtro.CNPJ));

                if (!string.IsNullOrEmpty(filtro.Telefone))
                    query = query.Where(f => f.Telefone.Contains(filtro.Telefone));

                if (!string.IsNullOrEmpty(filtro.Email))
                    query = query.Where(f => f.Email.Contains(filtro.Email));

                if (filtro.Ativo == true || filtro.Ativo == false)
                    query = query.Where(f => f.Ativo == filtro.Ativo);

                query = query.Select(f => new Fornecedor
                {
                    Id = f.Id,
                    Ativo = f.Ativo,
                    RazaoSocial = f.RazaoSocial,
                    CNPJ = f.CNPJ,
                    Telefone = f.Telefone,
                    Email = f.Email,
                    Servicos = _db.Servicos.Where(s => s.FornecedorId == f.Id && s.Ativo).ToList()

                });


                return Ok(query.ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao pesquisar: {ex.Message}");
            }
        }

    }
}
