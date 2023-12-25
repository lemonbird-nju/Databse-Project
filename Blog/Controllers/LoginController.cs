using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Blog.Models;
using Npgsql;

namespace Blog.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        // 实现登录功能
        [HttpPost]
        public ActionResult Index(string username, string password)
        {
            if (String.IsNullOrEmpty(username))
            {
                ViewBag.notice = "用户名不能为空！";
                return View();
            }
            if (String.IsNullOrEmpty(password))
            {
                ViewBag.notice = "密码不能为空！";
                return View();
            }

            // 去数据库查询用户
            DatabaseConnection db = new DatabaseConnection();
            NpgsqlConnection conn = db.GetConnection();
            conn.Open();
            string sql = $"SELECT password,is_admin FROM users WHERE user_name='{username}'";
            NpgsqlCommand command = new NpgsqlCommand(sql, conn);
            NpgsqlDataReader reader = command.ExecuteReader();

            if (!reader.Read())
            {
                ViewBag.notice = "用户不存在！";
            }
            else if (reader["password"].ToString() != password)
            {
                ViewBag.notice = "密码不正确!";
            }
            else
            {
                bool isAdmin = Convert.ToBoolean(reader["is_admin"]);
                conn.Close();
                if (isAdmin)
                    return Redirect("/Admin/Index");
                else
                    return Redirect("/General/Index");
            }
            conn.Close();

            return View();
        }

        public ActionResult Signup()
        {
            ViewBag.SignupSuccessMessage = TempData["SignupSuccessMessage"] as string;
            return View();
        }

        [HttpPost]
        public ActionResult Signup(string username, string password, string email)
        {
            // 去数据库查询用户
            DatabaseConnection db = new DatabaseConnection();
            NpgsqlConnection conn = db.GetConnection();
            conn.Open();
            string sql1 = $"SELECT 1 FROM users WHERE user_name='{username}'";
            NpgsqlCommand command = new NpgsqlCommand(sql1, conn);
            Object exist = command.ExecuteScalar();
            conn.Close();

            if (exist != null)
            {
                ViewBag.notice = "用户名已被使用!";
                return View();
            }
            string sql2 = $"SELECT 1 FROM users WHERE email='{email}'";
            command = new NpgsqlCommand(sql2, conn);
            exist = command.ExecuteScalar();
            if (exist != null)
            {
                ViewBag.notice = "邮箱已被使用!";
                return View();
            }

            string sql3 = $"INSERT INTO users (user_name, password, email) VALUES ('{username}', '{password}', '{email}');";
            command = new NpgsqlCommand(sql3, conn);
            command.ExecuteNonQuery();

            TempData["SignupSuccessMessage"] = "注册成功！3秒后将跳转回登录界面";
            return RedirectToAction("Signup");
        }
    }
}