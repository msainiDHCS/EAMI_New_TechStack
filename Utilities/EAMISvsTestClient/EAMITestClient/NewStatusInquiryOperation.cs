
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

using EAMITestClient.EAMISvcRef;
//using OHC.EAMI.ServiceContract;
//using OHC.EAMI.ServiceManager;


namespace EAMITestClient
{
    public partial class NewStatusInquiryOperation : Form
    {
        private static string reqTemplateName = "StatusInquiryRequest";
        private static string respTemplateName = "StatusInquiryResponse";
        private static string defaultTemplateFileName = reqTemplateName + "_default";
        private PaymentStatusInquiryRequest request = new PaymentStatusInquiryRequest();
        private PaymentStatusInquiryResponse response = null;

        public NewStatusInquiryOperation()
        {
            InitializeComponent();
        }

        private void StatusInquiryOperation_Load(object sender, EventArgs e)
        {
            lblExecutingStatus.Text = string.Empty;
            dgvPaymentRecords.AutoGenerateColumns = false;
            dgvPaymentRecStatus.AutoGenerateColumns = false;
            this.ImportFromFile(true);

            #region Original hardcoded values (keep just in case cant load initial values from xml)

            //// request transaction
            //txtReqSenderId.Text = "CAPMAN";
            //txtReqReceiverID.Text = "EAMI";
            //txtReqTranID.Text = Guid.NewGuid().ToString();
            //txtReqTranType.Text = "StatusInquiryRequest";
            //txtReqTranVersion.Text = "1.1";
            //txtReqTimeStamp.Text = DateTime.Now.ToString();
            //txtReqRecordCount.Text = "2";


            //// payment record 1
            //txtR1InvoiceNum.Text = "MC506-00136379";
            //txtR1RecordID.Text = "NR-44665-7769-4456";

            //txtR1Kvp1Name.Text = string.Empty;
            //txtR1Kvp1Value.Text = string.Empty;
            //txtR1Kvp2Name.Text = string.Empty;
            //txtR1Kvp2Value.Text = string.Empty;


            //// payment record 2
            //txtR2InvoiceNum.Text = "MC352-00107109";
            //txtR2RecordID.Text = "NR-44665-7769-4526";

            //txtR2Kvp1Name.Text = string.Empty;
            //txtR2Kvp1Value.Text = string.Empty;
            //txtR2Kvp2Name.Text = string.Empty;
            //txtR2Kvp2Value.Text = string.Empty;
          
            #endregion
        }


        private void btnNewTransactionID_Click(object sender, EventArgs e)
        {
            txtReqTranID.Text = Guid.NewGuid().ToString();
        }
        
        private void btnGetCurrentTime_Click(object sender, EventArgs e)
        {
            txtReqTimeStamp.Text = DateTime.Now.ToString();
        }

        private void lnkClearTran_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.ClearRequestTransactionInputFields();
        }

        private void lnkAddRecord_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.ClearResponseFields();
            if (request.PaymentRecordList == null)
            {
                request.PaymentRecordList = new List<BaseRecord>();
            }
            EditRecordNumber ernForm = new EditRecordNumber(request.PaymentRecordList);
            ernForm.ShowDialog(this);            
            this.LoadPaymentGridViewFromObjList(request.PaymentRecordList);
        }               

        private void btnClearResp_Click(object sender, EventArgs e)
        {
            this.EnableMainClickControls(true);
            this.EnableTransactionInputControls(true);
            dgvPaymentRecStatus.DataSource = null;
            dgvPaymentRecStatus.Visible = false;
            dgvPaymentRecords.Visible = true;
            this.ClearResponseFields();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }       

        private void newXMLFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ClearAllRequestInputControls();
            this.ClearResponseFields();
            this.DisableAllInputControls();

            txtFile.Text = string.Empty;
            request = new PaymentStatusInquiryRequest();
            request.TransactionType = Helper.ToEnum<TransactionType>(4);
            request.PaymentRecordList = new List<BaseRecord>();
            request.PaymentRecordList.Add(new BaseRecord());
            this.LoadRequestTransactionInputControlsFromObj(request);
        }       

        private void importXMLFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ClearResponseFields();
            this.ImportFromFile();
        }

        private void exportXMLFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.PopulatePaymentStatusInquiryRequestFromControls();
            this.ExportFile();            
        }

        private void exportXMLFileToolStripMenuItemWResponse_Click(object sender, EventArgs e)
        {
            this.PopulatePaymentStatusInquiryRequestFromControls();
            this.ExportFile(true);
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ClearResponseFields();
            this.ImportFromFile(true);
        }

        private void importFromDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ClearResponseFields();
            this.DisableAllInputControls();

            DbLogin loginForm = new DbLogin(enParentForm.PaymentStatusInquery);            
            loginForm.ShowDialog(this);

            // check if the the data was pulled or not
            if (loginForm.PymtStatusInquiryRequest == null)
            {
                MessageBox.Show("No data was imported.");
            }            
            else
            {
                txtFile.Text = string.Empty;
                MessageBox.Show("PaymentStatusInquiryRequest transaction with " + loginForm.PymtStatusInquiryRequest.PaymentRecordList.Count.ToString() + "  payment records successfully imported from db !", "Complete");
                request = loginForm.PymtStatusInquiryRequest;                
            }
           
            this.LoadRequestTransactionInputControlsFromObj(request);
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
                this.PopulatePaymentStatusInquiryRequestFromControls();

                // execute request
                response = eamiSvsCli.EAMIPaymentStatusInquiry(request);
                this.LoadResponseFromObj(response);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }

            //this.LoadRequestTransactionInputControlsFromObj(request);
            EnableMainClickControls(true);
            lblExecutingStatus.Text = string.Empty;
            btnExecuteRequest.BackColor = System.Drawing.Color.MediumSeaGreen;
        }


        #region populate Request Input Controls From Object

        private void LoadRequestTransactionInputControlsFromObj(PaymentStatusInquiryRequest psir)
        {
            this.ClearRequestTransactionInputFields();
            if (psir == null) return;
            this.EnableMainClickControls(true);
            this.EnableTransactionInputControls(true);

            txtReqSenderId.Text = psir.SenderID;
            txtReqReceiverID.Text = psir.ReceiverID;
            txtReqTranID.Text = psir.TransactionID;
            txtReqTranType.Text = psir.TransactionType.ToString();
            txtReqTranVersion.Text = psir.TransactionVersion;
            //txtReqTimeStamp.Text = psr.TimeStamp.ToString();
            txtReqTimeStamp.Text = DateTime.Now.ToString();
            txtReqRecordCount.Text = psir.PaymentRecordCount;

            txtActualRecordCount.Text = psir.PaymentRecordList.Count().ToString();

            this.LoadPaymentGridViewFromObjList(psir.PaymentRecordList);
            btnExecuteRequest.Focus();
        }

        private void LoadPaymentGridViewFromObjList(List<BaseRecord> psrList)
        {            
            // load request records into rquest record grid       
            dgvPaymentRecStatus.DataSource = null;
            dgvPaymentRecStatus.Visible = false;

            dgvPaymentRecords.Visible = true;
            dgvPaymentRecords.DataSource = null;
            dgvPaymentRecords.DataSource = psrList;
        }        
        

        #endregion


        #region populate Response controls from Object

        private void LoadResponseFromObj(PaymentStatusInquiryResponse response)
        {
            this.ClearResponseFields(false);
            if (response == null) return;
            exportXMLFileToolStripMenuItemWResponse.Enabled = true;

            txtRespSenderID.Text = response.SenderID;
            txtRespReceiverID.Text = response.ReceiverID;
            txtRespTranID.Text = response.TransactionID;
            txtRespTranType.Text = response.TransactionType.ToString();
            txtRespTranVersion.Text = response.TransactionVersion;
            txtRespTimeStamp.Text = response.TimeStamp.ToString();
            txtRespStatusCode.Text = response.ResponseStatusCode;
            txtRespStatusMsg.Text = response.ResponseMessage;

            // load response record list
            this.LoadResponseRecordGridFromObj(response.PaymentRecordStatuList);
        }

        private void LoadResponseRecordGridFromObj(List<PaymentRecordStatusPlus> prspList)
        {
            if (prspList == null) return;
            txtRespRecordCount.Text = prspList.Count.ToString();
            txtRespAccRecCount.Text = prspList.FindAll(item => item.StatusCode != "RejectedFD").Count().ToString();
            txtRespRejRecCount.Text = prspList.FindAll(item => item.StatusCode == "RejectedFD").Count().ToString();

            dgvPaymentRecStatus.DataSource = null;
            dgvPaymentRecStatus.Visible = true;
            dgvPaymentRecords.Visible = false;
            dgvPaymentRecStatus.DataSource = prspList;
        }        

        #endregion


        #region populate object from input controls

        private void PopulatePaymentStatusInquiryRequestFromControls()
        {
            // we override all transaction values from input controls
            request.SenderID = txtReqSenderId.Text;
            request.ReceiverID = txtReqReceiverID.Text;
            request.TransactionID = txtReqTranID.Text;
            request.TransactionType = Helper.ToEnum<TransactionType>(txtReqTranType.Text);
            request.TransactionVersion = txtReqTranVersion.Text;
            request.TimeStamp = Helper.GetDateFromString(txtReqTimeStamp.Text);
            request.PaymentRecordCount = txtReqRecordCount.Text;
            
            if (request.PaymentRecordList == null)
            {
                request.PaymentRecordList = new List<BaseRecord>();
            }            
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
            txtReqRecordCount.Text = string.Empty;
        }       


        private void ClearResponseFields(bool setResponseToNull = true)
        {
            if (setResponseToNull)
            {
                response = null;
                exportXMLFileToolStripMenuItemWResponse.Enabled = false;
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
            txtRespAccRecCount.Text = string.Empty;
            txtRespRejRecCount.Text = string.Empty;         
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
            lnkAddRecord.Enabled = enable;

            txtReqSenderId.Enabled = enable;
            txtReqReceiverID.Enabled = enable;
            txtReqTranID.Enabled = enable;
            txtReqTranType.Enabled = enable;
            txtReqTranVersion.Enabled = enable;
            txtReqTimeStamp.Enabled = enable;
            txtReqRecordCount.Enabled = enable;
        }

        #endregion       
      

        private void ImportFromFile(bool isDefaultTemplate = false)
        {            
            DisableAllInputControls();
            try
            {
                string fileName = string.Empty;
                if (isDefaultTemplate)
                {
                    fileName = Helper.GetExecutingDefaultXmlFilesLocationPath() + "\\" + defaultTemplateFileName;
                    if (!File.Exists(fileName))
                    {
                        throw new Exception("Default template file not found");
                    }
                }
                else
                {
                    lblExecutingStatus.Text = "IMPORTING...";
                    lblExecutingStatus.Refresh();

                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.InitialDirectory = Helper.GetExecutingExportLocationPath();
                    ofd.Filter = "xml files (*.xml)|*.xml";
                    //ofd.RestoreDirectory = true;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        ClearAllRequestInputControls();
                        Thread.Sleep(200);
                        fileName = ofd.FileName;
                    }
                    else
                    {
                        this.LoadRequestTransactionInputControlsFromObj(request);
                        lblExecutingStatus.Text = string.Empty;
                        return;
                    }
                }

                string xml = File.ReadAllText(fileName);
                request = Helper.Deserialize(xml, typeof(PaymentStatusInquiryRequest)) as PaymentStatusInquiryRequest;
                this.LoadRequestTransactionInputControlsFromObj(request);

                // we only want show this message any non-default template was loaded                
                if (isDefaultTemplate == false)
                {
                    MessageBox.Show("Loaded template xml file !" +
                       Environment.NewLine +
                       Environment.NewLine +
                       fileName, "Done !");
                    lblExecutingStatus.Text = string.Empty;
                }
                txtFile.Text = Path.GetFileName(fileName);
            }
            catch (Exception ex)
            {
                lblExecutingStatus.Text = string.Empty;
                MessageBox.Show(ex.Message, "ERROR");
                this.newXMLFileToolStripMenuItem_Click(null, null);
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
                    this.PopulatePaymentStatusInquiryRequestFromControls();
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
                    MessageBox.Show("Saved template xml file !" +
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

        private void dgvPaymentRecords_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string action = dgvPaymentRecords.Rows[e.RowIndex].Cells[e.ColumnIndex].FormattedValue.ToString();
            if (action == "edit" || action == "delete")
            {
                BaseRecord pr = (BaseRecord)dgvPaymentRecords.CurrentRow.DataBoundItem;
                if (action == "edit")
                {
                    EditRecordNumber ernForm = new EditRecordNumber(pr);
                    ernForm.ShowDialog(this);
                }
                else if (action == "delete")
                {
                    request.PaymentRecordList.RemoveAll(t => t.PaymentRecNumber == pr.PaymentRecNumber);
                }
                                
                txtActualRecordCount.Text = request.PaymentRecordList.Count().ToString();
                LoadPaymentGridViewFromObjList(request.PaymentRecordList);
            }
        }

        private void dgvPaymentRecords_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if (dgvPaymentRecords.Rows[e.RowIndex].DataBoundItem != null)
                {
                    switch (dgvPaymentRecords.Columns[e.ColumnIndex].DataPropertyName)
                    {                        
                        case "Edit":
                            e.Value = "edit";
                            break;
                        case "Delete":
                            e.Value = "delete";
                            break;
                    }
                }
            }
            catch { }
        }                   
       
    }
}
