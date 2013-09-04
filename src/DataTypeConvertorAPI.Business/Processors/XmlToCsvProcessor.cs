using System;

namespace DataTypeConvertorAPI.Business.Processors
{
    internal class XmlToCsvProcessor : BaseProcessor
    {
        public override bool ReadFileToEntity(string path, string cityFilter = "")
        {
            try
            {
                AddressInfo = FillAddressInfoFromXml(path, cityFilter);
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