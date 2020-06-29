using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Repository;

namespace TodoApi.Models
{
    public class TodoItem : BaseEntity
    {
       
        public string Name { get; set; }
        public bool IsComplete { get; set; }

    }
}
