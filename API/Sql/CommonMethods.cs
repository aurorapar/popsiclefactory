using API.Dtos;
using API.Enums;
using API.Validators;
using static API.Validators.PopsicleInventoryValidator;
using static API.Common.CommonMethods;

namespace API.Sql
{
    public static class CommonMethods
    {
        static List<PopsicleInventoryDto> PopsicleInventories = new List<PopsicleInventoryDto>()
        {
            new PopsicleInventoryDto(1, (uint) new Random().Next(100), PopsicleFlavor.Orange, "123456", DateTime.UtcNow, DateTime.UtcNow, "Initialization", "Initialization", true),
            new PopsicleInventoryDto(2, (uint) new Random().Next(100), PopsicleFlavor.Grape, "123457", DateTime.UtcNow, DateTime.UtcNow, "Initialization", "Initialization", true),
            new PopsicleInventoryDto(3, (uint) new Random().Next(100), PopsicleFlavor.Cherry, "123458", DateTime.UtcNow, DateTime.UtcNow, "Initialization", "Initialization", true),
            new PopsicleInventoryDto(4, (uint) new Random().Next(100), PopsicleFlavor.Raspberry, "123459", DateTime.UtcNow, DateTime.UtcNow, "Initialization", "Initialization", true),
            new PopsicleInventoryDto(5, (uint) new Random().Next(100), PopsicleFlavor.Lime, "123460", DateTime.UtcNow, DateTime.UtcNow, "Initialization", "Initialization", true),
            //new PopsicleInventoryDto(6, (uint) new Random().Next(100), PopsicleFlavor.Lemon, "123461", DateTime.UtcNow, DateTime.UtcNow, "Initialization", "Initialization", true),            
        };


        public static PopsicleInventoryDto RetrievePopsicleInventory(string? flavor, string? plu)
        {
            // I don't like needing to call the validation method twice, but it does protect against cases where one of the 
            // arguments isn't valid (and not empty!) and the other is valid (did someone reuse the API incorrectly?)
            if (!PopsicleInventoryValidator.IsValidPopsicleInventoryRequest(flavor, plu, out string _))
                return null;

            if (!PopsicleInventoryValidator.IsValidCriteriaMatch(flavor, plu, out string _))
                return null;

            var popsicleFlavor = PopsicleInventoryValidator.GetPopsicleFlavorFromString(flavor);
            var candidates = RetrievePopsicleInventories(popsicleFlavor, plu);
            
            if(candidates.Count != 1)
                    return null;
            
            return candidates.First();
        }

        public static List<PopsicleInventoryDto> RetrievePopsicleInventories(PopsicleFlavor? flavor, string? plu, bool? enabled = true)
        {
            return PopsicleInventories.Where(pi =>
                (flavor is null || pi.PopsicleFlavor.Equals(flavor))
                && (string.IsNullOrEmpty(plu) || pi.Plu.ToLower().Equals(plu.ToLower()))
                && pi.Quantity > 0
                && (enabled is null || pi.Enabled == enabled)
            ).ToList();
        }

        public static List<PopsicleInventoryDto> RetrieveAllPopsicleInventories(bool? enabled = true)
        {
            return RetrievePopsicleInventories(null, null, enabled);
        }

        public static PopsicleInventoryDto CreatePopsicleInventory(string flavor, string plu, uint quantity, string author)
        {
            if (!PopsicleInventoryValidator.IsValidPopsicleInventoryRequest(flavor, plu, out string _))
                return null;

            if (!PopsicleInventoryValidator.IsValidAuthor(author))
                return null;

            var popsicleFlavor = PopsicleInventoryValidator.GetPopsicleFlavorFromString(flavor);

            var testPopsicles = RetrievePopsicleInventories(null, null, enabled: null) // custom matching, so return em all
                .Where(p => p.PopsicleFlavor.Equals(popsicleFlavor) || p.Plu.Equals(plu))
                .ToList(); // always want a non-destructive type

            if (!testPopsicles.Any())
                return CreateNewPopsicleInventory(testPopsicles.Count() + 1, (PopsicleFlavor)popsicleFlavor, plu, quantity, author);
            else
                return RetrievePopsicleInventory(flavor, plu);
        }

        public static void EnablePopsicle(PopsicleInventoryDto popsicle, string author)
        {
            //TODO: Log updates to popsicles
            popsicle.Enabled = true;
            popsicle.Modifier = author;
            popsicle.DateModified = DateTime.UtcNow;
        }

        public static PopsicleInventoryDto CreateNewPopsicleInventory(int id, PopsicleFlavor popsicleFlavor, string plu, uint quantity, string author)
        {
            //TODO: Log creation of popsicles
            var newPopsicle = new PopsicleInventoryDto(id, quantity, (PopsicleFlavor)popsicleFlavor, plu, DateTime.UtcNow, DateTime.UtcNow, author, author, true);
            PopsicleInventories.Add(newPopsicle);
            return newPopsicle;
        }

        public static PopsicleInventoryDto? UpdatePopsicleInventory(string? flavor, string? plu, string? newFlavor, string? newPlu, uint? quantity, string author, bool? enabled = null)
        {
            if (!IsValidPopsicleInventoryRequest(flavor, plu, out string _))
                return null;

            if (!IsValidPopsicleInventoryUpdateRequest(flavor, plu, newFlavor, newPlu, quantity, out string _))
                return null;

            if (!IsValidAuthor(author))
                return null;

            if (!IsValidCriteriaMatch(flavor, plu, out string _))
                return null;

            var originalPopsicle = RetrievePopsicleInventory(flavor, plu);
            if (originalPopsicle is null)
                return null;

            var newPopsicle = Sql.CommonMethods.RetrievePopsicleInventory(newFlavor, newPlu);
            if (newPopsicle is not null && newPopsicle != originalPopsicle)
                return null;

            // TODO: Log updates to popsicles
            
            originalPopsicle.PopsicleFlavor = GetPopsicleFlavorFromString(newFlavor) ?? originalPopsicle.PopsicleFlavor;
            originalPopsicle.Plu = IsEmptyString(newPlu) ? originalPopsicle.Plu : newPlu;
            originalPopsicle.Quantity = quantity ?? originalPopsicle.Quantity;
            originalPopsicle.Enabled = enabled ?? originalPopsicle.Enabled;
            originalPopsicle.DateModified = DateTime.UtcNow;
            originalPopsicle.Modifier = author;

            return originalPopsicle;

        }

    }
}
