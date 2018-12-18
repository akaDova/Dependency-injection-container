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
            public Type implementationType;
            object instance;
            object syncRoot = new object();

            public Func<Type, object> createInstance;

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
            var implContainer = new ImplementationContainer
            {
                implementationType = type
            };
            if (isSingleton)
            {
                implContainer.createInstance = (dependency) => implContainer.GetInstance(() => CreateFromCtor(type, dependency))();
            }
            else
            {
                implContainer.createInstance = (dependency) => CreateFromCtor(type, dependency);
            }

            return implContainer;
        }

        public IEnumerable<TService> Resolve<TService>()
        {
            // stub
            Type type = typeof(TService);

            //if (type.IsGenericType)
            return Resolve(typeof(TService)).Cast<TService>();
        }

        private IEnumerable<object> Resolve(Type type)
        {
            
            var typeDependency = new List<ImplementationContainer>();
            

            if (dependencies.TryGetValue(type, out typeDependency))
            {
                return from dependency in typeDependency
                       select dependency.createInstance(type);
            }
            if (type.IsGenericType)
            {
                
                if (dependencies.TryGetValue(type.GetGenericTypeDefinition(), out typeDependency))
                {
                    return from dependency in typeDependency
                           select dependency.createInstance(type);
                }
            }
            
             return new List<object>();
            
        }

        object CreateFromCtor(Type type, Type dependency = null)
        {

            if (type.IsGenericTypeDefinition)
                type = type.MakeGenericType(dependency.GetGenericArguments());

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
