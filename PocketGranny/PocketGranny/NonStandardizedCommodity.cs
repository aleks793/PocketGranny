using System;

namespace PocketGranny
{
    [Serializable]
    public class NonStandardizedCommodity : ConsumedCommodity
    {
        public NonStandardizedCommodity()
        {

        }

        public NonStandardizedCommodity(Product product, float weight)
        {
            Product = product;
            Weight = weight;
            DateAdded = DateTime.Today;
        }

        public NonStandardizedCommodity(Product product, float weight, DateTime date)
        {
            Product = product;
            Weight = weight;
            DateAdded = date;
        }
    }
}
