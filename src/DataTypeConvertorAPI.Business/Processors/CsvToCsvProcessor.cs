using System;

namespace DataTypeConvertorAPI.Business.Processors
{
    internal class CsvToCsvProcessor : BaseProcessor
    {
        public override bool ReadFileToEntity(string path, string cityFilter = "")
        {
            try
            {
                AddressInfo = FillAddressInfoFromCSV(path, cityFilter);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override bool ExportFile(string exportPath)
        {
            return GenerateCSVFromAddressInfo(AddressInfo, exportPath);
        }
    }
}