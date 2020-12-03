using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace coreWeb31.ViewModels
{
    public class LoginViewModel
    {
        [DisplayName("帳號")]
        [MinLength(5,ErrorMessage ="帳號不能小於5碼")]
        [Required(ErrorMessage = "帳號為必填欄位")]
        public string Account { get; set; }

        [DisplayName("密碼")]
        [Required(ErrorMessage = "密碼為必填欄位")]
        public string Password { get; set; }
    }
}
