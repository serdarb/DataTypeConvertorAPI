using System;
using System.Xml.Serialization;

namespace DataTypeConvertorAPI.Business.Entities
{
    [Serializable]
    public class AddressInfoCityDistrictZip
    {
        [XmlAttribute(AttributeName = "code")]
        public string Code { get; set; }
    }
}