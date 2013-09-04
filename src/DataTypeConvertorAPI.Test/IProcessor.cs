namespace DataTypeConvertorAPI.Test
{
    public interface IProcessor
    {
        bool Process(string path, string exportPath, string cityFilter);
    }
}