using OHC.EAMI.WebUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OHC.EAMI.WebUI.Data
{
    public class InvoiceQueries
    {

        public static List<AssignInvoicesViewModel> GetVendorsDisplayList()
        {
            try
            {
                //Dictionary<long, bool> dictionaryForRetBNCList = new Dictionary<long, bool>();
                //List<BeneficiaryNonmanagedCare> retBNCList = new List<BeneficiaryNonmanagedCare>();

                AssignInvoicesViewModel assignInvoicesViewModel1 = new AssignInvoicesViewModel();
                AssignInvoicesViewModel assignInvoicesViewModel2 = new AssignInvoicesViewModel();
                AssignInvoicesViewModel assignInvoicesViewModel3 = new AssignInvoicesViewModel();
                AssignInvoicesViewModel assignInvoicesViewModel4 = new AssignInvoicesViewModel();
                AssignInvoicesViewModel assignInvoicesViewModel5a = new AssignInvoicesViewModel();
                AssignInvoicesViewModel assignInvoicesViewModel5b = new AssignInvoicesViewModel();
                AssignInvoicesViewModel assignInvoicesViewModel5c = new AssignInvoicesViewModel();

                assignInvoicesViewModel1.EntityName = "Santa Cruz-Monterey-Merced Managed Medical Care Commission";
                assignInvoicesViewModel2.EntityName = "Santa Cruz-Monterey-Merced Managed Medical Care Commission";
                assignInvoicesViewModel3.EntityName = "Vendor 3";
                assignInvoicesViewModel4.EntityName = "Vendor 4";
                assignInvoicesViewModel5a.EntityName = "Vendor 5";
                assignInvoicesViewModel5b.EntityName = "Vendor 5";
                assignInvoicesViewModel5c.EntityName = "Vendor 5";

                assignInvoicesViewModel1.ModelName = "Two-Plan Local Initiative";
                assignInvoicesViewModel2.ModelName = "San Francisco Family Mosaic";
                assignInvoicesViewModel3.ModelName = "Model 3";
                assignInvoicesViewModel4.ModelName = "Model 4";
                assignInvoicesViewModel5a.ModelName = "Model 5a";
                assignInvoicesViewModel5b.ModelName = "Model 5b";
                assignInvoicesViewModel5c.ModelName = "Model 5c";

                assignInvoicesViewModel1.TotalAmount = 1.01m;
                assignInvoicesViewModel2.TotalAmount = 2.02m;
                assignInvoicesViewModel3.TotalAmount = 3.03m;
                assignInvoicesViewModel4.TotalAmount = 4.04m;
                assignInvoicesViewModel5a.TotalAmount = 5.05m;
                assignInvoicesViewModel5b.TotalAmount = 6.05m;
                assignInvoicesViewModel5c.TotalAmount = 7.05m;

                assignInvoicesViewModel1.VendorDisplayId = ("VendorDisplayId_" + 1).Trim();
                assignInvoicesViewModel2.VendorDisplayId = ("VendorDisplayId_" + 2).Trim();
                assignInvoicesViewModel3.VendorDisplayId = ("VendorDisplayId_" + 3).Trim();
                assignInvoicesViewModel4.VendorDisplayId = ("VendorDisplayId_" + 4).Trim();
                assignInvoicesViewModel5a.VendorDisplayId = ("VendorDisplayId_" + "5a").Trim();
                assignInvoicesViewModel5b.VendorDisplayId = ("VendorDisplayId_" + "5b").Trim();
                assignInvoicesViewModel5c.VendorDisplayId = ("VendorDisplayId_" + "5c").Trim();

                List<AssignInvoicesViewModel> retList = new List<AssignInvoicesViewModel>();
                retList.Add(assignInvoicesViewModel1);
                retList.Add(assignInvoicesViewModel2);
                retList.Add(assignInvoicesViewModel3);
                retList.Add(assignInvoicesViewModel4);
                retList.Add(assignInvoicesViewModel5a);
                retList.Add(assignInvoicesViewModel5b);
                retList.Add(assignInvoicesViewModel5c);

                return retList;
            }
            catch (Exception e)
            {
                //TraceSourceWriter.TraceError(e.TargetSite.DeclaringType.Namespace, e.ToString());
                throw;
            }
        }
    }
}

