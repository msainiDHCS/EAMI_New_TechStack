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
    public enum enLoginParentForm
    {        
        PaymentSubmission,
        PaymentStatusInquery
    }

    public partial class Login : Form
    {
        internal LoginInfo loginInfo { get; private set; }
       
        public Login()
        {
            InitializeComponent();
            this.txtDomain.Text = "intra.dhs.ca.gov";
            this.txtUserName.Text = Helper.GetCurrentUserIdentity();
        }
     


        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (!ValidateInputControls())
                return;

            this.loginInfo = new LoginInfo(txtDomain.Text, txtUserName.Text, txtPassword.Text);            
            this.Close();
        }

      

        private bool ValidateInputControls()
        {            
            if (string.IsNullOrWhiteSpace(txtUserName.Text))
            {
                MessageBox.Show("Please enter user name !");
                return false;
            }

            if (txtUserName.Text.Length > 30)
            {
                MessageBox.Show("User name is over 30 char long (30 char is max) !");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter password !");
                return false;
            }

            if (txtPassword.Text.Length > 30)
            {
                MessageBox.Show("Password is over 30 char long (30 char is max) !");
                return false;
            }

            return true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.loginInfo = null;
            this.Close();
        }
             

        //private void txtMaxRecCount_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
        //    {
        //        e.Handled = true;
        //    }
        //}

        //private void txtPGHashNth_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
        //    {
        //        e.Handled = true;
        //    }
        //}


        //public void UpdateLoadStatus(int importRecIndex, int totalCount)
        //{            
        //    lblImportedRecNum.Text = "Importing: " + importRecIndex.ToString() + Environment.NewLine + " (total: " + totalCount + ")";
        //    lblImportedRecNum.Refresh();
        //    // refresh the form on every hundredth item to avoid it being unresponsive
        //    if (importRecIndex % 100 == 0 && importRecIndex != 0)
        //    {
        //        this.Refresh();
        //    }
            
        //}

        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
           
        }

       

       

    }
}
