using System;
using System.Collections.Generic;

namespace PocketGranny
{
    [Serializable]
    public class ListRecipes
    {
        private List<Recipe> _element = new List<Recipe>();

        public string NameCategory { get; set; }

        public List<Recipe> Elements
        {
            get => _element;
            set => _element = value;
        }

        public ListRecipes()
        {

        }

        public ListRecipes(string nameCategory)
        {
            NameCategory = nameCategory;
        }

        public void Add(Recipe value)
        {
            int k = Elements.IndexOf(value);

            if (k == -1)
            {
                Elements.Add(value);
            }
            else
            {
                throw new ArgumentException($"Рецепт с именем [{ value }] уже добавлен");
            }
        }

        public void RemoveAt(int index)
        {
            Elements.RemoveAt(index);
        }

        public Recipe FindElement(int index)
        {
            return Elements[index];
        }

        public void ChangeElement(int index, float weight)
        {
            try
            {
                Elements[index].SetWeight(weight);
            }
            catch (ArgumentException)
            {
                throw;
            }
        }

        public void Print(string index)
        {
            for (int i = 0; i < Elements.Count; i++)
            {
                Console.WriteLine($"  [{ index }:{ i }] { Elements[i].ToString() }");
            }
        }
    }
}