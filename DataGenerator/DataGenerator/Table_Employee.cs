﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGenerator
{
    public class Table_Employee
    {
        public Guid Id { get; set; }

        public Guid Department { get; set; }

        public string Timespan { get; set; }

        public bool Busy { get; set; }

    }
}
