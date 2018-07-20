namespace PSDUnity
{
    public interface INameAnalyzing<T>
    {
        T Analyzing(Data.RuleObject Rule, string name);
    }
}