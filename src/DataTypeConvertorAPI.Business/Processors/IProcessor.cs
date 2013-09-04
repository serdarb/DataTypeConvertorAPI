namespace DataTypeConvertorAPI.Business.Processors
{
    public interface IProcessor
    {
        bool ReadFileToEntity(string path, string cityFilter);
        bool SortEntity(string sortField, bool isAscending);
        bool ExportFile(string exportPath);

        bool Process(string path, string exportPath, string cityFilter, string sortField, bool isAscending);
    }
}