namespace CLib1
{
    public interface IConverterManager
    {
        IParameterConverter GetConverter(string name);
    }
}