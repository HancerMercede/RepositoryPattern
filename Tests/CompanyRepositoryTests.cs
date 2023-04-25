
namespace Tests;

    public class CompanyRepositoryTests
    {
        [Fact]
        public void GetAllCompaniesAsync_ReturnsListOfCompanies_WithSingleCompany()
        {
            //Arrange
            var mockRepo = new Mock<ICompanyRepository>();
            mockRepo.Setup(repo => (repo.GetAll(false)))
                .Returns(Task.FromResult(GetCompanies()));

            //Act
            var result = mockRepo.Object.GetAll(false)
                .GetAwaiter()
                .GetResult()
                .ToList();

            //Assert
            Assert.IsType<List<Company>>(result);
            Assert.Single(result);
        }

        public IEnumerable<Company> GetCompanies()
        {
            return new List<Company>
            {
                new Company
                {
                    Id=Guid.NewGuid(),
                    Name = "Test Company",
                    Country = "United States",
                    Address = "908 woodrow Way"
                }
            };
        }
    }
