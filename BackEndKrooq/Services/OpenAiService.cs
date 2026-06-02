using System.Text;
using System.Text.Json;

namespace BackEndKrooq.Services
{
    public class OpenAiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public OpenAiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GerarRespostaAsync(string prompt)
        {
            var apiKey = _configuration["OpenAI:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return "A chave da IA não foi configurada no servidor.";
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var body = new
            {
                model = "gpt-4o-mini",
                input = prompt
            };

            var json = JsonSerializer.Serialize(body);

            var response = await _httpClient.PostAsync(
                "https://api.openai.com/v1/responses",
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("========== ERRO DA OPENAI ==========");
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                Console.WriteLine(responseContent);
                Console.WriteLine("====================================");

                return "Não consegui gerar uma resposta agora. Tente novamente em alguns instantes.";
            }

            using var document = JsonDocument.Parse(responseContent);

            var outputText = document.RootElement
                .GetProperty("output")
                .EnumerateArray()
                .SelectMany(item => item.GetProperty("content").EnumerateArray())
                .Where(content => content.GetProperty("type").GetString() == "output_text")
                .Select(content => content.GetProperty("text").GetString())
                .FirstOrDefault();

            return outputText ?? "Não consegui interpretar a resposta da IA.";
        }
    }
}