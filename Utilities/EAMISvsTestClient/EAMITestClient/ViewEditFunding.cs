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
    public partial class ViewEditFunding : Form
    {
        private FundingDetail fdOrig = null;
        private List<FundingDetail> _fdList = null;
        private bool isNew = false;

        public ViewEditFunding(FundingDetail fd)
        {
            fdOrig = fd;
            InitializeComponent();
            if (fd != null)
            {
                txtR1F1FundSourceName.Text = fd.FundingSourceName;
                txtR1F1FFPAmount.Text = fd.FFPAmount;
                txtR1F1SGFAmount.Text = fd.SGFAmount;
                try
                {
                    txtTotalAmount.Text = (decimal.Parse(fd.FFPAmount) + decimal.Parse(fd.SGFAmount)).ToString();
                }
                catch { }
                
                txtR1F1FiscalYear.Text = fd.FiscalYear;
                txtR1F1FiscalQuarter.Text = fd.FiscalQuarter;                
                txtTitle.Text = fd.Title;
            }
        }

        public ViewEditFunding(List<FundingDetail> fdList)
        {
            InitializeComponent();
            isNew = true;
            this.Text = "New Funding Detail";
            _fdList = fdList;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            FundingDetail tempFd = isNew ? new FundingDetail() : fdOrig;
            tempFd.FundingSourceName = txtR1F1FundSourceName.Text;
            tempFd.FFPAmount = txtR1F1FFPAmount.Text;
            tempFd.SGFAmount = txtR1F1SGFAmount.Text;
            tempFd.FiscalYear = txtR1F1FiscalYear.Text;
            tempFd.FiscalQuarter = txtR1F1FiscalQuarter.Text;
            tempFd.Title = txtTitle.Text;
            if (isNew)
            {
                _fdList.Add(tempFd);                          
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
