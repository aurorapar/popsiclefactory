using Xunit;
using API.Validators;

namespace ApiUnitTests
{
    public class ApiUnitTests
    {


        [Theory]
        [InlineData("orange", "123456")]
        [InlineData("false flavor", "123456")]
        [InlineData("cherry", "12345")]
        [InlineData("false flavor", "12345")]
        public void Test_IsValidPopsicleInventoryRequest(string? flavor, string? plu)
        {
            bool isValidRequest = PopsicleInventoryValidator.IsValidPopsicleInventoryRequest(flavor, plu, out string error);
            Assert.True(isValidRequest, "Valid Request");
        }

        [Theory]
        [InlineData("orange")]
        [InlineData("false flavor")]
        [InlineData(null)]
        [InlineData("")]
        public void Test_IsValidFlavor(string? flavor)
        {
            bool isValidFlavor = PopsicleInventoryValidator.IsValidFlavor(flavor);
            Assert.True(isValidFlavor, "Valid Flavor");
        }

        [Theory]
        [InlineData("123456")]
        [InlineData("12345")]
        [InlineData("1234567")]
        [InlineData("false plu")]
        [InlineData(null)]
        [InlineData("")]
        public void Test_IsValidPlu(string? plu)
        {
            bool isValidPlu = PopsicleInventoryValidator.IsValidPlu(plu);
            Assert.True(isValidPlu, "Valid Plu");
        }
    }
}