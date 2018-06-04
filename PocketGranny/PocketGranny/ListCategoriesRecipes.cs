using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace PocketGranny
{
    [Serializable]
    public class ListCategoriesRecipes
    {
        private List<ListRecipes> _categories = new List<ListRecipes>();

        public List<ListRecipes> Categories
        {
            get => _categories;
            set => _categories = value;
        }

        public void Add(Recipe value)
        {
            try
            {
                Categories.Find(
                delegate (ListRecipes groupedRecipe)
                {
                    return groupedRecipe.NameCategory == value.CategoryName;
                }).Add(value);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (NullReferenceException)
            {
                var groupedRecipe= new ListRecipes(value.CategoryName);

                try
                {
                    groupedRecipe.Add(value);
                }
                catch (ArgumentException)
                {
                    throw;
                }

                Categories.Add(groupedRecipe);
            }
        }

        public void AddRange(List<Recipe> values)
        {
            foreach (var i in values)
            {
                Add(i);
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
                Categories[identifier[0]].RemoveAt(identifier[1]);

                if (Categories[identifier[0]].Elements.Count == 0)
                {
                    Categories.RemoveAt(identifier[0]);
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentException($"Второе значение [{ identifier[1] }] в идентификаторе находится за пределами допустимого диаппазона");
            }
        }

        public Recipe FindElement(int[] identifier)
        {
            if (identifier.Length != 2)
            {
                throw new ArgumentException("Формат идентификатора не верен, должно быть два индекса");
            }

            try
            {
                return Categories[identifier[0]].FindElement(identifier[1]);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentException($"Второе значение [{ identifier[1] }] в идентификаторе находится за пределами допустимого диаппазона");
            }
        }

        public void ChangeElement(int[] identifier, float weight)
        {
            if (identifier.Length != 2)
            {
                throw new ArgumentException("Формат идентификатора не верен, должно быть два индекса");
            }

            try
            {
                Categories[identifier[0]].ChangeElement(identifier[1], weight);
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

        public void Clear()
        {
            Categories.Clear();
        }

        public void Print()
        {
            for (int i = 0; i < Categories.Count; i++)
            {
                Console.WriteLine($"[{ i }] { Categories[i].NameCategory }:");
                Categories[i].Print(i.ToString());
            }
        }

        public void Save(string fileName)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(ListCategoriesRecipes));

            using (Stream fStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(fStream, this);
            }
        }

        public void Load(string fileName)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(ListCategoriesRecipes));

            using (Stream fStream = File.OpenRead(fileName))
            {
                var categoriesRecipes = (ListCategoriesRecipes)formatter.Deserialize(fStream);

                Categories = categoriesRecipes.Categories;
            }
        }
    }
}