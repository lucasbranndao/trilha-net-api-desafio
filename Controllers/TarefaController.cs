using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrilhaApiDesafio.Context;
using TrilhaApiDesafio.Models;
using System;
using System.Linq;

namespace TrilhaApiDesafio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TarefaController : ControllerBase
    {
        private readonly OrganizadorContext _context;

        public TarefaController(OrganizadorContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult ObterPorId(int id)
        {
            // Buscar por Id
            var tarefa = _context.Tarefas.Find(id);

            // Retornar 404 se não encontrar
            if (tarefa == null)
                return NotFound();

            // Retornar 200 com a tarefa
            return Ok(tarefa);
        }

        [HttpGet("ObterTodos")]
        public IActionResult ObterTodos()
        {
            // Buscar todas as tarefas
            var tarefas = _context.Tarefas.ToList();
            return Ok(tarefas);
        }

        [HttpGet("ObterPorTitulo")]
        public IActionResult ObterPorTitulo([FromQuery] string titulo)
        {
            // Buscar tarefas que contenham o título (case-insensitive opcional)
            if (string.IsNullOrWhiteSpace(titulo))
                return BadRequest(new { Erro = "Informe um título para pesquisa." });

            var tarefas = _context
                .Tarefas
                .Where(x => EF.Functions.Like(x.Titulo, $"%{titulo}%"))
                .ToList();

            return Ok(tarefas);
        }

        [HttpGet("ObterPorData")]
        public IActionResult ObterPorData([FromQuery] DateTime data)
        {
            var tarefa = _context.Tarefas.Where(x => x.Data.Date == data.Date).ToList();
            return Ok(tarefa);
        }

        [HttpGet("ObterPorStatus")]
        public IActionResult ObterPorStatus([FromQuery] EnumStatusTarefa status)
        {
            var tarefa = _context.Tarefas.Where(x => x.Status == status).ToList();
            return Ok(tarefa);
        }

        [HttpPost]
        public IActionResult Criar([FromBody] Tarefa tarefa)
        {
            if (tarefa == null)
                return BadRequest();

            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            // Adicionar e salvar
            _context.Tarefas.Add(tarefa);
            _context.SaveChanges();

            return CreatedAtAction(nameof(ObterPorId), new { id = tarefa.Id }, tarefa);
        }

        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, [FromBody] Tarefa tarefa)
        {
            var tarefaBanco = _context.Tarefas.Find(id);

            if (tarefaBanco == null)
                return NotFound();

            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            // Atualizar campos
            tarefaBanco.Titulo = tarefa.Titulo;
            tarefaBanco.Descricao = tarefa.Descricao;
            tarefaBanco.Data = tarefa.Data;
            tarefaBanco.Status = tarefa.Status;

            // Marcar como modificado e salvar
            _context.Tarefas.Update(tarefaBanco);
            _context.SaveChanges();

            return Ok(tarefaBanco);
        }

        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            var tarefaBanco = _context.Tarefas.Find(id);

            if (tarefaBanco == null)
                return NotFound();

            // Remover e salvar
            _context.Tarefas.Remove(tarefaBanco);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
