using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FoodDiary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Nancy.Json;

namespace FoodDiary.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DiaryController : ControllerBase
    {
        string connectionString = @"Server=localhost;Database=fooddiary;Uid=root;";

        List<Eating> categoryList = new List<Eating>();

        [Route("foods/{login}")]
        [HttpGet]
        [Authorize]
        public IEnumerable<Eating> GetFoods(String login)
        {

            List<Eating> categoryList = new List<Eating>();
            try
            {
                using (MySqlConnection sqlConnection = new MySqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    MySqlDataAdapter sqlDa = new MySqlDataAdapter("selectFoodFromDiary", sqlConnection);
                    sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                    sqlDa.SelectCommand.Parameters.AddWithValue("_Login_users", login);

                    DataTable dtbl = new DataTable();
                    sqlDa.Fill(dtbl);

                    foreach (DataRow row in dtbl.Rows)
                    {
                        var cells = row.ItemArray;

                        categoryList.Add(new Eating(
                        (int)cells[0], //Id
                        (DateTime)cells[1], //Дата
                        (int)cells[2], //Приём пищи
                        (int)cells[4], //Id продукта
                        "", //Название
                        (int)cells[3], //Грамм 
                        (string)cells[5])); //Логин
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            using (MySqlConnection sqlConnection = new MySqlConnection(connectionString))
            {
                sqlConnection.Open();

                foreach (Eating eating in categoryList)
                {
                    MySqlDataAdapter sqlGetName = new MySqlDataAdapter("selectProductName", sqlConnection);
                    sqlGetName.SelectCommand.CommandType = CommandType.StoredProcedure;
                    sqlGetName.SelectCommand.Parameters.AddWithValue("_Id", eating.Idproduct);
                    DataTable data = new DataTable();
                    sqlGetName.Fill(data);
                    string FoodName = "";
                    foreach (DataRow writing in data.Rows)
                    {
                        var cell = writing.ItemArray;
                        FoodName = (string)cell[0];
                    }
                    eating.Productname = FoodName;
                }
            }
            return categoryList;
        }

        [Route("delete/{id}")]
        [HttpDelete]
        [Authorize]
        public ActionResult<List<Eating>> Delete(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (MySqlConnection sqlConnection = new MySqlConnection(connectionString))
                    {
                        sqlConnection.Open();
                        MySqlCommand sqlCmd = new MySqlCommand("deleteFoodFromDiary", sqlConnection);
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("_Id", id);
                        sqlCmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                return Ok();
            }

            return BadRequest();
        }

        [Route("add")]
        [HttpPost]
        [Authorize]
        public ActionResult<Eating> Post([FromBody]Eating eating)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (MySqlConnection sqlConnection = new MySqlConnection(connectionString))
                    {
                        sqlConnection.Open();
                        MySqlCommand sqlCmd = new MySqlCommand("addFoodToDiary", sqlConnection);
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("_Id", 0);
                        sqlCmd.Parameters.AddWithValue("_Eating", eating.Eatingnumber);
                        sqlCmd.Parameters.AddWithValue("_Grams", eating.Gramms);
                        sqlCmd.Parameters.AddWithValue("_Id_foods", eating.Idproduct);
                        sqlCmd.Parameters.AddWithValue("_Login_users", eating.Login);
                        sqlCmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest();
                }

                return Ok();
            }

            return BadRequest();
        }

        private List<Eating> getEatingListFromDB(Report report)
        {
            List<Eating> usersEatingList = new List<Eating>();

            //Получить список съеденных продуктов за весь интервал (FromDate ToDate)
            try
            {
                using (MySqlConnection sqlConnection = new MySqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    MySqlDataAdapter sqlDa = new MySqlDataAdapter("selectFoodFromDiaryByDate", sqlConnection);
                    sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                    sqlDa.SelectCommand.Parameters.AddWithValue("_FromDate", report.FromDate.ToString("yyyy-MM-dd"));
                    sqlDa.SelectCommand.Parameters.AddWithValue("_ToDate", report.ToDate.ToString("yyyy-MM-dd"));
                    sqlDa.SelectCommand.Parameters.AddWithValue("_Login_users", report.UserLogin);

                    DataTable dtbl = new DataTable();
                    sqlDa.Fill(dtbl);

                    foreach (DataRow row in dtbl.Rows)
                    {
                        var cells = row.ItemArray;

                        usersEatingList.Add(new Eating(
                        (int)cells[0], //Id
                        (DateTime)cells[1], //Дата
                        (int)cells[2], //Приём пищи
                        (int)cells[4], //Id продукта
                        "", //Название
                        (int)cells[3], //Грамм 
                        (string)cells[5])); //Логин
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return usersEatingList;
        }

        [Route("report")]
        [HttpPost]
        [Authorize]
        public ActionResult<String> GetPerot([FromBody]Report report)
        {
            if (ModelState.IsValid)
            {
                // Список еды текущего юзера за период
                List<Eating> usersEatingList = getEatingListFromDB(report);
                List<FoodProduct> foodList = new List<FoodProduct>();
                try
                {
                    using (MySqlConnection sqlConnection = new MySqlConnection(connectionString))
                    {
                        sqlConnection.Open();
                        MySqlDataAdapter sqlDa = new MySqlDataAdapter("selectFood", sqlConnection);
                        sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                        DataTable dtbl = new DataTable();
                        sqlDa.Fill(dtbl);

                        foreach (DataRow row in dtbl.Rows)
                        {
                            var cells = row.ItemArray;
                            foodList.Add(new FoodProduct(
                                (int)cells[0], //Id
                                (FoodType.Types)cells[1], //Тип
                                (string)cells[2], //Название
                                (int)cells[3], //Калории
                                (double)cells[4], //Белки
                                (double)cells[5], //Жиры
                                (double)cells[6])); //Углеводы
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }


                // Список продуктов других юзеров за сегодня
                List<Eating> analogEatingList = new List<Eating>();
                try
                {
                    using (MySqlConnection sqlConnection = new MySqlConnection(connectionString))
                    {
                        sqlConnection.Open();
                        MySqlDataAdapter sqlDa = new MySqlDataAdapter("selectFoodFromDiaryByDate", sqlConnection);
                        sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                        sqlDa.SelectCommand.Parameters.AddWithValue("_FromDate", report.FromDate.ToString("yyyy-MM-dd"));
                        sqlDa.SelectCommand.Parameters.AddWithValue("_ToDate", report.ToDate.ToString("yyyy-MM-dd"));
                        sqlDa.SelectCommand.Parameters.AddWithValue("_Login_users", "all");

                        DataTable dtbl = new DataTable();
                        sqlDa.Fill(dtbl);

                        foreach (DataRow row in dtbl.Rows)
                        {
                            var cells = row.ItemArray;

                            analogEatingList.Add(new Eating(
                            (int)cells[0], //Id
                            (DateTime)cells[1], //Дата
                            (int)cells[2], //Приём пищи
                            (int)cells[4], //Id продукта
                            "", //Название
                            (int)cells[3], //Грамм 
                            (string)cells[5])); //Логин
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                int AnalogCaloriesPerDay = 0;
                double AnalogProteinsPerDay = 0;
                double AnalogFatsPerDay = 0;
                double AnalogCarbohidratesPerDay = 0;



                foreach (Eating eating in analogEatingList)
                {
                    FoodProduct currentProduct = foodList.First((x) => x.Id == eating.Idproduct);

                    AnalogCaloriesPerDay += currentProduct.Calories * eating.Gramms / 100;
                    AnalogProteinsPerDay += currentProduct.Proteins * eating.Gramms / 100;
                    AnalogFatsPerDay += currentProduct.Fats * eating.Gramms / 100;
                    AnalogCarbohidratesPerDay += currentProduct.Carbohydrates * eating.Gramms / 100;
                }

               
                List<string> usersLogins = new List<string>(); 
                foreach (Eating item in analogEatingList)
                {
                    if (usersLogins.IndexOf(item.Login) == -1)
                    {
                        usersLogins.Add(item.Login);
                    }
                }

                TimeSpan deltaTime = report.ToDate.Subtract(report.FromDate);
                int deltaDays = deltaTime.Days == 0 ? 1 : deltaTime.Days;

                AnalogCaloriesPerDay /= deltaDays * usersLogins.Count;
                AnalogProteinsPerDay /= deltaDays * usersLogins.Count;
                AnalogFatsPerDay /= deltaDays * usersLogins.Count;
                AnalogCarbohidratesPerDay /= deltaDays * usersLogins.Count;

                bool isAlcoholPresent = false;

                //М 66,5 + 13,75 х вес (кг) + 5,003 х рост (см) — 6,775 х возраст (лет)
                //Ж 655.1 + 9.563 х вес(кг) +1.85 х рост(см) — 4.676 х возраст(лет)
                // (a + b * вес + c * рост - d * возраст) * e
                //  
                //  e 
                //  1.2 — минимум или отсутствие;
                //  1.375 — 3 раза в неделю;
                //  1.4625 — 5 раз в неделю;
                //  1.6375 — каждый день;

                double a, b, c, d, e;
                if (report.Sex == 1)
                {
                    a = 66.5;
                    b = 13.75;
                    c = 5.003;
                    d = 6.775;
                }
                else
                {
                    a = 655.1;
                    b = 9.563;
                    c = 1.85;
                    d = 4.676;
                }

                switch (report.Activity) 
                {
                    case 1:
                        e = 1.2;
                        break;
                    case 2:
                        e = 1.375;
                        break;
                    case 3:
                        e = 1.4625;
                        break;
                    default:
                        e = 1.6375;
                        break;
                }

                int CaloriesForWeightKeep = (int)Math.Truncate((a + b * report.Weight + c * report.Height - d * report.Age) * e);
                int CaloriesForWeightLose = (int)(0.8 * CaloriesForWeightKeep);

                double ProteinsForWeightKeep = 0.3 * CaloriesForWeightKeep / 4;
                double FatsForWeightKeep = 0.3 * CaloriesForWeightKeep / 9;
                double CarbohidratesForWeightKeep = 0.4 * CaloriesForWeightKeep / 4;

                int CurrentCaloriesPerDay = 0;
                double CurrentProteinsPerDay = 0;
                double CurrentFatsPerDay = 0;
                double CurrentCarbohidratesPerDay = 0;

                foreach (Eating eating in usersEatingList)
                {
                    FoodProduct currentProduct = foodList.First((x) => x.Id == eating.Idproduct);
                    
                    if (currentProduct.Type == FoodType.Types.Alcohol)
                        isAlcoholPresent = true;

                    CurrentCaloriesPerDay += currentProduct.Calories * eating.Gramms / 100;
                    CurrentProteinsPerDay += currentProduct.Proteins * eating.Gramms / 100;
                    CurrentFatsPerDay += currentProduct.Fats * eating.Gramms / 100;
                    CurrentCarbohidratesPerDay += currentProduct.Carbohydrates * eating.Gramms / 100;
                }

                CurrentCaloriesPerDay /= deltaDays;
                CurrentProteinsPerDay /= deltaDays;
                CurrentFatsPerDay /= deltaDays;
                CurrentCarbohidratesPerDay /= deltaDays;

                // Формирование отчёта
                string CaloriesReport = $"Дневная норма калорий для поддержания веса - {CaloriesForWeightKeep}. Дневная норма для уменьшения веса - {CaloriesForWeightLose}. Сегодня было съедено в среднем {CurrentCaloriesPerDay} калорий. Дневная норма калорий соблюдена.";
                string ProteinsReport = $"Было съедено в среднем {String.Format("{0:0.##}", CurrentProteinsPerDay)} белков. Дневная норма белков соблюдена.";
                string FatsReport = $"Было съедено в среднем {String.Format("{0:0.##}", CurrentFatsPerDay)} жиров. Дневная норма жиров соблюдена.";
                string CarbohidratesReport = $"Было съедено в среднем {String.Format("{0:0.##}", CurrentCarbohidratesPerDay)} углеводов. Дневная норма углеводов соблюдена.";
                string AlcoholReport = "";
                string AnalogReport = $"В среднем по сайту было съедено {String.Format("{0:0.##}", AnalogCaloriesPerDay)} калорий, {String.Format("{0:0.##}", AnalogProteinsPerDay)} белок, {String.Format("{0:0.##}", AnalogFatsPerDay)} жиров, {String.Format("{0:0.##}", AnalogCarbohidratesPerDay)} углеводов.";

                if (CurrentCaloriesPerDay > 1.15 * CaloriesForWeightKeep)
                    CaloriesReport = $"Дневная норма калорий для поддержания веса - {CaloriesForWeightKeep}. Дневная норма для уменьшения веса - {CaloriesForWeightLose}. Было съедено в среднем {CurrentCaloriesPerDay} калорий. Необходимо уменьшить количество съедаемых калорий в день либо увеличить количество и интенсивость тренировок.";

                if (CurrentProteinsPerDay > 1.15 * ProteinsForWeightKeep)
                    ProteinsReport = $"Было съедено в среднем {String.Format("{0:0.##}", CurrentProteinsPerDay)} белков. Необходимо уменьшить количество съедаемых белков в день. Белком богаты в основном продукты животного происхождения – мясо, рыба, морепродукты, молоко и яйца.";
                if (CurrentProteinsPerDay < 0.7 * ProteinsForWeightKeep)
                    ProteinsReport = $"Было съедено в среднем {String.Format("{0:0.##}", CurrentProteinsPerDay)} белков. Необходимо увеличить количество съедаемых белков в день. Белком богаты в основном продукты животного происхождения – мясо, рыба, морепродукты, молоко и яйца.";

                if (CurrentFatsPerDay > 1.15 * FatsForWeightKeep)
                    FatsReport = $"Было съедено в среднем {String.Format("{0:0.##}", CurrentFatsPerDay)} жиров. Необходимо уменьшить количество съедаемых жиров в день. Для сокращения количества жиров в рационе нужно готовить почти без масла на посуде с антипригарным покрытием или готовить еду на пару, отказаться от любого фаст-фуда, снизить уровень потребления жирного мяса.";
                if (CurrentFatsPerDay < 0.7 * FatsForWeightKeep)
                    FatsReport = $"Было съедено в среднем {String.Format("{0:0.##}", CurrentFatsPerDay)} жиров. Необходимо увеличить количество съедаемых жиров в день. Для  увеличить количество жиров в рационе нужно в рацион добавить орехи, горький шоколад, сыр, свинину.";

                if (CurrentCarbohidratesPerDay > 1.15 * CarbohidratesForWeightKeep)
                    CarbohidratesReport = $"Было съедено в среднем {String.Format("{0:0.##}", CurrentCarbohidratesPerDay)} углеводов. Необходимо уменьшить количество съедаемых углеводов в день. Необходимо уменьшите количество потребляемых кондитерских (простых углеводов) и хлебобулочных изделий.";
                if (CurrentCarbohidratesPerDay < 0.7 * CarbohidratesForWeightKeep)
                    CarbohidratesReport = $"Было съедено в среднем {String.Format("{0:0.##}", CurrentCarbohidratesPerDay)} углеводов. Необходимо увеличить количество съедаемых углеводов в день. Необходимо увеличить количество потребляемых сложных углеводов - круп и овощей, особенно зелёных и листовых.";

                if (isAlcoholPresent)
                    AlcoholReport = "Для улучшения рациона необходимо ограничить потребление алкоголя до минимума или полностью от него отказаться.";



                string reportString = $"Отчёт: \n\r" +
                    $" {CaloriesReport} \n\r" +
                    $" {ProteinsReport} \n\r" +
                    $" {FatsReport} \n\r" +
                    $" {CarbohidratesReport} \n\r" +
                    $" {AlcoholReport} \n\r" +
                    $" {AnalogReport}";

                var response = new
                {
                    report = reportString
                };

                JavaScriptSerializer serializerJS = new JavaScriptSerializer();
                string responseString = serializerJS.Serialize(response);

                return Ok(response);
            }

            return BadRequest();
        }

    }
}