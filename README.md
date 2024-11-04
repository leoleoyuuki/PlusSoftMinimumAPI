# Challenge Plus Soft API

API para gerenciamento de dados de empresas, construída com .NET e Oracle Database. Este projeto adota práticas de Clean Code, inclui uma suíte de testes completa usando xUnit e uma funcionalidade de IA generativa para previsões financeiras.

## Funcionalidades

- **CRUD Completo para Tabelas**: Gerenciamento das tabelas `Empresas`, `TendenciaGastos`, `HistoricoInteracoes`, `ComportamentoNegocios` e `DesempenhoFinanceiro`.
- **Consultas Personalizadas**: Endpoints adicionais para retornar opções específicas de campos como `Setor`, `Tamanho` e `TipoEmpresa`, garantindo consistência dos dados.
- **IA Generativa**: Implementação com ML.NET para prever a receita de um cliente com base em seus gastos em marketing e automação.
- **Swagger UI**: Documentação completa dos endpoints, disponível através da interface do Swagger.

## Estrutura do Projeto

O projeto segue boas práticas de Clean Code para garantir legibilidade e manutenção:
- **Models**: Estruturas de dados para cada tabela.
- **Controllers**: Gerenciam a lógica de negócios e as rotas da API.
- **Testes**: Testes de integração para cada rota e tabela, assegurando a funcionalidade dos principais recursos.

## Testes

Os testes são implementados com xUnit e incluem:
- **Empresas**: CRUD completo, cobrindo criação, atualização e exclusão.
- **TendenciaGastos, HistoricoInteracoes, ComportamentoNegocios, e DesempenhoFinanceiro**: Testes para operações principais e respostas para IDs inválidos.

Os testes foram projetados para garantir que a API retorne o status correto (`200 OK`, `404 Not Found`, etc.) e que as operações de dados sejam executadas conforme esperado no banco de dados.


## Requisitos

- .NET 6+
- Oracle Database
- ML.NET para funcionalidade de IA

## Instalação e Execução

1. **Clone o repositório**:
   ```bash
   git clone https://github.com/seuusuario/ChallengePlusSoft.git
   ```
   
2. **Configuração do Banco de Dados**: 
   Atualize `appsettings.json` com a `ConnectionString` do Oracle Database.

3. **Executar a API**:
   ```bash
   dotnet run
   ```

4. **Executar Testes**:
   ```bash
   dotnet test
   ```

   ## IA Generativa com ML.NET

A API usa ML.NET para previsão de receita financeira. O modelo analisa os valores históricos de `GastoMarketing` e `GastoAutomacao` para prever a `Receita` da empresa.

## Documentação com Swagger

Todos os endpoints estão documentados e podem ser acessados na interface do Swagger, disponível ao iniciar o projeto em modo `Development`.

