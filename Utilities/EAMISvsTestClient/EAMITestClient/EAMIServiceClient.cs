
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using System.Configuration;
using System.ServiceModel;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.DirectoryServices;
using System.Text.RegularExpressions;

//using OHC.EAMI.ServiceContract;
using EAMITestClient.EAMISvcRef;

namespace EAMITestClient
{
    public partial class EAMIServiceClient : Form
    {
        public EAMIServiceClient()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        private void btnPaymentSubmissionNew_Click(object sender, EventArgs e)
        {
            NewPaymentSubmissionOp npso = new NewPaymentSubmissionOp();
            npso.ShowDialog(this);
            txtBasicAuthIdentity.Text = Helper.loginInfo != null ? "Basic Auth Identity: " + Helper.loginInfo.UserName : string.Empty;             
        }        



        private void btnNewStatusInquiry_Click(object sender, EventArgs e)
        {
            NewStatusInquiryOperation nsio = new NewStatusInquiryOperation();
            nsio.ShowDialog(this);
        }

        private void btnPing_Click(object sender, EventArgs e)
        {
            PingOperation po = new PingOperation();
            po.ShowDialog(this);
            txtBasicAuthIdentity.Text = Helper.loginInfo != null ? "Basic Auth Identity: " + Helper.loginInfo.UserName : string.Empty;
        }


        private void EAMIServiceClient_Load(object sender, EventArgs e)
        {            
            this.Text = this.Text + "  (v" + Helper.GetVersionOfExecutingAssembly() + ")";
            txtUserIdentity.Text = Helper.GetCurrentUserIdentity();
            //string defaultSvcAddr = "-"; 
            string defaultSvcAddr = new EAMIServiceOperationsClient().Endpoint.Address.Uri.ToString();                       
            lnkServiceURI.Text = defaultSvcAddr;
            Helper.serviceAddress = defaultSvcAddr;

            bool displaySvcAddrSelectOptions = bool.Parse(ConfigurationManager.AppSettings["DisplaySvcAddrSelectOptions"].ToString());
            CheckAddrOptionMatchingDefaultAddr(defaultSvcAddr);

            rbtnDevRX.Enabled = displaySvcAddrSelectOptions;
            rbtnStagRX.Enabled = displaySvcAddrSelectOptions;
            rbtnDevDntl.Enabled = displaySvcAddrSelectOptions;

            rbtnQARX.Enabled = displaySvcAddrSelectOptions;
            rbtnStagDntl.Enabled = displaySvcAddrSelectOptions;
            rbtnQADntl.Enabled = displaySvcAddrSelectOptions;                                   
        }

        private void lnkServiceURI_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            WebBrowser wb = new WebBrowser();
            wb.AllowNavigation = true;
            wb.Navigate(lnkServiceURI.Text, true);
            //wb.Show();            
        }

        private void btnNewRejectedPaymentInquiry_Click(object sender, EventArgs e)
        {
            NewRejectedPaymentInquiry nrpi = new NewRejectedPaymentInquiry();
            nrpi.ShowDialog(this);
        }


        private void rbtnDevRX_CheckedChanged(object sender, EventArgs e)
        {
            Helper.serviceAddress = GetDevRXSvcAddr();
            lnkServiceURI.Text = Helper.serviceAddress;
        }

        private void rbtnDevDntl_CheckedChanged(object sender, EventArgs e)
        {
            Helper.serviceAddress = GetDevDntlSvcAddr();
            lnkServiceURI.Text = Helper.serviceAddress;
        }

        private void rbtnQARX_CheckedChanged(object sender, EventArgs e)
        {
            Helper.serviceAddress = GetQARXSvcAddr();
            lnkServiceURI.Text = Helper.serviceAddress;
        }

        private void rbtnQADntl_CheckedChanged(object sender, EventArgs e)
        {
            Helper.serviceAddress = GetQADntlSvcAddr();
            lnkServiceURI.Text = Helper.serviceAddress;
        }

        private void rbtnStagRX_CheckedChanged(object sender, EventArgs e)
        {
            Helper.serviceAddress = GetStagRXSvcAddr();
            lnkServiceURI.Text = Helper.serviceAddress;
        }

        private void rbtnStagDntl_CheckedChanged(object sender, EventArgs e)
        {
            Helper.serviceAddress = GetStagDntlSvcAddr();
            lnkServiceURI.Text = Helper.serviceAddress;
        }


        private string GetDevRXSvcAddr()
        {
            return ConfigurationManager.AppSettings["DevRXSvcAddr"].ToString();
        }

        private string GetDevDntlSvcAddr()
        {
            return ConfigurationManager.AppSettings["DevDntlSvcAddr"].ToString();
        }

        private string GetQARXSvcAddr()
        {
            return ConfigurationManager.AppSettings["QARXSvcAddr"].ToString();
        }

        private string GetQADntlSvcAddr()
        {
            return ConfigurationManager.AppSettings["QADntlSvcAddr"].ToString();
        }

        private string GetStagRXSvcAddr()
        {
            return ConfigurationManager.AppSettings["StagRXSvcAddr"].ToString();
        }

        private string GetStagDntlSvcAddr()
        {
            return ConfigurationManager.AppSettings["StagDntlSvcAddr"].ToString();
        }


        private void CheckAddrOptionMatchingDefaultAddr(string defaultSvcAddr)
        {
            if (defaultSvcAddr == GetDevRXSvcAddr())
            {
                rbtnQARX.Checked = true;
            }            
            else if (defaultSvcAddr == GetDevDntlSvcAddr())
            {
                rbtnQADntl.Checked = true;
            }
            else if (defaultSvcAddr == GetQARXSvcAddr())
            {
                rbtnDevRX.Checked = true;
            }           
            else if (defaultSvcAddr == GetQADntlSvcAddr())
            {
                rbtnDevRX.Checked = true;
            }
            else if (defaultSvcAddr == GetStagRXSvcAddr())
            {
                rbtnStagRX.Checked = true;
            }
            else if (defaultSvcAddr == GetStagDntlSvcAddr())
            {
                rbtnStagDntl.Checked = true;
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            //string xml = File.ReadAllText(@"C:\MyShared\EAMI\sample EFT xml file\20161207-ADP-EFTDATA-94FB645CE2C64796AEB499CEAD028233_woAttachment_3.xml");            
            ////xml = xml.Replace("<EFTEnrollment", "<EFTEnrollment xmlns="http://schemas.datacontract.org/2004/07/EAMITestClient.EFTEntrollment");
            //EFTEnrollment efte = Helper.Deserialize(xml, typeof(EFTEnrollment)) as EFTEnrollment;


            //XDocument xdoc = XDocument.Load(@"C:\MyShared\EAMI\sample EFT xml file\20161207-ADP-EFTDATA-94FB645CE2C64796AEB499CEAD028233_woAttachment_2.xml");

            //XDocument xdoc = XDocument.Load(@"C:\FileReceiveLocations\2019-11-15-ADP-EFTDATA-4CB3575C-0C16-4485-9160-01FFE0E3CEDF.xml");
            //XDocument xdoc = XDocument.Load(@"C:\FileReceiveLocations\2020-01-31-ADP-EFTDATA-A04BB1B9-763B-4232-B50A-5A57BA28247D.xml");
            XDocument xdoc = XDocument.Load(@"C:\FileReceiveLocations\2020-01-31-ADP-EFTDATA-A04BB1B9-763B-4232-B50A-5A57BA28247D_file2.xml");

            //XDocument xdoc = XDocument.Load(@"C:\FileReceiveLocations\2020-01-31-ADP-EFTDATA-A04BB1B9-763B-4232-B50A-5A57BA28247D_file3.xml");

            EFTEnrollment efte = new EFTEnrollment(xdoc);

            //foreach(XElement xe in xdoc.Elements())
            //{

            //}


            //EFTEnrollment efte = Helper.DeserializeXMLFileToObject<EFTEnrollment>(@"C:\MyShared\EAMI\sample EFT xml file\new\2019-11-15-ADP-EFTDATA-4CB3575C-0C16-4485-9160-01FFE0E3CEDF.xml");
        }




        private void btnTest2_Click(object sender, EventArgs e)
        {
            string queryAllUsers = "(&(objectClass=user)(objectCategory=Person)" +
                      "(!displayName=zzz*)(!sAMAccountName=zzz*)" +
                      "(!extensionAttribute13=*)(distinguishedName=*)(givenName=*)(sn=*)" +
                      "(!UserAccountControl:1.2.840.113556.1.4.803:=2)" +
                      "(&(!(ou:dn:=TestOUs))(!(ou:dn:=UserRemoves))))";

            string queryUser = "(&(objectClass=user)(objectCategory=Person)" +
                                "(saMAccountName={0})" +
                                "(&(!(ou:dn:=TestOUs))(!(ou:dn:=UserRemoves))))";

            //string queryUser = "(&(objectClass=user)(objectCategory=Person)" +
            //                    "(saMAccountName={0})" +
            //                    "(!displayName=zzz*)(!sAMAccountName=zzz*)" +
            //                      "(!extensionAttribute13=*)(distinguishedName=*)(givenName=*)(sn=*)" +
            //                      "(!UserAccountControl:1.2.840.113556.1.4.803:=2)" +
            //                      "(&(!(ou:dn:=TestOUs))(!(ou:dn:=UserRemoves))))";

            //string queryUser = "(&(objectClass=user)(objectCategory=Person)" +
            //                    "(saMAccountName={0}))";

            //string user = "esamoylo@dhcs.ca.gov";
            //string user = "dhsintra\\esamoylo";
            //string user = "Eugene.Samoylovich@dhcs.ca.gov";
            //string user = "esamoylo";
            //string user = "ggidenko";
            //string user = "CZapata";
            string user = "mlepage";
            //string user = "PMolina";
            //string user = "BNegiA";
            // esamoylo@dhcs.ca.gov


            //GetLastLogon(user)
            SearchResultCollection result = null;
            //Dim results As SearchResultCollection = Nothing

            int intRowCount = 0;
            //Dim intRowCount As Integer = 0

            DataTable dtResults = new DataTable();
            //Dim dtResults As DataTable

            //DirectoryEntry entry = new DirectoryEntry("LDAP://intra.dhs.ca.gov", "dhsintra\\esamoylo", "Cloudking*8", AuthenticationTypes.Secure);
            //DirectoryEntry entry = new DirectoryEntry("LDAP://intra.dhs.ca.gov", "dhsintra\\esamoylo", "Cloudking*8", AuthenticationTypes.None);
            //DirectoryEntry entry = new DirectoryEntry("LDAP://intra.dhs.ca.gov", "dhsintra\\esamoylo", "Cloudking*8", AuthenticationTypes.Secure);
            DirectoryEntry entry = new DirectoryEntry("LDAP://intra.dhs.ca.gov"); //, string.Empty, string.Empty "dhsintra\\esamoylo", "Cloudking*8", AuthenticationTypes.Secure);

            //DirectorySearcher searcher = new DirectorySearcher(entry, null, null, SearchScope.OneLevel);
            DirectorySearcher searcher = new DirectorySearcher(entry);


            searcher.Filter = string.Format(queryUser, user);
            searcher.PageSize = 1000;
            searcher.PropertiesToLoad.Add("sAMAccountName");

            searcher.PropertiesToLoad.Add("physicalDeliveryOfficeName");
            searcher.PropertiesToLoad.Add("objectGUID");
            searcher.PropertiesToLoad.Add("extensionAttribute13");

            //mySearcher.PropertiesToLoad.Add("lastLogon")
            //mySearcher.PropertiesToLoad.Add("userAccountControl")
            //mySearcher.PropertiesToLoad.Add("Manager")
            //mySearcher.PropertiesToLoad.Add("Department")
            //mySearcher.PropertiesToLoad.Add("Division")

            searcher.PropertiesToLoad.Add("mail");

            //mySearcher.PropertiesToLoad.Add("homeMDB")
            //mySearcher.PropertiesToLoad.Add("employeeID")


            searcher.PropertiesToLoad.Add("employeeType");
            searcher.PropertiesToLoad.Add("displayName");
            searcher.PropertiesToLoad.Add("givenName");
            searcher.PropertiesToLoad.Add("sn");

            //mySearcher.PropertiesToLoad.Add("middleName")

            searcher.PropertiesToLoad.Add("telephoneNumber");

            //mySearcher.PropertiesToLoad.Add("Title")


            // Use the FindAll method to return objects to a SearchResultCollection.
            // 08-02-2011: Angela block for test.
            result = searcher.FindAll();


            string pattern = @"^(\s*[0-9a-zA-Z]([-\.\'\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$";
            Regex r = new Regex(pattern);
            bool ismatched = r.IsMatch(result[0].Properties["mail"][0].ToString());


            //If Regex.IsMatch(propertyValue, pattern) Then
            //    If propertyValue.ToString.Length <= 50 Then
            //        'sAngela = "Valid Email: " + propertyValue + vbCrLf
            //    Else
            //        invalidEmail = True
            //        sErrorMessage = "Error-Invalid Email Lenght: " + propertyValue
            //        sAngela = "Invalid Email Length: " + propertyValue + vbCrLf
            //        intErrorCount = intErrorCount + 1
            //    End If

            string check = string.Empty;
        }

 
    }
}
