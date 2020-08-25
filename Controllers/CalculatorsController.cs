using System;
using System.Net.Mime;
using FoodDiary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nancy.Json;

namespace FoodDiary.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CalculatorsController : ControllerBase
    {
        [Route("bmi")]
        [HttpPost]
        [AllowAnonymous]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<MassIndexData> CalculateMassIndex([FromBody]MassIndexData data)
        {
            if (ModelState.IsValid)
            {
                data.Index = data.Weight / Math.Pow((float)data.Height / (float)100, 2);
                
                JavaScriptSerializer serializerJS = new JavaScriptSerializer();
                string response = serializerJS.Serialize(data);
                
                return Ok(response);
            }

            return BadRequest();
        }

        [Route("idealweight")]
        [HttpPost]
        [AllowAnonymous]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IdealWeightData> CalculateIdealWeight([FromBody]IdealWeightData data)
        {
            if (ModelState.IsValid)
            {
                double coef = 0;
                if (data.IsMale)
                    coef = 100;
                else
                    coef = 110;

                data.IdealWeight = (data.Height - coef) * 1.15;

                JavaScriptSerializer serializerJS = new JavaScriptSerializer();
                string response = serializerJS.Serialize(data);

                return Ok(response);
            }

            return BadRequest();
        }

        [Route("water")]
        [HttpPost]
        [AllowAnonymous]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<WaterData> CalculateWater([FromBody]WaterData data)
        {
            if (ModelState.IsValid)
            {
                int coef = 0;
                if (data.IsMale)
                    coef = 27;
                else
                    coef = 24;

                data.Water = (data.Weight * coef) + (data.Activity - 1) * 150;

                JavaScriptSerializer serializerJS = new JavaScriptSerializer();
                string response = serializerJS.Serialize(data);

                return Ok(response);
            }

            return BadRequest();
        }
    }
}