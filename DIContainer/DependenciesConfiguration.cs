using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainer
{
    public class DependenciesConfiguration
    {
        public void Register<TDependency, TImplementation>() where TDependency : class
            where TImplementation : class, new()
        {

        }

        public void Register(Type dependency, Type impementation)
        {

        }
    }
}
