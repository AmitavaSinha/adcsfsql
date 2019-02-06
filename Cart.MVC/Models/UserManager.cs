using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cart.MVC.Models
{
    public class UserManager
    {
        public bool IsOnline { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string EmailID { get; set; }
        public bool HasAttachments { get; set; }
        public int OnBoardingID { get; set; }
        public string FileName { get; set; }
        public string Manager { get; set; }
        public List<string> Peers { get; set; }
        public List<string> Reportees { get; set; }
        public string Designation { get; set; }
        public bool isSiteAdmin { get; set; }
        public int GroupPermission { get; set; }
        public List<string> Groups { get; set; }
    }
}