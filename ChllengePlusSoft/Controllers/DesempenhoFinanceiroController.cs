using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using ChllengePlusSoft.Models;
using Microsoft.EntityFrameworkCore;

namespace ChllengePlusSoft.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DesempenhoFinanceiroController : ControllerBase
    {
        private readonly string _connectionString;

        public DesempenhoFinanceiroController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDbConnection");
        }

        [HttpGet]
        public async Task<IActionResult> GetDesempenhoFinanceiro()
        {
            var desempenhos = new List<DesempenhoFinanceiroModelSoID>();

            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    SELECT DF.ID,
                           DF.CRESCIMENTO,
                           DF.LUCRO,
                           DF.RECEITA,
                           DF.ID_EMPRESA
                    FROM DESEMPENHO_FINANCEIRO DF";

                using (var command = new OracleCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var desempenho = new DesempenhoFinanceiroModelSoID
                        {
                            Id = reader.GetInt64(0),
                            Crescimento = reader.GetDouble(1),
                            Lucro = reader.GetDouble(2),
                            Receita = reader.GetDouble(3),
                            EmpresaId = reader.GetInt32(4)
                        };
                        desempenhos.Add(desempenho);
                    }
                }
            }
            return Ok(desempenhos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDesempenhoFinanceiro(long id)
        {
            DesempenhoFinanceiroModelSoID? desempenho = null;

            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    SELECT DF.ID,
                           DF.CRESCIMENTO,
                           DF.LUCRO,
                           DF.RECEITA,
                           DF.ID_EMPRESA
                    FROM DESEMPENHO_FINANCEIRO DF
                    WHERE DF.ID = :id";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("id", id));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            desempenho = new DesempenhoFinanceiroModelSoID
                            {
                                Id = reader.GetInt64(0),
                                Crescimento = reader.GetDouble(1),
                                Lucro = reader.GetDouble(2),
                                Receita = reader.GetDouble(3),
                                EmpresaId = reader.GetInt32(4)
                            };
                        }
                    }
                }
            }
            return desempenho != null ? Ok(desempenho) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDesempenhoFinanceiro([FromBody] DesempenhoPostModel novoDesempenho)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    INSERT INTO DESEMPENHO_FINANCEIRO (CRESCIMENTO, LUCRO, RECEITA, ID_EMPRESA)
                    VALUES (:crescimento, :lucro, :receita, :empresaId)";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("crescimento", novoDesempenho.Crescimento));
                    command.Parameters.Add(new OracleParameter("lucro", novoDesempenho.Lucro));
                    command.Parameters.Add(new OracleParameter("receita", novoDesempenho.Receita));
                    command.Parameters.Add(new OracleParameter("empresaId", novoDesempenho.EmpresaId));

                    await command.ExecuteNonQueryAsync();
                }
            }
            return Ok("Desempenho criado com sucesso");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDesempenhoFinanceiro(long id, [FromBody] DesempenhoPostModel desempenhoAtualizado)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    UPDATE DESEMPENHO_FINANCEIRO
                    SET CRESCIMENTO = :crescimento,
                        LUCRO = :lucro,
                        RECEITA = :receita,
                        ID_EMPRESA = :empresaId
                    WHERE ID = :id";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("crescimento", desempenhoAtualizado.Crescimento));
                    command.Parameters.Add(new OracleParameter("lucro", desempenhoAtualizado.Lucro));
                    command.Parameters.Add(new OracleParameter("receita", desempenhoAtualizado.Receita));
                    command.Parameters.Add(new OracleParameter("empresaId", desempenhoAtualizado.EmpresaId));
                    command.Parameters.Add(new OracleParameter("id", id));

                    var rowsAffected = await command.ExecuteNonQueryAsync();

                    return rowsAffected > 0 ? NoContent() : NotFound();
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDesempenhoFinanceiro(long id)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    DELETE FROM DESEMPENHO_FINANCEIRO
                    WHERE ID = :id";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("id", id));

                    var rowsAffected = await command.ExecuteNonQueryAsync();

                    return rowsAffected > 0 ? NoContent() : NotFound();
                }
            }
        }
    }
}