using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace PocketGranny
{
    [Serializable]
    public class ListCategoriesCommodity
    {
        private DateTime _date;

        private List<GroupCommodity> _categories = new List<GroupCommodity>();

        public DateTime Date
        {
            get => _date;
            set => _date = value;
        }

        public List<GroupCommodity> Categories
        {
            get => _categories;
            set => _categories = value;
        }

        public ListCategoriesCommodity()
        {

        }

        public ListCategoriesCommodity(DateTime date)
        {
            Date = date;
        }

        public void Add(Commodity value)
        {
            try
            {
                Categories.Find(
                delegate (GroupCommodity groupedCommodity)
                {
                    return groupedCommodity.NameCategory == value.Product.CategoryName;
                }).Add(value);
            }
            catch (NullReferenceException)
            {
                var groupedCommodity = new GroupCommodity(value.Product.CategoryName);

                groupedCommodity.Add(value);
                Categories.Add(groupedCommodity);
            }
        }

        public void AddRange(List<Commodity> values)
        {
            foreach (var i in values)
            {
                Add(i);
            }
        }

        public void RemoveAt(int[] identifier)
        {
            if (identifier.Length != 3)
            {
                throw new ArgumentException("Формат идентификатора не верен, должно быть три индекса");
            }

            int[] levels = new int[identifier.Length - 1];
            Array.Copy(identifier, 1, levels, 0, identifier.Length - 1);

            try
            {
                Categories[identifier[0]].RemoveAt(levels);

                if (Categories[identifier[0]].Elements.Count == 0)
                {
                    Categories.RemoveAt(identifier[0]);
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentException($"Второе значение [{ identifier[1] }] в идентификаторе находится за пределами допустимого диаппазона");
            }
            catch (ArgumentException)
            {
                throw;
            }
        }

        public float ChangeTo(int[] identifier, float weight)
        {
            if (identifier.Length != 3)
            {
                throw new ArgumentException("Формат идентификатора не верен, должно быть три индекса");
            }

            int[] levels = new int[identifier.Length - 1];
            Array.Copy(identifier, 1, levels, 0, identifier.Length - 1);

            try
            {
                float changeWeight = Categories[identifier[0]].ChangeTo(levels, weight);

                if (Categories[identifier[0]].Elements.Count == 0)
                {
                    Categories.RemoveAt(identifier[0]);
                }

                return changeWeight;
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentException($"Второе значение [{ identifier[1] }] в идентификаторе находится за пределами допустимого диаппазона");
            }
            catch (ArgumentException)
            {
                throw;
            }
        }

        public Commodity FindElement(int[] identifier)
        {
            if (identifier.Length != 3)
            {
                throw new ArgumentException("Формат идентификатора не верен, должно быть три индекса");
            }

            int[] levels = new int[identifier.Length - 1];
            Array.Copy(identifier, 1, levels, 0, identifier.Length - 1);

            try
            {
                return Categories[identifier[0]].FindElement(levels);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentException($"Второе значение [{ identifier[1] }] в идентификаторе находится за пределами допустимого диаппазона");
            }
            catch (ArgumentException)
            {
                throw;
            }
        }

        public int[] IndexOf(Commodity value)
        {
            int[] identifier = new int[3];

            identifier[0] = Categories.FindIndex(
                delegate (GroupCommodity groupedCommodity)
                {
                    return groupedCommodity.NameCategory == value.Product.CategoryName;
                });

            if (identifier[0] == -1)
            {
                identifier[1] = -1;
                identifier[2] = -1;

                return identifier;
            }

            int[] id = Categories[identifier[0]].IndexOf(value);
            identifier[1] = id[0];
            identifier[2] = id[1];

            return identifier;
        }

        public Dictionary<string, float> ElementMerge()
        {
            Dictionary<string, float> elements = new Dictionary<string, float>();

            foreach (var i in Categories)
            {
                i.ElementMerge(ref elements);
            }

            return elements;
        }

        public List<Commodity> GetMissingProducts(List<Commodity> products)
        {
            Dictionary<string, float> k = ElementMerge();
            List<int> indices = new List<int>();

            for (int i = 0; i < products.Count; i++)
            {
                if (k.TryGetValue(products[i].Product.Name, out float weight))
                {
                    if (weight - products[i].Weight >= 0)
                    {
                        indices.Add(i);
                    }
                    else
                    {
                        products[i].Weight -= weight;
                    }
                }
            }

            if (indices.Count > 0)
            {
                indices.Reverse();

                foreach (var i in indices)
                {
                    products.RemoveAt(i);
                }
            }

            return products;
        }
        
        public List<Commodity> GetRecommendations(List<ConsumedCommodity> recommendations)
        {
            List<Commodity> listRecommendations = new List<Commodity>();
            Dictionary<string, float> elements = ElementMerge();

            foreach (var i in recommendations)
            {
                if (elements.TryGetValue(i.Product.Name, out float weight))
                {
                    if (i.Weight - weight > 0)
                    {
                        i.Weight -= weight;

                        var standardizedCommodity = i as StandardizedCommodity;

                        if (standardizedCommodity == null || standardizedCommodity.VolumesOfPackages.Length == 0)
                        {
                            listRecommendations.Add(new Commodity(i.Product, i.Weight, DateTime.Today));
                            continue;
                        }

                        i.Weight = StandardizedWeight(standardizedCommodity);
                        listRecommendations.Add(new Commodity(i.Product, i.Weight, DateTime.Today));
                    }
                }
                else
                {
                    listRecommendations.Add(new Commodity(i.Product, i.Weight, DateTime.Today));
                }
            }

            return listRecommendations;
        }

        private float StandardizedWeight(StandardizedCommodity product)
        {
            int index = 0;
            int fraction = 1;

            for (int k = 0; k < product.VolumesOfPackages.Length; k++)
            {
                int share = (int)(product.Weight % product.VolumesOfPackages[k]);

                if (share == 0)
                {
                    return product.VolumesOfPackages[k] * (product.Weight / product.VolumesOfPackages[k]);
                }
                else if (share > fraction)
                {
                    index = k;
                    fraction = share;
                }
            }

            return product.VolumesOfPackages[index] * ((product.Weight / product.VolumesOfPackages[index]) + 1);
        }


        public bool ChangeOverTime(List<Commodity> consumption)
        {
            bool change = false;
            List<int> indices = new List<int>();

            for (int i = 0; i < Categories.Count; i++)
            {
                List<Commodity> category = consumption.FindAll(
                delegate (Commodity element)
                {
                    return element.Product.CategoryName == Categories[i].NameCategory;
                });

                if (category.Count == 0)
                {
                    continue;
                }

                bool k = Categories[i].ChangeOverTime(category);

                if (!change)
                {
                    change = k;
                }

                if (Categories[i].Elements.Count == 0)
                {
                    indices.Add(i);
                }
            }

            if (indices.Count > 0)
            {
                indices.Reverse();

                foreach (var i in indices)
                {
                    Categories.RemoveAt(i);
                }
            }

            return change;
        }
        
        public bool RemovingSpoiledProducts()
        {
            bool change = false;
            List<int> indices = new List<int>();

            for (int i = 0; i < Categories.Count; i++)
            {
                bool k = Categories[i].RemovingSpoiledProducts();

                if (!change)
                {
                    change = k;
                }

                if (Categories[i].Elements.Count == 0)
                {
                    indices.Add(i);
                }
            }

            if (indices.Count > 0)
            {
                indices.Reverse();

                foreach (var i in indices)
                {
                    Categories.RemoveAt(i);
                }
            }

            return change;
        }

        public List<string> GetProductsAll()
        {
            List<string> products = new List<string>();

            foreach (var i in Categories)
            {
                i.GetProductsAll(ref products);
            }

            return products;
        }

        public List<Commodity> GetCommodityAll()
        {
            List<Commodity> products = new List<Commodity>();

            foreach (var i in Categories)
            {
                i.GetCommodityAll(ref products);
            }

            return products;
        }

        public void Clear()
        {
            Categories.Clear();
        }

        public void Print(bool color)
        {
            if (color)
            {
                for (int i = 0; i < Categories.Count; i++)
                {
                    Console.WriteLine($"[{ i }] {Categories[i].NameCategory}:");
                    Categories[i].PrintColor(i.ToString());
                }
            }
            else
            {
                for (int i = 0; i < Categories.Count; i++)
                {
                    Console.WriteLine($"[{ i }] {Categories[i].NameCategory}:");
                    Categories[i].Print(i.ToString());
                }
            }
        }

        public void Save(string fileName)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(ListCategoriesCommodity));

            using (Stream fStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(fStream, this);
            }
        }

        public void Load(string fileName)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(ListCategoriesCommodity));

            using (Stream fStream = File.OpenRead(fileName))
            {
                var categoriesCommodity = (ListCategoriesCommodity)formatter.Deserialize(fStream);

                Date = categoriesCommodity.Date;

                Categories = categoriesCommodity.Categories;
            }
        }
    }
}