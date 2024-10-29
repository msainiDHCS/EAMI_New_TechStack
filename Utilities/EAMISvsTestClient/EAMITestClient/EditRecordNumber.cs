using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using EAMITestClient.EAMISvcRef;
//using OHC.EAMI.ServiceContract;
//using OHC.EAMI.ServiceManager;

namespace EAMITestClient
{
    public partial class EditRecordNumber : Form
    {
        List<BaseRecord> _recList = null;
        BaseRecord _br = null;
        bool isNew = false;
         

        public EditRecordNumber(BaseRecord br)
        {
            InitializeComponent();
            _br = br;
            txtRecordNumber.Text = br.PaymentRecNumber;            
        }


        public EditRecordNumber(List<BaseRecord> recList)
        {
            InitializeComponent();
            _recList = recList;
            isNew = true;
            this.Text = "New Record Number";
        }      
       

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRecordNumber.Text))
            {
                MessageBox.Show("Invalid record entry.");
                return;
            }

            if (isNew)
            {                
                _recList.Add(new BaseRecord() { PaymentRecNumber = txtRecordNumber.Text } );
            }
            else
            {                
                _br.PaymentRecNumber = txtRecordNumber.Text;
            }
            
            this.Close();
        }
    }
}
