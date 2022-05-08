using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_Exercise.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Movies : ControllerBase
    {
        // GET: api/<Movies>
        [HttpGet("{user_role}")]
        public String Get(string user_role)
        {
            Database db = new Database();
            var json = String.Empty;
            if (user_role.Contains("@"))
            {
                json = JsonSerializer.Serialize(db.Get_Movie_Data_With_User(user_role));
            }
            else
            {
                json = JsonSerializer.Serialize(db.Get_Movies_Names_With_User_Role(user_role));
            }
            return json;
            
        }

        [HttpGet()]
        public String Get_All_Collection()
        {
            Database db = new Database();
            var json = String.Empty;
            json = JsonSerializer.Serialize(db.Get_All_Movies_Collection());
            return json;
        }

    }
}
