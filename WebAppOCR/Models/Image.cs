using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebAppOCR.Models
{
    public class Image
    {
        public int Id { get; set; }

        [Display(Name = "画像 URL")]
        [Required(ErrorMessage ="画像の URL を入力してください")]
        public string ImageUrl { get; set; }

        [Display(Name = "テキスト")]
        public string Result { get; set; }
    }
}
