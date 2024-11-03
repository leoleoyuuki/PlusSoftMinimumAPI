using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MarketAnalysisApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private const string AlphaVantageApiKey = "YOUR_API_KEY"; // Substitua pela sua chave de API

        public MarketController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet("stock/{symbol}")]
        public async Task<IActionResult> GetStockData(string symbol)
        {
            var url = $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={symbol}&apikey={AlphaVantageApiKey}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Erro ao obter dados do mercado.");
            }

            var jsonData = await response.Content.ReadAsStringAsync();
            var data = JObject.Parse(jsonData);

            // Verifica se a resposta contém dados
            if (data["Time Series (Daily)"] == null)
            {
                return NotFound("Dados não encontrados para o símbolo fornecido.");
            }

            // Processar os dados para obter o preço de fechamento
            var timeSeries = data["Time Series (Daily)"].Children<JProperty>().ToList();

            if (timeSeries.Count < 2)
            {
                return NotFound("Dados insuficientes para calcular o crescimento.");
            }

            // Obter os preços de fechamento das duas datas mais recentes
            var latestCloseData = timeSeries[0].Value;
            var previousCloseData = timeSeries[1].Value;

            double latestClose = double.Parse(latestCloseData["4. close"].ToString());
            double previousClose = double.Parse(previousCloseData["4. close"].ToString());
            double growthPercentage = CalculateGrowthPercentage(previousClose, latestClose);

            var result = new
            {
                Symbol = symbol,
                LatestClose = latestClose,
                PreviousClose = previousClose,
                GrowthPercentage = growthPercentage
            };

            return Ok(result);
        }

        [HttpGet("symbols")]
        public IActionResult GetSymbols()
        {
            // Retorna uma string descritiva sobre onde encontrar símbolos de ações
            string message = "Você pode encontrar símbolos de ações em sites como Yahoo Finance, Google Finance, ou através de APIs como Alpha Vantage e IEX Cloud. " +
                             "Por exemplo, para buscar símbolos de ações, você pode visitar: " +
                             "https://finance.yahoo.com/lookup/ .";
            return Ok(message);
        }

        private double CalculateGrowthPercentage(double oldValue, double newValue)
        {
            if (oldValue == 0) return 0; // Evitar divisão por zero
            return ((newValue - oldValue) / oldValue) * 100;
        }
    }
}