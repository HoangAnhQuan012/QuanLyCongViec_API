using System.Threading.Tasks;
using quanLyCongViec.Models.TokenAuth;
using quanLyCongViec.Web.Controllers;
using Shouldly;
using Xunit;

namespace quanLyCongViec.Web.Tests.Controllers
{
    public class HomeController_Tests: quanLyCongViecWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            await AuthenticateAsync(null, new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "123qwe"
            });

            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}