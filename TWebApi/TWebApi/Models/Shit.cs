﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using customApiApp_3.Controllers;

namespace customApiApp_3.Models
{
    public class Shit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime UpdateTime { get; set; }
        public Category Category { get; set; }
        public Location Location { get; set; }

    }
}
