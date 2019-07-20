using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstBot.Models
{
    public class Site
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Field> Fields { get; set; }
    }
}
