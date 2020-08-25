using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDiary.Models
{
    public class Eating
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Eatingnumber { get; set; }
        public int Idproduct { get; set; }
        public string Productname { get; set; }
        public int Gramms { get; set; }
        public string Login { get; set; }

        public Eating() { }

        public Eating(int Id, DateTime Date, int Eatingnumber, int Idproduct, string Productname, int Gramms, string Login)
        {
            this.Id = Id;
            this.Date = Date;
            this.Eatingnumber = Eatingnumber;
            this.Idproduct = Idproduct;
            this.Productname = Productname;
            this.Gramms = Gramms;
            this.Login = Login;
        }
    }
}
