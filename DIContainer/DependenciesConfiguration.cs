using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainer
{
    public class DependenciesConfiguration
    {

        Dictionary<Type, IEnumerable<Type>> dependencies = new Dictionary<Type, IEnumerable<Type>>(); 

        public void Register<TDependency, TImplementation>() where TDependency : class
            where TImplementation : class, TDependency, new()
        {
            Type dependency = typeof(TDependency);
            Type implementation = typeof(TImplementation);

            //if (dependencies.ContainsKey(dependency))
            //{
            //    List<Type> implementations = dependencies[dependency].ToList();
            //    implementations.Add(implementation);
            //    dependencies[dependency] = implementations.AsEnumerable();
            //}
            //else
            //{
            //    dependencies[dependency] = new List<Type>(new Type[] { implementation })
            //        .AsEnumerable();
            //}
            Register(dependency, implementation);
        }

        public void Register(Type dependency, Type implementation)
        {
            if (dependencies.ContainsKey(dependency))
            {
                List<Type> implementations = dependencies[dependency].ToList();
                implementations.Add(implementation);
                dependencies[dependency] = implementations.AsEnumerable();
            }
            else
            {
                dependencies[dependency] = new List<Type>(new Type[] { implementation })
                    .AsEnumerable();
            }
        }
    }
}
