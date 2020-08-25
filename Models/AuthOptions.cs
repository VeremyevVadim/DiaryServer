using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FoodDiary.Models
{
    public class AuthOptions
    {
        public const string ISSUER = "DiaryServer"; // издатель токена
        public const string AUDIENCE = "DiaryClient"; // потребитель токена
        const string KEY = "jh21jhhaWDhuvjlKAOSplkvasdhoi";   // ключ для шифрации
        public const int LIFETIME = 60; // время жизни токена - 1 минута
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
