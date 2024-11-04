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
    public class HistoricoInteracoesControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public HistoricoInteracoesControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetHistoricoInteracoes_ReturnsOkResult()
        {
            var response = await _client.GetAsync("/HistoricoInteracoes");
            response.EnsureSuccessStatusCode();
            Assert.NotNull(await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task CreateHistoricoInteracoes_ReturnsCreatedResult()
        {
            var novoHistorico = new HistoricoInteracoesPostModel
            {
                PropostaNegocio = "Nova Proposta",
                ContratoAssinado = "SIM",
                FeedbackServicosProdutos = "Bom",
                EmpresaId = 2
            };

            var content = new StringContent(JsonConvert.SerializeObject(novoHistorico), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/HistoricoInteracoes", content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
