namespace PSDUnity
{
    public interface INameAnalyzing<T>
    {
        T Analyzing(RuleObject Rule, string name);
    }
}