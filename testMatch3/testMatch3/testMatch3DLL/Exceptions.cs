using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testMatch3DLL
{
    public class customException : Exception
    {
        public customException(string str = "") : base(str)
        {

        }
    }
}
