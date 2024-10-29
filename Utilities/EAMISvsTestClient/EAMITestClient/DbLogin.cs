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
    public enum enParentForm
    {        
        PaymentSubmission,
        PaymentStatusInquery
    }

    public partial class DbLogin : Form
    {
        internal enParentForm ParentFormName { get; private set; }
       
        public DbLogin(enParentForm pf)
        {
            ParentFormName = pf;
            InitializeComponent();
            this.txtServer.Text = DataAccess.GetDefaultDBConnectionStringServer();
            this.txtDatabase.Text = DataAccess.GetDefaultDBConnectionStringDatabase();
            this.txtDomain.Text = DataAccess.DOMAIN_NAME;

            this.PymtSubmissionRequest = new PaymentSubmissionRequest();
            this.PymtStatusInquiryRequest = new PaymentStatusInquiryRequest();


            //if (ParentFormName == enParentForm.PaymentStatusInquery)
            //{
            //    this.txtPGHashNth.Enabled = false;
            //}
        }


        public PaymentSubmissionRequest PymtSubmissionRequest { get; set; }
        public PaymentStatusInquiryRequest PymtStatusInquiryRequest { get; set; }
     


        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (!ValidateInputControls(false)) return;

            try
            {
                btnTestConnection.Enabled = false;
                btnLoad.Enabled = false;
                Cursor.Current = Cursors.WaitCursor;

                switch (this.ParentFormName)
                {
                    case enParentForm.PaymentSubmission:
                        this.PymtSubmissionRequest = DataAccess.GetPaymentSubmissionRequestFromDB(txtServer.Text,
                                                                            txtDatabase.Text,
                                                                            txtUserName.Text,
                                                                            txtPassword.Text,
                                                                            txtTransactionId.Text,
                                                                            this);
                        break;
                    case enParentForm.PaymentStatusInquery:
                        this.PymtStatusInquiryRequest = DataAccess.GetPaymentStatusInquiryRequestFromDB(txtServer.Text,
                                                                            txtDatabase.Text,
                                                                            txtUserName.Text,
                                                                            txtPassword.Text,
                                                                            txtTransactionId.Text,
                                                                            this);
                        break;

                }                               
            }
            catch
            {
                this.PymtSubmissionRequest = null;
                this.PymtStatusInquiryRequest = null;
                MessageBox.Show("Error importing transaction from database", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {                
                Cursor.Current = Cursors.Default;
            }
            this.Close();
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            if (!ValidateInputControls(true)) return;

            bool connectionGood = DataAccess.TestConnection(txtServer.Text, txtDatabase.Text, txtUserName.Text, txtPassword.Text);
            if (connectionGood)
            {
                MessageBox.Show("Connection successful !");
            }
            else
            {
                MessageBox.Show("Connection failed !");
            }
        }

        private bool ValidateInputControls(bool testconn)
        {
            if (string.IsNullOrWhiteSpace(txtServer.Text) || string.IsNullOrWhiteSpace(txtDatabase.Text))
            {
                MessageBox.Show("Please enter server and datbase name !");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtUserName.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter user name and password !");
                return false;
            }

            if (!testconn && string.IsNullOrWhiteSpace(txtTransactionId.Text))
            {
                MessageBox.Show("Please enter transaction id !");
                return false;
            }

            //if (!testconn && (string.IsNullOrWhiteSpace(txtMaxRecCount.Text) || txtMaxRecCount.Text.Substring(0, 1) == "0"))
            //{
            //    MessageBox.Show("Please enter max rec count value (1 - 9999) !");
            //    return false;
            //}
            return true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.PymtSubmissionRequest = null;
            this.PymtStatusInquiryRequest = null;
            this.Close();
        }
             

        private void txtMaxRecCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtPGHashNth_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }


        public void UpdateLoadStatus(int importRecIndex, int totalCount)
        {            
            lblImportedRecNum.Text = "Importing: " + importRecIndex.ToString() + Environment.NewLine + " (total: " + totalCount + ")";
            lblImportedRecNum.Refresh();
            // refresh the form on every hundredth item to avoid it being unresponsive
            if (importRecIndex % 100 == 0 && importRecIndex != 0)
            {
                this.Refresh();
            }
            
        }

        private void DbLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.PymtSubmissionRequest = null;
            this.PymtStatusInquiryRequest = null;            
        }

       

       

    }
}
