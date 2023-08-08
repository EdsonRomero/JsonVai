using Newtonsoft.Json;
using System.Xml;

namespace JsonVai.Models
{
    

    public class JsonFileService<T> : IJsonFileService<T>
    {
        private readonly string _caminhoArquivo = "Data/arquivo.json";

        public List<T> LerDados()
        {
            using (var arquivo = File.OpenText(_caminhoArquivo))
            {
                var conteudo = arquivo.ReadToEnd();
                return JsonConvert.DeserializeObject<List<T>>(conteudo);
            }
        }

        public void GravarDados(List<T> dados)
        {
            var conteudo = JsonConvert.SerializeObject(dados, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(_caminhoArquivo, conteudo);
        }
    }
}