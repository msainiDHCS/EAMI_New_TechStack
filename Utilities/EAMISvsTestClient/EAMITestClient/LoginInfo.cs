using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMITestClient
{
    public class LoginInfo
    {
        public LoginInfo(string domain, string username, string password)
        {
            if (string.IsNullOrWhiteSpace(domain))
            {
                throw new ArgumentNullException("domain");
            }

            if (string.IsNullOrWhiteSpace(domain))
            {
                throw new ArgumentNullException("username");
            }

            if (string.IsNullOrWhiteSpace(domain))
            {
                throw new ArgumentNullException("password");
            } 

            this.Domain = domain;
            this.UserName = username;
            this.Password = password;
        }

        public string Domain { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
    }
}
