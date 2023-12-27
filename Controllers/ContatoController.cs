using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using WebApiAzureTable.Models;

namespace WebApiAzureTable.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class ContatoController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly string _tableName;

        public ContatoController(IConfiguration configuration)
        {
            _connectionString = configuration.GetValue<string>("SAConnectionString");
            _tableName = configuration.GetValue<string>("AzureTableName");
        }

        private TableClient GetTableClient()
        {
            var serviceClient = new TableServiceClient(_connectionString);
            var TableClient = serviceClient.GetTableClient(_tableName);

            TableClient.CreateIfNotExists();
            return TableClient;
        }

        [HttpPost]
        public IActionResult Criar(Contato contato)
        {
            var TableClient = GetTableClient();
            contato.RowKey = Guid.NewGuid().ToString();
            contato.PartitionKey = contato.RowKey;

            TableClient.UpsertEntity(contato);
            return Ok(contato);
        }

        [HttpPut("{id}")]
        public IActionResult Atualizar(string id, Contato contato)
        {
            var TableClient = GetTableClient();
            var contatoTable = TableClient.GetEntity<Contato>(id, id).Value;

            contatoTable.Nome = contato.Nome;
            contatoTable.Email = contato.Email;
            contatoTable.Telefone = contato.Telefone;

            TableClient.UpsertEntity(contatoTable);
            return Ok();
        }

        [HttpGet("Listar")]
        public IActionResult ObterTodos()
        {
            var TableClient = GetTableClient();
            var contatos = TableClient.Query<Contato>().ToList();
            return Ok(contatos);
        }

        [HttpGet("ObterPorNome/{Nome}")]
        public IActionResult ObterPorNome(string nome)
        {
            var TableClient = GetTableClient();
            var contatos = TableClient.Query<Contato>(x => x.Nome == nome).ToList();
            return Ok(contatos);
        }

        [HttpDelete("{id}")]
        public IActionResult Deletar(string id)
        {
            var tableClient = GetTableClient();
            tableClient.DeleteEntity(id, id);
            return NoContent();
        }
    }
}