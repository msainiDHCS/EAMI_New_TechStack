using EAMI.RuleEngine;
using EAMI.WebApi.Models;

namespace EAMI.WebApi.Endpoints
{
    public class LocationEndpointsConfig
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public static void AddEndpoints(WebApplication app)
        {
            app.MapGet("api/find", () =>
            {
                var rng = new Random();
                var userDetails = new List<EAMIUserModel>();
                //Enumerable.SelectMany(index => new EAMIUserModel
                //{
                //    UserID = index,
                //    UserName = "User" + index,
                //    //Date = DateTime.Now.AddDays(index),
                //    //TemperatureC = rng.Next(-20, 55),
                //    //Summary = Summaries[rng.Next(Summaries.Length)]
                //})
                //.ToList();

                return Results.Ok(userDetails);
            });
        }
    }
}
