namespace PSDUnity.Data
{
    public interface INameAnalyzing<T>
    {
        T Analyzing(RouleObject roule, string name);
    }
}