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
            object instance;
            object syncRoot = new object();

            public Func<object> createInstance;

            public Func<object> GetInstance(Func<object> creator)
            {
                
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

        Dictionary<Type, List<ImplementationContainer>> dependencies/* = new Dictionary<Type, IEnumerable<(Type, bool)>>()*/;
        
        //Dictionary<>

        public DependencyProvider(DependenciesConfiguration config)
        {
            dependencies = new Dictionary<Type, List<ImplementationContainer>>();
            foreach(var dependency in config.Dependencies)
            {
                dependencies.Add(dependency.Key, ConvertToFactories(dependency.Value));
            }
        }

        private List<ImplementationContainer> ConvertToFactories(IEnumerable<(Type, bool)> tuples)
        {
            return (from tuple in tuples
                    select ConvertToFactory(tuple)).ToList();
        }

        private ImplementationContainer ConvertToFactory((Type, bool) tuple)
        {
            Type type;
            bool isSingleton;
            (type, isSingleton) = tuple;
            var implContainer = new ImplementationContainer();
            if (isSingleton)
            {
                implContainer.createInstance = implContainer.GetInstance(() => CreateFromCtor(type));
            }
            else
            {
                implContainer.createInstance = () => CreateFromCtor(type);
            }

            return implContainer;
        }

        public IEnumerable<TService> Resolve<TService>()
        {
            // stub
            return Resolve(typeof(TService)).Cast<TService>();
        }

        private IEnumerable<object> Resolve(Type type)
        {
            return from dependency in dependencies[type]
                   select dependency.createInstance();
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
