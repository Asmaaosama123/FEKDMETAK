using Microsoft.AspNetCore.Mvc;
using FEKDMETAK.Models;
using Microsoft.EntityFrameworkCore;
using FEKDMETAK.Data;
using FEKDMETAK.Models;
using FEKDMETAK2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FEKDMETAK2.Controllers
{
    public class viewsController : Controller
    {
        private readonly mvcdbcontext _dbContext;
        private readonly IWebHostEnvironment environment;

        public viewsController(mvcdbcontext dbContext, IWebHostEnvironment _envir)
        {
            _dbContext = dbContext;
            environment = _envir;

        }
        public IActionResult Index()
        {
            return View();
        }
        //public async Task<IActionResult> specializationAsync() => _dbContext.specializations != null ?
        //                  View(await _dbContext.specializations.ToListAsync()) :
        //                  Problem("Entity set 'mvcdbcontext.'  is null.");

        public IActionResult viewusers(User user)
        {
            List<User> users = new List<User>();
            return View();
        }
    }
}
