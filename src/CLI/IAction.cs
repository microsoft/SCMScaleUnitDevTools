using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CLI
{
    interface IAction
    {
        public Task Execute();
    }
}
