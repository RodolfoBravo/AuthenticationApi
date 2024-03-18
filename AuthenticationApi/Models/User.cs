
namespace AuthenticationApi.Models
{
    public class User
    {
        public int id { get; set; }
        public string userName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int age { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string phone { get; set; }
        public int role {  get; set; }

    }
     public class LoginUser
    {
        public string email { get; set; }
        public string password { get; set; }
    }
}
