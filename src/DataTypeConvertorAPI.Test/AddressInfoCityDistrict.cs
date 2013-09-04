using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DataTypeConvertorAPI.Test
{
    [Serializable]
    public class AddressInfoCityDistrict
    {
        public AddressInfoCityDistrict()
        {
            Zip = new List<AddressInfoCityDistrictZip>();
        }

        [XmlAttribute]
        public string Name { get; set; }
        [XmlElement]
        public List<AddressInfoCityDistrictZip> Zip { get; set; }
    }
}