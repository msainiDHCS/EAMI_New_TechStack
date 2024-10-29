using OHC.EAMI.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OHC.EAMI.CommonEntity;
using OHC.EAMI.Common.ServiceInvoke;
using System.Collections;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            

            var x = OHC.EAMI.Util.ActiveDirectoryAccess.Instance.GetUserADProfileInfo("DHSINTRA", "rwoolf");

            WcfServiceInvoker _wcfService;
            _wcfService = new WcfServiceInvoker();

            //var cs = _wcfService.InvokeService<IEAMIWebUIDataService,CommonStatusPayload<List<RefCodeList>>>
            //                          (svc => svc.GetReferenceCode(enRefTables.TB_SYSTEM_OF_RECORD));


            //var cs = _wcfService.InvokeService<IEAMIWebUIDataService, List<EntityGroup<RefCode, PaymentRecGroup<string>>>>
            //                          (svc => svc.GetUnassignedPaymentRecsGroupedByPayeeAndPaymentType());

            // cs.FirstOrDefault().

            //var cs = _wcfService.InvokeService<IEAMIWebUIDataService, List<EAMIElement>>
            //                          (svc => svc.GetEAMIDataElements("aa"));

            var cs1 = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<RefCodeList>>>
                                      (svc => svc.GetReferenceCodeList(enRefTables.TB_DB_SETTING, enRefTables.TB_DRAWDATE_CALENDAR, enRefTables.TB_PAYDATE_CALENDAR, enRefTables.TB_TRANSACTION_TYPE));

            //var cs1 = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<RefCodeList>>>
            //                          (svc => svc.GetReferenceCode(enRefTables.TB_DB_SETTING, enRefTables.TB_DRAWDATE_CALENDAR, enRefTables.TB_PAYDATE_CALENDAR));

            Console.ReadLine();

            //CacheManager<EAMIUser>.Set(new EAMIUser(), "myuser", "AUTH",20);

            //var obj = CacheManager<EAMIUser>.Get("myuser","AUTH");

            //CacheManager<EAMIUser>.Remove("myuser", "AUTH");

            //var obj1 = CacheManager<EAMIUser>.Get("myuser", "AUTH");

            //EAMILogger.Instance.Error("Ram");

            //EAMILogger.Instance.Error(CultureInfo.InvariantCulture,"{0}", "Test log1");

            //try
            //{
            //    int x = 0; int y = 1;

            //    int z = y / 0;
            //}
            //catch (Exception ex)
            //{
            //    //EAMILogger.Instance.Error("{0}", "Test message");
            //    EAMILogger.Instance.Error(ex);
            //}

            //Console.ReadLine();
            //using (OHC.EAMI.DataAccess.EAMIAuthorization acc = new OHC.EAMI.DataAccess.EAMIAuthorization(OHC.EAMI.DataAccess.EAMIDatabaseType.Authorization))
            //{
            //    var x = acc.GetAllEAMIMasterData("ROLE");
            //}
        }
    }
}
