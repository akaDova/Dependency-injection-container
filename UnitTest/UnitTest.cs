using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using DIContainer;
using System.Threading;

namespace UnitTest
{
    
    public class UnitTest
    {
        DependenciesConfiguration config;
        DependencyProvider provider;

        public UnitTest()
        {
            config = new DependenciesConfiguration();
            
        }

        [Fact]
        public void SingletonResolveTest()
        {
            config.Register<IImplementation, Implementation>(true);
            config.Register<IImplementation2, Implementation2>(true);
            provider = new DependencyProvider(config);

            var obj1 = provider.Resolve<IImplementation>().First();

            Assert.Same(provider.Resolve<IImplementation>().First(), provider.Resolve<IImplementation>().First());
            Assert.Same(provider.Resolve<IImplementation2>().First(), provider.Resolve<IImplementation2>().First());
            new Thread(() =>
            {
                Assert.Same(obj1, provider.Resolve<IImplementation>().First());
            }).Start();
            
            Assert.Same(obj1, provider.Resolve<IImplementation>().First());
        }

        [Fact]
        public void GenericTypeResolveTest()
        {
            config.Register<IGenericImplementation<IImplementation>, GenericImplementation1<IImplementation>>();
            config.Register<IGenericImplementation<IImplementation>, GenericImplementation2<IImplementation>>();
            provider = new DependencyProvider(config);
            var instances = provider.Resolve<IGenericImplementation<IImplementation>>();
            Assert.Equal(2, instances.Count());

            List<Type> expectedInstancesTypes = new List<Type>
            {
                typeof(GenericImplementation1<IImplementation>),
                typeof(GenericImplementation2<IImplementation>)
            };
            //CollectionAssert.AreEquivalent(expectedInstancesTypes,
            //    instances.Select((instance) => instance.GetType()).ToList());
        }

        [Fact]
        public void GenericDefinitionTypeResolveTest()
        {
            config.Register(typeof(IGenericImplementation<>), typeof(GenericImplementation1<>));
            config.Register(typeof(IGenericImplementation<>), typeof(GenericImplementation2<>));
            provider = new DependencyProvider(config);
            var instances = provider.Resolve<IGenericImplementation<IImplementation>>();
            Assert.Equal(2, instances.Count());

            List<Type> expectedInstancesTypes = new List<Type>
            {
                typeof(GenericImplementation1<IImplementation>),
                typeof(GenericImplementation2<IImplementation>)
            };
            //CollectionAssert.AreEquivalent(expectedInstancesTypes,
            //    instances.Select((instance) => instance.GetType()).ToList());
        }
    }

    interface IGenericImplementation<T>
    {

    }

    class GenericImplementation1<T> : IGenericImplementation<T>
       where T : IImplementation
    {
        public T field;

        public GenericImplementation1(T dep)
        {
            field = dep;
        }
    }

    class GenericImplementation2<T> : IGenericImplementation<T>
    {
    }

    public interface IImplementation
    {

    }

    public class Implementation : IImplementation
    {

    }

    public interface IImplementation2
    {

    }

    public class Implementation2 : IImplementation2
    {

    }
}
