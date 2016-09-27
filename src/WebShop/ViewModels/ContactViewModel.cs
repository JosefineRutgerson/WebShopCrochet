using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebShop.ViewModels
{
    public class ContactViewModel
    {
        public string CurrentDateAndTime { get; set; }
        public int Id { get; set; }
        public IEnumerable<string> Names { get; set; }

    }
}
