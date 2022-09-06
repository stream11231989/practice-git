using GuestBooks.Models;
using GuestBooks.Services;
using GuestBooks.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GuestBooks.Controllers
{
    public class GuestbooksController : Controller
    {
        //宣告GuestBooks資料表的Service物件
        private readonly GuestbooksDBService GuestbookService = new GuestbooksDBService();

        // GET: Guestbooks
        //public ActionResult Index(string Search ,int Page=1)
        //{
        //    //宣告一個新的頁面模型
        //    GuestbooksViewModel Data = new GuestbooksViewModel();
        //    //將Search(搜尋)傳入夜面膜型中
        //    Data.Search = Search;
        //    //新增頁面模型中的分頁
        //    Data.Paging = new ForPaging(Page);
        //    //從Service中取得頁面所需資料(GetDataKist==>GuestbooksDBService)
        //    Data.DataList = GuestbookService.GetDataList(Data.Paging, Data.Search);
        //    //將頁面傳入View中
        //    return View(Data);
        //}
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDataList(string Search,int Page = 1)
        {
            //宣告一個新的頁面模型
            GuestbooksViewModel Data = new GuestbooksViewModel();
            //將傳入值Search(搜尋)放入頁面模型中
            Data.Search = Search;
            //新增頁面模型中的分頁
            Data.Paging = new ForPaging(Page);
            //從Service中取得頁面所需的資料
            Data.DataList = GuestbookService.GetDataList(Data.Paging, Data.Search);
            //將頁面返回View中
            return View(Data);
        }
        [HttpPost]
        //設定搜尋為接受頁面Post傳入
        //使用Bind的Include來定義只接受的欄位，用來避免傳入其他不相干值
        public ActionResult GetDataList([Bind(Include ="Search")]GuestbooksViewModel Data)
        {
            //重心導向頁面致開始頁面，並傳入搜尋值
            return RedirectToAction("GetDataList", new { Search = Data.Search });
        }

      

        #region 新增留言
        public ActionResult Create()
        {
            //因此頁面用於載入至開始頁面中，故使用部分檢視回傳
            return PartialView();
        }

        //新增傳入留言時的動作
        [Authorize]//設定此Action需要登入
        [HttpPost]
        //設定此Action只接受頁面POST傳入
        //使用Binding的Include來定義只接受的欄位，用來避免傳入其他不相干直
        public ActionResult Create([Bind(Include = "Content")] Guestbooks Data)
        {
            //設定新增留言的留言者為登入者
            Data.Account = User.Identity.Name;

            //使用Service來新增一筆資料
            GuestbookService.InsertGuestbooks(Data);
            //重新導向頁面致開始畫面
            return RedirectToAction("Index");

        }
        #endregion

        #region 修改留言
        //修改留言葉面要根據傳入的編號來決定要修改的資料
        [Authorize]//設定此Action需登入
        public ActionResult Edit(int Id)
        {
            //藉由Service，取得頁面所需資料
            Guestbooks Data = GuestbookService.GetDataById(Id);
            return View(Data);
        }
        //修改留言傳入資料時的Action
        [Authorize]//設定此Action需登入
        [HttpPost]//設定此Action只接受頁面Post傳入
        //使用Bind的Include來定義只接受的欄位，用來避免其他不相干值
        public ActionResult Edit(int Id,[Bind(Include ="Content")]Guestbooks UpdateData)
        {
            //判斷修改資料是否可修改
            if (GuestbookService.CheckUpdate(Id))
            {
                //將編號設定至修改資料中
                UpdateData.Id = Id;
                //設定修改留言的留言者為登入者
                UpdateData.Account = User.Identity.Name;

                //使用Service來修改資料
                GuestbookService.UpdateGuestbooks(UpdateData);
                //重新導向致開始頁面
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        #endregion


        #region 回覆留言
        //回覆留言頁面要根據傳入編號來決定要回覆的資料
        [Authorize(Roles = "Admin")] //設定此Action只有Admin角色才可使用
        public ActionResult Reply(int Id)
        {
            //取得頁面所需資料，藉由Service取得
            Guestbooks Data = GuestbookService.GetDataById(Id);
            //將資料傳入View中
            return View(Data);
        }

        //修改留言傳入資料時的Action
        [HttpPost] //設定此Action只接受頁面POST資料傳入
                   //使用Bind的Inculde來定義只接受的欄位，用來避免傳入其他不相干值
        [Authorize(Roles = "Admin")] //設定此Action只有Admin角色才可使用
        public ActionResult Reply(int Id, [Bind(Include = "Reply,ReplyTime")] Guestbooks ReplyData)
        {
            //修改資料的是否可修改判斷
            if (GuestbookService.CheckUpdate(Id))
            {
                //將編號設定至回覆留言資料中
                ReplyData.Id = Id;
                //使用Service來回覆留言資料
                GuestbookService.ReplyGuestbooks(ReplyData);
                //重新導向頁面至開始頁面
                return RedirectToAction("Index");
            }
            else
            {
                //重新導向頁面至開始頁面
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region 刪除留言
        //根據傳入的Id做資料刪除
        [Authorize(Roles ="Admin")]//設定此Action只有Admin腳色才可以刪除
        public ActionResult Delete(int Id)
        {
            //使用Service做刪除資料
            GuestbookService.DeleGuestbooks(Id);
            //重心導向至開始頁面
            return RedirectToAction("Index");
        }
        #endregion
    }
}