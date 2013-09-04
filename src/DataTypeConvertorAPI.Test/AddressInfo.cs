using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DataTypeConvertorAPI.Test
{
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
}