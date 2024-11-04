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
    public class DesempenhoFinanceiroControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public DesempenhoFinanceiroControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetDesempenhoFinanceiro_ReturnsOkResult()
        {
            var response = await _client.GetAsync("/DesempenhoFinanceiro");
            response.EnsureSuccessStatusCode();
            Assert.NotNull(await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task GetDesempenhoFinanceiroById_ReturnsOkResult_WhenDesempenhoFinanceiroExists()
        {
            int testId = 1;
            var response = await _client.GetAsync($"/DesempenhoFinanceiro/{testId}");
            response.EnsureSuccessStatusCode();
            Assert.NotNull(await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task CreateDesempenhoFinanceiro_ReturnsCreatedResult()
        {
            var novoDesempenho = new DesempenhoPostModel
            {
                Crescimento = 10,
                Lucro = 15000,
                Receita = 50000,
                EmpresaId = 2
            };

            var content = new StringContent(JsonConvert.SerializeObject(novoDesempenho), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/DesempenhoFinanceiro", content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Fact]
        public async Task DeleteDesempenhoFinanceiro_ReturnsNoContent_WhenDesempenhoFinanceiroExists()
        {
            int testId = 22;
            var response = await _client.DeleteAsync($"/DesempenhoFinanceiro/{testId}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}
