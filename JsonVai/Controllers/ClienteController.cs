using JsonVai.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OpenAI_API;
using OpenAI_API.Completions;
using System.Linq;
using System.Text;

namespace JsonVai.Controllers
{
    public class ClienteController : Controller
    {
        private readonly JsonFileService<Cliente> _jsonFileService;

        public ClienteController(JsonFileService<Cliente> jsonFileService)
        {
            _jsonFileService = jsonFileService;
        }

        public IActionResult Index()
        {
            var clientes = _jsonFileService.LerDados();
            return View(clientes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Cliente novoCliente)
        {
            if (ModelState.IsValid)
            {
                var clientes = _jsonFileService.LerDados();
                novoCliente.Id = clientes.Count + 1;
                clientes.Add(novoCliente);
                _jsonFileService.GravarDados(clientes);
                return RedirectToAction("Index");
            }
            return View(novoCliente);
        }

        public IActionResult Edit(int id)
        {
            var clientes = _jsonFileService.LerDados();
            var cliente = clientes.FirstOrDefault(c => c.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        [HttpPost]
        public IActionResult Edit(Cliente clienteEditado)
        {
            if (ModelState.IsValid)
            {
                var clientes = _jsonFileService.LerDados();
                var clienteExistente = clientes.FirstOrDefault(c => c.Id == clienteEditado.Id);
                if (clienteExistente != null)
                {
                    clienteExistente.Nome = clienteEditado.Nome;
                    clienteExistente.Email = clienteEditado.Email;
                    _jsonFileService.GravarDados(clientes);
                    return RedirectToAction("Index");
                }
            }
            return View(clienteEditado);
        }

        public IActionResult Delete(int id)
        {
            var clientes = _jsonFileService.LerDados();
            var cliente = clientes.FirstOrDefault(c => c.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var clientes = _jsonFileService.LerDados();
            var cliente = clientes.FirstOrDefault(c => c.Id == id);
            if (cliente != null)
            {
                clientes.Remove(cliente);
                _jsonFileService.GravarDados(clientes);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Pesquisar()
        {
            return View();
        }
        /*
        [HttpPost]
        public IActionResult Pesquisar(string pergunta)
        {
            var respostaAssistente = PesquisarComCatGPT(pergunta);

            var clientes = _jsonFileService.LerDados();
            var resultados = clientes.Where(c => c.Nome.Contains(respostaAssistente, StringComparison.OrdinalIgnoreCase)).ToList();

            ViewBag.Pergunta = pergunta;
            ViewBag.Resultados = resultados;

            return View();
        }*/
        [HttpPost]
        public IActionResult Pesquisar(string pergunta)
        {
            var respostaAssistente = PesquisarComCatGPT(pergunta);

            var clientes = _jsonFileService.LerDados();
            var resultados = clientes.Where(c => c.Nome.Contains(respostaAssistente, StringComparison.OrdinalIgnoreCase)).ToList();

            ViewBag.Pergunta = pergunta;
            ViewBag.Resultados = resultados;

            return View();
        }

        private string PesquisarComCatGPT(string pergunta)
        {
            string apiKey = "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";
            string resposta = string.Empty;

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var prompt = $"Usuário: {pergunta}\nAssistente: ";
                var requestBody = new
                {
                    prompt = prompt,
                    max_tokens = 50
                };

                var jsonRequest = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                    resposta = result.choices[0].text;
                }
            }

            return resposta;
        }
        /*
         * 
        private string PesquisarComCatGPT(string pergunta)
        {
            string APIKey = "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";
            string answer = string.Empty;
            var openAI = new OpenAIAPI(APIKey);
            var prompt = $"Usuário: {pergunta}\nAssistente: ";

            CompletionRequest completion = new CompletionRequest();
            completion.Prompt = prompt;
            completion.Model = OpenAI_API.Models.Model.DavinciText;
            completion.MaxTokens = 50;

            var result = openAI.Completions.CreateCompletionAsync(completion).Result;
            foreach (var item in result.Completions)
            {
                answer = item.Text;
            }

            return answer;
        }
        */
    }

}