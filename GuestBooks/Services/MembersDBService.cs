using GuestBooks.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace GuestBooks.Services
{
    public class MembersDBService
    {
        //建立與資料庫的連線字串
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["ASP.NET MVC"].ConnectionString;
        //建立與資料庫的連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);
        #region 註冊
        public void Register(Members newMember)
        {
            //將密碼HASH過
            newMember.Password = HashPassword(newMember.Password);
            //sql新增語法 //IsAdmin預設為0
            string sql = $@" INSERT INTO Members (Account,Password,Name,Email,AuthCode,IsAdmin) VALUES 
('{newMember.Account}','{newMember.Password}','{newMember.Name}','{newMember.Email}','{newMember.AuthCode}','0') ";
            //確保程式部會因執行錯誤而中斷
            try
            {
                //開啟資料庫
                conn.Open();
                //執行sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }


        #endregion

        #region Hash密碼
        //Hash密碼用的方法
        public string HashPassword(string Password)
        {
            //宣告Hash時所添加的無意義亂碼值
            string saltkey = "1q2w3e4r5t6y7u8ui9o0po7tyy";
            //將剛剛宣告的字串與密碼結合
            string saltAndPassword = string.Concat(Password, saltkey);
            //定義SHA256的HASH物件
            SHA256CryptoServiceProvider sha256Hasher = new SHA256CryptoServiceProvider();
            //取得密碼轉換成byte資料
            byte[] PasswordData = Encoding.Default.GetBytes(saltAndPassword);
            //取得HASH後byte資料
            byte[] HashDate = sha256Hasher.ComputeHash(PasswordData);
            //將Hash後byte資料轉成string
            string Hashresult = Convert.ToBase64String(HashDate);
            //傳回Hash結果
            return Hashresult;
        }

        #endregion

        #region 查詢一筆資料
        //藉由帳號取得單筆資料的方法
        private Members GetDataByAccount(string Account)
        {
            Members Data = new Members();
            //sql
            string sql = $@" select * from Members where Account = '{Account}' ";
            //確保程式不會因執行錯誤而中斷
            try
            {
                conn.Open();
                //執行sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得sql指令
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.Account = dr["Account"].ToString();
                Data.Password = dr["Password"].ToString();
                Data.Name = dr["Name"].ToString();
                Data.Email = dr["Email"].ToString();
                Data.AuthCode = dr["AuthCode"].ToString();
                Data.IsAdmin = Convert.ToBoolean(dr["IsAdmin"]);
            }
            catch (Exception e)
            {
                //查無資料
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            //回傳根據編號所取得的資料
            return Data;
        }
        #endregion

        #region 帳號註冊重覆確認
        //確認要註冊帳號是否有被註冊過的方法
        public bool AccountCheck(string Account)
        {
            //藉由導入帳號取得會員資料
            Members Data = GetDataByAccount(Account);
            //判斷是否有查詢到會員
            bool result = (Data == null);
            //回傳結果
            return result;
        }
        #endregion

        #region 信箱驗證
        //信箱驗證碼驗證方法
        public string EmailValidate(string Account,string AuthCode)
        {
            //取得傳入帳號的會員資料
            Members ValidateMember = GetDataByAccount(Account);

            //宣告驗證後訊息字串
            string ValidateStr = string.Empty;
            if(ValidateMember != null)
            {
                //判斷傳入驗證碼與資料庫是否相同
                if(ValidateMember.AuthCode == AuthCode)
                {
                    //將資料庫中的驗證碼設為空值
                    //sql更新語法
                    string sql = $@" update Members set AuthCode = '{string.Empty}' where Account = '{Account}' ";
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message.ToString());
                    }
                    finally
                    {
                        conn.Close();
                    }
                    ValidateStr = "帳號信箱驗證成功，現在可以登入了";
                }
                else
                {
                    ValidateStr = "驗證碼錯誤，請重新確認或再次註冊";
                }              
            }
            else
            {
                ValidateStr = "傳送資料錯誤，請重新確認或再註冊";
            }
            //回傳驗證訊息
            return ValidateStr;
        }
        #endregion

        #region 登入確認
        public string LoginCheck(string Account, string Password)
        {
            //取得傳入帳號的會員資料

            Members LoginMember = GetDataByAccount(Account);
            
            //判斷是否有此會員
            if (LoginMember != null)
            {
                //判斷是否有經過信箱驗證，有經驗證，驗證碼欄位會被清空
                if (string.IsNullOrWhiteSpace(LoginMember.AuthCode))
                {
                    //進行帳密確認
                    if(PasswordCheck(LoginMember, Password))
                    {
                        return "";
                    }
                    else
                    {
                        return "密碼輸入錯誤";
                    }
                }
                else
                {
                    return "此帳號尚未經過Email驗證";
                }
            }
            else
            {
                return "無此會員帳號，請去註冊";
            }
        }

        #endregion

        #region
        public bool PasswordCheck(Members CheckMember, string Password)
        {
            //判斷資料庫裡的密碼資料與傳入密碼資料HASH後是否一樣
            bool result = CheckMember.Password.Equals(HashPassword(Password));
            //回傳結果
            return result;
        }

        #endregion

        #region 取得腳色資料
        //取得會員的權限腳色資料
        public string GetRole(string Account)
        {
            //宣告初始腳色字串
            string Role = "User";
            //取得傳入帳號的會員資料
            Members LoginMember = GetDataByAccount(Account);
            if (LoginMember.IsAdmin)
            {
                Role += ",Admin";//添加Admin
            }
            //回傳最後結果
            return Role;
        }

        #endregion

    }
}