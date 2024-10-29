
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Security.Principal;
using System.Xml.Linq;
using System.Xml;

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml.Serialization;

using EAMITestClient.EAMISvcRef;
//using OHC.EAMI.ServiceContract;
//using OHC.EAMI.ServiceManager;


namespace EAMITestClient
{    
    internal static class Helper
    {
        internal static string serviceAddress = string.Empty;
        internal static LoginInfo loginInfo = null;
        //internal static string userName = string.Empty;
        //internal static string password = string.Empty;


        internal static T DeserializeXMLFileToObject<T>(string XmlFilename)
        {
            T returnObject = default(T);
            if (string.IsNullOrEmpty(XmlFilename)) return default(T);

            try
            {
                StreamReader xmlStream = new StreamReader(XmlFilename);
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                returnObject = (T)serializer.Deserialize(xmlStream);
            }
            catch (Exception ex)
            {
                //ExceptionLogger.WriteExceptionToConsole(ex, DateTime.Now);
            }
            return returnObject;
        }


        internal static object Deserialize(string xml, Type toType)
        {
            using (Stream stream = new MemoryStream())
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                DataContractSerializer deserializer = new DataContractSerializer(toType);
                return deserializer.ReadObject(stream);
            }
        }


        internal static string Serialize(object obj, Type type)
        {
            DataContractSerializer serializer = new DataContractSerializer(type);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, obj);
                //string xml =  @"<?xml version=""1.0"" encoding=""utf-8""?>" + Encoding.UTF8.GetString(memoryStream.GetBuffer());
                return Encoding.UTF8.GetString(memoryStream.GetBuffer());
            }
        }

        internal static T GetCloneCopy<T>(T obj)
        {
            XmlDocument xdoc = new XmlDocument();            
            xdoc.LoadXml(Serialize(obj, obj.GetType()));
            return (T)Deserialize(xdoc.OuterXml, typeof(T));
        }

        internal static string GetFileSizeText(string filename)
        {
            if (!File.Exists(filename)) return string.Empty;
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = new FileInfo(filename).Length;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            return String.Format("{0:0.##} {1}", len, sizes[order]);
        }

        internal static string GetTimeStamp()
        {
            DateTime dtNow = DateTime.Now;
            string sDate = string.Format("{0}{1:D2}{2:D2}", dtNow.Year, dtNow.Month, dtNow.Day);
            string sTime = string.Format("{0:D2}{1:D2}{2:D2}", dtNow.Hour, dtNow.Minute, dtNow.Second);
            return string.Concat(sDate, "-", sTime);
        }

        internal static string GetExecutingLocationPath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        internal static string GetExecutingDefaultXmlFilesLocationPath()
        {
            string tempPath = GetExecutingLocationPath() + "\\default";
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            return tempPath;
        }

        internal static string GetExecutingTempLocationPath()
        {
            string tempPath = GetExecutingLocationPath() + "\\Temp";
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            return tempPath;
        }

        internal static string GetExecutingExportLocationPath()
        {
            string exportFilePath = GetExecutingLocationPath() + "\\ExportFile";
            if (!Directory.Exists(exportFilePath))
            {
                Directory.CreateDirectory(exportFilePath);
            }
            return exportFilePath;
        }

        internal static T ToEnum<T>(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return ToEnum<T>(1);
            }
            return (T)Enum.Parse(typeof(T), value, true);            
        }

        internal static T ToEnum<T>(this int value)
        {
            var name = Enum.GetName(typeof(T), value);
            return name.ToEnum<T>();
        }

        internal static decimal GetPaymentRecordListTotalAmount(List<PaymentRecord> recordList)
        {
            decimal retValue = 0;
            try
            {
                retValue = (recordList != null && recordList.Count > 0) ? recordList.Sum(s => decimal.Parse(s.Amount)) : 0;
            }
            catch{}
            return retValue;
        }

        internal static decimal GetFundingListTotalAmount(List<FundingDetail> fundingList)
        {
            decimal retValue = 0;
            try
            {
                retValue = (fundingList != null && fundingList.Count > 0) ? fundingList.Sum(s => decimal.Parse(s.FFPAmount) + decimal.Parse(s.SGFAmount)) : 0;
            }
            catch { }
            return retValue;
        }

        internal static string GetStringFromDate(DateTime date)
        {
           return (date == DateTime.MinValue) ? string.Empty : date.ToString();
        }

        internal static DateTime GetDateFromString(string date)
        {
            DateTime outDate;
            try
            {
                outDate = Convert.ToDateTime(date);
            }
            catch
            {
                outDate  = DateTime.MinValue;
            }
            return outDate;
        }

        internal static string GetCurrentUserIdentity()
        {
            return WindowsIdentity.GetCurrent().Name;
        }

        internal static string GetVersionOfExecutingAssembly()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        
        internal static IEAMIServiceOperations GetEAMISvsClientInstance()
        {
            // code to setup EAMI secure binding with dynamic URL"
            EAMIServiceOperationsClient defaultClient = new EAMIServiceOperationsClient();
            EndpointIdentity epi = defaultClient.Endpoint.Address.Identity;
            EndpointAddress ea = new EndpointAddress(new Uri(serviceAddress), epi);
            Binding b = defaultClient.Endpoint.Binding;

            //BasicHttpBinding bhb = new EAMIServiceOperationsClient().Endpoint.Binding as BasicHttpBinding;
            BasicHttpBinding bhb = new BasicHttpBinding("BasicHttpBindingM1Dev_IEAMIServiceOperations");
            //BasicHttpsBinding bhsb = new EAMIServiceOperationsClient().Endpoint.Binding as BasicHttpsBinding;
            BasicHttpsBinding bhsb = new BasicHttpsBinding("BasicHttpBinding_IEAMIServiceOperations");

            // code to setup EAMI-Rx secure binding with dynamic URL
            bool isBasicHTTPsBinding = ea.Uri.ToString().Contains("https") ? true : false;
            EAMIServiceOperationsClient returnClient = isBasicHTTPsBinding ? new EAMIServiceOperationsClient(bhsb, ea) : new EAMIServiceOperationsClient(bhb, ea);
            //EAMIServiceOperationsClient returnClient = new EAMIServiceOperationsClient(b, ea);

            // when using basic https bidnding and basic authentication - prompt user for UID and PWD
            if (returnClient.Endpoint.Binding.Name == "BasicHttpsBinding")
            {
                if (loginInfo == null)
                {
                    Login login = new Login();
                    login.ShowDialog();
                    loginInfo = login.loginInfo;
                }

                returnClient.ClientCredentials.UserName.UserName = loginInfo.UserName;
                returnClient.ClientCredentials.UserName.Password = loginInfo.Password;
                //returnClient.ClientCredentials.UserName.UserName = "dhsintra/esamoylo";
                //returnClient.ClientCredentials.UserName.Password = "*******";
            }

            return returnClient;



            /*
            // code to setup EAMI secure binding with dynamic URL"
            EAMIServiceOperationsClient defaultClient = new EAMIServiceOperationsClient();            
            EndpointIdentity epi = defaultClient.Endpoint.Address.Identity;            
            EndpointAddress ea = new EndpointAddress(new Uri(serviceAddress), epi);            
            Binding b = defaultClient.Endpoint.Binding;

            //BasicHttpBinding bhb = new EAMIServiceOperationsClient().Endpoint.Binding as BasicHttpBinding;
            //return new EAMIServiceOperationsClient(b, ea);

            //BasicHttpsBinding bhsb = new EAMIServiceOperationsClient().Endpoint.Binding as BasicHttpsBinding;
            //return new EAMIServiceOperationsClient(b, ea);             

            // code to setup EAMI-Rx secure binding with dynamic URL
            EAMIServiceOperationsClient returnClient = new EAMIServiceOperationsClient(b, ea);

            // when using basic https bidnding and basic authentication - prompt user for UID and PWD
            if(returnClient.Endpoint.Binding.Name == "BasicHttpsBinding")
            {
                if (loginInfo == null)
                {
                    Login login = new Login();
                    login.ShowDialog();
                    loginInfo = login.loginInfo;                    
                }                

                returnClient.ClientCredentials.UserName.UserName = loginInfo.UserName;
                returnClient.ClientCredentials.UserName.Password = loginInfo.Password;
                //returnClient.ClientCredentials.UserName.UserName = "dhsintra/esamoylo";
                //returnClient.ClientCredentials.UserName.Password = "*******";
            }

            return returnClient; 
           

            //return new EAMIServiceManager();
            */
        }

        

    }
}
