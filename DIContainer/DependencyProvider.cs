using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DIContainer
{
    public class DependencyProvider
    {

        class ImplementationContainer
        {
            static object instance;
            static object syncRoot = new object();

            public static Func<object> GetInstance(Func<object> creator)
            {
                object instance = null;
                return () =>
                {
                    if (instance == null)
                    {
                        lock (syncRoot)
                        {
                            if (instance == null)
                                instance = creator();
                        }
                    }
                    return instance;
                };
                
                
            }

             
        }

        Dictionary<Type, IEnumerable<Func<object>>> dependencies/* = new Dictionary<Type, IEnumerable<(Type, bool)>>()*/;
        
        //Dictionary<>

        public DependencyProvider(DependenciesConfiguration config)
        {
            dependencies = new Dictionary<Type, IEnumerable<Func<object>>>();
            foreach(var dependency in config.Dependencies)
            {
                dependencies.Add(dependency.Key, ConvertToFactories(dependency.Value));
            }
        }

        private IEnumerable<Func<object>> ConvertToFactories(IEnumerable<(Type, bool)> tuples)
        {
            return from tuple in tuples
                   select ConvertToFactory(tuple);
        }

        private Func<object> ConvertToFactory((Type, bool) tuple)
        {
            Type type;
            bool isSingleton;
            (type, isSingleton) = tuple;

            if (isSingleton)
            {
                return ImplementationContainer.GetInstance(() => CreateFromCtor(type));
            }
            else
            {
                return () => CreateFromCtor(type);
            }
        }

        public IEnumerable<TService> Resolve<TService>()
        {
            // stub
            return Resolve(typeof(TService)).Cast<TService>();
        }

        private IEnumerable<object> Resolve(Type type)
        {
            return from dependency in dependencies[type]
                   select dependency();
        }

        object CreateFromCtor(Type type)
        {
            ConstructorInfo constructor = (
                from ctor in type.GetConstructors()
                orderby ctor.GetParameters().Length
                select ctor
            ).First();

            var parameters = new List<object>();
            foreach (var currParameter in constructor.GetParameters())
            {
                // add some suff
                parameters.Add(Resolve(currParameter.ParameterType).FirstOrDefault());
            }

            return constructor.Invoke(parameters.ToArray());
            
        }

        private static TService CreateDependency<TService>() where TService : new()
        {
            // stub
            return new TService();
        }

    }
}
