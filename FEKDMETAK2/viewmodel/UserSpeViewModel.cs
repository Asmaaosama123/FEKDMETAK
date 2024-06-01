using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FEKDMETAK.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FEKDMETAK2.viewmodel
{
    public class UserSpeViewModel
    {
       
        public SelectList specialization { get; set; }
        public User user { get; set; }
        // Constructor to initialize specializations
       

    }
}
