namespace JsonVai.Models
{
    public interface IJsonFileService<T>
    {
        List<T> LerDados();
        void GravarDados(List<T> dados);
    }
}
