using System;
using System.Collections.Generic;
using System.Linq;

namespace DataGenerator
{
    class Program
    {
        static void Main()
        {
            using DataGeneratorDbContext Dbcontext = new DataGeneratorDbContext();

            List<string> array = new List<string>() { "management", "development", "analytics" };

            foreach (var elem in array)
            {
                Table_Department newDepartment = new Table_Department();

                Guid guid = Guid.NewGuid();

                newDepartment.Name = elem;

                newDepartment.Id = guid;

                Dbcontext.Table_Department.Add(newDepartment);
            }

            Dbcontext.SaveChanges();




           for(int i = 0; i < 100; i++)
           {
                Table_Employee newEmployee = new Table_Employee();
                Guid newGuid = Guid.NewGuid();

                newEmployee.Id = newGuid;

                var departmentId = Dbcontext.Table_Department.Select(x => x.Id).ToList();
                Random random = new Random();
                int rand = random.Next(0, 3);

                newEmployee.Department = departmentId[rand];

                List<string> timeSpan = new List<string>() { "08:00-17:00", "09:00-18:00", "10:00-19:00" };

                newEmployee.Timespan = timeSpan[rand];

                newEmployee.Busy = false;


                Dbcontext.Table_Employee.Add(newEmployee);
           }
           Dbcontext.SaveChanges();


        }
        
    }   
}
