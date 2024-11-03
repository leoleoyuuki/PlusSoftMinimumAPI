using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using ChllengePlusSoft.Models;

namespace ChllengePlusSoft.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TendenciaGastosController : ControllerBase
    {
        private readonly string _connectionString;

        public TendenciaGastosController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDbConnection");
        }

        [HttpGet]
        public async Task<IActionResult> GetTendenciaGastos()
        {
            var tendencias = new List<TendenciaGastosModel>();

            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    SELECT TG.ID,
                           TG.ANO,
                           TG.GASTO_MARKETING,
                           TG.GASTO_AUTOMACAO,
                           TG.ID_EMPRESA
                    FROM TENDENCIAS_GASTOS TG";

                using (var command = new OracleCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var tendencia = new TendenciaGastosModel
                        {
                            Id = reader.GetInt64(0),
                            Ano = reader.GetInt32(1),
                            GastoMarketing = reader.GetDouble(2),
                            GastoAutomacao = reader.GetDouble(3),
                            EmpresaId = reader.GetInt64(4)
                        };
                        tendencias.Add(tendencia);
                    }
                }
            }
            return Ok(tendencias);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTendenciaGasto(long id)
        {
            TendenciaGastosModel? tendencia = null;

            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    SELECT TG.ID,
                           TG.ANO,
                           TG.GASTO_MARKETING,
                           TG.GASTO_AUTOMACAO,
                           TG.ID_EMPRESA
                    FROM TENDENCIAS_GASTOS TG
                    WHERE TG.ID = :id";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("id", id));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            tendencia = new TendenciaGastosModel
                            {
                                Id = reader.GetInt64(0),
                                Ano = reader.GetInt32(1),
                                GastoMarketing = reader.GetDouble(2),
                                GastoAutomacao = reader.GetDouble(3),
                                EmpresaId = reader.GetInt64(4)
                            };
                        }
                    }
                }
            }
            return tendencia != null ? Ok(tendencia) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTendenciaGasto([FromBody] TendenciaGastosPostModel novaTendencia)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    INSERT INTO TENDENCIAS_GASTOS (ANO, GASTO_MARKETING, GASTO_AUTOMACAO, ID_EMPRESA)
                    VALUES (:ano, :gastoMarketing, :gastoAutomacao, :empresaId)";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("ano", novaTendencia.Ano));
                    command.Parameters.Add(new OracleParameter("gastoMarketing", novaTendencia.GastoMarketing));
                    command.Parameters.Add(new OracleParameter("gastoAutomacao", novaTendencia.GastoAutomacao));
                    command.Parameters.Add(new OracleParameter("empresaId", novaTendencia.EmpresaId));

                    await command.ExecuteNonQueryAsync();
                }
            }
            return Ok("Tendência de gastos criada com sucesso");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTendenciaGasto(long id, [FromBody] TendenciaGastosPostModel tendenciaAtualizada)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    UPDATE TENDENCIAS_GASTOS
                    SET ANO = :ano,
                        GASTO_MARKETING = :gastoMarketing,
                        GASTO_AUTOMACAO = :gastoAutomacao,
                        ID_EMPRESA = :empresaId
                    WHERE ID = :id";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("ano", tendenciaAtualizada.Ano));
                    command.Parameters.Add(new OracleParameter("gastoMarketing", tendenciaAtualizada.GastoMarketing));
                    command.Parameters.Add(new OracleParameter("gastoAutomacao", tendenciaAtualizada.GastoAutomacao));
                    command.Parameters.Add(new OracleParameter("empresaId", tendenciaAtualizada.EmpresaId));
                    command.Parameters.Add(new OracleParameter("id", id));

                    var rowsAffected = await command.ExecuteNonQueryAsync();

                    return rowsAffected > 0 ? NoContent() : NotFound();
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTendenciaGasto(long id)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    DELETE FROM TENDENCIAS_GASTOS
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