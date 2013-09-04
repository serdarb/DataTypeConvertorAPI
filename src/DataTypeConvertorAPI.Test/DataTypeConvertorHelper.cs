using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace DataTypeConvertorAPI.Test
{
    public class DataTypeConvertorHelper
    {
        public bool Process(string path, string exportPath, string cityFilter = "")
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
            return processor.Process(path, exportPath, cityFilter);
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

    internal class CsvToCsvProcessor : BaseProcessor, IProcessor
    {
        public bool Process(string path, string exportPath, string cityFilter)
        {
            var addressInfo = FillAddressInfoFromCSV(path, cityFilter);
            return GenerateXMLFromAddressInfo(addressInfo, exportPath);
        }
    }

    internal class XmlToCsvProcessor : BaseProcessor, IProcessor
    {
        public bool Process(string path, string exportPath, string cityFilter)
        {
            var addressInfo = FillAddressInfoFromXml(path, cityFilter);
            return GenerateCSVFromAddressInfo(addressInfo, exportPath);
        }
    }

    public class CsvToXmlProcessor : BaseProcessor, IProcessor
    {
        public bool Process(string path, string exportPath, string cityFilter)
        {
            var addressInfo = FillAddressInfoFromCSV(path, cityFilter);
            return GenerateXMLFromAddressInfo(addressInfo, exportPath);
        }
    }

    public class BaseProcessor
    {
        protected bool GenerateXMLFromAddressInfo(AddressInfo addressInfo, string exportPath)
        {
            try
            {
                var xmlContent = SerializationHelper.Serialize(addressInfo);
                File.WriteAllText(exportPath, xmlContent);
                return true;
            }
            catch (Exception ex)
            {
                //todo: log
                return false;
            }
        }

        protected bool GenerateCSVFromAddressInfo(AddressInfo addressInfo, string exportPath)
        {
            try
            {
                var lines = new List<string>();
                foreach (var item in addressInfo.City)
                {
                    foreach (var subItem in item.District)
                    {
                        foreach (var subzip in subItem.Zip)
                        {
                            lines.Add(string.Format("{0},{1},{2},{3}", item.Name, item.Code, subItem.Name, subzip.Code));
                        }
                    }
                }

                File.WriteAllLines(exportPath, lines);
                return true;
            }
            catch (Exception ex)
            {
                //todo: log
                return false;
            }
        }

        protected static AddressInfo FillAddressInfoFromCSV(string path, string cityFilter)
        {
            var content = File.ReadAllLines(path).ToList().OrderBy(line => line).ToArray();
            var length = content.Length;

            var addressInfo = new AddressInfo();
            for (var i = 1; i < length; i++)
            {
                var lineItems = content[i].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var cityName = lineItems[0];
                var cityCode = lineItems[1];
                var districtName = lineItems[2];
                var zipCode = lineItems[3];

                if (!string.IsNullOrEmpty(cityFilter))
                {
                    var loweredFilter = cityFilter.ToLower(new CultureInfo("tr-TR"));
                    var loweredCityName = cityName.ToLower(new CultureInfo("tr-TR"));
                    if (loweredCityName != loweredFilter)
                    {
                        continue;
                    }
                }

                if (addressInfo.City.Exists(x => x.Name == cityName))
                {
                    var districts = addressInfo.City.First(x => x.Name == cityName).District;
                    if (districts.Exists(x => x.Name == districtName))
                    {
                        var zips = districts.First(x => x.Name == districtName).Zip;
                        if (!zips.Exists(x => x.Code == zipCode))
                        {
                            zips.Add(new AddressInfoCityDistrictZip { Code = zipCode });
                        }
                    }
                    else
                    {
                        var aicd = new AddressInfoCityDistrict { Name = districtName };
                        aicd.Zip.Add(new AddressInfoCityDistrictZip { Code = zipCode });
                        districts.Add(aicd);
                    }
                }
                else
                {
                    var aic = new AddressInfoCity { Name = cityName, Code = cityCode };
                    var aicd = new AddressInfoCityDistrict { Name = districtName };
                    aicd.Zip.Add(new AddressInfoCityDistrictZip { Code = zipCode });

                    aic.District.Add(aicd);
                    addressInfo.City.Add(aic);
                }
            }

            return addressInfo;
        }

        protected static AddressInfo FillAddressInfoFromXml(string path, string cityFilter)
        {
            var content = File.ReadAllText(path);
            var addressInfo = SerializationHelper.Deserialize<AddressInfo>(content);

            if (!string.IsNullOrEmpty(cityFilter))
            {
                var loweredFilter = cityFilter.ToLower(new CultureInfo("tr-TR"));
                addressInfo.City.RemoveAll(x => x.Name.ToLower(new CultureInfo("tr-TR")) != loweredFilter);
            }

            return addressInfo;
        }
    }
}