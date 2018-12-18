using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainer
{
    public class DependenciesConfiguration
    {

        Dictionary<Type, IEnumerable<(Type, bool)>> dependencies = new Dictionary<Type, IEnumerable<(Type, bool)>>();

        internal Dictionary<Type, IEnumerable<(Type, bool)>> Dependencies
        {
            get => dependencies;
        }

        public void Register<TDependency, TImplementation>(bool isSingleton = false) where TDependency : class
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
            Register(dependency, implementation, isSingleton);
        }

        public void Register(Type dependency, Type implementation, bool isSingleton = false)
        {
            if (dependencies.ContainsKey(dependency))
            {
                List<(Type, bool)> implementations = dependencies[dependency].ToList();
                implementations.Add((implementation, isSingleton));
                dependencies[dependency] = implementations.AsEnumerable();
            }
            else
            {
                dependencies[dependency] = new List<(Type, bool)>(new (Type, bool)[] { (implementation, isSingleton) })
                    .AsEnumerable();
            }
        }
    }
}
