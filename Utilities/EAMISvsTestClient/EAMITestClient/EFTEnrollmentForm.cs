using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
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
    public partial class EFTEnrollmentForm : Form
    {
        private static string reqTemplateName = "PaymentSubmissionRequest";
        private static string respTemplateName = "PaymentSubmissionResponse";
        private static string defaultTemplateFileName = reqTemplateName + "_default";
        private PaymentSubmissionRequest request = new PaymentSubmissionRequest();
        private PaymentSubmissionResponse response = null;
        private List<PaymentRecord> filteredList = null;

        public EFTEnrollmentForm()
        {
            InitializeComponent();
        }

        private void EFTEnrollmentForm_Load(object sender, EventArgs e)
        {
            dgvPaymentRecords.AutoGenerateColumns = false;
            lblExecutingStatus.Text = string.Empty;
            this.ImportFromFile(true);
        }


        private void btnGetCurrentTime_Click(object sender, EventArgs e)
        {
            txtReqTimeStamp.Text = DateTime.Now.ToString();
        }

        private void btnNewTransactionID_Click(object sender, EventArgs e)
        {
            txtReqTranID.Text = Guid.NewGuid().ToString();
        }

        private void lnkClearTranInputFields_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ClearRequestTransactionInputFields();
        }
       

        private void dgvPaymentRecords_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if (dgvPaymentRecords.Rows[e.RowIndex].DataBoundItem != null)
                {
                    PaymentRecord pr = null;
                    switch (dgvPaymentRecords.Columns[e.ColumnIndex].DataPropertyName)
                    {
                        case "PaymentRecNumber":
                            pr = (PaymentRecord)dgvPaymentRecords.Rows[e.RowIndex].DataBoundItem;
                            //e.Value = pr.PaymentRecNumber + "_" + pr.PaymentRecNumberExt;
                            e.Value = pr.PaymentRecNumber;   
                            break;
                        case "PaymentSetNumber":
                            pr = (PaymentRecord)dgvPaymentRecords.Rows[e.RowIndex].DataBoundItem;
                            //e.Value = pr.PaymentSetNumber + "_" + pr.PaymentSetNumberExt;
                            e.Value = pr.PaymentSetNumber;
                            break;
                        case "PayeeInfo.EntityID":
                            pr = (PaymentRecord)dgvPaymentRecords.Rows[e.RowIndex].DataBoundItem;
                            e.Value = pr.PayeeInfo.EntityID + "-" + pr.PayeeInfo.EntityIDSuffix;
                            break;
                        case "PayeeInfo.Name":
                            e.Value = ((PaymentRecord)dgvPaymentRecords.Rows[e.RowIndex].DataBoundItem).PayeeInfo.Name;
                            break;
                        //case "ContractNum":
                        //    pr = (PaymentRecord)dgvPaymentRecords.Rows[e.RowIndex].DataBoundItem;
                        //    e.Value = pr.GenericNameValueList["CONTRACT_NUMBER"];
                        //    break;
                        //case "IHSS":
                        //    pr = (PaymentRecord)dgvPaymentRecords.Rows[e.RowIndex].DataBoundItem;
                        //    e.Value = pr.GenericNameValueList["IsIHSS"];
                        //    break;
                        //case "SCHIP":
                        //    pr = (PaymentRecord)dgvPaymentRecords.Rows[e.RowIndex].DataBoundItem;
                        //    e.Value = pr.GenericNameValueList["IsSCHIP"];
                        //    break;
                        //case "HQAF":
                        //    pr = (PaymentRecord)dgvPaymentRecords.Rows[e.RowIndex].DataBoundItem;
                        //    e.Value = pr.GenericNameValueList["IsHQAF"];
                        //    break;
                        case "Edit":
                            e.Value = "view/edit";
                            break;
                        case "Delete":
                            e.Value = "delete";
                            break;
                        case "Clone":
                            e.Value = "clone";
                            break;
                    }
                }
            }
            catch { }


            //if ((dgvPaymentRecords.Rows[e.RowIndex].DataBoundItem != null) &&
            //    (dgvPaymentRecords.Columns[e.ColumnIndex].DataPropertyName.Contains(".")))
            //{
            //    e.Value = BindProperty(
            //                  dgvPaymentRecords.Rows[e.RowIndex].DataBoundItem,
            //                  dgvPaymentRecords.Columns[e.ColumnIndex].DataPropertyName
            //                );
            //}

        }


        private void dgvPaymentRecords_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                PaymentRecord pr = (PaymentRecord)dgvPaymentRecords.CurrentRow.DataBoundItem;               

                // populate response status if exists
                if (response != null && response.PaymentRecordStatuList != null && response.PaymentRecordStatuList.Count > 0)
                {
                    PaymentRecordStatus prs = response.PaymentRecordStatuList.Find(t => t.PaymentRecNumber == pr.PaymentRecNumber);
                    //this.PopulateR1ResponseControlsFromObj(prs);
                }
            }
            catch { }
        }


        private void dgvPaymentRecords_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string action = dgvPaymentRecords.Rows[e.RowIndex].Cells[e.ColumnIndex].FormattedValue.ToString();
            if (action == "view/edit" || action == "delete" || action == "clone")
            {
                PaymentRecord pr = (PaymentRecord)dgvPaymentRecords.CurrentRow.DataBoundItem;
                if (action == "view/edit")
                {
                    ViewEditPaymentRecord veprForm = new ViewEditPaymentRecord(pr);
                    veprForm.ShowDialog(this);
                }
                else if (action == "delete")
                {
                    request.PaymentRecordList.RemoveAll(t => t.PaymentRecNumber == pr.PaymentRecNumber);
                    if (filteredList != null)
                    {
                        filteredList.RemoveAll(t => t.PaymentRecNumber == pr.PaymentRecNumber);
                    }
                }
                else if (action == "clone")
                {                                       
                    PaymentRecord prClone = Helper.GetCloneCopy<PaymentRecord>(pr);
                    prClone.PaymentRecNumber = RandomString(14);                    
                    request.PaymentRecordList.Add(prClone);

                    if (filteredList != null)
                    {
                        filteredList.Add(prClone);
                    }
                }
                txtReqARecTotalAmount.Text = Helper.GetPaymentRecordListTotalAmount(request.PaymentRecordList).ToString();
                txtActualRecordCount.Text = request.PaymentRecordList.Count().ToString();

                if (filteredList != null)
                {
                    LoadPaymentGridViewFromObjList(filteredList);
                }
                else
                {
                    LoadPaymentGridViewFromObjList(request.PaymentRecordList);
                }                
            }
        }


        private void lnkAddRecord_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ClearFilterControlAndContent();

            ViewEditPaymentRecord veprForm = new ViewEditPaymentRecord(request.PaymentRecordList);
            veprForm.ShowDialog(this);
            dgvPaymentRecords.DataSource = null;
            dgvPaymentRecords.DataSource = request.PaymentRecordList;
            txtReqARecTotalAmount.Text = Helper.GetPaymentRecordListTotalAmount(request.PaymentRecordList).ToString();
            txtActualRecordCount.Text = request.PaymentRecordList.Count().ToString();
        }

        /// <summary>
        /// generates new payment set and payment rec numbers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lnkGenerateNewPymtRecSetNumbers_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ClearFilterControlAndContent();
                 
            // check to make sure some payment data is displayed
            if (request == null || request.PaymentRecordList == null || request.PaymentRecordList.Count == 0)
            {
                MessageBox.Show("No payment data loaded to perform this action", 
                                "Payment data not present", 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Warning); 
                return;
            }

            // check to make sure no response is displayed 
            //if (response != null && response.PaymentRecordStatuList != null && response.PaymentRecordStatuList.Count > 0)
            //{
            //    MessageBox.Show("Response data is present. Please clear response data first and try again.",
            //    "Response data is present",
            //    MessageBoxButtons.OK,
            //    MessageBoxIcon.Warning); 
            //    return;
            //}
                  

            // generate new random peyment set/record numbers so that transaction can be re-submitted
            string timeStampText = string.Format("{0:yyMMddhh}", DateTime.Now);
            int psCount = 0;
            int prCount = 0;
            foreach (var gps in request.PaymentRecordList.GroupBy(t => new { t.PaymentSetNumber }).ToList())
            {
                psCount++;
                foreach (PaymentRecord pr in gps)
                {
                    prCount++;
                    pr.PaymentRecNumber = string.Format("{0}-R{1}", timeStampText, prCount.ToString("D" + 4));
                    pr.PaymentSetNumber = string.Format("{0}-S{1}", timeStampText, psCount.ToString("D" + 4));
                }                
            }

            LoadPaymentGridViewFromObjList(request.PaymentRecordList);
        }


        private string BindProperty(object property, string propertyName)
        {
            string retValue = "";

            if (propertyName.Contains("."))
            {
                PropertyInfo[] arrayProperties;
                string leftPropertyName;

                leftPropertyName = propertyName.Substring(0, propertyName.IndexOf("."));
                arrayProperties = property.GetType().GetProperties();

                foreach (PropertyInfo propertyInfo in arrayProperties)
                {
                    if (propertyInfo.Name == leftPropertyName)
                    {
                        retValue = BindProperty(
                          propertyInfo.GetValue(property, null),
                          propertyName.Substring(propertyName.IndexOf(".") + 1));
                        break;
                    }
                }
            }
            else
            {
                Type propertyType;
                PropertyInfo propertyInfo;

                propertyType = property.GetType();
                propertyInfo = propertyType.GetProperty(propertyName);
                retValue = propertyInfo.GetValue(property, null).ToString();
            }

            return retValue;
        }



        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExecuteRequest_Click(object sender, EventArgs e)
        {
            this.ClearFilterControlAndContent();
            this.DisableAllInputControls();
            btnExecuteRequest.BackColor = System.Drawing.Color.Salmon;
            lblExecutingStatus.Text = "EXECUTING...";
            this.Refresh();
            Thread.Sleep(400);

            try
            {
                IEAMIServiceOperations eamiSvsCli = Helper.GetEAMISvsClientInstance();
                this.PopulatePaymentSubmissionRequestFromControls();

                // execute request
                response = eamiSvsCli.EAMIPaymentSubmission(request);
                //this.LoadResponseFromObj(response);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.LoadRequestTransactionInputControlsFromObj(request);
            EnableMainClickControls(true);
            lblExecutingStatus.Text = string.Empty;
            btnExecuteRequest.BackColor = System.Drawing.Color.MediumSeaGreen;
        }


        #region file & database menu event handlers

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ClearAllRequestInputControls();
            this.DisableAllInputControls();

            txtFile.Text = string.Empty;
            txtFileSize.Text = string.Empty;
            request = new PaymentSubmissionRequest();
            request.TransactionType = Helper.ToEnum<TransactionType>(2);
            request.PaymentRecordList = new List<PaymentRecord>();
            this.LoadRequestTransactionInputControlsFromObj(request);
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ImportFromFile();
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ExportFile(false, false);
        }


        private void exportToolStripMenuItemWAttachment_Click(object sender, EventArgs e)
        {
            this.ExportFile(true, false);
        }

        private void exportToolStripMenuItemReqAndResp_Click(object sender, EventArgs e)
        {
            this.ExportFile(false, true);
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ImportFromFile(true);
        }


        private void importFromDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DisableAllInputControls();

            DbLogin loginForm = new DbLogin(enParentForm.PaymentSubmission);
            loginForm.ShowDialog(this);


            // check if the the data was pulled or not
            if (loginForm.PymtSubmissionRequest == null)
            {
                MessageBox.Show("No data was imported.");
            }            
            else
            {
                txtFile.Text = string.Empty;
                txtFileSize.Text = string.Empty;
                MessageBox.Show("PaymentSubmissionRequest transaction with " + loginForm.PymtSubmissionRequest.PaymentRecordList.Count.ToString() + "  payment records successfully imported from db !", "Complete");
                request = loginForm.PymtSubmissionRequest;
            }

            this.LoadRequestTransactionInputControlsFromObj(request);
        }


        #endregion


        #region disable/enable, clear controls


        private void DisableAllInputControls()
        {
            this.EnableMainClickControls(false);
            this.EnableTransactionInputControls(false);           
        }


        public void EnableMainClickControls(bool enable)
        {
            // menu and transaction click controls
            menuFile.Enabled = enable;
            btnExecuteRequest.Enabled = enable;
        }


        public void EnableTransactionInputControls(bool enable)
        {
            btnGetCurrentTime.Enabled = enable;
            lnkClearTranInputFields.Enabled = enable;
            lnkAddRecord.Enabled = enable;
            lnkGenerateNewPymtRecSetNumbers.Enabled = enable;

            cmbFilterType.Enabled = enable;
            lnkResetFilter.Enabled = enable;

            txtReqSenderId.Enabled = enable;
            txtReqReceiverID.Enabled = enable;
            txtReqTranID.Enabled = enable;
            txtReqTranType.Enabled = enable;
            txtReqTranVersion.Enabled = enable;
            txtReqTimeStamp.Enabled = enable;
            txtReqRecordCount.Enabled = enable;
            txtReqRecTotalAmount.Enabled = enable;
            txtPayerEntityID.Enabled = enable;
            txtPayerEntityIDType.Enabled = enable;
            txtPayerName.Enabled = enable;
            txtPayerNameSfx.Enabled = enable;
            txtPayerAddrLine.Enabled = enable;
            txtPayerAddrCity.Enabled = enable;
            txtPayerAddrState.Enabled = enable;
            txtPayerAddrZip.Enabled = enable;
        }


        private void ClearAllRequestInputControls()
        {
            ClearFilterControlAndContent();
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

            txtPayerEntityID.Text = string.Empty;
            txtPayerEntityIDType.Text = string.Empty;
            txtPayerName.Text = string.Empty;
            txtPayerNameSfx.Text = string.Empty;
            txtPayerAddrLine.Text = string.Empty;
            txtPayerAddrCity.Text = string.Empty;

            txtReqRecordCount.Text = string.Empty;
            txtReqRecTotalAmount.Text = string.Empty;
            txtReqARecTotalAmount.Text = string.Empty;
        }






        #endregion





        #region populate object from input controls

        private void PopulatePaymentSubmissionRequestFromControls()
        {
            // we override all transaction values from input controls
            request.SenderID = txtReqSenderId.Text;
            request.ReceiverID = txtReqReceiverID.Text;
            request.TransactionID = txtReqTranID.Text;
            request.TransactionType = Helper.ToEnum<TransactionType>(txtReqTranType.Text);
            request.TransactionVersion = txtReqTranVersion.Text;
            request.TimeStamp = Helper.GetDateFromString(txtReqTimeStamp.Text);

            if (request.PayerInfo == null) request.PayerInfo = new PaymentExchangeEntity();
            request.PayerInfo.Name = txtPayerName.Text;
            request.PayerInfo.EntityIDSuffix = txtPayerNameSfx.Text;
            request.PayerInfo.EntityID = txtPayerEntityID.Text;
            request.PayerInfo.EntityIDType = txtPayerEntityIDType.Text;
            request.PayerInfo.AddressLine1 = txtPayerAddrLine.Text;
            request.PayerInfo.AddressLine2 = string.Empty;
            request.PayerInfo.AddressLine3 = string.Empty;
            request.PayerInfo.AddressCity = txtPayerAddrCity.Text;
            request.PayerInfo.AddressState = txtPayerAddrState.Text;
            request.PayerInfo.AddressZip = txtPayerAddrZip.Text;

            request.PaymentRecordCount = txtReqRecordCount.Text;
            request.PaymentRecordTotalAmount = txtReqRecTotalAmount.Text;

        }

        private void LoadRequestTransactionInputControlsFromObj(PaymentSubmissionRequest psr)
        {
            this.ClearRequestTransactionInputFields();
            if (psr == null) return;
            this.EnableMainClickControls(true);
            this.EnableTransactionInputControls(true);

            txtReqSenderId.Text = psr.SenderID;
            txtReqReceiverID.Text = psr.ReceiverID;
            txtReqTranID.Text = psr.TransactionID;
            txtReqTranType.Text = psr.TransactionType.ToString();
            txtReqTranVersion.Text = psr.TransactionVersion;
            txtReqTimeStamp.Text = DateTime.Now.ToString();
            txtReqRecordCount.Text = psr.PaymentRecordCount;
            txtReqRecTotalAmount.Text = psr.PaymentRecordTotalAmount;

            // payer info
            if (psr.PayerInfo != null)
            {
                txtPayerEntityID.Text = psr.PayerInfo.EntityID;
                txtPayerEntityIDType.Text = psr.PayerInfo.EntityIDType;
                txtPayerName.Text = psr.PayerInfo.Name;
                txtPayerNameSfx.Text = psr.PayerInfo.EntityIDSuffix;
                txtPayerAddrLine.Text = psr.PayerInfo.AddressLine1;
                txtPayerAddrCity.Text = psr.PayerInfo.AddressCity;
                txtPayerAddrState.Text = psr.PayerInfo.AddressState;
                txtPayerAddrZip.Text = psr.PayerInfo.AddressZip;
            }

            this.LoadPaymentGridViewFromObjList(psr.PaymentRecordList);
            txtReqARecTotalAmount.Text = Helper.GetPaymentRecordListTotalAmount(psr.PaymentRecordList).ToString();
            txtActualRecordCount.Text = psr.PaymentRecordList.Count().ToString();
            btnExecuteRequest.Focus();
        }      


        private void LoadPaymentGridViewFromObjList(List<PaymentRecord> psrList)
        {
            dgvPaymentRecords.DataSource = null;
            dgvPaymentRecords.DataSource = psrList;
        }

        #endregion
               

        private void ImportFromFile(bool isDefaultTemplate = false)
        {
            this.ClearFilterControlAndContent();
            this.DisableAllInputControls();
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
                        Thread.Sleep(1000);
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
                request = Helper.Deserialize(xml, typeof(PaymentSubmissionRequest)) as PaymentSubmissionRequest;
                this.LoadRequestTransactionInputControlsFromObj(request);

                // we only want show this message when a non-default xml was loaded                
                if (isDefaultTemplate == false)
                {
                    MessageBox.Show(this, "Loaded xml file !" +
                       Environment.NewLine +
                       Environment.NewLine +
                       fileName, "Done !", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lblExecutingStatus.Text = string.Empty;
                }
                txtFile.Text = Path.GetFileName(fileName);
                txtFileSize.Text = Helper.GetFileSizeText(fileName);
            }
            catch (Exception ex)
            {
                lblExecutingStatus.Text = string.Empty;
                MessageBox.Show(this, ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.newToolStripMenuItem_Click(null, null);
            }
        }


        private void ExportFile(bool withAttachment, bool includeResponse)
        {
            try
            {
                ClearFilterControlAndContent();

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "xml files (*.xml)|*.xml";
                sfd.InitialDirectory = Helper.GetExecutingExportLocationPath();
                string timeStamp = Helper.GetTimeStamp();
                sfd.FileName = Helper.GetCurrentUserIdentity().Substring(9) +
                                "_" + reqTemplateName + "_" +
                                timeStamp +
                                (withAttachment ? "_wAtch" : string.Empty) +
                                ".xml";
                //sfd.RestoreDirectory = true;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    this.PopulatePaymentSubmissionRequestFromControls();
                    PaymentSubmissionRequest tempRequest = !withAttachment ? CreateCopyWithoutAttachments(request) : request;
                    string xml = Helper.Serialize(tempRequest, tempRequest.GetType());
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
                    txtFileSize.Text = Helper.GetFileSizeText(sfd.FileName);
                    MessageBox.Show(this, "Saved xml file !" +
                        Environment.NewLine +
                        Environment.NewLine +
                        sfd.FileName, "Done !", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private PaymentSubmissionRequest CreateCopyWithoutAttachments(PaymentSubmissionRequest psr)
        {
            if (psr == null &&
                psr.PaymentRecordList == null &&
                psr.PaymentRecordList.Count == 0)
            {
                return psr;
            }

            // create a clone copy, remove attachments
            PaymentSubmissionRequest requestWoAttachments = Helper.GetCloneCopy<PaymentSubmissionRequest>(psr);
            foreach (PaymentRecord pr in requestWoAttachments.PaymentRecordList)
            {
                pr.Attachment = null;
            }
            return requestWoAttachments;
        }


        private static Random random = new Random();
        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
                

        private void cmbFilterType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmbFilterType.SelectedItem != null)
            {

                cmbFilterValue.BackColor = SystemColors.Info;
                cmbFilterType.BackColor = SystemColors.Info; 

                cmbFilterValue.Enabled = true;
                lblFilterResult.Text = string.Empty;

                // here we just reset previously filtered resuls
                // until user can select specific filter value
                if (filteredList != null)
                {                    
                    LoadPaymentGridViewFromObjList(request.PaymentRecordList);                    
                }

                string selectedFilterType = cmbFilterType.SelectedItem as string;
                cmbFilterValue.DataSource = GetFilterValuesBasedOnFilterType(selectedFilterType);
                cmbFilterValue.DisplayMember = "";
                cmbFilterValue.ValueMember = "";
                cmbFilterValue.SelectedIndex = -1;
            }           
        }

        private void cmbFilterValue_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmbFilterValue.SelectedItem != null)
            {
                cmbFilterValue.BackColor = Color.Gold;
                cmbFilterType.BackColor = Color.Gold;                

                //List<PaymentRecord> filteredPymtList = null;

                switch ((string)cmbFilterType.SelectedItem)
                {
                    case "PaymentType":
                        filteredList = request.PaymentRecordList.Where(t => t.PaymentType == cmbFilterValue.Text).ToList();
                        break;
                    case "PayeeName":
                        filteredList = request.PaymentRecordList.Where(t => t.PayeeInfo.Name == cmbFilterValue.Text).ToList();
                        break;
                    case "PayeeCode":
                        filteredList = request.PaymentRecordList.Where(t => t.PayeeInfo.EntityID == cmbFilterValue.Text).ToList();
                        break;
                    case "PayeeEIN":
                        filteredList = request.PaymentRecordList.Where(t => t.PayeeInfo.EIN == cmbFilterValue.Text).ToList();
                        break;
                }

                lblFilterResult.Text = filteredList.Count.ToString();
                LoadPaymentGridViewFromObjList(filteredList);
                this.ActiveControl = dgvPaymentRecords;
            }
        }


        private List<string> GetFilterValuesBasedOnFilterType(string filterType)
        {
            List<string> retValue = new List<string>();
            switch (filterType)
            {
                case "PaymentType":
                    retValue = request.PaymentRecordList.Select(t => t.PaymentType).Distinct().ToList();
                    break;
                case "PayeeName":
                    retValue = request.PaymentRecordList.Select(t => t.PayeeInfo.Name).Distinct().ToList();
                    break;
                case "PayeeCode":
                    retValue = request.PaymentRecordList.Select(t => t.PayeeInfo.EntityID).Distinct().ToList();
                    break;
                case "PayeeEIN":
                    retValue = request.PaymentRecordList.Select(t => t.PayeeInfo.EIN).Distinct().ToList();
                    break;
            }
            //retValue.Insert(0, "< SELECT >");
            return retValue;
        }

        private void lnkResetFilter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ClearFilterControlAndContent();
        }


        private void ClearFilterControlAndContent()
        {
            lblFilterResult.Text = string.Empty;
            if (filteredList != null)
            {
                filteredList = null;

                if (request != null && request.PaymentRecordList != null)
                {
                    LoadPaymentGridViewFromObjList(request.PaymentRecordList);
                }

                cmbFilterValue.DataSource = null;
                cmbFilterValue.SelectedIndex = -1;                

                cmbFilterType.SelectedIndex = -1;

                cmbFilterValue.BackColor = SystemColors.Info;
                cmbFilterType.BackColor = SystemColors.Info;
            }
            cmbFilterValue.Enabled = false;
        }



    }
}
