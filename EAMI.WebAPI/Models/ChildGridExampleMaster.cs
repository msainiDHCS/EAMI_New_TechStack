using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EAMI.WebApi.Models
{
    public class ChildGridExampleFirst
    {

        public int EmployeeID { get; set; }

        public string EmployeeName { get; set; }

        public double Salary { get; set; }

        public DateTime JoinDate { get; set; }
    }

    public class ChildGridExampleSecond
    {

        public int PatentID { get; set; }

        public string PaentName { get; set; }
        
        public DateTime ObtainDate { get; set; }

        public string Collaborators { get; set; }
    }
}