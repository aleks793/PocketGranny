using System;
using System.Xml;
using System.Xml.Serialization;

namespace PocketGranny
{
    [Serializable]
    public class StandardizedCommodity : ConsumedCommodity
    {
        [XmlArray(ElementName = "weight")]
        public float[] VolumesOfPackages { get; set; }

        public StandardizedCommodity()
        {

        }

        public StandardizedCommodity(Product product, float weight)
        {
            Product = product;
            Weight = weight;
            DateAdded = DateTime.Today;

            try
            {
                VolumesOfPackages = GetVolumesOfPackages();
            }
            catch (ArgumentException)
            {
                throw;
            }
        }

        public StandardizedCommodity(Product product, float weight, DateTime date)
        {
            Product = product;
            Weight = weight;
            DateAdded = date;

            try
            {
                VolumesOfPackages = GetVolumesOfPackages();
            }
            catch (ArgumentException)
            {
                throw;
            }
        }

        public StandardizedCommodity(Product product, float weight, DateTime date, float[] volumesOfPackages)
        {
            Product = product;
            Weight = weight;
            DateAdded = date;
            VolumesOfPackages = volumesOfPackages;
        }

        private float[] GetVolumesOfPackages()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"..\..\DataPacks.xml");
            XmlElement xmlRoot = xmlDoc.DocumentElement;
            XmlNode xmlNode = xmlRoot.SelectSingleNode($"StandardizedCommodity[@name='{ Product.Name }']");

            if (xmlNode == null)
            {
                throw new ArgumentException("Упаковки с таким именем нет в базе");
            }

            XmlNodeList xmlNodeList = xmlNode.ChildNodes;
            float[] volumesOfPackages = new float[xmlNodeList.Count];

            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                if (float.TryParse(xmlNodeList.Item(i).InnerText, out float a))
                {
                    volumesOfPackages[i] = a;
                }
                else
                {
                    throw new ArgumentException("Не удается преобразовать элемент из базы");
                }
            }
            
            return volumesOfPackages;
        }
    }
}