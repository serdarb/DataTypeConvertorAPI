﻿using System;
using System.Linq;

namespace DataTypeConvertorAPI.Test
{
    public class CsvToXmlProcessor : BaseProcessor
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
            return GenerateXMLFromAddressInfo(AddressInfo, exportPath);
        }
    }
}