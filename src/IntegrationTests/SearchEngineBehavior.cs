using System.Linq;
using System.Threading.Tasks;
using MyLab.Elastic;
using Xunit;

namespace IntegrationTests
{
    public partial class SearchEngineBehavior 
    {
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task ShouldSearchAllWhenQueryIsEmpty(string emptyQuery)
        {
            //Arrange
            

            //Act
            var found = await InvokeTest<SimpleSearchEngine>(engine => engine.SearchAsync(emptyQuery));

            //Assert
            Assert.NotNull(found);

            string expectedArray = string.Join(",", Models.Select(m => m.Id).Take(10));
            string actualArray = string.Join(",", found.Select(f => f.Id));

            _output.WriteLine("Expected: " + expectedArray);
            _output.WriteLine("Actual: " + actualArray);

            Assert.Equal(expectedArray, actualArray);
        }

        [Fact]
        public async Task ShouldFoundByTerms()
        {
            //Arrange


            //Act
            var found = await InvokeTest<SimpleSearchEngine>(engine => engine.SearchAsync("term5"));

            //Assert
            Assert.NotNull(found);
            Assert.Single(found);

            Assert.Equal(5, found.First().Id);
        }

        [Fact]
        public async Task ShouldFoundByTextStart()
        {
            //Arrange


            //Act
            var found = await InvokeTest<SimpleSearchEngine>(engine => engine.SearchAsync("text1"));

            //Assert

            Assert.NotNull(found);

            string expectedArray = string.Join(",", Models.Where(m => m.Id == 1 || m.Id >= 10).Select(m => m.Id));
            string actualArray = string.Join(",", found.Select(f => f.Id));

            _output.WriteLine("Expected: " + expectedArray);
            _output.WriteLine("Actual: " + actualArray);

            Assert.Equal(expectedArray, actualArray);
        }

        [Fact]
        public async Task ShouldFoundByTextStartAndTermStart()
        {
            //Arrange


            //Act
            var found = await InvokeTest<SimpleSearchEngine>(engine => engine.SearchAsync("text1 term4", sortKey:"norm"));

            //Assert

            Assert.NotNull(found);

            string expectedArray = string.Join(",", Models.Where(m => m.Id == 1 || m.Id == 4 || m.Id >= 10).Select(m => m.Id));
            string actualArray = string.Join(",", found.Select(f => f.Id));

            _output.WriteLine("Expected: " + expectedArray);
            _output.WriteLine("Actual: " + actualArray);

            Assert.Equal(expectedArray, actualArray);
        }

        [Fact]
        public async Task ShouldApplyRegisteredFilter()
        {
            //Arrange


            //Act
            var found = await InvokeTest<SimpleSearchEngine>(engine => engine.SearchAsync("", filterKey: "single-digit"));

            //Assert
            Assert.NotNull(found);

            string expectedArray = string.Join(",", Models.Where(m => m.Id < 10).Select(m => m.Id));
            string actualArray = string.Join(",", found.Select(f => f.Id));

            _output.WriteLine("Expected: " + expectedArray);
            _output.WriteLine("Actual: " + actualArray);

            Assert.Equal(expectedArray, actualArray);
        }

        [Fact]
        public async Task ShouldApplyRegisteredSort()
        {
            //Arrange


            //Act
            var found = await InvokeTest<SimpleSearchEngine>(engine => engine.SearchAsync("", sortKey:"revert"));

            //Assert
            Assert.NotNull(found);

            string expectedArray = string.Join(",", Models.Reverse().Take(10).Select(m => m.Id));
            string actualArray = string.Join(",", found.Select(f => f.Id));

            _output.WriteLine("Expected: " + expectedArray);
            _output.WriteLine("Actual: " + actualArray);

            Assert.Equal(expectedArray, actualArray);
        }

        [Fact]
        public async Task ShouldApplyPaging()
        {
            //Arrange


            //Act
            var found = await InvokeTest<SimpleSearchEngine>(engine => engine.SearchAsync("", paging: new EsPaging
            {
                Size = 2,
                From = 3
            }));

            //Assert
            Assert.NotNull(found);

            string expectedArray = string.Join(",", Models.Skip(3).Take(2).Select(m => m.Id));
            string actualArray = string.Join(",", found.Select(f => f.Id));

            _output.WriteLine("Expected: " + expectedArray);
            _output.WriteLine("Actual: " + actualArray);

            Assert.Equal(expectedArray, actualArray);
        }

        [Fact]
        public async Task ShouldApplyStrategyPredefinedFilter()
        {
            //Arrange


            //Act
            var found = await InvokeTest<SearchEngineWithStrategyPredefinedFilter>(engine => engine.SearchAsync(""));

            //Assert
            Assert.NotNull(found);

            string expectedArray = string.Join(",", Models.Where(m => m.Id < 10).Select(m => m.Id));
            string actualArray = string.Join(",", found.Select(f => f.Id));

            _output.WriteLine("Expected: " + expectedArray);
            _output.WriteLine("Actual: " + actualArray);

            Assert.Equal(expectedArray, actualArray);
        }

        [Fact]
        public async Task ShouldApplyQueryExtractedFilters()
        {
            //Arrange


            //Act
            var found = await InvokeTest<SimpleSearchEngine>(engine => engine.SearchAsync("exclude5"));

            //Assert
            Assert.NotNull(found);

            string expectedArray = string.Join(",", Models.Where(m => m.Id != 5).Select(m => m.Id).Take(10));
            string actualArray = string.Join(",", found.Select(f => f.Id));

            _output.WriteLine("Expected: " + expectedArray);
            _output.WriteLine("Actual: " + actualArray);

            Assert.Equal(expectedArray, actualArray);
        }

        [Fact]
        public async Task ShouldProvideTotalCount()
        {
            //Arrange


            //Act
            var found = await InvokeTest<SimpleSearchEngine>(engine => engine.SearchAsync("text1 term4", sortKey: "norm", 
                paging: new EsPaging
                {
                    Size = 2,
                    From = 1
                }));

            //Assert

            Assert.NotNull(found);
            Assert.Equal(2, found.Count);
            Assert.Equal(7, found.Total);
        }
    }
}
