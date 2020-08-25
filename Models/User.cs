using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDiary.Models
{
    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public User() { }

        public User(string Login, string Password)
        {
            this.Login = Login;
            this.Password = Password;
            this.Role = "User";
        }
    }
}
