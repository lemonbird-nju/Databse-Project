using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Models;
using PagedList.Mvc;
using Npgsql;
using PagedList;
using System.Web.UI;

namespace Blog.Controllers
{
    public class AdminController : Controller
    {
        // private PersonalBlogEntities db = new PersonalBlogEntities();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PostBlog()
        {
            return View();
        }

        public ActionResult UserManagement()
        {
            DatabaseConnection db = new DatabaseConnection();
            NpgsqlConnection conn = db.GetConnection();
            conn.Open();
            string sql = $"SELECT * FROM users";
            NpgsqlCommand command = new NpgsqlCommand(sql, conn);
            NpgsqlDataReader reader = command.ExecuteReader();

            List<users> userList = new List<users>();
            while (reader.Read())
            {
                users usr = new users
                {
                    user_id = reader.GetInt32(0),
                    user_name = reader.GetString(1),
                    email = reader.GetString(3),
                    is_admin = reader.GetBoolean(4)
                };
                userList.Add(usr);
            }
            conn.Close();

            return View(userList);
        }

        public ActionResult BlogManagement()
        {
            DatabaseConnection db = new DatabaseConnection();
            NpgsqlConnection conn = db.GetConnection();
            conn.Open();
            string sql = $"SELECT * FROM articles ORDER BY created_at DESC";
            NpgsqlCommand command = new NpgsqlCommand(sql, conn);
            NpgsqlDataReader reader = command.ExecuteReader();

            List<articles> blogList = new List<articles>();
            while (reader.Read())
            {
                articles blg = new articles
                {
                    article_id = reader.GetInt32(0),
                    title = reader.GetString(1),
                    created_at = reader.GetDateTime(3),
                    views_count = reader.GetInt32(4),
                    comments_count = reader.GetInt32(5)
                };
                blogList.Add(blg);
            }
            conn.Close();

            return View(blogList);
        }

        public ActionResult BlogDelete(int id)
        {
            DatabaseConnection db = new DatabaseConnection();
            NpgsqlConnection conn = db.GetConnection();
            conn.Open();
            string sql = $"DELETE FROM articles WHERE article_id='{id}'";
            NpgsqlCommand command = new NpgsqlCommand(sql, conn);
            command.ExecuteNonQuery();

            return Redirect("/Admin/BlogManagement");
        }
    }
}