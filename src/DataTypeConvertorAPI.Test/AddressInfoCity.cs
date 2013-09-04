using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DataTypeConvertorAPI.Test
{
    [Serializable]
    public class AddressInfoCity
    {
        public AddressInfoCity()
        {
            District = new List<AddressInfoCityDistrict>();
        }

        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Code { get; set; }
        [XmlElement]
        public List<AddressInfoCityDistrict> District { get; set; }
    }
}