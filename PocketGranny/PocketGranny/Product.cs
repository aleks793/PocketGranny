using System;
using System.Xml;
using System.Xml.Serialization;

namespace PocketGranny
{
    [Serializable]
    public class Product
    {
        private string _categoryName;

        private string _name;

        private TimeSpan _expiryIn;

        private string[] _weightUnit;

        [XmlAttribute(AttributeName = "categoryName")]
        public string CategoryName
        {
            get => _categoryName;
            set => _categoryName = value;
        }

        [XmlAttribute(AttributeName = "name")]
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        [XmlElement(ElementName = "expiryIn", Type = typeof(XmlTimeSpan))]
        public TimeSpan ExpiryIn
        {
            get => _expiryIn;
            set => _expiryIn = value;
        }

        [XmlArray(ElementName = "weightUnit")]
        public string[] WeightUnit
        {
            get => _weightUnit;
            set => _weightUnit = value;
        }

        public Product()
        {

        }

        public Product(string categoryName, string name, TimeSpan expiryIn, string[] weightUnit)
        {
            CategoryName = categoryName;
            Name = name;
            ExpiryIn = expiryIn;
            WeightUnit = weightUnit;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is Product product && product.Name == Name)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}