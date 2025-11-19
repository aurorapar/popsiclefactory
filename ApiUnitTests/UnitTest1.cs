using API.Dtos;
using API.Validators;
using Microsoft.VisualStudio.TestPlatform.TestHost;


namespace ApiUnitTests
{
    public class ApiUnitTests
    {
        [Theory]
        [InlineData("orange", "123456")]
        public void Test_IsValidPopsicleInventoryRequestTrue(string? flavor, string? plu)
        {
            bool isValidRequest = PopsicleInventoryValidator.IsValidPopsicleInventoryRequest(flavor, plu, out string error);
            Assert.True(isValidRequest, "Valid Request");
        }

        [Theory]        
        [InlineData("false flavor", "123456")]
        [InlineData("cherry", "12345")]
        [InlineData("false flavor", "12345")]
        public void Test_IsValidPopsicleInventoryRequestFalse(string? flavor, string? plu)
        {
            bool isValidRequest = PopsicleInventoryValidator.IsValidPopsicleInventoryRequest(flavor, plu, out string error);
            Assert.False(isValidRequest, "Valid Request");
        }

        [Theory]
        [InlineData("orange")]
        public void Test_IsValidFlavorTrue(string? flavor)
        {
            bool isValidFlavor = PopsicleInventoryValidator.IsValidFlavor(flavor);
            Assert.True(isValidFlavor, "Valid Flavor");
        }

        [Theory]
        [InlineData("false flavor")]
        [InlineData(null)]
        [InlineData("")]
        public void Test_IsValidFlavorFalse(string? flavor)
        {
            bool isValidFlavor = PopsicleInventoryValidator.IsValidFlavor(flavor);
            Assert.False(isValidFlavor, "Valid Flavor");
        }

        [Theory]
        [InlineData("123456")]
        public void Test_IsValidPluTrue(string? plu)
        {
            bool isValidPlu = PopsicleInventoryValidator.IsValidPlu(plu);
            Assert.True(isValidPlu, "Valid Plu");
        }

        [Theory]
        [InlineData("12345")]
        [InlineData("1234567")]
        [InlineData("false plu")]
        [InlineData(null)]
        [InlineData("")]
        public void Test_IsValidPluFalse(string? plu)
        {
            bool isValidPlu = PopsicleInventoryValidator.IsValidPlu(plu);
            Assert.False(isValidPlu, "Valid Plu");
        }

        [Theory]
        [InlineData("orange", "123456")]
        [InlineData("orange", "")]
        [InlineData("cherry", "123458")]
        
        public void Test_RetrievePopsicleInventoryTrue(string? flavor, string? plu)
        {
            var allPopsicles = API.Sql.CommonMethods.RetrieveAllPopsicleInventories();

            var popsicleInventory = API.Sql.CommonMethods.RetrievePopsicleInventory(flavor, plu);
            Assert.True(popsicleInventory is PopsicleInventoryDto, "Valid Plu");
        }

        [Theory]
        [InlineData("false flavor", "123456")]
        [InlineData("cherry", "12345")]
        [InlineData("false flavor", "12345")]
        public void Test_RetrievePopsicleInventoryFalse(string? flavor, string? plu)
        {
            var allPopsicles = API.Sql.CommonMethods.RetrieveAllPopsicleInventories();

            var popsicleInventory = API.Sql.CommonMethods.RetrievePopsicleInventory(flavor, plu);
            Assert.True(popsicleInventory is null, "Valid Plu");
        }


    }
}