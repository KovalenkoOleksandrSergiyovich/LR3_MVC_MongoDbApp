using System.Collections.Generic;

namespace LR3_MVC_MongoDbApp.Models
{
    public class ComputerList
    {
        public IEnumerable<Computer> Computers { get; set; }
        public ComputerFilter Filter { get; set; }
    }
}
