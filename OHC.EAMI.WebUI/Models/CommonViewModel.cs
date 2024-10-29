using OHC.EAMI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OHC.EAMI.WebUI.Models
{
    public class CommonViewModel
    {
    }

    #region Common Models

    public class JSONReturnStatus
    {
        public bool status { get; set; }
        public dynamic returnedData { get; set; }

        public JSONReturnStatus()
        {
            status = false;
        }

        public static JSONReturnStatus GetStatus(CommonStatus cs)
        {
            JSONReturnStatus returnStatus = new JSONReturnStatus();
            List<string> errorMessageList = new List<string>();
            if (cs.Status)
            {
                returnStatus.status = true;
            }
            else
            {
                foreach (string messsage in cs.MessageDetailList)
                {
                    errorMessageList.Add(messsage);
                }
                returnStatus.returnedData = errorMessageList;
            }
            return returnStatus;
        }
    }

    #endregion
}