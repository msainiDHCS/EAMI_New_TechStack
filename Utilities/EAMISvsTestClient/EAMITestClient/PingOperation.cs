using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.ServiceModel;

using EAMITestClient.EAMISvcRef;
//using OHC.EAMI.ServiceContract;
//using OHC.EAMI.ServiceManager;

namespace EAMITestClient
{
    public partial class PingOperation : Form
    {
        public PingOperation()
        {
            InitializeComponent();            
        }

        private void btnExecutePing_Click(object sender, EventArgs e)
        {
            this.ClearResponseControls();
            this.EnableInputControls(false);
            btnExecutePing.BackColor = System.Drawing.Color.Salmon;
            this.Refresh();
            Thread.Sleep(100);
            try
            {
                //EAMIServiceOperationsClient defaultClient = new EAMIServiceOperationsClient();
                //EndpointIdentity epi = defaultClient.Endpoint.Address.Identity;
                //EndpointAddress ea = new EndpointAddress(new Uri(serviceAddress), epi);
                //Binding b = defaultClient.Endpoint.Binding;
                ////BasicHttpBinding bhb = new EAMIServiceOperationsClient().Endpoint.Binding as BasicHttpBinding;
                //return new EAMIServiceOperationsClient(b, ea);  

                

                IEAMIServiceOperations eamiSvsCli = Helper.GetEAMISvsClientInstance();
                
                PingRequest pReq = GetPingRequestFromControls();
                PingResponse pResp = eamiSvsCli.Ping(pReq);
                //PingResponse pResp = defaultClient.Ping(pReq);
                this.PopulateInputControlsFromObj(pResp);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");            
            }
            this.EnableInputControls(true);
            btnExecutePing.BackColor = System.Drawing.Color.MediumSeaGreen;
        }       

        private void btnGetCurrentTime_Click(object sender, EventArgs e)
        {
            txtReqCliTimeStamp.Text = DateTime.Now.ToString();
            txtReqCliTimeStamp.ReadOnly = !timer1.Enabled;
            timer1.Enabled = !timer1.Enabled;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            txtReqCliTimeStamp.Text = DateTime.Now.ToString();
            txtReqCliTimeStamp.Refresh();
        }      

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PingOperation_Load(object sender, EventArgs e)
        {
            txtReqSenderId.Text = "CAPMAN";
            txtReqReceiverID.Text = "EAMI";
            txtReqCliTimeStamp.Text = DateTime.Now.ToString();          
        }

        private PingRequest GetPingRequestFromControls()
        {
            PingRequest pr = new PingRequest();
            pr.SenderID = txtReqSenderId.Text;
            pr.ReceiverID = txtReqReceiverID.Text;
            pr.ClientTimeStamp = Convert.ToDateTime(txtReqCliTimeStamp.Text);
            return pr;
        }

        private void PopulateInputControlsFromObj(PingResponse pResp)
        {
            txtRespSenderID.Text = pResp.SenderID;
            txtRespReceiverId.Text = pResp.ReceiverID;
            txtRespCliTimeStamp.Text = pResp.ClientTimeStamp.ToString();
            txtRespSvsTimeStamp.Text = pResp.ServerTimeStamp.ToString();
            txtRespMsg.Text = pResp.ResponseMessage;
        }

        private void EnableInputControls(bool enable)
        {
            btnExecutePing.Enabled = enable;
            btnGetCurrentTime.Enabled = enable;
            txtReqSenderId.Enabled = enable;
            txtReqReceiverID.Enabled = enable;
            txtReqCliTimeStamp.Enabled = enable;
        }

        private void ClearResponseControls()
        {
            txtRespReceiverId.Text = string.Empty;
            txtRespSenderID.Text = string.Empty;
            txtRespSvsTimeStamp.Text = string.Empty;
            txtRespCliTimeStamp.Text = string.Empty;
            txtRespMsg.Text = string.Empty;
        }

    }
}
