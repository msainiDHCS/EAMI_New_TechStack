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


using System.Threading;
using System.Xml;

using EAMITestClient.EAMISvcRef;
//using OHC.EAMI.ServiceContract;
//using OHC.EAMI.ServiceManager;

namespace EAMITestClient
{
    public partial class NewRejectedPaymentInquiry : Form
    {
        private static string reqTemplateName = "RejectedPaymentInquiryRequest";
        private static string respTemplateName = "RejectedPaymentInquiryResponse";
        private static string defaultTemplateFileName = reqTemplateName + "_default";
        private RejectedPaymentInquiryRequest request = new RejectedPaymentInquiryRequest();
        private RejectedPaymentInquiryResponse response = null;

        public NewRejectedPaymentInquiry()
        {
            InitializeComponent();
        }

        private void RejectedPaymentInquiry_Load(object sender, EventArgs e)
        {            
            lblExecutingStatus.Text = string.Empty;
            dgvPaymentRecStatus.AutoGenerateColumns = false;
            this.ImportFromFile();

            #region Original hardcoded values (keep just in case cant load initial values from xml)

            // request transaction
            //txtReqSenderId.Text = "CAPMAN";
            //txtReqReceiverID.Text = "EAMI";
            //txtReqTranID.Text = Guid.NewGuid().ToString();
            //txtReqTranType.Text = templateName;
            //txtReqTranVersion.Text = "1.1";
            //txtReqTimeStamp.Text = DateTime.Now.ToString();         

            #endregion
        }        

        private void btnGetCurrentTime_Click(object sender, EventArgs e)
        {
            txtReqTimeStamp.Text = DateTime.Now.ToString();
        }

        private void lnkClearTran_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.ClearRequestTransactionInputFields();
        }

        private void btnGetCurDate2_Click(object sender, EventArgs e)
        {
            txtReqDateFrom.Text = DateTime.Now.ToString();
        }

        private void btnGetCurDate3_Click(object sender, EventArgs e)
        {
            txtReqDateTo.Text = DateTime.Now.ToString();
        }
        
        private void btnClearResp_Click(object sender, EventArgs e)
        {
            this.ClearResponseFields();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }       

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            this.ClearAllRequestInputControls();
            this.ClearResponseFields();
            this.DisableAllInputControls();

            txtFile.Text = string.Empty;
            request = new RejectedPaymentInquiryRequest();
            request.TransactionType = Helper.ToEnum<TransactionType>(6);
            this.LoadRequestTransactionInputControlsFromObj(request);
        }

        private void exportToolStripMenuItemWResponse_Click(object sender, EventArgs e)
        {
            this.PopulateRejectedPaymentInquiryRequestFromControls();
            this.ExportFile(true);
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ClearAllRequestInputControls();
            this.ClearResponseFields();     
            this.ImportFromFile();
        }      


        private void btnExecuteRequest_Click(object sender, EventArgs e)
        {
            this.DisableAllInputControls();
            btnExecuteRequest.BackColor = System.Drawing.Color.Salmon;
            lblExecutingStatus.Text = "EXECUTING...";
            ClearResponseFields();
            this.Refresh();
            Thread.Sleep(400);

            try
            {
                IEAMIServiceOperations eamiSvsCli = Helper.GetEAMISvsClientInstance();
                this.PopulateRejectedPaymentInquiryRequestFromControls();

                // execute request
                response = eamiSvsCli.EAMIRejectedPaymentInquiry(request);
                this.LoadResponseFromObj(response);         
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }

            this.LoadRequestTransactionInputControlsFromObj(request);
            EnableMainClickControls(true);            
            lblExecutingStatus.Text = string.Empty;
            btnExecuteRequest.BackColor = System.Drawing.Color.MediumSeaGreen;
        }



        #region populate Request Input Controls From Object

        private void LoadRequestTransactionInputControlsFromObj(RejectedPaymentInquiryRequest rpir)
        {
            this.ClearRequestTransactionInputFields();
            if (rpir == null) return;
            this.EnableMainClickControls(true);
            this.EnableTransactionInputControls(true);

            txtReqSenderId.Text = rpir.SenderID;
            txtReqReceiverID.Text = rpir.ReceiverID;
            txtReqTranID.Text = rpir.TransactionID;
            txtReqTranType.Text = rpir.TransactionType.ToString();
            txtReqTranVersion.Text = rpir.TransactionVersion;
            //txtReqTimeStamp.Text = rpir.TimeStamp.ToString();
            txtReqTimeStamp.Text = DateTime.Now.ToString();
            txtReqDateFrom.Text = rpir.RejectedDateFrom.ToString();
            txtReqDateTo.Text = rpir.RejectedDateTo.ToString();
        }

        
       
        #endregion


        #region populate Response controls from Object

        private void LoadResponseFromObj(RejectedPaymentInquiryResponse response)
        {
            this.ClearResponseFields(false);
            if (response == null)
            {
                return;
            }

            exportToolStripMenuItemWResponse.Enabled = true;

            txtRespSenderID.Text = response.SenderID;
            txtRespReceiverID.Text = response.ReceiverID;
            txtRespTranID.Text = response.TransactionID;
            txtRespTranType.Text = response.TransactionType.ToString();
            txtRespTranVersion.Text = response.TransactionVersion;
            txtRespTimeStamp.Text = response.TimeStamp.ToString();
            txtRespStatusCode.Text = response.ResponseStatusCode;
            txtRespStatusMsg.Text = response.ResponseMessage;

            // load response record list            
            this.LoadResponseRecordsFromObj(response.PaymentRecordStatuList);
        }

        private void LoadResponseRecordsFromObj(List<PaymentRecordStatus> prspList)
        {
            if (prspList == null) return;
            txtRespRecordCount.Text = prspList.Count.ToString();
            dgvPaymentRecStatus.DataSource = null;
            dgvPaymentRecStatus.DataSource = prspList;
        }               

        #endregion


        #region populate object from input controls

        private void PopulateRejectedPaymentInquiryRequestFromControls()
        {
            // we override all transaction values from input controls
            request.SenderID = txtReqSenderId.Text;
            request.ReceiverID = txtReqReceiverID.Text;
            request.TransactionID = txtReqTranID.Text;
            request.TransactionType = Helper.ToEnum<TransactionType>(txtReqTranType.Text);
            request.TransactionVersion = txtReqTranVersion.Text;
            request.TimeStamp = Convert.ToDateTime(txtReqTimeStamp.Text);
            request.RejectedDateFrom = Convert.ToDateTime(txtReqDateFrom.Text);
            request.RejectedDateTo = Convert.ToDateTime(txtReqDateTo.Text);            
        }       

        #endregion


        #region clear, enable/disable input controls


        private void ClearAllRequestInputControls()
        {
            ClearRequestTransactionInputFields();
        }

        private void ClearRequestTransactionInputFields()
        {
            // request transaction
            txtReqSenderId.Text = string.Empty;
            txtReqReceiverID.Text = string.Empty;
            txtReqTranID.Text = string.Empty;
            txtReqTranType.Text = string.Empty;
            txtReqTranVersion.Text = string.Empty;
            txtReqTimeStamp.Text = string.Empty;
            txtReqDateFrom.Text = string.Empty;
            txtReqDateTo.Text = string.Empty;
        }
      

        private void ClearResponseFields(bool setResponseToNull = true)
        {
            if (setResponseToNull)
            {
                response = null;
                exportToolStripMenuItemWResponse.Enabled = false;
            }
        
            // response transaction 
            txtRespSenderID.Text = string.Empty;
            txtRespReceiverID.Text = string.Empty;
            txtRespTranID.Text = string.Empty;
            txtRespTranType.Text = string.Empty;
            txtRespTranVersion.Text = string.Empty;
            txtRespTimeStamp.Text = string.Empty;
            txtRespStatusCode.Text = string.Empty;
            txtRespStatusMsg.Text = string.Empty;
            txtRespRecordCount.Text = string.Empty;           
        }       


        private void DisableAllInputControls()
        {
            this.EnableMainClickControls(false);
            this.EnableTransactionInputControls(false);
        }        

        public void EnableMainClickControls(bool enable)
        {
            menuFile.Enabled = enable;
            btnExecuteRequest.Enabled = enable;           
            btnClearResp.Enabled = enable;
        }

        public void EnableTransactionInputControls(bool enable)
        {
            btnGetCurrentTime.Enabled = enable;
            lnkClearTran.Enabled = enable;

            txtReqSenderId.Enabled = enable;
            txtReqReceiverID.Enabled = enable;
            txtReqTranID.Enabled = enable;
            txtReqTranType.Enabled = enable;
            txtReqTranVersion.Enabled = enable;
            txtReqTimeStamp.Enabled = enable;
            txtReqDateFrom.Enabled = enable;
            txtReqDateTo.Enabled = enable;
        }


        #endregion


        private void ImportFromFile()
        {
            DisableAllInputControls();

            try
            {
                string fileName = string.Empty;

                fileName = Helper.GetExecutingDefaultXmlFilesLocationPath() + "\\" + defaultTemplateFileName;
                if (!File.Exists(fileName))
                {
                    throw new Exception("Default template file not found");
                }

                string xml = File.ReadAllText(fileName);
                request = Helper.Deserialize(xml, typeof(RejectedPaymentInquiryRequest)) as RejectedPaymentInquiryRequest;
                this.LoadRequestTransactionInputControlsFromObj(request);
                txtFile.Text = Path.GetFileName(fileName);
            }
            catch (Exception ex)
            {
                lblExecutingStatus.Text = string.Empty;
                MessageBox.Show(ex.Message, "ERROR");
                this.newToolStripMenuItem_Click(null, null);
            }
        }



        private void ExportFile(bool includeResponse = false)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "xml files (*.xml)|*.xml";
                sfd.InitialDirectory = Helper.GetExecutingExportLocationPath();
                string timeStamp = Helper.GetTimeStamp();
                sfd.FileName = Helper.GetCurrentUserIdentity().Substring(9) +
                                "_" + reqTemplateName + "_" +
                                timeStamp + ".xml";
                //sfd.RestoreDirectory = true;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    this.PopulateRejectedPaymentInquiryRequestFromControls();
                    string xml = Helper.Serialize(request, request.GetType());
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.LoadXml(xml);
                    xdoc.Save(sfd.FileName);

                    // save response file
                    if (includeResponse && response != null)
                    {
                        string respFileName = Helper.GetCurrentUserIdentity().Substring(9) +
                                                "_" + respTemplateName + "_" +
                                                timeStamp +
                                                ".xml";
                        xml = Helper.Serialize(response, response.GetType());
                        xdoc = new XmlDocument();
                        xdoc.LoadXml(xml);
                        xdoc.Save(Path.GetDirectoryName(sfd.FileName) + "\\" + respFileName);
                    }
                    txtFile.Text = Path.GetFileName(sfd.FileName);
                    MessageBox.Show("Saved xml file !" +
                        Environment.NewLine +
                        Environment.NewLine +
                        sfd.FileName, "Done !");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }

        }

        private void btnNewTransactionID_Click(object sender, EventArgs e)
        {
            txtReqTranID.Text = Guid.NewGuid().ToString();
        }

        
      

    }
}
