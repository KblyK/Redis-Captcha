using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Models.Examples
{
    public class ViewModelForLogin
    {
        public string userName { get; set; }
        public int ipCount { get; set; }
        public int userCount { get; set; }
        public string ipMessage { get; set; }
        public string userMessage { get; set; }
        public string ErrorMessage { get; set; }

        [StringLength(4)]
        public string CaptchaCode{ get; set; }
    }
}
