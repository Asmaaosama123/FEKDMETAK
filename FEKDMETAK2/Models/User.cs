using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FEKDMETAK2.Models;

namespace FEKDMETAK.Models
{
    public enum UserType { مستخدم, موظف ,ادمن}
    public enum Gender { انثى, ذكر }
    public class User
    {
        [Display(Name = "Uid")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Uid { set; get; }

        [Display(Name = "UserType")]
        public UserType Type { set; get; }

        [Required(ErrorMessage = "ادخل الاسم الاول")]
        [Display(Name = "FName")]
        [RegularExpression(@"^[\u0600-\u06FF\s]*$", ErrorMessage = "استحدم حروف فقط")]
        public string FName { set; get; }
        [Required(ErrorMessage = "ادخل الاسم التاني")]
        [Display(Name = "LName")]
        [RegularExpression(@"^[\u0600-\u06FF\s]*$", ErrorMessage = "استحدم حروف فقط")]

        public string LName { set; get; }
        [Required(ErrorMessage = "ادخل البريد الالكتروني ")]
        [RegularExpression(".+@.+\\..+", ErrorMessage = "الايميل غير صحيح")]
        [Display(Name = "EMail")]
        public string email { set; get; }

        [Required(ErrorMessage = "ادخل رقم التليفون ")]
        [RegularExpression(@"^\(?([0-9]{11})$",ErrorMessage = "ادخل رقم صحيح")]
        [Display(Name = " Phone")]
        public string phone { set; get; }

        [Required(ErrorMessage = "ادخل العنوان ")]
        [Display(Name = "Addres")]
        public string Adderess { set; get; }

        [Required(ErrorMessage = "ادخل المحافظة ")]
        // public Town Town { set; get; }
        [Display(Name = "twon")]
        // public Town Town { set; get; }


        public int TownId { get; set; }
        // public int CityId { get; set; }

      
        public Town Town { get; set; }
        // public int CityId { get; set; }

        // Navigation property

        [Required]
        [Display(Name = "Gender")]
        public Gender Gender { set; get; }

       
        [Display(Name = "specializationid")]
        public int? specializationId { set; get; }
        [Range(15, 80, ErrorMessage = "ادخل عمر من 15-80")]
        [Required]
        [Display(Name = "age")]
        public int age { set; get; }

        

        [Required(ErrorMessage = "ادخل كلمة المرور ")]
        [Display(Name = "Pass")]
        //[StringLength(50, ErrorMessage = "كلمة السر قوية لا تقل عن 8 أحرف او ارقام وتحتوي على علامات ورموز ", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*\d)(?=.*[@$!%#*?&])[A-Za-z\d@$!%#*?&]{8,}$", ErrorMessage = "(&@#%!)ادخل كلمة مرور قوية تحتوى ع الاقل 8 احرف باللغه الانجليزيه وارقام ورمز خاص")]
        public string password { set; get; }
        [Compare("password")]

        public string ConfirmPassword { get; set; }

        [NotMapped]
        public IFormFile clientFile { get; set; }
        public byte[]? Photo { get; set; }

        public ICollection<Notification> sentNotifications { set; get; }
        public ICollection<Notification> recievedNotifiaction { set; get; }
        public ICollection<Review> RecievedReview { set; get; }
        public ICollection<Review> GivenReview { set; get; }
        public ICollection<Specialization> UserSpecliaztion { set; get; }
        public List<Notification> Notifications { get; set; }
    }
}
