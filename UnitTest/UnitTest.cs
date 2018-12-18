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
        DependenciesConfiguration config2;
        DependencyProvider provider;

        public UnitTest()
        {
            config = new DependenciesConfiguration();
            config2 = new DependenciesConfiguration();
            
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

            config2.Register<IGenericImplementation<IImplementation>, GenericImplementation1<IImplementation>>();
            provider = new DependencyProvider(config);
            var provider2 = new DependencyProvider(config2);
            var instance = provider.Resolve<IGenericImplementation<IImplementation>>().First();
            var instance2 = provider2.Resolve<IGenericImplementation<IImplementation>>().First();

            Assert.Equal(instance.GetType(), instance2.GetType());

            
            //CollectionAssert.AreEquivalent(expectedInstancesTypes,
            //    instances.Select((instance) => instance.GetType()).ToList());
        }

        [Fact]
        public void ManyRegisters()
        {
            config.Register<IImplementation, Implementation>();
            config.Register<IImplementation, Implemntation3>();

            
            provider = new DependencyProvider(config);
            
            var instance = provider.Resolve<IImplementation>();
            

            Assert.Equal(2, instance.Count());


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

    public class Implemntation3: IImplementation
    {

    }

    public class Implementation2 : IImplementation2
    {

    }
}
