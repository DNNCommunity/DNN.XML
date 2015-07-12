using System.Net;

namespace DotNetNuke.Modules.Xml.Components
{
    public class Credential
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public NetworkCredential NetworkCredential
        {
            get { return new NetworkCredential(UserName, Password); }
        }

        public Credential(string username, string password)
        {
            UserName = username;
            Password = password;
        }
    }
}