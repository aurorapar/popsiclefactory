using API.Dtos;
using API.Enums;
using API.Validators;
using static API.Common.CommonMethods;


namespace ApiUnitTests
{
    public class ApiUnitTests
    {
        [Theory]
        [InlineData("orange", "123456")]
        public void Test_IsValidPopsicleInventoryRequest_True(string? flavor, string? plu)
        {
            bool isValidRequest = PopsicleInventoryValidator.IsValidPopsicleInventoryRequest(flavor, plu, out string error);
            Assert.True(isValidRequest, "Invalid Request");
        }

        [Theory]        
        [InlineData("false flavor", "123456")]
        [InlineData("cherry", "12345")]
        [InlineData("false flavor", "12345")]
        public void Test_IsValidPopsicleInventoryRequest_False(string? flavor, string? plu)
        {
            bool isValidRequest = PopsicleInventoryValidator.IsValidPopsicleInventoryRequest(flavor, plu, out string error);
            Assert.False(isValidRequest, "Valid Request");
        }

        [Theory]
        [InlineData("orange")]
        public void Test_IsValidFlavor_True(string? flavor)
        {
            bool isValidFlavor = PopsicleInventoryValidator.IsValidFlavor(flavor);
            Assert.True(isValidFlavor, "Invalid Flavor");
        }

        [Theory]
        [InlineData("false flavor")]
        [InlineData(null)]
        [InlineData("")]
        public void Test_IsValidFlavor_False(string? flavor)
        {
            bool isValidFlavor = PopsicleInventoryValidator.IsValidFlavor(flavor);
            Assert.False(isValidFlavor, "Valid Flavor");
        }

        [Theory]
        [InlineData("123456")]
        public void Test_IsValidPlu_True(string? plu)
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
        public void Test_IsValidPlu_False(string? plu)
        {
            bool isValidPlu = PopsicleInventoryValidator.IsValidPlu(plu);
            Assert.False(isValidPlu, "Valid Plu");
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData("\r\n")]
        [InlineData(null)]
        public void Test_IsEmptyString_True(string? input)
        {
            Assert.True(IsEmptyString(input));
        }

        [Theory]
        [InlineData("  a")]
        [InlineData("\t      1")]
        [InlineData("46494946fdas498g4rea4h98eraha")]
        [InlineData("1" + "2")]
        [InlineData("1" + "2\n")]
        public void Test_IsEmptyString_False(string? input)
        {
            Assert.False(IsEmptyString(input));
        }

        [Theory]
        [InlineData("orange", "123456")]
        [InlineData("orange", "")]
        [InlineData("cherry", "123458")]        
        public void Test_RetrievePopsicleInventory_True(string? flavor, string? plu)
        {
            var allPopsicles = API.Sql.CommonMethods.RetrieveAllPopsicleInventories();

            var popsicleInventory = API.Sql.CommonMethods.RetrievePopsicleInventory(flavor, plu);
            Assert.True(popsicleInventory is PopsicleInventoryDto, "Invalid Popsicle");
        }

        [Theory]
        [InlineData("false flavor", "123456")]
        [InlineData("cherry", "12345")]
        [InlineData("false flavor", "12345")]
        public void Test_RetrievePopsicleInventory_False(string? flavor, string? plu)
        {
            var allPopsicles = API.Sql.CommonMethods.RetrieveAllPopsicleInventories();

            var popsicleInventory = API.Sql.CommonMethods.RetrievePopsicleInventory(flavor, plu);
            Assert.True(popsicleInventory is null, "Valid Popsicle");
        }

        [Theory]
        [InlineData("orange")]
        [InlineData("raspberry")]
        [InlineData("Orange")]
        public void Test_GetPopsicleFlavorFromString_True(string? flavor)
        {
            Assert.True(PopsicleInventoryValidator.GetPopsicleFlavorFromString(flavor) is PopsicleFlavor, "Invalid Flavor");
        }

        [Theory]
        [InlineData("fdagrea")]
        [InlineData(null)]
        public void Test_GetPopsicleFlavorFromString_False(string? flavor)
        {
            Assert.False(PopsicleInventoryValidator.GetPopsicleFlavorFromString(flavor) is PopsicleFlavor, "Valid Flavor");
        }

        [Theory]
        [InlineData("orange", "123456")]
        [InlineData("Raspberry", "123459")]
        [InlineData("", "123459")]
        public void Test_IsValidCriteriaMatch_True(string? flavor, string? plu)
        {
            Assert.True(PopsicleInventoryValidator.IsValidCriteriaMatch(flavor, plu, out string _), "Valid Match");
        }

        [Theory]
        [InlineData("orange", "12345")]
        [InlineData("Raspberry", "123458")]
        [InlineData("", "")]
        public void Test_IsValidCriteriaMatch_False(string? flavor, string? plu)
        {
            Assert.False(PopsicleInventoryValidator.IsValidCriteriaMatch(flavor, plu, out string _), "Invalid Match");
        }

        [Theory]
        [InlineData("orange", "123457")]
        [InlineData("Raspberry", "123457")]
        public void Test_IsMultipleMatches_True(string? flavor, string? plu)
        {
            Assert.True(PopsicleInventoryValidator.IsMultipleMatches(flavor, plu), "Valid Match");
        }

        [Theory]
        [InlineData("orange", "123456")]
        [InlineData("Raspberry", "123459")]
        public void Test_IsMultipleMatches_False(string? flavor, string? plu)
        {
            Assert.False(PopsicleInventoryValidator.IsMultipleMatches(flavor, plu), "Invalid Match");
        }

        [Theory]
        [InlineData("lemon", "123123", 241, "aurora")]
        public void Test_CreatePopsicleInventory_True(string flavor, string plu, uint quantity, string author)
        {
            var popsicleInventory = API.Sql.CommonMethods.CreatePopsicleInventory(flavor, plu, quantity, author);
            Assert.True(popsicleInventory is PopsicleInventoryDto, "Invalid Popsicle Creation");
        }

        [Theory]
        [InlineData("lemon", "123123", 241, "")]
        [InlineData("lemon", "123123", 241, null)]
        [InlineData("orange", "123123", 46546, "aurora")]
        [InlineData("orange", "123123", 46546, "")]
        public void Test_CreatePopsicleInventory_False(string flavor, string plu, uint quantity, string author)
        {
            var popsicleInventory = API.Sql.CommonMethods.CreatePopsicleInventory(flavor, plu, quantity, author);
            Assert.True(popsicleInventory is null, "Valid Popsicle Creation");
        }

        [Theory]
        [InlineData("orange", "",  "orange", "", 1)]
        [InlineData("", "123456", "orange", "", 1)]
        [InlineData("orange", "", "", "123456", null)]
        [InlineData("", "123456", "", "123456", 1)]
        public void Test_IsValidPopsicleInventoryUpdateRequest_True(string? flavor, string? plu, string? newFlavor, string? newPlu, int? quantity)
        {
            uint? q = (uint?) quantity;
            Assert.True(PopsicleInventoryValidator.IsValidPopsicleInventoryUpdateRequest(flavor, plu, newFlavor, newPlu, q, out string _), "Invalid Request");
        }

        [Theory]
        [InlineData("", "", "", "", null)]
        [InlineData("orange", "123457", "", "", null)]
        [InlineData("", "123457", "", "", null)]
        [InlineData("orange", "", "", "", null)]
        [InlineData("orange", "", "raspberry", "", null)]
        [InlineData("orange", "", "", "123457", null)]
        public void Test_IsValidPopsicleInventoryUpdateRequest_False(string? flavor, string? plu, string? newFlavor, string? newPlu, int? quantity)
        {
            uint? q = (uint?)quantity;
            Assert.False(PopsicleInventoryValidator.IsValidPopsicleInventoryUpdateRequest(flavor, plu, newFlavor, newPlu, q, out string _), "Valid Request");
        }
    }
}