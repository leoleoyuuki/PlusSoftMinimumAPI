using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using ChllengePlusSoft.Models;

namespace ChllengePlusSoft.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ComportamentoNegociosController : ControllerBase
    {
        private readonly string _connectionString;

        public ComportamentoNegociosController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDbConnection");
        }

        [HttpGet]
        public async Task<IActionResult> GetComportamentoNegocios()
        {
            var comportamentos = new List<ComportamentoNegociosModel>();

            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    SELECT CN.ID,
                           CN.INTERACOES_PLATAFORMA,
                           CN.FREQUENCIA_USO,
                           CN.FEEDBACK,
                           CN.USO_RECURSOS_ESPECIFICOS,
                           CN.ID_EMPRESA
                    FROM COMPORTAMENTO_NEGOCIOS CN";

                using (var command = new OracleCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var comportamento = new ComportamentoNegociosModel
                        {
                            Id = reader.GetInt64(0),
                            InteracoesPlataforma = reader.GetInt64(1),
                            FrequenciaUso = reader.GetInt64(2),
                            Feedback = reader.GetString(3),
                            UsoRecursosEspecificos = reader.IsDBNull(4) ? null : reader.GetString(4),
                            Empresa = await ObterEmpresaPorIdAsync(reader.GetInt64(5), connection)
                        };
                        comportamentos.Add(comportamento);
                    }
                }
            }
            return Ok(comportamentos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetComportamentoNegocio(long id)
        {
            ComportamentoNegociosModel? comportamento = null;

            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    SELECT CN.ID,
                           CN.INTERACOES_PLATAFORMA,
                           CN.FREQUENCIA_USO,
                           CN.FEEDBACK,
                           CN.USO_RECURSOS_ESPECIFICOS,
                           CN.ID_EMPRESA
                    FROM COMPORTAMENTO_NEGOCIOS CN
                    WHERE CN.ID = :id";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("id", id));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            comportamento = new ComportamentoNegociosModel
                            {
                                Id = reader.GetInt64(0),
                                InteracoesPlataforma = reader.GetInt64(1),
                                FrequenciaUso = reader.GetInt64(2),
                                Feedback = reader.GetString(3),
                                UsoRecursosEspecificos = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Empresa = await ObterEmpresaPorIdAsync(reader.GetInt64(5), connection)
                            };
                        }
                    }
                }
            }
            return comportamento != null ? Ok(comportamento) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateComportamentoNegocio([FromBody] ComportamentoNegociosModelSoID novoComportamento)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    INSERT INTO COMPORTAMENTO_NEGOCIOS 
                        (INTERACOES_PLATAFORMA, FREQUENCIA_USO, FEEDBACK, USO_RECURSOS_ESPECIFICOS, ID_EMP RESA)
                    VALUES
                        (:InteracoesPlataforma, :FrequenciaUso, :Feedback, :UsoRecursosEspecificos, :IdEmpresa)";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("InteracoesPlataforma", novoComportamento.InteracoesPlataforma));
                    command.Parameters.Add(new OracleParameter("FrequenciaUso", novoComportamento.FrequenciaUso));
                    command.Parameters.Add(new OracleParameter("Feedback", novoComportamento.Feedback));
                    command.Parameters.Add(new OracleParameter("UsoRecursosEspecificos", novoComportamento.UsoRecursosEspecificos ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("IdEmpresa", novoComportamento?.EmpresaId)); // Espera que o ID da Empresa esteja preenchido, mas não será inserida

                    await command.ExecuteNonQueryAsync();
                }
            }
            return Created($"/comportamento_negocios/{novoComportamento.Id}", novoComportamento);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComportamentoNegocio(long id, [FromBody] ComportamentoNegociosModelSoID comportamentoAtualizado)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    UPDATE COMPORTAMENTO_NEGOCIOS
                    SET INTERACOES_PLATAFORMA = :InteracoesPlataforma,
                        FREQUENCIA_USO = :FrequenciaUso,
                        FEEDBACK = :Feedback,
                        USO_RECURSOS_ESPECIFICOS = :UsoRecursosEspecificos,
                        ID_EMPRESA = :IdEmpresa
                    WHERE ID = :Id";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("InteracoesPlataforma", comportamentoAtualizado.InteracoesPlataforma));
                    command.Parameters.Add(new OracleParameter("FrequenciaUso", comportamentoAtualizado.FrequenciaUso));
                    command.Parameters.Add(new OracleParameter("Feedback", comportamentoAtualizado.Feedback));
                    command.Parameters.Add(new OracleParameter("UsoRecursosEspecificos", comportamentoAtualizado.UsoRecursosEspecificos ?? (object)DBNull.Value));
                    command.Parameters.Add(new OracleParameter("IdEmpresa", comportamentoAtualizado?.EmpresaId));
                    command.Parameters.Add(new OracleParameter("Id", id));

                    var linhasAfetadas = await command.ExecuteNonQueryAsync();

                    if (linhasAfetadas == 0)
                    {
                        return NotFound(); // Retorna 404 se o comportamento não for encontrado
                    }
                }
            }
            return Ok(new { message = "Atualização bem-sucedida." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComportamentoNegocio(long id)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    DELETE FROM COMPORTAMENTO_NEGOCIOS
                    WHERE ID = :Id";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("Id", id));

                    var linhasAfetadas = await command.ExecuteNonQueryAsync();

                    if (linhasAfetadas == 0)
                    {
                        return NotFound(); // Retorna 404 se o comportamento não for encontrado
                    }
                }
            }
            return Ok(new { message = "Exclusão bem-sucedida." });
        }

        async Task<EmpresaModelComId?> ObterEmpresaPorIdAsync(long empresaId, OracleConnection connection)
        {
            EmpresaModelComId? empresa = null;

            var queryEmpresa = @"
                SELECT ID,
                       NOME,
                       TAMANHO,
                       SETOR,
                       LOCALIZACAO_GEOGRAFICA,
                       NUMERO_FUNCIONARIOS,
                       TIPO_EMPRESA,
                       CLIENTE
                FROM EMPRESAS E
                WHERE E.ID = :empresaId";

            using (var commandEmpresa = new OracleCommand(queryEmpresa, connection))
            {
                commandEmpresa.Parameters.Add(new OracleParameter("empresaId", empresaId));

                using (var readerEmpresa = await commandEmpresa.ExecuteReaderAsync())
                {
                    if (await readerEmpresa.ReadAsync())
                    {
                        empresa = new EmpresaModelComId
                        {
                            Id = readerEmpresa.GetInt32(0),
                            Nome = readerEmpresa.GetString(1),
                            Tamanho = readerEmpresa.GetString(2),
                            Setor = readerEmpresa.GetString(3),
                            LocalizacaoGeografica = readerEmpresa.GetString(4),
                            NumeroFuncionarios = readerEmpresa.GetInt32(5),
                            TipoEmpresa = readerEmpresa.GetString(6),
                            Cliente = readerEmpresa.GetInt32(7)
                        };
                    }
                }
            }
            return empresa;
        }
    }
}