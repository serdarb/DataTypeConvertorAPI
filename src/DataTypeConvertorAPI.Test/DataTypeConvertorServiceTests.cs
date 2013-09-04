using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace DataTypeConvertorAPI.Test
{

    /* 
     * 
     *  Test case #1:
     *      Generate XML output from CSV input,
     *      filtered by City name = ’Antalya’
     *  
     */

    [TestFixture]
    public class DataTypeConvertorHelperTests
    {
        private string TestFilesPath;
        private string CsvPath;
        private string ExportXmlPath;

        [SetUp]
        public void Setup()
        {
            TestFilesPath = ConfigurationManager.AppSettings["TestFiles"];

            CsvPath = string.Format("{0}\\sample_data.csv", TestFilesPath);
            ExportXmlPath = string.Format("{0}\\sample_data_export.xml", TestFilesPath);

            if (File.Exists(ExportXmlPath))
            {
                File.Delete(ExportXmlPath);
            }
        }

        [Test]
        public void should_generate_xml_from_csv()
        {
            var helper = new DataTypeConvertorHelper();

            var path = string.Format("{0}\\sample_data.csv", TestFilesPath);
            var exportpath = string.Format("{0}\\sample_data_export.xml", TestFilesPath);

            Assert.IsTrue(helper.Process(path, exportpath, string.Empty));
            Assert.IsTrue(File.Exists(exportpath));
        }

        [Test]
        public void should_generate_xml_from_csv_and_filter_city(
            [Values("Ankara", "Antalya")] string cityFilter)
        {
            var helper = new DataTypeConvertorHelper();

            if (File.Exists(ExportXmlPath))
            {
                File.Delete(ExportXmlPath);
            }

            Assert.IsTrue(helper.Process(CsvPath, ExportXmlPath, cityFilter));
            Assert.IsTrue(File.Exists(ExportXmlPath));

            var content = File.ReadAllText(ExportXmlPath);
            Assert.IsTrue(content.Contains(cityFilter));

            var anotherCityCode = ",34,";
            Assert.IsFalse(content.Contains(anotherCityCode));
        }
    }

    public class DataTypeConvertorHelper
    {
        public bool Process(string path, string exportPath, string filter)
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

            var addressInfo = FillAddressInfoFromCSV(path, filter);

            return GenerateXMLFromAddressInfo(addressInfo, exportPath);
        }

        private bool GenerateXMLFromAddressInfo(AddressInfo addressInfo, string exportPath)
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

        private static AddressInfo FillAddressInfoFromCSV(string path, string filter)
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

                if (!string.IsNullOrEmpty(filter))
                {
                    var loweredFilter = filter.ToLower(new CultureInfo("tr-TR"));
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
    }
}
