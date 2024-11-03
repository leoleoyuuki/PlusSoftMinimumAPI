using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using ChllengePlusSoft.Models;

namespace ChllengePlusSoft.Controllers

{
    [ApiController]
    [Route("[controller]")]
    public class EmpresasController : ControllerBase
    {
        private readonly string _connectionString;

        public EmpresasController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDbConnection");
        }

        [HttpGet]
        public async Task<IActionResult> GetEmpresas()
        {
            var empresas = new List<EmpresasModel>();

            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    SELECT Id, Nome, Tamanho, Setor, Localizacao_Geografica, Numero_Funcionarios, Tipo_Empresa, Cliente
                    FROM Empresas";

                using (var command = new OracleCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var empresa = new EmpresasModel
                        {
                            Id = reader.GetInt32(0),
                            Nome = reader.GetString(1),
                            Tamanho = reader.GetString(2),
                            Setor = reader.GetString(3),
                            LocalizacaoGeografica = reader.GetString(4),
                            NumeroFuncionarios = reader.GetInt32(5),
                            TipoEmpresa = reader.GetString(6),
                            Cliente = reader.GetBoolean(7)
                        };
                        empresas.Add(empresa);
                    }
                }
            }
            return Ok(empresas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmpresa(int id)
        {
            EmpresasModel empresa = null;

            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    SELECT Id, Nome, Tamanho, Setor, Localizacao_Geografica, Numero_Funcionarios, Tipo_Empresa, Cliente
                    FROM Empresas
                    WHERE Id = :empresaId";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("empresaId", id));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            empresa = new EmpresasModel
                            {
                                Id = reader.GetInt32(0),
                                Nome = reader.GetString(1),
                                Tamanho = reader.GetString(2),
                                Setor = reader.GetString(3),
                                LocalizacaoGeografica = reader.GetString(4),
                                NumeroFuncionarios = reader.GetInt32(5),
                                TipoEmpresa = reader.GetString(6),
                                Cliente = reader.GetBoolean(7)
                            };
                        }
                    }
                }
            }
            return empresa != null ? Ok(empresa) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmpresa([FromBody] EmpresaPostModel empresa)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    INSERT INTO Empresas (Nome, Tamanho, Setor, Localizacao_Geografica, Numero_Funcionarios, Tipo_Empresa, Cliente)
                    VALUES (:Nome, :Tamanho, :Setor, :LocalizacaoGeografica, :NumeroFuncionarios, :TipoEmpresa, :Cliente)";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("Nome", empresa.Nome));
                    command.Parameters.Add(new OracleParameter("Tamanho", empresa.Tamanho));
                    command.Parameters.Add(new OracleParameter("Setor", empresa.Setor));
                    command.Parameters.Add(new OracleParameter("LocalizacaoGeografica", empresa.LocalizacaoGeografica));
                    command.Parameters.Add(new OracleParameter("NumeroFuncionarios", empresa.NumeroFuncionarios));
                    command.Parameters.Add(new OracleParameter("TipoEmpresa", empresa.TipoEmpresa));
                    command.Parameters.Add(new OracleParameter("Cliente", empresa.Cliente));

                    await command.ExecuteNonQueryAsync();
                }
            }
            return Ok(empresa);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmpresa(int id, [FromBody] EmpresaPostModel empresaAtualizada)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    UPDATE Empresas
                    SET Nome = :Nome,
                        Tamanho = :Tamanho,
                        Setor = :Setor,
                        Localizacao_Geografica = :LocalizacaoGeografica,
                        Numero_Funcionarios = :NumeroFuncionarios,
                        Tipo_Empresa = :TipoEmpresa,
                        Cliente = :Cliente
                    WHERE Id = :empresaId";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("Nome", empresaAtualizada.Nome));
                    command.Parameters.Add(new OracleParameter("Tamanho", empresaAtualizada.Tamanho));
                    command.Parameters.Add(new OracleParameter("Setor", empresaAtualizada.Setor));
                    command.Parameters.Add(new OracleParameter("LocalizacaoGeografica", empresaAtualizada.LocalizacaoGeografica));
                    command.Parameters.Add(new OracleParameter("NumeroFuncionarios", empresaAtualizada.NumeroFuncionarios));
                    command.Parameters.Add(new OracleParameter("TipoEmpresa", empresaAtualizada.TipoEmpresa));
                    command.Parameters.Add(new OracleParameter("Cliente", empresaAtualizada.Cliente));
                    command.Parameters.Add(new OracleParameter("empresaId", id));

                    var linhasAfetadas = await command.ExecuteNonQueryAsync();

                    if (linhasAfetadas == 0)
                    {
                        return NotFound(); // Retorna 404 se a empresa não for encontrada
                    }
                }
            }
            return Ok(new { message = "Atualização bem-sucedida." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmpresa(int id)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Excluir todos os registros filhos
                await ExcluirRegistrosFilhos(connection, id);

                // Excluir a empresa
                var deleteEmpresaQuery = @"
                    DELETE FROM Empresas
                    WHERE Id = :empresaId";

                using (var deleteEmpresaCommand = new OracleCommand(deleteEmpresaQuery, connection))
                {
                    deleteEmpresaCommand.Parameters.Add(new OracleParameter("empresaId", id));

                    var linhasAfetadas = await deleteEmpresaCommand.ExecuteNonQueryAsync();

                    if (linhasAfetadas == 0)
                    {
                        return NotFound(); // Retorna 404 se a empresa não for encontrada
                    }
                }
            }
            return Ok(new { message = "Exclusão bem-sucedida." }); // Retorna 200 OK com a mensagem de sucesso
        }

        static async Task ExcluirRegistrosFilhos(OracleConnection connection, int empresaId)
        {
            var deleteTendenciasQuery = @"
                DELETE FROM TENDENCIAS_GASTOS
                WHERE ID_EMPRESA = :empresaId";

            using (var deleteTendenciasCommand = new OracleCommand(deleteTendenciasQuery, connection))
            {
                deleteTendenciasCommand.Parameters.Add(new OracleParameter("empresaId", empresaId));
                await deleteTendenciasCommand.ExecuteNonQueryAsync();
            }

            // Repita para outras tabelas relacionadas
            var deleteDesempenhosQuery = @"
                DELETE FROM DESEMPENHO_FINANCEIRO
                WHERE ID_EMPRESA = :empresaId";

            using (var deleteDesempenhosCommand = new OracleCommand(deleteDesempenhosQuery, connection))
            {
                deleteDesempenhosCommand.Parameters.Add(new OracleParameter("empresaId", empresaId));
                await deleteDesempenhosCommand.ExecuteNonQueryAsync();
            }

            var deleteHistoricosQuery = @"
                DELETE FROM HISTORICO_INTERACOES
                WHERE ID_EMPRESA = :empresaId";

            using (var deleteHistoricosCommand = new OracleCommand(deleteHistoricosQuery, connection))
            {
                deleteHistoricosCommand.Parameters.Add(new OracleParameter("empresaId", empresaId));
                await deleteHistoricosCommand.ExecuteNonQueryAsync();
            }

            var deleteComportamentosQuery = @"
                DELETE FROM COMPORTAMENTO_NEGOCIOS
                WHERE ID_EMPRESA = :empresaId";

            using (var deleteComportamentosCommand = new OracleCommand(deleteComportamentosQuery, connection))
            {
                deleteComportamentosCommand.Parameters.Add(new OracleParameter("empresaId", empresaId));
                await deleteComportamentosCommand.ExecuteNonQueryAsync();
            }
        }
    }
}