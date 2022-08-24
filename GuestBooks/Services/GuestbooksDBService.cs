using GuestBooks.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common.CommandTrees.ExpressionBuilder;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.WebPages;

namespace GuestBooks.Services
{
    public class GuestbooksDBService
    {
        //建立與資料庫的連線字串
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["ASP.NET MVC"].ConnectionString;
        //建立與資料庫的連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        public List<Models.Guestbooks> GetDataList()
        {
            List<Models.Guestbooks> DataList = new List<Models.Guestbooks>();
            //Sql語法
            //資料庫資料表我取名為GuestBooks以方便練習區分
            string sql = @" SELECT * FROM GuestBooks; ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())//獲得下一筆資料直到沒有為止
                {
                    Models.Guestbooks Data = new Models.Guestbooks();
                    Data.Id = Convert.ToInt32(dr["Id"]);
                    Data.Name = dr["Name"].ToString();
                    Data.Content = dr["Content"].ToString();
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    //確定此留言是否回復，且不允許空白
                    //因為C#是強型別語言，所以轉換時DataTime不允許存取Null
                    if (!dr["ReplyTime"].Equals(DBNull.Value))
                    {
                        Data.Reply = dr["Reply"].ToString();
                        Data.ReplyTime = Convert.ToDateTime(dr["ReplyTime"]);
                    }
                    DataList.Add(Data);

                }
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return DataList;
        }
        #region
        //新增方法
        public void InsertGuestbooks(Guestbooks newData)
        {
            //sql新增語法
            //設定新增時間為現在
            string sql = $@" INSERT INTO GuestBooks(Name,Content,CreateTime) VALUES('{newData.Name}','{newData.Content}',
                                                                        '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}');";
            //確保程式不會因執行錯誤而失敗
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();


            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());

            }
            finally
            //關閉資料庫連線
            {
                conn.Close();

            }


        }
        #endregion


        #region 查詢一筆資料
        //藉由編號取得單筆資料的方法
        public Guestbooks GetDataById(int Id)
        {
            Guestbooks Data = new Guestbooks();
            //sql語法
            string sql = $@" SELECT * FROM GuestBooks WHERE Id = {Id};";
            //確保程式部會因為執行錯誤而中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.Id = Convert.ToInt32(dr["Id"]);
                Data.Name = dr["Name"].ToString();
                Data.Content = dr["Content"].ToString();
                Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                //確定此則留言是否回覆，且不允許空白
                if (!string.IsNullOrWhiteSpace(dr["Reply"].ToString()))
                {
                    Data.Reply = dr["Reply"].ToString();
                    Data.ReplyTime = Convert.ToDateTime(dr["ReplyTime"]);
                }
            }
            catch (Exception e)
            {
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            //回傳資料
            return Data;
        }
        #endregion

        #region 修改留言
        //修改留言方法
        public void UpdateGuestbooks(Guestbooks UpdateData)
        {
            //sql修改語法
            string sql = $@" UPDATE GuestBooks SET Name = '{UpdateData.Name}',Content = '{UpdateData.Content}' WHERE Id = '{UpdateData.Id}'";

            //確保程式部會因執行錯誤而整個中斷
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
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫
                conn.Close();
            }
        }
        #endregion

        #region 回覆留言
        public void ReplyGuestbooks(Guestbooks ReplyData)
        {
            //Sql修改語法;
            //設定回覆時間為現在
            string sql = $@" UPDATE GuestBooks SET Reply = '{ReplyData.Reply}', ReplyTime ='{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}' WHERE Id = {ReplyData.Id}";
            //確保程式部會因執行錯誤而被中斷
            try
            {
                //開啟連線資料庫
                conn.Open();
                //執行sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫
                conn.Close();
            }

        }
        #endregion

        #region 檢查相關
        //修改資料判斷的方法
        public bool CheckUpdate(int Id)
        {
            //根據Id取得要修改的資料
            Guestbooks Data = GetDataById(Id);
            //判斷並回傳(判斷是否有資料及是否有回覆)
            return (Data != null && Data.ReplyTime == null);

        }
        #endregion

        #region 刪除資料
        public void DeleGuestbooks(int Id)
        {//sql刪除語法
            //根據Id取得要刪除的資料
            string sql = $@"DELETE FROM Guestbooks WHERE Id ={Id}; ";
            //確保程式部會因為執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //丟出錯誤訊息
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫
                conn.Close();
            }

        }

        #endregion
        #region 查詢陣列資料
        //根據分頁以及搜尋來取得陣列資料的方法

        //找陣列資料中的Search類別(見GuestbooksViewModels)
        public List<Guestbooks> GetDataList(ForPaging Paging, string Search)
        {
            List<Guestbooks> DataList = new List<Guestbooks>();
            //sql語法
            if (!string.IsNullOrWhiteSpace(Search))
            {
                //有搜尋條件時
                SetMaxPaging(Paging, Search);
                DataList = GetAllDataList(Paging, Search);
            }
            else
            {
                //無搜尋條件時
                SetMaxPaging(Paging);
                DataList = GetAllDataList(Paging);
            }


            //sql語法(單Search，未導入分業功能)
            ////string sql = string.Empty;
            ////if (!string.IsNullOrWhiteSpace(Search))
            ////{
            ////    //有搜尋條件時
            ////    sql = $@"SELECT * FROM GuestBooks WHERE Name LIKE '%{Search}%' OR Content LIKE '%{Search}%' OR Reply LIKE'%{Search}%';";
            ////}
            ////else
            ////{
            ////    //無搜尋條件時
            ////    sql = $@"SELECT * FROM GuestBooks;";
            ////}
            //////確保程式部會因為執行錯誤而整個中斷
            ////try
            ////{
            ////    //開啟資料庫
            ////    conn.Open();
            ////    //執行sql指令
            ////    SqlCommand cmd = new SqlCommand(sql, conn);
            ////    //取得sql資料
            ////    SqlDataReader dr = cmd.ExecuteReader();
            ////    while (dr.Read())//獲取下一筆資料直到沒有資料
            ////    {
            ////        Guestbooks Data = new Guestbooks();
            ////        Data.Id = Convert.ToInt32(dr["Id"]);
            ////        Data.Name = dr["Name"].ToString();
            ////        Data.Content = dr["Content"].ToString();
            ////        Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
            ////        //確定此則留言是否回復，且不允許空白
            ////        //因C#是強型別語法，所以轉換時DataTime不允許存取Null
            ////        if (!dr["ReplyTime"].Equals(DBNull.Value))
            ////        {
            ////            Data.Reply = dr["Reply"].ToString();
            ////            Data.ReplyTime = Convert.ToDateTime(dr["ReplyTime"]);

            ////        }
            ////        DataList.Add(Data);

            ////    }
            ////}
            ////catch(Exception e)
            ////{
            ////    throw new Exception(e.Message.ToString());
            ////}
            ////finally
            ////{
            ////    //關閉資料庫
            ////    conn.Close();
            ////}
            return DataList;

        }

        #endregion


        //無搜尋值得搜尋資料方法
        #region 設定最大頁數方法
        //無搜尋值得"設定最大頁數方法"
        public void SetMaxPaging(ForPaging Paging)
        {
            //計算列
            int Row = 0;
            //sql語法
            string sql = $@"SELECT * FROM GuestBooks; ";
            //確保程式部會因執行錯誤而整個中斷
            try
            {
                //開啟連線資料庫
                conn.Open();
                //執行sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) //獲取下一筆資料直到沒有資料
                {
                    Row++;

                }


            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row) / Paging.ItemNum));
            //重新設定正確的頁數，避免有不正確值傳入
            Paging.SetRightPage();
        }

        //有搜尋值的設定最大頁數方法
        public void SetMaxPaging(ForPaging Paging, string Search)
        {
            //計算列數
            int Row = 0;
            //sql語法
            string sql = $@" SELECT * FROM GuestBooks WHERE Name LIKE '%{Search}%'  OR Content LIKE '%{Search}%' OR Reply LIKE '%{Search}%' ";
            //string sql = $@" select * from Guestbooks Where Name like '%{Search}%'  or Content like '%{Search}%' or Reply like '%{Search}%' ";
            //確保程式部會因為執行錯誤而整個中斷
            try
            {
                //開啟資料庫
                conn.Open();
                //執行sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())//獲取下一筆直到沒有資料
                {
                    Row++;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            //計算所需的總頁數
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row) / Paging.ItemNum));
            //重新設定正確的頁數，避免有不正確值傳入
            Paging.SetRightPage();

        }
        #endregion

        #region 搜尋資料方法

        //無搜尋值的資料搜尋方法
        public List<Guestbooks> GetAllDataList(ForPaging paging)
        {
            //宣告要回傳的搜尋資料為資料庫中的Guestbooks資料表
            List<Guestbooks> DataList = new List<Guestbooks>();
            //sql語法
            string sql = $@" SELECT * FROM (SELECT row_number() OVER(order by Id) AS sort,* FROM Guestbooks ) m
                       WHERE m.sort BETWEEN {(paging.NowPage - 1) * paging.ItemNum + 1} AND {paging.NowPage * paging.ItemNum}; ";


            //string sql = $@" select * from (select row_number() over(order by Id) as sort,* from Guestbooks ) m
            //           Where m.sort Between {(paging.NowPage - 1) * paging.ItemNum + 1} and {paging.NowPage * paging.ItemNum} ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())//獲得下一筆資料直到沒有資料
                {
                    Guestbooks Data = new Guestbooks();
                    Data.Id = Convert.ToInt32(dr["Id"]);
                    Data.Name = dr["Name"].ToString();
                    Data.Content = dr["Content"].ToString();
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    //確定此則留言是否回復，且不允許空白
                    //因C#為強型別，所以轉換時Datetime型態不允許為Null
                    if (!dr["ReplyTime"].Equals(DBNull.Value))
                    {
                        Data.Reply = dr["Reply"].ToString();
                        Data.ReplyTime = Convert.ToDateTime(dr["ReplyTime"]);
                    }
                    DataList.Add(Data);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            //回傳資料
            return DataList;
        }

        //有搜尋值的搜尋資料方法
        public List<Guestbooks> GetAllDataList(ForPaging paging,string Search)
        {
            //宣告要回傳的搜尋資料為資料庫中的Guestbooks資料表
            List<Guestbooks> DataList = new List<Guestbooks>();
            //sql語法
            string sql = $@"SELECT * FROM (SELECT row_number() OVER (order by Id) AS sort, * FROM GuestBooks WHERE Name LIKE '%{Search}%' OR 
                                                                                                             Content LIKE'%{Search}%' OR
                                                                                                             Reply LIKE '%{Search}%' ) m WHERE m.sort BETWEEN
                                          {(paging.NowPage - 1) * paging.ItemNum + 1} AND {paging.NowPage * paging.ItemNum}; ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) //獲取下一筆資料直到沒有資料
                {
                    Guestbooks Data = new Guestbooks();
                    Data.Id = Convert.ToInt32(dr["Id"]);
                    Data.Name = dr["Name"].ToString();
                    Data.Content = dr["Content"].ToString();
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    //確定此則留言是否回復，且不允許空白
                    //因C#為強型別，所以轉換時DateTime不允許存取Null
                    if (!dr["ReplyTime"].Equals(DBNull.Value))
                    {
                        Data.Reply = dr["Reply"].ToString();
                        Data.ReplyTime = Convert.ToDateTime(dr["ReplyTime"]);
                    }
                    DataList.Add(Data);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return DataList;
        }

        #endregion




    }
}