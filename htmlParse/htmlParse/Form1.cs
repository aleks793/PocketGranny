using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace htmlParse
{
    public partial class Form1 : Form
    {
            public Form1()
            {
                InitializeComponent();
            }

            private void button1_Click(object sender, EventArgs e)
            {
                HtmlElementCollection elmCol;
                elmCol = webBrowser1.Document.GetElementsByTagName("button");
                foreach (HtmlElement elmBtn in elmCol)
                {
                    if (elmBtn.GetAttribute("className") == "submit_check")
                    {
                        elmBtn.InvokeMember("Click");
                    }
                }
            }

            private void button2_Click(object sender, EventArgs e)
            {
                StreamWriter str = new StreamWriter("receipt.txt");
                string html = webBrowser1.Document.Url.OriginalString;
                HtmlDocument HD = new HtmlDocument();
                var web = new HtmlWeb
                {
                    AutoDetectEncoding = false,
                    OverrideEncoding = Encoding.UTF8,
                };
                HD = web.Load(html);
                HtmlNodeCollection NoAltElements = HD.DocumentNode.SelectNodes("//div[@class='check-product-name']");

                // Проверяем наличие узлов
                if (NoAltElements != null)
                {
                    foreach (HtmlNode HN in NoAltElements)
                    {
                        // Получаем строчки
                        string outputText = HN.InnerText;
                        str.WriteLine(outputText);
                    }
                    MessageBox.Show("Чек успешно сохранен", "Состояние чека", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                str.Close();
            }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }
    }
}