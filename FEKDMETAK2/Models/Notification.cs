using System.ComponentModel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FEKDMETAK2.Models;
using Microsoft.Extensions.Configuration.UserSecrets;
namespace FEKDMETAK.Models
{
    public enum Clock
    {
        one = 1, two = 2, three = 3, four = 4, five = 5, six = 6, seven = 7, eight = 8,
        nine = 9, ten = 10, eleven = 11, twelve = 12,
    }
    public enum AmPm
    {
        am, pm
    }
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NOId { set; get; }
        [Required(ErrorMessage = "The Message field is required.")]
        public string Message { set; get; }
        public bool IsAccepted { set; get; }
        public bool IsRejected { set; get; }
        public int SenderId { set; get; }
        public int RecieverId { set; get; }
        //[Required(ErrorMessage = "Please enter a date.")]
        //[Display(Name = "Date")]
        public DateTime Date { set; get; }
        
        public Clock Clock { set; get; }
        public AmPm AmPm { set; get; }
        public User Sender { set; get; }
        public User Reciever { set; get; }

    }
}
