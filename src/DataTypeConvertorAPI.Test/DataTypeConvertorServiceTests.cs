﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
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

        [SetUp]
        public void Setup()
        {
            TestFilesPath = ConfigurationManager.AppSettings["TestFiles"];
        }

        [Test]
        public void should_generate_xml_from_csv()
        {
            var helper = new DataTypeConvertorHelper();

            var filter = "Antalya";
            var path = string.Format("{0}\\sample_data.csv", TestFilesPath);
            var exportpath = string.Format("{0}\\sample_data_export.xml", TestFilesPath);

            Assert.IsTrue(helper.Process(path, exportpath, filter));
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

            var addressInfo = FillAddressInfoFromCSV(path);

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

        private static AddressInfo FillAddressInfoFromCSV(string path)
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


    [Serializable]
    public class AddressInfo
    {
        public AddressInfo()
        {
            City = new List<AddressInfoCity>();
        }

        [XmlElement]
        public List<AddressInfoCity> City { get; set; }
    }

    [Serializable]
    public class AddressInfoCity
    {
        public AddressInfoCity()
        {
            District = new List<AddressInfoCityDistrict>();
        }

        [XmlAttributeAttribute]
        public string Name { get; set; }
        [XmlAttributeAttribute]
        public string Code { get; set; }
        [XmlElement]
        public List<AddressInfoCityDistrict> District { get; set; }
    }

    [Serializable]
    public class AddressInfoCityDistrict
    {
        public AddressInfoCityDistrict()
        {
            Zip = new List<AddressInfoCityDistrictZip>();
        }

        [XmlAttributeAttribute]
        public string Name { get; set; }
        [XmlElement]
        public List<AddressInfoCityDistrictZip> Zip { get; set; }
    }

    [Serializable]
    public class AddressInfoCityDistrictZip
    {
        [XmlAttributeAttribute]
        public string Code { get; set; }
    }
}
