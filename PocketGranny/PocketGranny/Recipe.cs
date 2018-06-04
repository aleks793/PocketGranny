using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace PocketGranny
{
    [Serializable]
    public class Recipe
    {
        private string _categoryName;

        private string _name;

        private string _info;

        private float _weight;

        private TimeSpan _expiryIn;

        private XmlDictionary<Product, float> _products = new XmlDictionary<Product, float>();

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

        [XmlAttribute(AttributeName = "info")]
        public string Info
        {
            get => _info;
            set => _info = value;
        }     

        [XmlElement(ElementName = "weight")]
        public float Weight
        {
            get => _weight;
            set => _weight = value;
        }

        [XmlElement(ElementName = "expiryIn", Type = typeof(XmlTimeSpan))]
        public TimeSpan ExpiryIn
        {
            get => _expiryIn;
            set => _expiryIn = value;
        }

        [XmlElement(ElementName = "products")]
        public XmlDictionary<Product, float> Products
        {
            get => _products;
            set => _products = value;
        }

        [XmlArray(ElementName = "weightUnit")]
        public string[] WeightUnit
        {
            get => _weightUnit;
            set => _weightUnit = value;
        }

        public Recipe()
        {

        }

        public Recipe(string categoryName, string name, string info, float weight, TimeSpan date, XmlDictionary<Product, float> products, string[] weightUnit)
        {
            CategoryName = categoryName;
            Name = name;
            Info = info;
            Weight = weight;
            ExpiryIn = date;
            Products = products;
            WeightUnit = weightUnit;
        }

        public void SetWeight(float weight)
        {
            if (weight <= 0)
            {
                throw new ArgumentException($"Вес [{ weight }] не может быть отрицателен или равен 0");
            }

            float k = weight / Weight;

            List<Product> name = new List<Product>();

            foreach (var i in Products)
            {
                name.Add(i.Key);
            }

            foreach (var i in name)
            {
                Products[i] *= k;
            }

            Weight = weight;
        }

        public List<Commodity> GetProducts()
        {
            List<Commodity> products = new List<Commodity>();

            foreach (var i in Products)
            {
                var product = new Commodity(i.Key, i.Value, DateTime.Today);
                products.Add(product);
            }

            return products;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is Recipe recipe && recipe.Name == Name)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            if (WeightUnit.Length > 1 && Weight >= 1000)
            {
                return $"{ Name } ({ Weight } { WeightUnit[1] })";
            }

            return $"{ Name } ({ Weight } { WeightUnit[0] })";
        }
    }
}