using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Oracle.ManagedDataAccess.Client;
using ChllengePlusSoft.Controllers;
using ChllengePlusSoft.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ChllengePlusSoft.Tests
{
    public class EmpresasControllerTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly EmpresasController _controller;

        public EmpresasControllerTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(c => c["ConnectionStrings:OracleDbConnection"])
                .Returns("UserId=rm99119;Password=200705;Data Source=oracle.fiap.com.br:1521/orcl");

            _controller = new EmpresasController(_configurationMock.Object);
        }

        [Fact]
        public async Task GetEmpresas_ReturnsOkResult_WithListOfEmpresas()
        {
            // Simulando o retorno do banco de dados
            var empresas = new List<EmpresasModel>
            {
                new EmpresasModel { Id = 1, Nome = "Empresa A", Tamanho = "Grande", Setor = "Tecnologia", LocalizacaoGeografica = "S�o Paulo", NumeroFuncionarios = 100, TipoEmpresa = "Privada", Cliente = true },
                new EmpresasModel { Id = 2, Nome = "Empresa B", Tamanho = "M�dia", Setor = "Sa�de", LocalizacaoGeografica = "Rio de Janeiro", NumeroFuncionarios = 50, TipoEmpresa = "P�blica", Cliente = false }
            };

            // Aqui voc� deve simular o comportamento do m�todo GetEmpresas,
            // se voc� estiver usando um reposit�rio ou um servi�o que faz a chamada ao banco de dados.

            // Act
            var result = await _controller.GetEmpresas();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<EmpresasModel>>>(result);
            var okResult = Assert.IsType<List<EmpresasModel>>(actionResult.Value);
            Assert.NotNull(okResult); // Verifica se a lista n�o � nula
            Assert.True(okResult.Count > 0); // Verifica se a lista cont�m elementos
        }

        [Fact]
        public async Task GetEmpresa_ReturnsOkResult_WhenEmpresaExists()
        {
            // Arrange
            int testId = 1; // ID de teste

            // Simulando o retorno do banco de dados para o m�todo GetEmpresa
            var empresa = new EmpresasModel { Id = testId, Nome = "Empresa A", Tamanho = "Grande", Setor = "Tecnologia", LocalizacaoGeografica = "S�o Paulo", NumeroFuncionarios = 100, TipoEmpresa = "Privada", Cliente = true };

            // Aqui voc� deve simular o comportamento do m�todo GetEmpresa,
            // se voc� estiver usando um reposit�rio ou um servi�o que faz a chamada ao banco de dados.

            // Act
            var result = await _controller.GetEmpresa(testId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<EmpresasModel>>(result);
            var okResult = Assert.IsType<EmpresasModel>(actionResult.Value);
            Assert.NotNull(okResult); // Verifica se a empresa retornada n�o � nula
            Assert.Equal(testId, okResult.Id); // Verifica se o ID da empresa retornada � o esperado
        }
    }
}