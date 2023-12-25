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
using System.Web.Helpers;

namespace Blog.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PostBlog()
        {
            ViewBag.ReleaseSuccessMessage = TempData["ReleaseSuccessMessage"] as string;
            if (!string.IsNullOrEmpty(ViewBag.ReleaseSuccessMessage))
                ViewBag.notice = "";
            return View();
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult PostBlog(string title, string summary, string summernote)
        {
            DatabaseConnection db = new DatabaseConnection();
            NpgsqlConnection conn = db.GetConnection();
            conn.Open();
            string sql1 = $"SELECT 1 FROM articles WHERE title='{title}'";
            NpgsqlCommand command = new NpgsqlCommand(sql1, conn);
            Object exist = command.ExecuteScalar();

            if (exist != null)
            {
                ViewBag.notice = "标题重复，请换一个!";
                return View();
            }

            string sql2 = $"INSERT INTO articles (title, summary, content) VALUES ('{title}', '{summary}', '{summernote}');";
            command = new NpgsqlCommand(sql2, conn);
            command.ExecuteNonQuery();
            conn.Close();

            TempData["ReleaseSuccessMessage"] = "发布成功！";
            return RedirectToAction("PostBlog");
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

        public ActionResult BlogContent(int id)
        {
            DatabaseConnection db = new DatabaseConnection();
            NpgsqlConnection conn = db.GetConnection();
            conn.Open();
            string sql1 = $"SELECT content FROM articles WHERE article_id={id}";
            NpgsqlCommand command = new NpgsqlCommand(sql1, conn);
            Object ctt = command.ExecuteScalar();
            string sql2 = $"SELECT title FROM articles WHERE article_id={id}";
            command = new NpgsqlCommand(sql2, conn);
            Object ttl = command.ExecuteScalar();

            var model = new BlogContentModel
            {
                content = ctt.ToString(),
                title = ttl.ToString(),
            };
            conn.Close();

            return View(model);
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