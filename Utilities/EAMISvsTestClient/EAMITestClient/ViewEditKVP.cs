using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EAMITestClient
{
    public partial class ViewEditKVP : Form
    {
        Dictionary<string, string> _kvpList = null;
        string _key = string.Empty;
        bool isNew = false;

        public ViewEditKVP(Dictionary<string, string> kvpList, string key)
        {
            InitializeComponent();

            txtR1Kvp1Name.ReadOnly = true;
            _kvpList = kvpList;
            _key = key;

            if (!string.IsNullOrWhiteSpace(key))
            {
                txtR1Kvp1Name.Text = key;
                txtR1Kvp1Value.Text = kvpList[key];
            }            
        }

        public ViewEditKVP(Dictionary<string, string> kvpList)
        {
            InitializeComponent();
            _kvpList = kvpList;
            isNew = true;
            this.Text = "New KVP";
        }       

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (isNew)
            {
                if (string.IsNullOrWhiteSpace(txtR1Kvp1Name.Text) || _kvpList.ContainsKey(txtR1Kvp1Name.Text))
                {
                    MessageBox.Show("Invalid key entry. key is empty or already exist.");
                    return;
                }
                _kvpList.Add(txtR1Kvp1Name.Text, txtR1Kvp1Value.Text);
            }
            else
            {                
                _kvpList[_key] = txtR1Kvp1Value.Text;
            }
            
            this.Close();
        }
    }
}
