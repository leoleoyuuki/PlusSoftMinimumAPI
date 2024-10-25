using ChllengePlusSoft.Models;
using Oracle.ManagedDataAccess.Client;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("OracleDbConnection");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Api empresas clintes PlusSoft",
        Version = "v1.1",
        Description = "Uma API para gerenciar empresas.\n obs:\n os campos: Setor, Tamanho, Tipo ,Contrato, Recursos possuem opções específicas a serem utilizadas para gravar no banco,\n cada um deles tem um endpoint para consulta das opções."
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/tipo_empresa", async () =>
{
    var tipos = "SOCIEDADE_EMPRESARIA_LIMITADA, MICROEMPREENDEDOR_INDIVIDUAL, SOCIEDADE_LIMITADA_UNIPESSOAL, SOCIEDADE_ANONIMA, EMPRESARIO_INDIVIDUAL, SOCIEDADE_SIMPLES";
    return tipos;
}).WithTags("(Opções)");

app.MapGet("/setor", async () =>
{
    var setores = "COMERCIAL, INDUSTRIAL, RURAL, SERVIÇOS, OUTROS";
    return setores;
}).WithTags("(Opções)");

app.MapGet("/tamanho", async () =>
{
    var tamanhos = "MICRO, PEQUENO, MEDIO, GRANDE, MULTINACIONAL";
    return tamanhos;
}).WithTags("(Opções)");

app.MapGet("/empresas", async () =>
{
    var empresas = new List<EmpresasModel>();

    using (var connection = new OracleConnection(connectionString))
    {
        await connection.OpenAsync(); 

        var queryEmpresas = @"
            SELECT
                Id AS id,
                Nome AS nome,
                Tamanho AS tamanho,
                Setor AS setor,
                Localizacao_Geografica AS localizacao_geografica,
                Numero_Funcionarios AS numero_funcionarios,
                Tipo_Empresa AS tipo_empresa,
                Cliente AS cliente
            FROM Empresas
        ";

        using (var commandEmpresas = new OracleCommand(queryEmpresas, connection))
        using (var readerEmpresas = await commandEmpresas.ExecuteReaderAsync())
        {
            while (await readerEmpresas.ReadAsync())
            {
                var empresa = new EmpresasModel
                {
                    Id = readerEmpresas.GetInt32(0),
                    Nome = readerEmpresas.GetString(1),
                    Tamanho = readerEmpresas.GetString(2),
                    Setor = readerEmpresas.GetString(3),
                    LocalizacaoGeografica = readerEmpresas.GetString(4),
                    NumeroFuncionarios = readerEmpresas.GetInt32(5),
                    TipoEmpresa = readerEmpresas.GetString(6),
                    Cliente = readerEmpresas.GetBoolean(7),
                    Tendencias = new List<TendenciaGastosModel>(),
                    Desempenhos = new List<DesempenhoFinanceiroModelSoID>(),
                    Historicos = new List<HistoricoInteracoes>(),
                    Comportamentos = new List<ComportamentoNegociosModel>()
                };

                var queryTendencias = @"
                    SELECT
                        Id AS id,
                        Ano AS ano,
                        Gasto_Marketing AS gasto_marketing,
                        Gasto_Automacao AS gasto_automacao
                    FROM TENDENCIAS_GASTOS
                    WHERE ID_EMPRESA = :empresaId
                ";

                using (var commandTendencias = new OracleCommand(queryTendencias, connection))
                {
                    commandTendencias.Parameters.Add(new OracleParameter("empresaId", empresa.Id));
                    using (var readerTendencias = await commandTendencias.ExecuteReaderAsync())
                    {
                        while (await readerTendencias.ReadAsync())
                        {
                            var tendencia = new TendenciaGastosModel
                            {
                                Id = readerTendencias.GetInt32(0),
                                Ano = readerTendencias.GetInt32(1),
                                GastoMarketing = readerTendencias.GetDouble(2),
                                GastoAutomacao = readerTendencias.GetDouble(3)
                            };
                            empresa.Tendencias.Add(tendencia);
                        }
                    }
                }


                empresas.Add(empresa);
            }
        }
    }
    return empresas;
}).WithTags("Empresas");

app.MapGet("/empresas/{id}", async (int id) =>
{
    EmpresasModel empresa = null;

    using (var connection = new OracleConnection(connectionString))
    {
        await connection.OpenAsync();

        var queryEmpresa = @"
            SELECT
                Id AS id,
                Nome AS nome,
                Tamanho AS tamanho,
                Setor AS setor,
                Localizacao_Geografica AS localizacao_geografica,
                Numero_Funcionarios AS numero_funcionarios,
                Tipo_Empresa AS tipo_empresa,
                Cliente AS cliente
            FROM Empresas
            WHERE Id = :empresaId
        ";

        using (var commandEmpresa = new OracleCommand(queryEmpresa, connection))
        {
            commandEmpresa.Parameters.Add(new OracleParameter("empresaId", id));

            using (var readerEmpresa = await commandEmpresa.ExecuteReaderAsync())
            {
                if (await readerEmpresa.ReadAsync())
                {
                    empresa = new EmpresasModel
                    {
                        Id = readerEmpresa.GetInt32(0),
                        Nome = readerEmpresa.GetString(1),
                        Tamanho = readerEmpresa.GetString(2),
                        Setor = readerEmpresa.GetString(3),
                        LocalizacaoGeografica = readerEmpresa.GetString(4),
                        NumeroFuncionarios = readerEmpresa.GetInt32(5),
                        TipoEmpresa = readerEmpresa.GetString(6),
                        Cliente = readerEmpresa.GetBoolean(7),
                        Tendencias = new List<TendenciaGastosModel>(),
                        Desempenhos = new List<DesempenhoFinanceiroModelSoID>(),
                        Historicos = new List<HistoricoInteracoes>(),
                        Comportamentos = new List<ComportamentoNegociosModel>()
                    };

                }
            }
        }
    }

    return empresa != null ? Results.Ok(empresa) : Results.NotFound();
}).WithTags("Empresas");


// Endpoint para criar uma nova empresa
app.MapPost("/empresas", async (EmpresaPostModel empresa) =>
{
    using (var connection = new OracleConnection(connectionString))
    {
        await connection.OpenAsync();

        // Query para inserir uma nova empresa
        var query = @"
            INSERT INTO EMPRESAS (NOME, TAMANHO, SETOR, LOCALIZACAO_GEOGRAFICA, NUMERO_FUNCIONARIOS, TIPO_EMPRESA, CLIENTE)
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

    return Results.Ok(empresa);
}).WithTags("Empresas");
// Define a rota para o método PUT
app.MapPut("/empresas/{id}", async (int id, EmpresaPostModel empresaAtualizada) =>
{
    // Verifica se o corpo da requisição não é nulo
    if (empresaAtualizada == null)
    {
        return Results.BadRequest("O corpo da requisição não pode ser nulo.");
    }

    // Abre a conexão com o banco de dados
    using (var connection = new OracleConnection(connectionString))
    {
        await connection.OpenAsync();

        // Query para atualizar a empresa
        var query = @"
            UPDATE EMPRESAS
            SET NOME = :Nome,
                TAMANHO = :Tamanho,
                SETOR = :Setor,
                LOCALIZACAO_GEOGRAFICA = :LocalizacaoGeografica,
                NUMERO_FUNCIONARIOS = :NumeroFuncionarios,
                TIPO_EMPRESA = :TipoEmpresa,
                CLIENTE = :Cliente
            WHERE Id = :empresaId"; 

        // Cria um comando para executar a query
        using (var command = new OracleCommand(query, connection))
        {
            // Adiciona os parâmetros para o comando
            command.Parameters.Add(new OracleParameter("Nome", empresaAtualizada.Nome));
            command.Parameters.Add(new OracleParameter("Tamanho", empresaAtualizada.Tamanho));
            command.Parameters.Add(new OracleParameter("Setor", empresaAtualizada.Setor));
            command.Parameters.Add(new OracleParameter("LocalizacaoGeografica", empresaAtualizada.LocalizacaoGeografica));
            command.Parameters.Add(new OracleParameter("NumeroFuncionarios", empresaAtualizada.NumeroFuncionarios));
            command.Parameters.Add(new OracleParameter("TipoEmpresa", empresaAtualizada.TipoEmpresa));
            command.Parameters.Add(new OracleParameter("Cliente", empresaAtualizada.Cliente));
            command.Parameters.Add(new OracleParameter("empresaId", id));


            // Executa o comando
            var linhasAfetadas = await command.ExecuteNonQueryAsync();

            // Verifica se alguma linha foi afetada (ou seja, se a atualização ocorreu)
            if (linhasAfetadas == 0)
            {
                return Results.NotFound(); // Retorna 404 se a empresa não for encontrada
            }
        }
    }

    // Retorna 200 OK com a mensagem de sucesso
    return Results.Ok(new { message = "Atualização bem-sucedida." });
}).WithTags("Empresas");



app.MapDelete("/empresas/{id}", async (int id) =>
{
    using (var connection = new OracleConnection(connectionString))
    {
        await connection.OpenAsync();

        // Excluir todos os registros filhos
        await ExcluirRegistrosFilhos(connection, id);

        // Excluir a empresa
        var deleteEmpresaQuery = @"
            DELETE FROM EMPRESAS
            WHERE Id = :empresaId";

        using (var deleteEmpresaCommand = new OracleCommand(deleteEmpresaQuery, connection))
        {
            deleteEmpresaCommand.Parameters.Add(new OracleParameter("empresaId", id));

            var linhasAfetadas = await deleteEmpresaCommand.ExecuteNonQueryAsync();

            if (linhasAfetadas == 0)
            {
                return Results.NotFound(); // Retorna 404 se a empresa não for encontrada
            }
        }
    }

    return Results.Ok(new { message = "Exclusão bem-sucedida." }); // Retorna 200 OK com a mensagem de sucesso
}).WithTags("Empresas");

// Método auxiliar para excluir registros filhos
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


// COMPORTAMENTO NEGOCIOS
app.MapGet("/comportamento_negocios", async () =>
{
    var comportamentos = new List<ComportamentoNegociosModel>();

    using (var connection = new OracleConnection(connectionString))
    {
        await connection.OpenAsync();

        var query = @"
            SELECT
                CN.ID,
                CN.INTERACOES_PLATAFORMA,
                CN.FREQUENCIA_USO,
                CN.FEEDBACK,
                CN.USO_RECURSOS_ESPECIFICOS,
                CN.ID_EMPRESA
            FROM COMPORTAMENTO_NEGOCIOS CN
        ";

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
                    Empresa = null // Buscaremos a empresa posteriormente
                };

                // Buscando a empresa relacionada com o ID_EMPRESA
                var empresaId = reader.GetInt64(5);
                comportamento.Empresa = await ObterEmpresaPorIdAsync(empresaId, connection);

                comportamentos.Add(comportamento);
            }
        }
    }

    return comportamentos;
}).WithTags("Comportamento Negócios");
async Task<EmpresaModelComId?> ObterEmpresaPorIdAsync(long empresaId, OracleConnection connection)
{
    EmpresaModelComId? empresa = null;

    var queryEmpresa = @"
        SELECT
            ID,
            NOME,
            TAMANHO,
            SETOR,
            LOCALIZACAO_GEOGRAFICA,
            NUMERO_FUNCIONARIOS,
            TIPO_EMPRESA,
            CLIENTE
        FROM EMPRESAS E
        WHERE E.ID = :empresaId
    ";

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




app.MapGet("/comportamento_negocios/{id}", async (long id) =>
{
    ComportamentoNegociosModel? comportamento = null;

    using (var connection = new OracleConnection(connectionString))
    {
        await connection.OpenAsync();

        var query = @"
            SELECT
                CN.ID,
                CN.INTERACOES_PLATAFORMA,
                CN.FREQUENCIA_USO,
                CN.FEEDBACK,
                CN.USO_RECURSOS_ESPECIFICOS,
                CN.ID_EMPRESA
            FROM COMPORTAMENTO_NEGOCIOS CN
            WHERE CN.ID = :id
        ";

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

    return comportamento != null ? Results.Ok(comportamento) : Results.NotFound();
}).WithTags("Comportamento Negócios");


app.MapPost("/comportamento_negocios", async (ComportamentoNegociosModelSoID novoComportamento) =>
{
    if (novoComportamento == null)
    {
        return Results.BadRequest("O corpo da requisição não pode ser nulo.");
    }

    using (var connection = new OracleConnection(connectionString))
    {
        await connection.OpenAsync();

        var query = @"
            INSERT INTO COMPORTAMENTO_NEGOCIOS 
                (INTERACOES_PLATAFORMA, FREQUENCIA_USO, FEEDBACK, USO_RECURSOS_ESPECIFICOS, ID_EMPRESA)
            VALUES
                (:InteracoesPlataforma, :FrequenciaUso, :Feedback, :UsoRecursosEspecificos, :IdEmpresa)
        ";

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

    return Results.Created($"/comportamento_negocios/{novoComportamento.Id}", novoComportamento);
}).WithTags("Comportamento Negócios");


app.MapPut("/comportamento_negocios/{id}", async (long id, ComportamentoNegociosModelSoID comportamentoAtualizado) =>
{
    if (comportamentoAtualizado == null)
    {
        return Results.BadRequest("O corpo da requisição não pode ser nulo.");
    }

    using (var connection = new OracleConnection(connectionString))
    {
        await connection.OpenAsync();

        var query = @"
            UPDATE COMPORTAMENTO_NEGOCIOS
            SET INTERACOES_PLATAFORMA = :InteracoesPlataforma,
                FREQUENCIA_USO = :FrequenciaUso,
                FEEDBACK = :Feedback,
                USO_RECURSOS_ESPECIFICOS = :UsoRecursosEspecificos,
                ID_EMPRESA = :IdEmpresa
            WHERE ID = :Id
        ";

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
                return Results.NotFound(); // Retorna 404 se o comportamento não for encontrado
            }
        }
    }

    return Results.Ok(new { message = "Atualização bem-sucedida." });
}).WithTags("Comportamento Negócios");

app.MapDelete("/comportamento_negocios/{id}", async (long id) =>
{
    using (var connection = new OracleConnection(connectionString))
    {
        await connection.OpenAsync();

        var query = @"
            DELETE FROM COMPORTAMENTO_NEGOCIOS
            WHERE ID = :Id
        ";

        using (var command = new OracleCommand(query, connection))
        {
            command.Parameters.Add(new OracleParameter("Id", id));

            var linhasAfetadas = await command.ExecuteNonQueryAsync();

            if (linhasAfetadas == 0)
            {
                return Results.NotFound(); // Retorna 404 se o comportamento não for encontrado
            }
        }
    }

    return Results.Ok(new { message = "Exclusão bem-sucedida." });
}).WithTags("Comportamento Negócios");

// desempenho_financeiro
app.MapGet("/desempenho_financeiro", async () =>
{
    var desempenhos = new List<DesempenhoFinanceiroModelSoID>();

    using (var connection = new OracleConnection(connectionString))
    {
        await connection.OpenAsync();

        var query = @"
            SELECT
                DF.ID,
                DF.CRESCIMENTO,
                DF.LUCRO,
                DF.RECEITA,
                DF.ID_EMPRESA
            FROM DESEMPENHO_FINANCEIRO DF
        ";

        using (var command = new OracleCommand(query, connection))
        using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var desempenho = new DesempenhoFinanceiroModelSoID
                {
                    Id = reader.GetInt64(0),
                    Crescimento = reader.GetDouble(1), // Alterado para GetDouble
                    Lucro = reader.GetDouble(2),       // Alterado para GetDouble
                    Receita = reader.GetDouble(3),     // Alterado para GetDouble
                    EmpresaId = reader.GetInt32(4) // Buscaremos a empresa posteriormente
                };

                
                desempenhos.Add(desempenho);
            }
        }
    }

    return desempenhos;
}).WithTags("Desempenho Financeiro");

// GET by ID
app.MapGet("/desempenho_financeiro/{id}", async (long id) =>
{
    DesempenhoFinanceiroModelSoID? desempenho = null;

    using (var connection = new OracleConnection(connectionString))
    {
        await connection.OpenAsync();

        var query = @"
            SELECT
                DF.ID,
                DF.CRESCIMENTO,
                DF.LUCRO,
                DF.RECEITA,
                DF.ID_EMPRESA
            FROM DESEMPENHO_FINANCEIRO DF
            WHERE DF.ID = :id
        ";

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

    return desempenho is not null ? Results.Ok(desempenho) : Results.NotFound();
}).WithTags("Desempenho Financeiro");

// POST
app.MapPost("/desempenho_financeiro", async (DesempenhoPostModel novoDesempenho) =>
{
    using (var connection = new OracleConnection(connectionString))
    {
        await connection.OpenAsync();

        var query = @"
            INSERT INTO DESEMPENHO_FINANCEIRO (CRESCIMENTO, LUCRO, RECEITA, ID_EMPRESA)
            VALUES (:crescimento, :lucro, :receita, :empresaId)
        ";

        using (var command = new OracleCommand(query, connection))
        {
            command.Parameters.Add(new OracleParameter("crescimento", OracleDbType.Decimal)).Value = novoDesempenho.Crescimento;
            command.Parameters.Add(new OracleParameter("lucro", OracleDbType.Decimal)).Value = novoDesempenho.Lucro;
            command.Parameters.Add(new OracleParameter("receita", OracleDbType.Decimal)).Value = novoDesempenho.Receita;
            command.Parameters.Add(new OracleParameter("empresaId", OracleDbType.Int32)).Value = novoDesempenho.EmpresaId;

            await command.ExecuteNonQueryAsync();
        }
     
    }
    return Results.Ok("Desempenho criado com sucesso");
}).WithTags("Desempenho Financeiro");


// PUT
app.MapPut("/desempenho_financeiro/{id}", async (long id, DesempenhoPostModel desempenhoAtualizado) =>
{
    using (var connection = new OracleConnection(connectionString))
    {
        await connection.OpenAsync();

        var query = @"
            UPDATE DESEMPENHO_FINANCEIRO
            SET CRESCIMENTO = :crescimento,
                LUCRO = :lucro,
                RECEITA = :receita,
                ID_EMPRESA = :empresaId
            WHERE ID = :id
        ";

        using (var command = new OracleCommand(query, connection))
        {
            command.Parameters.Add(new OracleParameter("crescimento", desempenhoAtualizado.Crescimento));
            command.Parameters.Add(new OracleParameter("lucro", desempenhoAtualizado.Lucro));
            command.Parameters.Add(new OracleParameter("receita", desempenhoAtualizado.Receita));
            command.Parameters.Add(new OracleParameter("empresaId", desempenhoAtualizado.EmpresaId));
            command.Parameters.Add(new OracleParameter("id", id));

            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0 ? Results.NoContent() : Results.NotFound();
        }
    }
}).WithTags("Desempenho Financeiro");

// DELETE
app.MapDelete("/desempenho_financeiro/{id}", async (long id) =>
{
    using (var connection = new OracleConnection(connectionString))
    {
        await connection.OpenAsync();

        var query = @"
            DELETE FROM DESEMPENHO_FINANCEIRO
            WHERE ID = :id
        ";

        using (var command = new OracleCommand(query, connection))
        {
            command.Parameters.Add(new OracleParameter("id", id));

            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0 ? Results.NoContent() : Results.NotFound();
        }
    }
}).WithTags("Desempenho Financeiro");


// GET all historico_interacoes
app.MapGet("/historico_interacoes", async () =>
{
    var historicos = new List<HistoricoInteracoes>();

    using (var connection = new OracleConnection(connectionString))
    {
        await connection.OpenAsync();

        var query = @"
            SELECT
                HI.ID,
                HI.PROPOSTA_NEGOCIO,
                HI.CONTRATO_ASSINADO,
                HI.FEEDBACK_SERVICOS_PRODUTOS,
                HI.ID_EMPRESA
            FROM HISTORICO_INTERACOES HI
        ";

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

    return historicos;
}).WithTags("Historico Interações");

// GET by ID
app.MapGet("/historico_interacoes/{id}", async (int id) =>
{
    HistoricoInteracoes? historico = null;

    using (var connection = new OracleConnection(connectionString))
    {
        await connection.OpenAsync();

        var query = @"
            SELECT
                HI.ID,
                HI.PROPOSTA_NEGOCIO,
                HI.CONTRATO_ASSINADO,
                HI.FEEDBACK_SERVICOS_PRODUTOS,
                HI.ID_EMPRESA
            FROM HISTORICO_INTERACOES HI
            WHERE HI.ID = :id
        ";

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

    return historico is not null ? Results.Ok(historico) : Results.NotFound();
}).WithTags("Historico Interações");

// POST
app.MapPost("/historico_interacoes", async (HistoricoInteracoesPostModel novoHistorico) =>
{
    using (var connection = new OracleConnection(connectionString))
    {
        await connection.OpenAsync();

        var query = @"
            INSERT INTO HISTORICO_INTERACOES (PROPOSTA_NEGOCIO, CONTRATO_ASSINADO, FEEDBACK_SERVICOS_PRODUTOS, ID_EMPRESA)
            VALUES (:propostaNegocio, :contratoAssinado, :feedbackServicosProdutos, :empresaId)
        ";

        using (var command = new OracleCommand(query, connection))
        {
            command.Parameters.Add(new OracleParameter("propostaNegocio", novoHistorico.PropostaNegocio));
            command.Parameters.Add(new OracleParameter("contratoAssinado", novoHistorico.ContratoAssinado));
            command.Parameters.Add(new OracleParameter("feedbackServicosProdutos", novoHistorico.FeedbackServicosProdutos));
            command.Parameters.Add(new OracleParameter("empresaId", novoHistorico.EmpresaId));

            await command.ExecuteNonQueryAsync();
        }
    }

    return Results.Ok("Histórico de Interação criado com sucesso");
}).WithTags("Historico Interações");

// PUT
app.MapPut("/historico_interacoes/{id}", async (int id, HistoricoInteracoesPostModel historicoAtualizado) =>
{
    using (var connection = new OracleConnection(connectionString))
    {
        await connection.OpenAsync();

        var query = @"
            UPDATE HISTORICO_INTERACOES
            SET PROPOSTA_NEGOCIO = :propostaNegocio,
                CONTRATO_ASSINADO = :contratoAssinado,
                FEEDBACK_SERVICOS_PRODUTOS = :feedbackServicosProdutos,
                ID_EMPRESA = :empresaId
            WHERE ID = :id
        ";

        using (var command = new OracleCommand(query, connection))
        {
            command.Parameters.Add(new OracleParameter("propostaNegocio", historicoAtualizado.PropostaNegocio));
            command.Parameters.Add(new OracleParameter("contratoAssinado", historicoAtualizado.ContratoAssinado));
            command.Parameters.Add(new OracleParameter("feedbackServicosProdutos", historicoAtualizado.FeedbackServicosProdutos));
            command.Parameters.Add(new OracleParameter("empresaId", historicoAtualizado.EmpresaId));
            command.Parameters.Add(new OracleParameter("id", id));

            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0 ? Results.NoContent() : Results.NotFound();
        }
    }
}).WithTags("Historico Interações");

// DELETE
app.MapDelete("/historico_interacoes/{id}", async (int id) =>
{
    using (var connection = new OracleConnection(connectionString))
    {
        await connection.OpenAsync();

        var query = @"
            DELETE FROM HISTORICO_INTERACOES
            WHERE ID = :id
        ";

        using (var command = new OracleCommand(query, connection))
        {
            command.Parameters.Add(new OracleParameter("id", id));

            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0 ? Results.NoContent() : Results.NotFound();
        }
    }
}).WithTags("Historico Interações");


// Obter todos os registros
app.MapGet("/tendencia_gastos", async () =>
{
    var tendencias = new List<TendenciaGastosModel>();

    using (var connection = new OracleConnection(connectionString))
    {
        await connection.OpenAsync();

        var query = @"
            SELECT
                TG.ID,
                TG.ANO,
                TG.GASTO_MARKETING,
                TG.GASTO_AUTOMACAO,
                TG.ID_EMPRESA
            FROM TENDENCIAS_GASTOS TG
        ";

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

    return tendencias;
}).WithTags("Tendência Gastos");

// Obter registro por ID
app.MapGet("/tendencia_gastos/{id}", async (long id) =>
{
    TendenciaGastosModel? tendencia = null;

    using (var connection = new OracleConnection(connectionString))
    {
        await connection.OpenAsync();

        var query = @"
            SELECT
                TG.ID,
                TG.ANO,
                TG.GASTO_MARKETING,
                TG.GASTO_AUTOMACAO,
                TG.ID_EMPRESA
            FROM TENDENCIAS_GASTOS TG
            WHERE TG.ID = :id
        ";

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

    return tendencia is not null ? Results.Ok(tendencia) : Results.NotFound();
}).WithTags("Tendência Gastos");

// Inserir novo registro
app.MapPost("/tendencia_gastos", async (TendenciaGastosPostModel novaTendencia) =>
{
    using (var connection = new OracleConnection(connectionString))
    {
        await connection.OpenAsync();

        var query = @"
            INSERT INTO TENDENCIAS_GASTOS (ANO, GASTO_MARKETING, GASTO_AUTOMACAO, ID_EMPRESA)
            VALUES (:ano, :gastoMarketing, :gastoAutomacao, :empresaId)
        ";

        using (var command = new OracleCommand(query, connection))
        {
            command.Parameters.Add(new OracleParameter("ano", OracleDbType.Int32)).Value = novaTendencia.Ano;
            command.Parameters.Add(new OracleParameter("gastoMarketing", OracleDbType.Decimal)).Value = novaTendencia.GastoMarketing;
            command.Parameters.Add(new OracleParameter("gastoAutomacao", OracleDbType.Decimal)).Value = novaTendencia.GastoAutomacao;
            command.Parameters.Add(new OracleParameter("empresaId", OracleDbType.Int64)).Value = novaTendencia.EmpresaId;

            await command.ExecuteNonQueryAsync();
        }
    }

    return Results.Ok("Tendência de gastos criada com sucesso");
}).WithTags("Tendência Gastos");

// Atualizar registro por ID
app.MapPut("/tendencia_gastos/{id}", async (long id, TendenciaGastosPostModel tendenciaAtualizada) =>
{
    using (var connection = new OracleConnection(connectionString))
    {
        await connection.OpenAsync();

        var query = @"
            UPDATE TENDENCIAS_GASTOS
            SET ANO = :ano,
                GASTO_MARKETING = :gastoMarketing,
                GASTO_AUTOMACAO = :gastoAutomacao,
                ID_EMPRESA = :empresaId
            WHERE ID = :id
        ";

        using (var command = new OracleCommand(query, connection))
        {
            command.Parameters.Add(new OracleParameter("ano", tendenciaAtualizada.Ano));
            command.Parameters.Add(new OracleParameter("gastoMarketing", tendenciaAtualizada.GastoMarketing));
            command.Parameters.Add(new OracleParameter("gastoAutomacao", tendenciaAtualizada.GastoAutomacao));
            command.Parameters.Add(new OracleParameter("empresaId", tendenciaAtualizada.EmpresaId));
            command.Parameters.Add(new OracleParameter("id", id));

            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0 ? Results.NoContent() : Results.NotFound();
        }
    }
}).WithTags("Tendência Gastos");

// Deletar registro por ID
app.MapDelete("/tendencia_gastos/{id}", async (long id) =>
{
    using (var connection = new OracleConnection(connectionString))
    {
        await connection.OpenAsync();

        var query = @"
            DELETE FROM TENDENCIAS_GASTOS
            WHERE ID = :id
        ";

        using (var command = new OracleCommand(query, connection))
        {
            command.Parameters.Add(new OracleParameter("id", id));

            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0 ? Results.NoContent() : Results.NotFound();
        }
    }
}).WithTags("Tendência Gastos");



app.Run();
