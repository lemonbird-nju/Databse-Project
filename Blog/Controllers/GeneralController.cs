using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Models;
using Npgsql;

namespace Blog.Controllers
{
    public class GeneralController : Controller
    {
        // GET: General
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BlogList()
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
                    comments_count = reader.GetInt32(5),
                    summary = reader.GetString(6)
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
            string sql3 = $"UPDATE articles SET views_count=views_count+1 WHERE article_id={id}";
            command = new NpgsqlCommand(sql3, conn);
            command.ExecuteNonQuery();
            conn.Close();

            return View(model);
        }
    }
}