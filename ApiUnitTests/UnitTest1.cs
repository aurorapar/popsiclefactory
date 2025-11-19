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
            Assert.True(isValidRequest, "Invalid Request");
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
            Assert.True(isValidFlavor, "Invalid Flavor");
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
            Assert.True(isValidPlu, "Invalid Plu");
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
            Assert.True(popsicleInventory is PopsicleInventoryDto, "Invalid Popsicle");
        }

        [Theory]
        [InlineData("false flavor", "123456")]
        [InlineData("cherry", "12345")]
        [InlineData("false flavor", "12345")]
        public void Test_RetrievePopsicleInventoryFalse(string? flavor, string? plu)
        {
            var allPopsicles = API.Sql.CommonMethods.RetrieveAllPopsicleInventories();

            var popsicleInventory = API.Sql.CommonMethods.RetrievePopsicleInventory(flavor, plu);
            Assert.True(popsicleInventory is null, "Valid Popsicle");
        }

        [Theory]
        [InlineData("lemon", "123123", 241, "aurora")]
        [InlineData("orange", "123456", 241, "aurora")]
        public void Test_CreatePopsicleInventoryTrue(string flavor, string plu, uint quantity, string author)
        {
            var popsicleInventory = API.Sql.CommonMethods.CreatePopsicleInventory(flavor, plu, quantity, author);
            Assert.True(popsicleInventory is PopsicleInventoryDto, "Invalid Popsicle Creation");
        }

        [Theory]
        [InlineData("lemon", "123123", 241, "")]
        [InlineData("lemon", "123123", 241, null)]
        [InlineData("orange", "123123", 46546, "aurora")]
        [InlineData("orange", "123123", 46546, "")]
        public void Test_CreatePopsicleInventoryFalse(string flavor, string plu, uint quantity, string author)
        {
            var popsicleInventory = API.Sql.CommonMethods.CreatePopsicleInventory(flavor, plu, quantity, author);
            Assert.True(popsicleInventory is null, "Valid Popsicle Creation");
        }


    }
}