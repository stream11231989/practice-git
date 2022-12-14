using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GuestBooks.Models
{
    public class Guestbooks
    {
        #region for會員系統用資料

        [DisplayName("編號：")]
        //編號
        public int Id { get; set; }

        //帳號
        public string Account { get; set; }

        [DisplayName("留言內容：")]
        [Required(ErrorMessage = "請輸入留言內容")]
        [StringLength(100, ErrorMessage = "留言內容不可超過100字元")]
        //留言內容
        public string Content { get; set; }

        [DisplayName("新增時間：")]
        //新增時間
        public DateTime CreateTime { get; set; }

        [DisplayName("回覆內容：")]
        //回覆內容
        [StringLength(100, ErrorMessage = "回覆內容不可超過100字元")]
        public string Reply { get; set; }

        [DisplayName("回覆時間：")]
        //回覆時間
        //DateTime?資料型態，允許DateTime有NULL產生
        public DateTime? ReplyTime { get; set; }

        //會員 從 Member表來
        //預設時就將Members物件建立完畢
        public Members Member { get; set; } = new Members();

        #endregion





        #region 單純for留言板用的資料
        //[DisplayName("編號:")]
        //public int Id { get; set; }
        //[DisplayName("名字:")]
        //[Required(ErrorMessage ="請輸入名字")]
        //[StringLength(20,ErrorMessage ="名字不可超過20字元")]
        //public string Name { get; set; }
        //[DisplayName("留言內容")]
        //[Required(ErrorMessage ="請輸入留言內容")]
        //[StringLength(100, ErrorMessage = "留言內容不可超過100字元")]
        //public string Content { get; set; }
        //public DateTime CreateTime { get; set; }
        //public string Reply { get; set; }
        ////DateTime?資料型態，允許有Null產生
        //[DisplayName("回覆時間")]
        //public DateTime? ReplyTime { get; set; }
        #endregion

    }
}