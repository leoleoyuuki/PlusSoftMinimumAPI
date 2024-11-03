using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using ChllengePlusSoft.Models;

namespace ChllengePlusSoft.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HistoricoInteracoesController : ControllerBase
    {
        private readonly string _connectionString;

        public HistoricoInteracoesController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDbConnection");
        }

        [HttpGet]
        public async Task<IActionResult> GetHistoricoInteracoes()
        {
            var historicos = new List<HistoricoInteracoes>();

            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    SELECT HI.ID,
                           HI.PROPOSTA_NEGOCIO,
                           HI.CONTRATO_ASSINADO,
                           HI.FEEDBACK_SERVICOS_PRODUTOS,
                           HI.ID_EMPRESA
                    FROM HISTORICO_INTERACOES HI";

                using (var command = new OracleCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var historico = new HistoricoInteracoes
                        {
                            Id = reader.GetInt32(0),
                            PropostaNegocio = reader.GetString(1),
                            ContratoAssinado = reader.GetString(2),
                            FeedbackServicosProdutos = reader.GetString(3),
                            EmpresaId = reader.GetInt64(4)
                        };
                        historicos.Add(historico);
                    }
                }
            }
            return Ok(historicos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHistoricoInteracao(int id)
        {
            HistoricoInteracoes? historico = null;

            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    SELECT HI.ID,
                           HI.PROPOSTA_NEGOCIO,
                           HI.CONTRATO_ASSINADO,
                           HI.FEEDBACK_SERVICOS_PRODUTOS,
                           HI.ID_EMPRESA
                    FROM HISTORICO_INTERACOES HI
                    WHERE HI.ID = :id";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("id", id));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            historico = new HistoricoInteracoes
                            {
                                Id = reader.GetInt32(0),
                                PropostaNegocio = reader.GetString(1),
                                ContratoAssinado = reader.GetString(2),
                                FeedbackServicosProdutos = reader.GetString(3),
                                EmpresaId = reader.GetInt64(4)
                            };
                        }
                    }
                }
            }
            return historico != null ? Ok(historico) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateHistoricoInteracao([FromBody] HistoricoInteracoesPostModel novoHistorico)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    INSERT INTO HISTORICO_INTERACOES (PROPOSTA_NEGOCIO, CONTRATO_ASSINADO, FEEDBACK_SERVICOS_PRODUTOS, ID_EMPRESA)
                    VALUES (:propostaNegocio, :contratoAssinado, :feedbackServicosProdutos, :empresaId)";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("propostaNegocio", novoHistorico.PropostaNegocio));
                    command.Parameters.Add(new OracleParameter("contratoAss inado", novoHistorico.ContratoAssinado));
                    command.Parameters.Add(new OracleParameter("feedbackServicosProdutos", novoHistorico.FeedbackServicosProdutos));
                    command.Parameters.Add(new OracleParameter("empresaId", novoHistorico.EmpresaId));

                    await command.ExecuteNonQueryAsync();
                }
            }
            return Ok("Histórico de Interação criado com sucesso");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHistoricoInteracao(int id, [FromBody] HistoricoInteracoesPostModel historicoAtualizado)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    UPDATE HISTORICO_INTERACOES
                    SET PROPOSTA_NEGOCIO = :propostaNegocio,
                        CONTRATO_ASSINADO = :contratoAssinado,
                        FEEDBACK_SERVICOS_PRODUTOS = :feedbackServicosProdutos,
                        ID_EMPRESA = :empresaId
                    WHERE ID = :id";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("propostaNegocio", historicoAtualizado.PropostaNegocio));
                    command.Parameters.Add(new OracleParameter("contratoAssinado", historicoAtualizado.ContratoAssinado));
                    command.Parameters.Add(new OracleParameter("feedbackServicosProdutos", historicoAtualizado.FeedbackServicosProdutos));
                    command.Parameters.Add(new OracleParameter("empresaId", historicoAtualizado.EmpresaId));
                    command.Parameters.Add(new OracleParameter("id", id));

                    var rowsAffected = await command.ExecuteNonQueryAsync();

                    return rowsAffected > 0 ? NoContent() : NotFound();
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHistoricoInteracao(int id)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    DELETE FROM HISTORICO_INTERACOES
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