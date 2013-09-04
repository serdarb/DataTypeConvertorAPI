using System.Configuration;
using System.IO;
using Moq;
using NUnit.Framework;

namespace DataTypeConvertorAPI.Test
{
    [TestFixture]
    public class DataTypeConvertorHelperTests
    {
        private DataTypeConvertorHelper helper;
        private string TestFilesPath;
        private string CsvPath;
        private string XmlPath;
        private string ExportXmlPath;
        private string ExportCsvPath;

        [SetUp]
        public void Setup()
        {
            helper = new DataTypeConvertorHelper();

            TestFilesPath = ConfigurationManager.AppSettings["TestFiles"];

            CsvPath = string.Format("{0}\\sample_data.csv", TestFilesPath);
            XmlPath = string.Format("{0}\\sample_data.xml", TestFilesPath);
            ExportXmlPath = string.Format("{0}\\sample_data_export.xml", TestFilesPath);
            ExportCsvPath = string.Format("{0}\\sample_data_export.csv", TestFilesPath);

            if (File.Exists(ExportXmlPath))
            {
                File.Delete(ExportXmlPath);
            }
        }

        [Test]
        public void should_generate_xml_from_csv()
        {
            Assert.IsTrue(helper.Process(CsvPath, ExportXmlPath));
            Assert.IsTrue(File.Exists(ExportXmlPath));
        }

        [Test]
        public void should_generate_xml_from_csv_and_filter_city(
            [Values("Ankara", "Antalya")] string cityFilter)
        {
            Assert.IsTrue(helper.Process(CsvPath, ExportXmlPath, cityFilter));
            Assert.IsTrue(File.Exists(ExportXmlPath));

            var content = File.ReadAllText(ExportXmlPath);
            Assert.IsTrue(content.Contains(cityFilter));

            var anotherCityCode = ",34,";
            Assert.IsFalse(content.Contains(anotherCityCode));
        }

        [Test]
        public void should_generate_csv_from_csv()
        {
            Assert.IsTrue(helper.Process(CsvPath, ExportCsvPath));
            Assert.IsTrue(File.Exists(ExportCsvPath));
        }

        [Test]
        public void should_generate_csv_from_xml()
        {
            Assert.IsTrue(helper.Process(XmlPath, ExportCsvPath));
            Assert.IsTrue(File.Exists(ExportCsvPath));
        }

        [Test]
        public void should_generate_csv_from_xml_and_filter_city(
            [Values("Ankara")] string cityFilter)
        {
            Assert.IsTrue(helper.Process(XmlPath, ExportCsvPath, cityFilter));
            Assert.IsTrue(File.Exists(ExportCsvPath));

            var content = File.ReadAllText(ExportCsvPath);
            Assert.IsTrue(content.Contains(cityFilter));

            var anotherCityCode = ",34,";
            Assert.IsFalse(content.Contains(anotherCityCode));
        }

        [Test]
        public void should_generate_csv_from_xml_and_filter_city_and_sorted_by_cityname(
            [Values("Ankara")] string cityFilter)
        {
            const string sortField = "CityName";
            const bool isAscending = false;

            Assert.IsTrue(helper.Process(XmlPath, ExportCsvPath, cityFilter, sortField, isAscending));
            Assert.IsTrue(File.Exists(ExportCsvPath));

            var firstLine = "Ankara,06,Akyurt,06750";

            var content = File.ReadAllText(ExportCsvPath);
            Assert.IsTrue(content.StartsWith(firstLine));
        }
    }
}
