using System;
using System.Xml.Serialization;

namespace DataTypeConvertorAPI.Test
{
    [Serializable]
    public class AddressInfoCityDistrictZip
    {
        [XmlAttribute(AttributeName = "code")]
        public string Code { get; set; }
    }
}