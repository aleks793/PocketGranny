using System;
using System.Collections.Generic;

namespace PocketGranny
{
    [Serializable]
    public class ListCommodity
    {
        private string _name;

        private List<Commodity> _goods = new List<Commodity>();

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public List<Commodity> Goods
        {
            get => _goods;
            set => _goods = value;
        }

        public ListCommodity()
        {

        }

        public ListCommodity(string nameProduct)
        {
            Name = nameProduct;
        }

        public void Add(Commodity value)
        {
            Goods.Add(value);
        }

        public void RemoveAt(int index)
        {
            Goods.RemoveAt(index);
        }

        public float ChangeTo(int index, float weight)
        {
            if (weight < 0)
            {
                throw new ArgumentException($"Вес [{ weight }] не может быть отрицателен");
            }

            float differenceWeight = Goods[index].Weight - weight;

            if (weight == 0)
            {
                RemoveAt(index);
                return differenceWeight;
            }
            else if (weight > Goods[index].Weight)
            {
                differenceWeight = 0;
            }

            Goods[index].Weight = weight;

            return differenceWeight;
        }

        public Commodity FindElement(int index)
        {
            return Goods[index];
        }

        public int IndexOf(Commodity value)
        {
            return Goods.IndexOf(value);
        }

        public float AmountWeight()
        {
            float weight = 0;

            foreach (var i in Goods)
            {
                TimeSpan days = i.ExpiryDate.Subtract(DateTime.Today);

                if (days.TotalDays > 2)
                {
                    weight += i.Weight;
                }          
            }

            return weight;
        }

        public void ChangeOverTime(Commodity consumption)
        {
            List<int> indices = new List<int>();

            for (int i = 0; i < Goods.Count; i++)
            {
                if (Goods[i].Weight - consumption.Weight < 0)
                {
                    indices.Add(i);
                    consumption.Weight -= Goods[i].Weight;
                }
                else if (Goods[i].Weight - consumption.Weight == 0)
                {
                    Goods.RemoveAt(i);
                    break;
                }
                else
                {
                    Goods[i].Weight -= consumption.Weight;
                    break;
                }
            }

            if (indices.Count > 0)
            {
                indices.Reverse();

                foreach (var i in indices)
                {
                    Goods.RemoveAt(i);
                }
            }
        }

        public bool RemovingSpoiledProducts()
        {
            bool change = false;
            List<int> indices = new List<int>();

            for (int i = 0; i < Goods.Count; i++)
            {
                TimeSpan days = Goods[i].ExpiryDate.Subtract(DateTime.Today);

                if (days.TotalDays <= 2)
                {
                    change = true;
                    indices.Add(i);
                }
            }

            if (indices.Count > 0)
            {
                indices.Reverse();

                foreach (var i in indices)
                {
                    Goods.RemoveAt(i);
                }
            }

            return change;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is ListCommodity commodity && commodity.Name == Name)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void Print(string index)
        {
            for (int i = 0; i < Goods.Count; i++)
            {
                Console.WriteLine($"    [{ index }:{ i }] { Goods[i].ToString() }");
            }
        }

        public void PrintColor(string index)
        {
            for (int i = 0; i < Goods.Count; i++)
            {
                Console.ForegroundColor = Goods[i].GetColorProduct();
                Console.WriteLine($"    [{ index }:{ i }] { Goods[i].ToString() }");
            }
        }
    }
}