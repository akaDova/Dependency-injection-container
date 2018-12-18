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
