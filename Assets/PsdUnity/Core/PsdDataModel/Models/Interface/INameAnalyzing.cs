namespace PSDUnity.Data
{
    public interface INameAnalyzing<T>
    {
        T Analyzing(RuleObject Rule, string name);
    }
}