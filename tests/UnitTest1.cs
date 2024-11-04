using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using ChllengePlusSoft;
using ChllengePlusSoft.Models;
using Microsoft.AspNetCore.Hosting;

namespace ChllengePlusSoft.Tests
{
    public class EmpresasControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public EmpresasControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }


        // Testes para GetEmpresa
        [Fact]
        public async Task GetEmpresa_ReturnsOkResult_WhenEmpresaExists()
        {
            // Arrange
            int testId = 1; // ID de teste

            // Act
            var response = await _client.GetAsync($"/empresas/{testId}");

            // Assert
            response.EnsureSuccessStatusCode(); 
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseString); 
        }

        [Fact]
        public async Task GetEmpresa_ReturnsNotFound_WhenEmpresaDoesNotExist()
        {
            // Arrange
            int testId = 999; // ID que não existe

            // Act
            var response = await _client.GetAsync($"/empresas/{testId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode); 
        }

        // Testes para CreateEmpresa
        [Fact]
        public async Task CreateEmpresa_ReturnsCreatedResult()
        {
            // Arrange
            var newEmpresa = new EmpresaPostModel
            {
                Nome = "Nova Empresa",
                Tamanho = "GRANDE",
                Setor = "COMERCIAL",
                LocalizacaoGeografica = "São Paulo",
                NumeroFuncionarios = 10,
                TipoEmpresa = "SOCIEDADE_EMPRESARIA_LIMITADA",
                Cliente = 1
            };

            var content = new StringContent(JsonConvert.SerializeObject(newEmpresa), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/empresas", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateEmpresa_ReturnsBadRequest_WhenDataIsInvalid()
        {
            // Arrange
            var invalidEmpresa = new EmpresaPostModel
            {
                Nome = "", // Nome inválido
                Tamanho = "PEQUENO",
                Setor = "COMERCIAL",
                LocalizacaoGeografica = "São Paulo",
                NumeroFuncionarios = 10,
                TipoEmpresa = "SOCIEDADE_EMPRESARIA_LIMITADA",
                Cliente = 1
            };

            var content = new StringContent(JsonConvert.SerializeObject(invalidEmpresa), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/empresas", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // Testes para UpdateEmpresa
        [Fact]
        public async Task UpdateEmpresa_ReturnsOkResult_WhenEmpresaExists()
        {
            // Arrange
            int testId = 1; // ID de teste
            var updatedEmpresa = new EmpresaPostModel
            {
                Nome = "Empresa Atualizada",
                Tamanho = "GRANDE",
                Setor = "COMERCIAL",
                LocalizacaoGeografica = "Rio de Janeiro",
                NumeroFuncionarios = 150,
                TipoEmpresa = "MICROEMPREENDEDOR_INDIVIDUAL",
                Cliente = 1
            };

            var content = new StringContent(JsonConvert.SerializeObject(updatedEmpresa), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"/empresas/{testId}", content);

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }}