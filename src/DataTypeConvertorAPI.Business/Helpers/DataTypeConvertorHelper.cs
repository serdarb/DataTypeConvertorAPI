using System;
using System.IO;
using DataTypeConvertorAPI.Business.Processors;

namespace DataTypeConvertorAPI.Business.Helpers
{
    public class DataTypeConvertorHelper
    {
        public bool Process(string path, string exportPath, string cityFilter = "", string sortField = "", bool isAscending = true)
        {
            if (string.IsNullOrEmpty(path) ||
                string.IsNullOrEmpty(exportPath))
            {
                return false;
            }

            if (!File.Exists(path))
            {
                return false;
            }

            var processor = GetProcessor(path, exportPath);
            return processor.Process(path, exportPath, cityFilter, sortField, isAscending);
        }

        private static IProcessor GetProcessor(string path, string exportPath)
        {
            var xmlPosfix = ".xml";
            var csvPosfix = ".csv";

            if (path.EndsWith(csvPosfix) &&
                exportPath.EndsWith(xmlPosfix))
            {
                return new CsvToXmlProcessor();
            }

            if (path.EndsWith(xmlPosfix) &&
               exportPath.EndsWith(csvPosfix))
            {
                return new XmlToCsvProcessor();
            }

            if (path.EndsWith(csvPosfix) &&
                exportPath.EndsWith(csvPosfix))
            {
                return new CsvToCsvProcessor();
            }

            throw new Exception(string.Format("Can't find expected Processor for {0} to {1}", path.Substring(path.Length - 4, 4), exportPath.Substring(exportPath.Length - 4, 4)));
        }
    }
}