using System;
using System.Xml.Serialization;

namespace PocketGranny
{
    [XmlInclude(typeof(StandardizedCommodity)), XmlInclude(typeof(NonStandardizedCommodity))]
    public abstract class ConsumedCommodity
    {
        public Product Product { get; set; }

        public float Weight { get; set; }

        public DateTime DateAdded { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is ConsumedCommodity product && product.Product.Name == Product.Name)
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
            if (Product.WeightUnit.Length > 1 && Weight >= 1000)
            {
                return $"{ Product.Name }({ Weight.ToString() } {Product.WeightUnit[1]})";
            }

            return $"{ Product.Name }({ Weight.ToString() } {Product.WeightUnit[0]})";
        }
    }
}