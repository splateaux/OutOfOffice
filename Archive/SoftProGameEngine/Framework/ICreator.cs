namespace SoftProGameEngine.Framework
{
    public interface ICreator
    {
        T CreateInstance<T>(string className);
    }
}