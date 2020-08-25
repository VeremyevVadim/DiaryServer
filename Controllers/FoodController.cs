using System;
using System.Collections.Generic;
using System.Linq;
using FoodDiary.Models;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FoodDiary.Controllers
{
    public struct TypeData { 
        public string TypeName { get; set; }
        public int TypeId { get; set; }

        public TypeData(string TypeName, int TypeId)
        {
            this.TypeName = TypeName;
            this.TypeId = TypeId;
        }
    } 

    [Route("[controller]")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        string connectionString = @"Server=localhost;Database=fooddiary;Uid=root;";

        [Route("categories")]
        [HttpGet]
        public IEnumerable<TypeData> GetTypes()
        {
            List<TypeData> categoryList = new List<TypeData>();
            
            foreach (string TypeName in FoodType.TypesRU)
            {
                categoryList.Add(new TypeData(TypeName, Array.IndexOf(FoodType.TypesRU, TypeName)));
            }

            return categoryList;
        }

        [Route("categories/{id}")]
        [HttpGet]
        public IEnumerable<FoodProduct> GetFoods(int id)
        {
            List<FoodProduct> categoryList = new List<FoodProduct>();
            try
            {
                using (MySqlConnection sqlConnection = new MySqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    MySqlDataAdapter sqlDa = new MySqlDataAdapter("selectFoodGroup", sqlConnection);
                    sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                    sqlDa.SelectCommand.Parameters.AddWithValue("_type", id);

                    DataTable dtbl = new DataTable();
                    sqlDa.Fill(dtbl);

                    foreach (DataRow row in dtbl.Rows)
                    { 
                        var cells = row.ItemArray;
                        categoryList.Add(new FoodProduct(
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
           
            return categoryList;
        }

    }
}