using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainer
{
    public class DependencyProvider
    {
        public DependencyProvider(DependenciesConfiguration config)
        {

        }

        public TService Resolve<TService>()
        {
            // stub
            return (TService)Activator.CreateInstance(typeof(TService));
        }
    }
}
