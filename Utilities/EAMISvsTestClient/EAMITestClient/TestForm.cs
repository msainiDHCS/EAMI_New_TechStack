using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace EAMITestClient
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            //webBrowser1.Url = new Uri("https://www.google.com");
            try
            {
                string fileName = string.Empty;

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = Helper.GetExecutingExportLocationPath();
                ofd.Filter = "xml files (*.xml)|*.xml";
                //ofd.RestoreDirectory = true;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    fileName = ofd.FileName;
                }
                else
                {
                    return;
                }


                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(File.ReadAllText(fileName));
                //webBrowser1.Url = new Uri(xdoc.BaseURI);
                //webBrowser1.DocumentText = xdoc.OuterXml;
                
                webBrowser1.Navigate(fileName, true);
                webBrowser1.Show();

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }

      
    }
}
