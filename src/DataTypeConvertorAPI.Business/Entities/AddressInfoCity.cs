using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DataTypeConvertorAPI.Business.Entities
{
    [Serializable]
    public class AddressInfoCity
    {
        public AddressInfoCity()
        {
            District = new List<AddressInfoCityDistrict>();
        }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "code")]
        public string Code { get; set; }
        [XmlElement]
        public List<AddressInfoCityDistrict> District { get; set; }
    }
}