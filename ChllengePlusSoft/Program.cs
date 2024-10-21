using ChllengePlusSoft.Models;
using Oracle.ManagedDataAccess.Client;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("OracleDbConnection");

builder.Services.AddSingleton<OracleConnection>(_ =>
{
    return new OracleConnection(connectionString);
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/empresas", async (OracleConnection connection) =>
{
    var empresas = new List<EmpresasModel>();

    await connection.OpenAsync();  // Abre a conexão com o banco de dados

    // Consulta para obter as empresas
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
                Desempenhos = new List<DesempenhoFinanceiroModel>(),
                Historicos = new List<HistoricoInteracoes>(),
                Comportamentos = new List<ComportamentoNegociosModel>()
            };

            // Consulta para obter as Tendências de Gastos
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

            // Consulta para obter os Desempenhos Financeiros
            var queryDesempenhos = @"
                SELECT
                    Id AS id,
                    Receita AS receita,
                    Lucro AS lucro,
                    Crescimento AS crescimento
                FROM DESEMPENHO_FINANCEIRO
                WHERE ID_EMPRESA = :empresaId
            ";

            using (var commandDesempenhos = new OracleCommand(queryDesempenhos, connection))
            {
                commandDesempenhos.Parameters.Add(new OracleParameter("empresaId", empresa.Id));
                using (var readerDesempenhos = await commandDesempenhos.ExecuteReaderAsync())
                {
                    while (await readerDesempenhos.ReadAsync())
                    {
                        var desempenho = new DesempenhoFinanceiroModel
                        {
                            Id = readerDesempenhos.GetInt32(0),
                            Receita = readerDesempenhos.GetDouble(1),
                            Lucro = readerDesempenhos.GetDouble(2),
                            Crescimento = readerDesempenhos.GetDouble(3)
                        };
                        empresa.Desempenhos.Add(desempenho);
                    }
                }
            }

            // Consulta para obter os Históricos de Interações
            var queryHistoricos = @"
                SELECT
                    Id AS id,
                    Proposta_Negocio AS proposta_negocio,
                    Contrato_Assinado AS contrato_assinado,
                    Feedback_Servicos_Produtos AS feedback_servicos_produtos
                FROM HISTORICO_INTERACOES
                WHERE ID_EMPRESA = :empresaId
            ";

            using (var commandHistoricos = new OracleCommand(queryHistoricos, connection))
            {
                commandHistoricos.Parameters.Add(new OracleParameter("empresaId", empresa.Id));
                using (var readerHistoricos = await commandHistoricos.ExecuteReaderAsync())
                {
                    while (await readerHistoricos.ReadAsync())
                    {
                        var historico = new HistoricoInteracoes
                        {
                            Id = readerHistoricos.GetInt32(0),
                            PropostaNegocio = readerHistoricos.GetString(1),
                            ContratoAssinado = readerHistoricos.GetString(2),
                            FeedbackServicosProdutos = readerHistoricos.GetString(3)
                        };
                        empresa.Historicos.Add(historico);
                    }
                }
            }

            // Consulta para obter os Comportamentos de Negócios
            var queryComportamentos = @"
                SELECT
                    Id AS id,
                    Interacoes_Plataforma AS interacoes_plataforma,
                    Frequencia_Uso AS frequencia_uso,
                    Feedback AS feedback,
                    Uso_Recursos_Especificos AS uso_recursos_especificos
                FROM COMPORTAMENTO_NEGOCIOS
                WHERE ID_EMPRESA = :empresaId
            ";

            using (var commandComportamentos = new OracleCommand(queryComportamentos, connection))
            {
                commandComportamentos.Parameters.Add(new OracleParameter("empresaId", empresa.Id));
                using (var readerComportamentos = await commandComportamentos.ExecuteReaderAsync())
                {
                    while (await readerComportamentos.ReadAsync())
                    {
                        var comportamento = new ComportamentoNegociosModel
                        {
                            Id = readerComportamentos.GetInt32(0),
                            InteracoesPlataforma = readerComportamentos.GetInt32(1),
                            FrequenciaUso = readerComportamentos.GetInt32(2),
                            Feedback = readerComportamentos.GetString(3),
                            UsoRecursosEspecificos = readerComportamentos.GetString(4)
                        };
                        empresa.Comportamentos.Add(comportamento);
                    }
                }
            }

            empresas.Add(empresa);
        }
    }
    connection.Close();
    return empresas;
});




app.MapGet("/empresas/{id}", async (int id, OracleConnection connection) =>
{
    EmpresasModel empresa = null;

    await connection.OpenAsync();  // Abre a conexão com o banco de dados

    // Consulta para obter a empresa específica pelo ID
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
                    Desempenhos = new List<DesempenhoFinanceiroModel>(),
                    Historicos = new List<HistoricoInteracoes>(),
                    Comportamentos = new List<ComportamentoNegociosModel>()
                };

                // Consultas aninhadas para obter os dados relacionados (Tendências, Desempenhos, etc.)
                // (Reutilize as consultas aninhadas conforme necessário, como no código anterior)

                // Consulta para obter as Tendências de Gastos
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
                    commandTendencias.Parameters.Add(new OracleParameter("empresaId", id));
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

                // Consulta para obter os Desempenhos Financeiros
                var queryDesempenhos = @"
                    SELECT
                        Id AS id,
                        Receita AS receita,
                        Lucro AS lucro,
                        Crescimento AS crescimento
                    FROM DESEMPENHO_FINANCEIRO
                    WHERE ID_EMPRESA = :empresaId
                ";

                using (var commandDesempenhos = new OracleCommand(queryDesempenhos, connection))
                {
                    commandDesempenhos.Parameters.Add(new OracleParameter("empresaId", id));
                    using (var readerDesempenhos = await commandDesempenhos.ExecuteReaderAsync())
                    {
                        while (await readerDesempenhos.ReadAsync())
                        {
                            var desempenho = new DesempenhoFinanceiroModel
                            {
                                Id = readerDesempenhos.GetInt32(0),
                                Receita = readerDesempenhos.GetDouble(1),
                                Lucro = readerDesempenhos.GetDouble(2),
                                Crescimento = readerDesempenhos.GetDouble(3)
                            };
                            empresa.Desempenhos.Add(desempenho);
                        }
                    }
                }

                // Consulta para obter os Históricos de Interações
                var queryHistoricos = @"
                    SELECT
                        Id AS id,
                        Proposta_Negocio AS proposta_negocio,
                        Contrato_Assinado AS contrato_assinado,
                        Feedback_Servicos_Produtos AS feedback_servicos_produtos
                    FROM HISTORICO_INTERACOES
                    WHERE ID_EMPRESA = :empresaId
                ";

                using (var commandHistoricos = new OracleCommand(queryHistoricos, connection))
                {
                    commandHistoricos.Parameters.Add(new OracleParameter("empresaId", id));
                    using (var readerHistoricos = await commandHistoricos.ExecuteReaderAsync())
                    {
                        while (await readerHistoricos.ReadAsync())
                        {
                            var historico = new HistoricoInteracoes
                            {
                                Id = readerHistoricos.GetInt32(0),
                                PropostaNegocio = readerHistoricos.GetString(1),
                                ContratoAssinado = readerHistoricos.GetString(2),
                                FeedbackServicosProdutos = readerHistoricos.GetString(3)
                            };
                            empresa.Historicos.Add(historico);
                        }
                    }
                }

                // Consulta para obter os Comportamentos de Negócios
                var queryComportamentos = @"
                    SELECT
                        Id AS id,
                        Interacoes_Plataforma AS interacoes_plataforma,
                        Frequencia_Uso AS frequencia_uso,
                        Feedback AS feedback,
                        Uso_Recursos_Especificos AS uso_recursos_especificos
                    FROM COMPORTAMENTO_NEGOCIOS
                    WHERE ID_EMPRESA = :empresaId
                ";

                using (var commandComportamentos = new OracleCommand(queryComportamentos, connection))
                {
                    commandComportamentos.Parameters.Add(new OracleParameter("empresaId", id));
                    using (var readerComportamentos = await commandComportamentos.ExecuteReaderAsync())
                    {
                        while (await readerComportamentos.ReadAsync())
                        {
                            var comportamento = new ComportamentoNegociosModel
                            {
                                Id = readerComportamentos.GetInt32(0),
                                InteracoesPlataforma = readerComportamentos.GetInt32(1),
                                FrequenciaUso = readerComportamentos.GetInt32(2),
                                Feedback = readerComportamentos.GetString(3),
                                UsoRecursosEspecificos = readerComportamentos.GetString(4)
                            };
                            empresa.Comportamentos.Add(comportamento);
                        }
                    }
                }
            }
        }
    }
    connection.Close();

    return empresa != null ? Results.Ok(empresa) : Results.NotFound();
});


//app.MapPost("/empresas", (EmpresasModel empresa) =>
//{
//    empresas.Add(empresa);
//    return empresas;
//});

//app.MapPut("/empresas/{id}", (EmpresasModel empresaAtualizada, int id) =>
//{
//    var empresa = empresas.Find(x => x.Id == id);
//    if (empresa is null)
//    {
//        return Results.NotFound("Empresa não encontrada/cadastrada");
//    }

//    // Atualiza os campos da empresa encontrada
//    empresa.Nome = empresaAtualizada.Nome;
//    empresa.Tamanho = empresaAtualizada.Tamanho;
//    empresa.Setor = empresaAtualizada.Setor;
//    empresa.LocalizacaoGeografica = empresaAtualizada.LocalizacaoGeografica;
//    empresa.NumeroFuncionarios = empresaAtualizada.NumeroFuncionarios;
//    empresa.TipoEmpresa = empresaAtualizada.TipoEmpresa;
//    empresa.Cliente = empresaAtualizada.Cliente;
//    empresa.Tendencias = empresaAtualizada.Tendencias; // Aqui, considere a lógica para atualizar a lista, se necessário
//    empresa.Desempenhos = empresaAtualizada.Desempenhos; // Lógica similar para desempenhos
//    empresa.Historicos = empresaAtualizada.Historicos; // E para históricos
//    empresa.Comportamentos = empresaAtualizada.Comportamentos; // E para comportamentos

//    return Results.Ok(empresa);
//});


//app.MapDelete("/empresas/{id}", (int id) =>
//{
//    var empresa = empresas.Find(x => x.Id == id);
//    if ( empresa is null)
//    {
//        return Results.NotFound("Empresa não encontrada");
//    }
//    empresas.Remove(empresa);
//    return Results.Ok(empresas);
//});

app.Run();