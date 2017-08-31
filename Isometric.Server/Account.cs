namespace Isometric.Server
{
    public class Account<T>
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public string[] Groups { get; set; }

        public T ExternalData { get; set; }



        public Account(string login, string password, string[] groups, T externalData)
        {
            Login = login;
            Password = password;
            Groups = groups;
            ExternalData = externalData;
        }
    }
}