using API_Exercise.Controllers;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace API_Exercise
{
    public class Database
    {
        private UserModel login_user;
        private string db_name = string.Empty;
        private string db_url = string.Empty;
        private string ConnectionString = string.Empty;  //"Data Source=localhost\\SQLEXPRESS;Initial Catalog=Gflix;Integrated Security=True";

        public Database()
        {
            var app_settings = System.Configuration.ConfigurationManager.AppSettings;
            string db_url = app_settings.Get("db_connection");
            string db_name = app_settings.Get("db_name");
            this.ConnectionString = "Data Source="+db_url+ ";Initial Catalog="+db_name+ ";Integrated Security=True";
            Console.WriteLine(this.ConnectionString);
        }

        public void Set_User(UserModel user)
        {
            this.login_user = user;
        }

        public SqlConnection Connection()
        {
            string connectionString = ConnectionString;
            SqlConnection connection = new SqlConnection();

            connection.ConnectionString = connectionString;

            connection.Open();

            Console.WriteLine("State: {0}", connection.State);
            Console.WriteLine("ConnectionString: {0}",
                connection.ConnectionString);

            return connection;
        }

        internal List<Movies_Model> Get_Movies_Names_With_User_Role(string user_role)
        {
            DataTable dt = new DataTable();
            List<KeyValuePair<string, string>> sql_param = new List<KeyValuePair<string, string>>();
            KeyValuePair<string, string> param = new KeyValuePair<string, string>("UserR", user_role);
            sql_param.Add(param);
            param = new KeyValuePair<string, string>("UserName", String.Empty);
            sql_param.Add(param);
            dt = Execute_StoredProcedure_With_Parameters("dbo.CollectionAsPer_UserRole_UserName", sql_param);
            List<Movies_Model> mv_mod = new List<Movies_Model>();
            foreach (DataRow dr in dt.Rows)
            {
                Movies_Model mv = new Movies_Model();
                mv.movie_name = dr.ItemArray[2].ToString();
                mv.movie_release_date = DateTime.Parse(dr.ItemArray[3].ToString());
                mv.movie_rating = dr.ItemArray[4].ToString();
                mv.movie_genre = dr.ItemArray[5].ToString();
                mv.movie_casts = dr.ItemArray[6].ToString();
                mv_mod.Add(mv);
            }
            return mv_mod;
        }

        internal DataTable Execute_StoredProcedure(string storedProcedureName)
        {
            //spGetDataofMovies
            List<string> movies = new List<string>();
            SqlConnection connection = new SqlConnection();
            connection = Connection();
            SqlCommand cmd = new SqlCommand(storedProcedureName, connection);
            cmd.CommandType = CommandType.StoredProcedure;
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(ds);
            return ds.Tables[0];
        }

        internal DataTable Execute_StoredProcedure_With_Parameters(string storedProcedureName, List<KeyValuePair<string, string>> sql_param)
        {
            //spGetDataofMovies

            List<string> movies = new List<string>();
            SqlConnection connection = new SqlConnection();
            connection = Connection();
            //SqlCommand cmd = new SqlCommand("dbo.spGetDataofMovies", connection);
            SqlCommand cmd = new SqlCommand(storedProcedureName, connection);
            foreach (KeyValuePair<string, string> keys in sql_param)
            {
                cmd.Parameters.Add(keys.Key, SqlDbType.NVarChar).Value = keys.Value;
            }
            cmd.CommandType = CommandType.StoredProcedure;
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(ds);
            return ds.Tables[0];
        }

        internal User_Model Get_User_Data(string user_email)
        {
            User_Model user = new User_Model();
            DataTable dt = new DataTable();
            List<KeyValuePair<string, string>> sql_param = new List<KeyValuePair<string, string>>();
            KeyValuePair<string, string> param = new KeyValuePair<string, string>("@UserEID", user_email);
            sql_param.Add(param);
            dt = Execute_StoredProcedure_With_Parameters("dbo.User_DetailsByUserID", sql_param);
            if (dt != null && dt.Rows.Count > 0)
            {
                user.user_name = dt.Rows[0].ItemArray[0].ToString();
                user.email_address = dt.Rows[0].ItemArray[1].ToString();
                user.date_of_birth = DateTime.Parse(dt.Rows[0].ItemArray[3].ToString());
                user.address = dt.Rows[0].ItemArray[4].ToString();
                user.user_role = dt.Rows[0].ItemArray[6].ToString();
                user.user_phone_number = dt.Rows[0].ItemArray[5].ToString();
            }
            return user;
        }

        internal bool User_Login()
        {
            bool is_login = false;
            if (login_user != null)
            {
                DataTable dt = new DataTable();
                List<KeyValuePair<string, string>> sql_param = new List<KeyValuePair<string, string>>();
                KeyValuePair<string, string> param = new KeyValuePair<string, string>("@User_EID", login_user.email_address);
                sql_param.Add(param);
                param = new KeyValuePair<string, string>("@User_PWD", login_user.password);
                sql_param.Add(param);
                dt = Execute_StoredProcedure_With_Parameters("dbo.UserAuthenticator", sql_param);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var res = dt.Rows[0].ItemArray[0].ToString();
                    if (res.Equals(string.Empty) || res.Equals(0.ToString()))
                    {
                        is_login = false;
                    }
                    else if (res.Equals(1.ToString()))
                    {
                        is_login = true;
                    }

                }

            }
            else
            {
                is_login = false;
            }
            return is_login;
        }

        internal List<Movies_Model> Get_Movie_Data_With_User(string user_email) 
        {
            {
                DataTable dt = new DataTable();
                List<KeyValuePair<string, string>> sql_param = new List<KeyValuePair<string, string>>();
                KeyValuePair<string, string> param = new KeyValuePair<string, string>("@UserEID", user_email);
                sql_param.Add(param);
                dt = Execute_StoredProcedure_With_Parameters("dbo.CollectionAsPer_UserEmailID", sql_param);
                List<Movies_Model> mv_mod = new List<Movies_Model>();
                foreach (DataRow dr in dt.Rows)
                {
                    Movies_Model mv = new Movies_Model();
                    mv.movie_name = dr.ItemArray[3].ToString();
                    mv.movie_release_date = DateTime.Parse(dr.ItemArray[4].ToString());
                    mv.movie_rating = dr.ItemArray[5].ToString();
                    mv.movie_genre = dr.ItemArray[6].ToString();
                    mv.movie_casts = dr.ItemArray[7].ToString();
                    mv_mod.Add(mv);
                }
                return mv_mod;
            }
        }

        internal List<Movies_Model> Get_All_Movies_Collection()
        {
            DataTable dt = new DataTable();
            dt = Execute_StoredProcedure("dbo.spGetDataofMovies");
            List<Movies_Model> mv_mod = new List<Movies_Model>();
            foreach (DataRow dr in dt.Rows)
            {
                Movies_Model mv = new Movies_Model();
                mv.movie_name = dr.ItemArray[1].ToString();
                mv.movie_release_date = DateTime.Parse(dr.ItemArray[2].ToString());
                mv.movie_rating = dr.ItemArray[3].ToString();
                mv.movie_genre = dr.ItemArray[4].ToString();
                mv.movie_casts = dr.ItemArray[5].ToString();
                mv.assignee = dr.ItemArray[6].ToString();
                mv_mod.Add(mv);
            }
            return mv_mod;
        }
    }
}
