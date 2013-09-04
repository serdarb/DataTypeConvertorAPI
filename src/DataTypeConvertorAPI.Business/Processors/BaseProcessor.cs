using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using DataTypeConvertorAPI.Business.Entities;
using DataTypeConvertorAPI.Business.Helpers;

namespace DataTypeConvertorAPI.Business.Processors
{
    public abstract class BaseProcessor : IProcessor
    {
        public AddressInfo AddressInfo { get; set; }


        protected bool GenerateXMLFromAddressInfo(AddressInfo addressInfo, string exportPath)
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

        protected bool GenerateCSVFromAddressInfo(AddressInfo addressInfo, string exportPath)
        {
            try
            {
                var lines = new List<string>();
                foreach (var item in addressInfo.City)
                {
                    foreach (var subItem in item.District)
                    {
                        foreach (var subzip in subItem.Zip)
                        {
                            lines.Add(string.Format("{0},{1},{2},{3}", item.Name, item.Code, subItem.Name, subzip.Code));
                        }
                    }
                }

                File.WriteAllLines(exportPath, lines);
                return true;
            }
            catch (Exception ex)
            {
                //todo: log
                return false;
            }
        }

        protected static AddressInfo FillAddressInfoFromCSV(string path, string cityFilter)
        {
            var content = File.ReadAllLines(path).ToList()
                .OrderBy(line => line) //sorted by City name ascending, then District name ascending
                .ToArray();
            var length = content.Length;

            var addressInfo = new AddressInfo();
            for (var i = 1; i < length; i++)
            {
                var lineItems = content[i].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var cityName = lineItems[0];
                var cityCode = lineItems[1];
                var districtName = lineItems[2];
                var zipCode = lineItems[3];

                if (!string.IsNullOrEmpty(cityFilter))
                {
                    var loweredFilter = cityFilter.ToLower(new CultureInfo("tr-TR"));
                    var loweredCityName = cityName.ToLower(new CultureInfo("tr-TR"));
                    if (loweredCityName != loweredFilter)
                    {
                        continue;
                    }
                }

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

        protected static AddressInfo FillAddressInfoFromXml(string path, string cityFilter)
        {
            var content = File.ReadAllText(path);
            var addressInfo = SerializationHelper.Deserialize<AddressInfo>(content);

            if (!string.IsNullOrEmpty(cityFilter))
            {
                var loweredFilter = cityFilter.ToLower(new CultureInfo("tr-TR"));
                addressInfo.City.RemoveAll(x => x.Name.ToLower(new CultureInfo("tr-TR")) != loweredFilter);
            }

            return addressInfo;
        }


        public abstract bool ReadFileToEntity(string path, string cityFilter = "");
        public abstract bool ExportFile(string exportPath);

        public bool SortEntity(string sortField, bool isAscending = true)
        {
            try
            {
                if (!string.IsNullOrEmpty(sortField))
                {
                    if (isAscending)
                    {
                        if (sortField == "CityName")
                        {
                            AddressInfo.City = AddressInfo.City.OrderBy(x => x.Name).ToList();
                        }
                    }
                    else
                    {
                        if (sortField == "CityName")
                        {
                            AddressInfo.City = AddressInfo.City.OrderByDescending(x => x.Name).ToList();
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Process(string path, string exportPath, string cityFilter = "", string sortField = "", bool isAscending = true)
        {
            try
            {
                if (ReadFileToEntity(path, cityFilter))
                {
                    if (SortEntity(sortField, isAscending))
                    {
                        return ExportFile(exportPath);
                    }
                }
            }
            catch (Exception)
            {
                
            }

            return false;
        }
    }
}