using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
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
        [Test]
        public void should_generate_xml_from_csv()
        {
            var helper = new DataTypeConvertorHelper();

            var filter = "Antalya";
            var path = string.Format("{0}\\sample_data.csv", ConfigurationManager.AppSettings["TestFiles"]);
            var fileStream = new FileStream(path, FileMode.Open);

            Assert.IsTrue(helper.Process(fileStream, filter));
        }
    }

    public class DataTypeConvertorHelper
    {
        public bool Process(FileStream fileStream, string filter)
        {
            throw new NotImplementedException();
        }
    }
}
