using System;
using System.Collections.Generic;

namespace PocketGranny
{
    [Serializable]
    public class GroupCommodity
    {
        private string _nameCategory;

        private List<ListCommodity> _element = new List<ListCommodity>();

        public string NameCategory
        {
            get => _nameCategory;
            set => _nameCategory = value;
        }

        public List<ListCommodity> Elements
        {
            get => _element;
            set => _element = value;
        }

        public GroupCommodity()
        {

        }

        public GroupCommodity(string nameCategory)
        {
            NameCategory = nameCategory;
        }

        public void Add(Commodity value)
        {
            try
            {
                Elements.Find(
                delegate (ListCommodity listElements)
                {
                    return listElements.Name == value.Product.Name;
                }).Add(value);
            }
            catch (NullReferenceException)
            {
                var listCommodity = new ListCommodity(value.Product.Name);

                listCommodity.Add(value);
                Elements.Add(listCommodity);
            }
        }

        public void RemoveAt(int[] identifier)
        {
            if (identifier.Length != 2)
            {
                throw new ArgumentException("Формат идентификатора не верен, должно быть два индекса");
            }

            try
            {
                Elements[identifier[0]].RemoveAt(identifier[1]);

                if (Elements[identifier[0]].Goods.Count == 0)
                {
                    Elements.RemoveAt(identifier[0]);
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentException($"Третье значение [{ identifier[1] }] в идентификаторе находится за пределами допустимого диаппазона");
            }
            catch (ArgumentException)
            {
                throw;
            }
        }

        public float ChangeTo(int[] identifier, float weight)
        {
            if (identifier.Length != 2)
            {
                throw new ArgumentException("Формат идентификатора не верен, должно быть два индекса");
            }

            try
            {
                float changeWeight = Elements[identifier[0]].ChangeTo(identifier[1], weight);

                if (Elements[identifier[0]].Goods.Count == 0)
                {
                    Elements.RemoveAt(identifier[0]);
                }

                return changeWeight;
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentException($"Третье значение [{ identifier[1] }] в идентификаторе находится за пределами допустимого диаппазона");
            }
            catch (ArgumentException)
            {
                throw;
            }
        }

        public Commodity FindElement(int[] identifier)
        {
            if (identifier.Length != 2)
            {
                throw new ArgumentException("Формат идентификатора не верен, должно быть два индекса");
            }

            try
            {
                return Elements[identifier[0]].FindElement(identifier[1]);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentException($"Третье значение [{ identifier[1] }] в идентификаторе находится за пределами допустимого диаппазона");
            }
            catch (ArgumentException)
            {
                throw;
            }
        }

        public int[] IndexOf(Commodity value)
        {
            int[] identifier = new int[2];

            identifier[0] = Elements.FindIndex(
                delegate (ListCommodity listElements)
                {
                    return listElements.Name == value.Product.Name;
                });

            if (identifier[0] == -1)
            {
                identifier[1] = -1;

                return identifier;
            }

            identifier[1] = Elements[identifier[0]].IndexOf(value);

            return identifier;
        }

        public void ElementMerge(ref Dictionary<string, float> elements)
        {
            foreach (var i in Elements)
            {
                elements.Add(i.Name, i.AmountWeight());
            }
        }

        public bool ChangeOverTime(List<Commodity> consumption)
        {
            bool change = false;
            List<int> indices = new List<int>();

            for (int i = 0; i < Elements.Count; i++)
            {
                Commodity element = consumption.Find(
                delegate (Commodity value)
                {
                    return value.Product.Name == Elements[i].Name;
                });

                if (element == null)
                {
                    continue;
                }

                Elements[i].ChangeOverTime(element);

                if (!change)
                {
                    change = true;
                }

                if (Elements[i].Goods.Count == 0)
                {
                    indices.Add(i);
                }
            }

            if (indices.Count > 0)
            {
                indices.Reverse();

                foreach (var i in indices)
                {
                    Elements.RemoveAt(i);
                }
            }

            return change;
        }

        public bool RemovingSpoiledProducts()
        {
            bool change = false;
            List<int> indices = new List<int>();

            for (int i = 0; i < Elements.Count; i++)
            {
                bool k = Elements[i].RemovingSpoiledProducts();

                if (!change)
                {
                    change = k;
                }

                if (Elements[i].Goods.Count == 0)
                {
                    indices.Add(i);
                }
            }

            if (indices.Count > 0)
            {
                indices.Reverse();

                foreach (var i in indices)
                {
                    Elements.RemoveAt(i);
                }
            }

            return change;
        }

        public void GetProductsAll(ref List<string> elements)
        {
            foreach (var i in Elements)
            {
                elements.Add(i.Name);
            }
        }

        public List<Commodity> GetCommodityAll(ref List<Commodity> elements)
        {
            foreach (var i in Elements)
            {
                elements.AddRange(i.Goods); ;
            }

            return elements;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is GroupCommodity product && product.NameCategory == NameCategory)
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
            for (int i = 0; i < Elements.Count; i++)
            {
                Console.WriteLine($"  [{ index }:{ i }] { Elements[i].Name }:");
                Elements[i].Print($"{ index }:{ i }");
            }
        }

        public void PrintColor(string index)
        {
            for (int i = 0; i < Elements.Count; i++)
            {
                Console.WriteLine($"  [{ index }:{ i }] { Elements[i].Name }:");
                Elements[i].PrintColor($"{ index }:{ i }");
                Console.ResetColor();
            }
        }
    }
}