using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDiary.Models
{
    public class Report
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int Age { get; set; }
        public int Weight { get; set; }
        public int Height { get; set; }
        public int Sex { get; set; }
        public int Activity { get; set; }
        public string UserLogin { get; set; }

        public Report() { }

        public Report(DateTime FromDate, DateTime ToDate, int Age, int Weight, int Height, int Sex, int Activity, string UserLogin)
        {
            this.FromDate = FromDate;
            this.ToDate = ToDate;
            this.Age = Age;
            this.Weight = Weight;
            this.Height = Height;
            this.Sex = Sex;
            this.Activity = Activity;
            this.UserLogin = UserLogin;
        }
    }
}
