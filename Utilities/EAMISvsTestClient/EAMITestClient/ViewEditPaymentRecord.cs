
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

using EAMITestClient.EAMISvcRef;
//using OHC.EAMI.ServiceContract;
//using OHC.EAMI.ServiceManager;

namespace EAMITestClient
{

    public partial class ViewEditPaymentRecord : Form
    {
        private PaymentRecord prOrig = null;
        private PaymentRecord prClone = null;
        private List<PaymentRecord> prList = null;
        private bool isNew = false;
 
        public ViewEditPaymentRecord(PaymentRecord paymentRecord)
        {
            prOrig = paymentRecord;
            prClone = Helper.GetCloneCopy<PaymentRecord>(paymentRecord);
            
            InitializeComponent();
            dgvFunding.AutoGenerateColumns = false;
            dgvPRKeyValuePairList.AutoGenerateColumns = false;
            LoadRec1InputControlsFromObj(prClone);
        }

        public ViewEditPaymentRecord(List<PaymentRecord> paymentRecordList)
        {
            isNew = true;
            prClone = new PaymentRecord();
            prClone.GenericNameValueList = new Dictionary<string, string>();
            prClone.FundingDetailList = new List<FundingDetail>();
            prList = paymentRecordList;
            
            InitializeComponent();
            this.Text = "New Payment Record";
            dgvFunding.AutoGenerateColumns = false;
            dgvPRKeyValuePairList.AutoGenerateColumns = false;
            LoadRec1InputControlsFromObj(prClone);
        }

        private void LoadRec1InputControlsFromObj(PaymentRecord pr)
        {
            // reset controls
            if (pr == null)
            {
                this.ClearRequestRec1InputFields();
                this.EnableRecord1InputControls(false);
                return;
            }

            this.EnableRecord1InputControls(true);

            // populate input controls
            txtR1RecordID.Text = pr.PaymentRecNumber;
            txtR1PaymentRecNumExt.Text = pr.PaymentRecNumberExt;
            txtR1PaymentSetNum.Text = pr.PaymentSetNumber;
            txtR1PaymentSetNumExt.Text = pr.PaymentSetNumberExt;
            txtR1InvoiceType.Text = pr.PaymentType;
            txtR1InvoiceDate.Text = pr.PaymentDate.ToString();
            txtR1Amount.Text = pr.Amount;
            txtR1FiscalYear.Text = pr.FiscalYear;
            txtR1IndexCode.Text = pr.IndexCode;
            txtR1ObjDetailCode.Text = pr.ObjectDetailCode;
            txtR1ObjAgencyCode.Text = pr.ObjectAgencyCode;
            txtR1PCACode.Text = pr.PCACode;
            txtR1ApprovedBy.Text = pr.ApprovedBy;
            
            chkR1Attachment.Checked = (pr.Attachment != null && pr.Attachment.Count() > 0);
            lnkR1AddAttachment.Enabled = (chkR1Attachment.Checked == false);
            lnkR1ViewAttachment.Enabled = chkR1Attachment.Checked;
            lnkR1DeleteAttachment.Enabled = chkR1Attachment.Checked;

            // payee info
            if (pr.PayeeInfo != null)
            {
                txtR1PayeeEntityId.Text = pr.PayeeInfo.EntityID;
                txtR1PayeeEntityIDType.Text = pr.PayeeInfo.EntityIDType;
                txtR1PayeeName.Text = pr.PayeeInfo.Name;
                txtR1PayeeNameSfx.Text = pr.PayeeInfo.EntityIDSuffix;
                txtR1PayeeEIN.Text = pr.PayeeInfo.EIN;
                txtR1PayeeVendorTypeCode.Text = pr.PayeeInfo.VendorTypeCode;
                txtR1PayeeAddrLine1.Text = pr.PayeeInfo.AddressLine1;
                txtR1PayeeAddrLine2.Text = pr.PayeeInfo.AddressLine2;
                txtR1PayeeAddrLine3.Text = pr.PayeeInfo.AddressLine3;
                txtR1PayeeAddrCity.Text = pr.PayeeInfo.AddressCity;
                txtR1PayeeAddrState.Text = pr.PayeeInfo.AddressState;
                txtR1PayeeAddrZip.Text = pr.PayeeInfo.AddressZip;
            }

            if (pr.GenericNameValueList != null && pr.GenericNameValueList.Count > 0)
            {
                var keyValuePairList = from row in pr.GenericNameValueList select new { Key = row.Key, Value = row.Value };
                dgvPRKeyValuePairList.DataSource = keyValuePairList.ToArray();
            }

            if (pr.FundingDetailList != null && pr.FundingDetailList.Count > 0)
            {
                dgvFunding.DataSource = null;                  
                dgvFunding.DataSource = pr.FundingDetailList;
                texTxtTotalFundAmount.Text = Helper.GetFundingListTotalAmount(pr.FundingDetailList).ToString();
            }
        }


        private void ClearRequestRec1InputFields()
        {
            // payment record 1
            txtR1RecordID.Text = string.Empty;
            txtR1PaymentRecNumExt.Text = string.Empty;
            txtR1PaymentSetNum.Text = string.Empty;
            txtR1PaymentSetNumExt.Text = string.Empty;
            txtR1InvoiceType.Text = string.Empty;
            txtR1InvoiceDate.Text = string.Empty;
            txtR1Amount.Text = string.Empty;
            txtR1FiscalYear.Text = string.Empty;
            txtR1IndexCode.Text = string.Empty;
            txtR1ObjDetailCode.Text = string.Empty;
            txtR1ObjAgencyCode.Text = string.Empty;
            txtR1PCACode.Text = string.Empty;
            txtR1ApprovedBy.Text = string.Empty;            

            txtR1PayeeEntityId.Text = string.Empty;
            txtR1PayeeEntityIDType.Text = string.Empty;
            txtR1PayeeName.Text = string.Empty;
            txtR1PayeeNameSfx.Text = string.Empty;
            txtR1PayeeEIN.Text = string.Empty;
            txtR1PayeeVendorTypeCode.Text = string.Empty;
            txtR1PayeeAddrLine1.Text = string.Empty;
            txtR1PayeeAddrLine2.Text = string.Empty;
            txtR1PayeeAddrLine3.Text = string.Empty;
            txtR1PayeeAddrCity.Text = string.Empty;
            txtR1PayeeAddrState.Text = string.Empty;
            txtR1PayeeAddrZip.Text = string.Empty;

            chkR1Attachment.Checked = false;
            lnkR1AddAttachment.Enabled = true;
            lnkR1ViewAttachment.Enabled = false;
            lnkR1DeleteAttachment.Enabled = false;

            texTxtTotalFundAmount.Text = string.Empty;
        }

        public void EnableRecord1InputControls(bool enable)
        {            
            // record 1 input controls
            txtR1RecordID.Enabled = enable;
            txtR1PaymentRecNumExt.Enabled = enable;
            txtR1PaymentSetNum.Enabled = enable;
            txtR1PaymentSetNumExt.Enabled = enable;
            txtR1InvoiceType.Enabled = enable;
            txtR1InvoiceDate.Enabled = enable;
            txtR1Amount.Enabled = enable;
            txtR1FiscalYear.Enabled = enable;
            txtR1IndexCode.Enabled = enable;
            txtR1ObjDetailCode.Enabled = enable;
            txtR1ObjAgencyCode.Enabled = enable;
            txtR1PCACode.Enabled = enable;
            txtR1ApprovedBy.Enabled = enable;
           
            txtR1PayeeEntityId.Enabled = enable;
            txtR1PayeeEntityIDType.Enabled = enable;
            txtR1PayeeName.Enabled = enable;
            txtR1PayeeNameSfx.Enabled = enable;
            txtR1PayeeEIN.Enabled = enable;
            txtR1PayeeVendorTypeCode.Enabled = enable;
            txtR1PayeeAddrLine1.Enabled = enable;
            txtR1PayeeAddrLine2.Enabled = enable;
            txtR1PayeeAddrLine3.Enabled = enable;
            txtR1PayeeAddrCity.Enabled = enable;
            txtR1PayeeAddrState.Enabled = enable;
            txtR1PayeeAddrZip.Enabled = enable;

            // record 1 click controls
            lnkR1AddAttachment.Enabled = enable;
            lnkR1ViewAttachment.Enabled = enable;
            lnkR1DeleteAttachment.Enabled = enable;
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            PaymentRecord tempPr = isNew ? prClone : prOrig;                                                
            tempPr.PaymentRecNumber = txtR1RecordID.Text;
            tempPr.PaymentRecNumberExt = txtR1PaymentRecNumExt.Text;
            tempPr.PaymentSetNumber = txtR1PaymentSetNum.Text;
            tempPr.PaymentSetNumberExt = txtR1PaymentSetNumExt.Text;
            tempPr.PaymentType = txtR1InvoiceType.Text;
            tempPr.PaymentDate = Helper.GetDateFromString(txtR1InvoiceDate.Text);
            tempPr.PCACode = txtR1PCACode.Text;
            tempPr.ObjectDetailCode = txtR1ObjDetailCode.Text;
            tempPr.ObjectAgencyCode = txtR1ObjAgencyCode.Text;
            tempPr.IndexCode = txtR1IndexCode.Text;
            tempPr.FiscalYear = txtR1FiscalYear.Text;
            tempPr.Amount = txtR1Amount.Text;
            tempPr.ApprovedBy = txtR1ApprovedBy.Text;

            if (tempPr.PayeeInfo == null) tempPr.PayeeInfo = new PaymentExchangeEntity();
            tempPr.PayeeInfo.EntityID = txtR1PayeeEntityId.Text;
            tempPr.PayeeInfo.EntityIDType = txtR1PayeeEntityIDType.Text;
            tempPr.PayeeInfo.Name = txtR1PayeeName.Text;
            tempPr.PayeeInfo.EntityIDSuffix = txtR1PayeeNameSfx.Text;
            tempPr.PayeeInfo.EIN = txtR1PayeeEIN.Text;
            tempPr.PayeeInfo.VendorTypeCode = txtR1PayeeVendorTypeCode.Text;
            tempPr.PayeeInfo.AddressLine1 = txtR1PayeeAddrLine1.Text;
            tempPr.PayeeInfo.AddressLine2 = txtR1PayeeAddrLine2.Text;
            tempPr.PayeeInfo.AddressLine3 = txtR1PayeeAddrLine3.Text;
            tempPr.PayeeInfo.AddressCity = txtR1PayeeAddrCity.Text;
            tempPr.PayeeInfo.AddressState = txtR1PayeeAddrState.Text;
            tempPr.PayeeInfo.AddressZip = txtR1PayeeAddrZip.Text;

            tempPr.Attachment = prClone.Attachment;

            tempPr.GenericNameValueList = prClone.GenericNameValueList;
            tempPr.FundingDetailList = prClone.FundingDetailList;

            if (isNew)
            {
                prList.Add(prClone);
            }

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void lnkR1AddAttachment_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            prClone.Attachment = GetAttachmentFile();
            this.LoadRec1InputControlsFromObj(prClone);
        }

        private void lnkR1ViewAttachment_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                string fileName = Helper.GetExecutingTempLocationPath() + "\\" + Guid.NewGuid() + ".pdf";
                File.WriteAllBytes(fileName, prClone.Attachment);
                System.Diagnostics.Process.Start(fileName);
            }
            catch
            {
                MessageBox.Show("Could not open attachment file", "ERROR");
            }
        }

        private void lnkR1DeleteAttachment_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            prClone.Attachment = null;
            this.LoadRec1InputControlsFromObj(prClone);
        }

        private byte[] GetAttachmentFile()
        {
            byte[] returnValue = null;
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = Helper.GetExecutingLocationPath();
                ofd.Filter = "pdf files (*.pdf)|*.pdf";
                //ofd.RestoreDirectory = true;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    returnValue = File.ReadAllBytes(ofd.FileName);

                    // validate proper pdf content
                    byte[] first4Chars = returnValue.Take(4).ToArray();
                    if (Encoding.UTF8.GetString(first4Chars, 0, first4Chars.Length) != "%PDF")
                    {
                        throw new Exception("Incorrect Attachment file/content, expecting PDF");
                    }
                }
            }
            catch (Exception ex)
            {               
                MessageBox.Show(this, ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return returnValue;
        }

        private void dgvPRKeyValuePairList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string action = dgvPRKeyValuePairList.Rows[e.RowIndex].Cells[e.ColumnIndex].FormattedValue.ToString();
            if (action == "edit" || action == "delete")
            {
                dynamic kvp = dgvPRKeyValuePairList.CurrentRow.DataBoundItem;
                if (action == "edit")
                {              
                    ViewEditKVP vekvp = new ViewEditKVP(prClone.GenericNameValueList, kvp.Key);
                    vekvp.ShowDialog(this);
                    //LoadRec1InputControlsFromObj(prClone);
                }
                else if (action == "delete")
                {
                    prClone.GenericNameValueList.Remove(kvp.Key);
                    //LoadRec1InputControlsFromObj(prClone);
                }
                var keyValuePairList = from row in prClone.GenericNameValueList select new { Key = row.Key, Value = row.Value };
                dgvPRKeyValuePairList.DataSource = keyValuePairList.ToArray();
            }            
        }

        private void dgvPRKeyValuePairList_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if (dgvPRKeyValuePairList.Rows[e.RowIndex].DataBoundItem != null)
                {
                    if (dgvPRKeyValuePairList.Columns[e.ColumnIndex].DataPropertyName == "Edit")
                    {
                        e.Value = "edit";
                    }
                    else if (dgvPRKeyValuePairList.Columns[e.ColumnIndex].DataPropertyName == "Delete")
                    {
                        e.Value = "delete";
                    }
                }
            }
            catch { }
        }

        private void dgvFunding_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string action = dgvFunding.Rows[e.RowIndex].Cells[e.ColumnIndex].FormattedValue.ToString();
            if (action == "edit" || action == "delete" || action == "clone")
            {
                FundingDetail fd = (FundingDetail)dgvFunding.CurrentRow.DataBoundItem;
                if (action == "edit")
                {                    
                    ViewEditFunding vef = new ViewEditFunding(fd);
                    vef.ShowDialog(this);
                }
                else if (action == "delete")
                {
                    prClone.FundingDetailList.RemoveAll(t => t.FundingSourceName == fd.FundingSourceName &&
                                                            t.FFPAmount == fd.FFPAmount &&
                                                            t.SGFAmount == fd.SGFAmount &&
                                                            t.FiscalYear == fd.FiscalYear &&
                                                            t.FiscalQuarter == fd.FiscalQuarter &&
                                                            t.Title == fd.Title);                                  
                }
                else if (action == "clone")
                {
                    FundingDetail fdClone = Helper.GetCloneCopy<FundingDetail>(fd);
                    prClone.FundingDetailList.Add(fdClone);                                        
                }

                texTxtTotalFundAmount.Text = Helper.GetFundingListTotalAmount(prClone.FundingDetailList).ToString();

                dgvFunding.DataSource = null;
                dgvFunding.DataSource = prClone.FundingDetailList;
            }
        }

        private void dgvFunding_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if (dgvFunding.Rows[e.RowIndex].DataBoundItem != null)
                {
                    if (dgvFunding.Columns[e.ColumnIndex].DataPropertyName == "Edit")
                    {
                        e.Value = "edit";
                    }
                    else if (dgvFunding.Columns[e.ColumnIndex].DataPropertyName == "Delete")
                    {
                        e.Value = "delete";
                    }
                    else if (dgvFunding.Columns[e.ColumnIndex].DataPropertyName == "Clone")
                    {
                        e.Value = "clone";
                    }
                }
            }
            catch { }
        }

        private void lnkAddNewKVP_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ViewEditKVP vekvp = new ViewEditKVP(prClone.GenericNameValueList);
            vekvp.ShowDialog(this);
            LoadRec1InputControlsFromObj(prClone);
        }

        private void lnkAddNewFunding_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ViewEditFunding vef = new ViewEditFunding(prClone.FundingDetailList);
            vef.ShowDialog(this);
            LoadRec1InputControlsFromObj(prClone);
            texTxtTotalFundAmount.Text = Helper.GetFundingListTotalAmount(prClone.FundingDetailList).ToString();
        }


    }
}
