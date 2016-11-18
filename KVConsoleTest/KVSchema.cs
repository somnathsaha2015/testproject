using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KVConsoleTest
{
    public class KVUser
    {
        //public int Id { get; set; }
        public string Email { get; set; }
        public string PwdHash { get; set; }
        public char Role { get; set; }
    }
}
