using System;
using WebShop.Interfaces;


namespace WebShop
{
    public class SystemDateTime : IDateTime
    {
        private DateTime _now;
        public DateTime Now {
            get
            {
                return _now;
            }
        }

        public SystemDateTime()
        {
            _now = DateTime.Now;
        }
    }
}
