using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Notes.Controllers
{
    public class NotesController : Controller
    {
        // 
        // GET: /Notes/Index/
        public IActionResult Index => View("Notes");

        // 
        // GET: /Notes/IndexString/
        public IActionResult IndexString => Index;
    }
}