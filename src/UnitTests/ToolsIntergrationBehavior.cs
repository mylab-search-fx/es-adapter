using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyLab.Elastic;
using Xunit;

namespace UnitTests
{
    public class ToolsIntegrationBehavior
    {
        [Fact]
        public void ShouldIntegrate()
        {
            //Arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new []
            {
                new KeyValuePair<string, string>("ElasticSearch", "http://foo.com" )
            }).Build();

            //Act
            var serviceProvider = new ServiceCollection()
                .AddEsTools(config)
                .BuildServiceProvider();

            var testService = ActivatorUtilities.CreateInstance<TestService>(serviceProvider);
            var url = testService.GetUrl();

            //Assert
            Assert.Equal("http://foo.com", url);
        }

        class TestService
        {
            private readonly IEsManager _es;

            public TestService(IEsManager es)
            {
                _es = es;
            }

            public string GetUrl()
            {
                return ((DefaultEsManager) _es).Options.Url;
            }
        }
    }
}
