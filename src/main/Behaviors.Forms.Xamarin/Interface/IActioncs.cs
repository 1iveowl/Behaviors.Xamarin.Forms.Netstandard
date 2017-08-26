using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Behaviors.Interface
{
    public interface IAction
    {
        Task<bool> Execute(object sender, object parameter);
    }
}
