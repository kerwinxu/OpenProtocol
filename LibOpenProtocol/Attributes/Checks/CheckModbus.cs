using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Io.Github.KerwinXu.OpenProtocol.Attributes.Checks
{
    public class CheckModbus:CheckCRC16
    {
        public CheckModbus(int start, int end) :base(start, end,  0xA001 ,0xffff, "BA")  // modbus的多项式，初值，
        {

        }
    }
}
