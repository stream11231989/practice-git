using GuestBooks.Models;
using GuestBooks.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace GuestBooks.ViewModels
{
    public class GuestbooksViewModel
    {
        [DisplayName("搜尋:")]
        public string Search { get; set; }

        //顯示陣列資料
        public List<Guestbooks> DataList { get; set; }
        //分頁內容
        public ForPaging Paging { get; set; }

      

    }
}