using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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
            var content = File.ReadAllLines(path);
            var length = content.Length;

            var addressInfo = new AddressInfo();
            for (var i = 1; i < length; i++)
            {
                var lineItems = content[i].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var cityName = lineItems[0];
                var cityCode = lineItems[1];
                var districtName = lineItems[2];
                var zipCode = lineItems[3];


            }




            return false;
        }
    }


    class AddressInfo
    {
        public List<AddressInfoCity> City { get; set; }
    }

    class AddressInfoCity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public List<AddressInfoCityDistrict> District { get; set; }
    }

    class AddressInfoCityDistrict
    {
        public string Name { get; set; }
        public List<AddressInfoCityDistrictZip> Zip { get; set; }
    }

    class AddressInfoCityDistrictZip
    {
        public string Code { get; set; }
    }
}
