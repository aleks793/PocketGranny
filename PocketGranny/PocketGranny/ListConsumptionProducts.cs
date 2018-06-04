using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace PocketGranny
{
    [Serializable]
    public class ListConsumptionProducts
    {
        private TimeSpan _amountDays;

        private List<ConsumedCommodity> _products = new List<ConsumedCommodity>();

        public TimeSpan AmountDays
        {
            get => _amountDays;
            set => _amountDays = value;
        }

        public List<ConsumedCommodity> Products
        {
            get => _products;
            set => _products = value;
        }

        public ListConsumptionProducts()
        {
            
        }

        public ListConsumptionProducts(int amountDays)
        {
            AmountDays = new TimeSpan(amountDays, 0, 0, 0);
        }

        public void UpdateAmountDays(DateTime dateOfPurchase, DateTime previousDate)
        {
            double amountDays =  dateOfPurchase.Subtract(previousDate).TotalDays;
            double days = (AmountDays.TotalDays + amountDays) / 2;

            if (days < 1)
            {
                return;
            }

            AmountDays = new TimeSpan((int)days, 0, 0, 0);
        }

        public void Add(Product value, DateTime date)
        {
            var k = Products.FindIndex(
                delegate(ConsumedCommodity product)
                {
                    return product.Product.Name == value.Name;
                });

            if (k == -1)
            {
                try
                {
                    Products.Add(new StandardizedCommodity(value, 0, date));
                }
                catch (ArgumentException)
                {
                    Products.Add(new NonStandardizedCommodity(value, 0, date));
                }
            }
        }

        public void AddRange(List<Product> values, DateTime date)
        {
            foreach (var i in values)
            {
                Add(i, date);
            }
        }

        public void ChangeElement(Product value, float weight)
        {
            var k = Products.FindIndex(
                delegate (ConsumedCommodity product)
                {
                    return product.Product.Name == value.Name;
                });

            if (k == -1 || weight < 0)
            {
                return;
            }

            Products[k].Weight += weight;
        }

        private float ConsumptionPerDay(ConsumedCommodity product)
        {
            TimeSpan days = DateTime.Today.Subtract(product.DateAdded);

            if (days.TotalDays > 1)
            {
                return product.Weight / (float)days.TotalDays;
            }

            return product.Weight;
        }

        public List<ConsumedCommodity> RecommendedProducts()
        {
            List<ConsumedCommodity> products = new List<ConsumedCommodity>();

            foreach (var i in Products)
            {
                if (i.Weight == 0)
                {
                    continue;
                }

                float weight = ConsumptionPerDay(i) * (float)AmountDays.TotalDays;

                if (i is StandardizedCommodity)
                {
                    var standardizedCommodity = i as StandardizedCommodity;
                    products.Add(new StandardizedCommodity(standardizedCommodity.Product, weight, DateTime.Today, standardizedCommodity.VolumesOfPackages));
                }
                else
                {
                    products.Add(new NonStandardizedCommodity(i.Product, weight, DateTime.Today));
                }
            }

            return products;
        }


        public void Save(string fileName)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(ListConsumptionProducts));

            using (Stream fStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(fStream, this);
            }
        }

        public void Load(string fileName)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(ListConsumptionProducts));

            using (Stream fStream = File.OpenRead(fileName))
            {
                var list = (ListConsumptionProducts)formatter.Deserialize(fStream);

                AmountDays = list.AmountDays;

                Products = list.Products;
            }
        }
    }
}