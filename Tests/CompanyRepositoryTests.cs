
using Shared.RequestFeatures;
using Shared.Shared;

namespace Tests;

    public class CompanyRepositoryTests
    {
        [Fact]
        public async void GetAllCompaniesAsync_ReturnsListOfCompanies_WithSingleCompany()
        {
            //Arrange
            var mockRepo = new Mock<ICompanyRepository>();
            var pagination = new PaginationParameters();
          
        mockRepo.Setup(repo => (repo.GetAll(pagination, false)))
            .Returns(Task.FromResult(await GetCompanies_wIth_one_element()));
            //Act
            var result = mockRepo.Object.GetAll(pagination, false)
                .GetAwaiter()
                .GetResult()
                .ToList();

            //Assert
            Assert.IsType<List<Company>>(result);
            Assert.Single(result);
        }


        [Fact]
        public async void GetAllCompaniesAsync_ReturnsListOfCompanies()
        {
            //Arrange
            var mockRepo = new Mock<ICompanyRepository>();
            var pagination = new PaginationParameters();

            mockRepo.Setup(repo => (repo.GetAll(pagination, false)))
                .Returns(Task.FromResult(await GetCompanies()));
            //Act
            var result = mockRepo.Object.GetAll(pagination, false)
                .GetAwaiter()
                .GetResult()
                .ToList();

            //Assert
            Assert.IsType<List<Company>>(result);
            Assert.All(result, C=>C.Country?.Contains("United States"));
        }

        public async Task<PageList<Company>> GetCompanies_wIth_one_element()
        {
            var companies = new PageList<Company>(new List<Company>(), 0, 0, 0)
                {
                    new Company
                    {
                        Id=Guid.NewGuid(),
                        Name = "Test Company",
                        Country = "United States",
                        Address = "908 woodrow Way"
                    }
                };
            return await Task.FromResult(companies);
        }

    public async Task<PageList<Company>> GetCompanies()
    {
        var companies = new PageList<Company>(new List<Company>(), 0, 0, 0)
                {
                    new Company
                    {
                        Id=Guid.NewGuid(),
                        Name = "Test Company",
                        Country = "United States",
                        Address = "908 woodrow Way"
                    },
                     new Company
                    {
                        Id=Guid.NewGuid(),
                        Name = "Test Company",
                        Country = "United States",
                        Address = "908 woodrow Way"
                    }
                };
        return await Task.FromResult(companies);
    }
}

  