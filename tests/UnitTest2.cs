using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using ChllengePlusSoft.Models;

namespace ChllengePlusSoft.Tests
{
    public class TendenciaGastosControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public TendenciaGastosControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetTendenciaGastos_ReturnsOkResult()
        {
            var response = await _client.GetAsync("/TendenciaGastos");
            response.EnsureSuccessStatusCode();
            Assert.NotNull(await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task GetTendenciaGastosById_ReturnsOkResult_WhenTendenciaGastosExists()
        {
            int testId = 21;
            var response = await _client.GetAsync($"/TendenciaGastos/{testId}");
            response.EnsureSuccessStatusCode();
            Assert.NotNull(await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task CreateTendenciaGastos_ReturnsCreatedResult()
        {
            var newTendencia = new TendenciaGastosPostModel
            {
                Ano = 2023,
                GastoMarketing = 5000,
                GastoAutomacao = 3000,
                EmpresaId = 1
            };

            var content = new StringContent(JsonConvert.SerializeObject(newTendencia), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/TendenciaGastos", content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
