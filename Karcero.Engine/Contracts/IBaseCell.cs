using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Karcero.Engine.Contracts
{
    public interface IBaseCell
    {
        int Row { get; set; }
        int Column { get; set; }
    }
}
