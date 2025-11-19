using API.Dtos;
using API.Enums;
using API.Validators;

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
            // arguments isn't valid (and not empty!) and the other is valid
            if (!PopsicleInventoryValidator.IsValidPopsicleInventoryRequest(flavor, plu, out string _))
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

            var popsicleFlavor = PopsicleInventoryValidator.GetPopsicleFlavorFromString(flavor);

            var testPopsicles = RetrievePopsicleInventories(popsicleFlavor, plu, enabled: null)
                .Where(p => p.PopsicleFlavor.Equals(popsicleFlavor) || p.Plu.Equals(plu))
                .ToList(); // always want a non-destructive type

            if (!testPopsicles.Any())
            {
                return CreateNewPopsicleInventory(testPopsicles.Count() + 1, (PopsicleFlavor)popsicleFlavor, plu, quantity, author);
            }
            else
            {
                if (testPopsicles.Count > 1)
                    // This would DEFINITELY be a case worth logging. The logic should straight up prevent multiples from being created,
                    // even with the enabled flag
                    return null;

                PopsicleInventoryDto popsicle = testPopsicles.First();

                if (!popsicle.PopsicleFlavor.Equals(popsicleFlavor) || !popsicle.Plu.Equals(plu))
                    return null;

                if (!popsicle.Enabled)
                    EnablePopsicle(popsicle, author);

                return popsicle;
            }
        }

        public static void EnablePopsicle(PopsicleInventoryDto popsicle, string author)
        {
            popsicle.Enabled = true;
            popsicle.Modifier = author;
            popsicle.DateModified = DateTime.UtcNow;
        }

        public static PopsicleInventoryDto CreateNewPopsicleInventory(int id, PopsicleFlavor popsicleFlavor, string plu, uint quantity, string author)
        {
            var newPopsicle = new PopsicleInventoryDto(id, quantity, (PopsicleFlavor)popsicleFlavor, plu, DateTime.UtcNow, DateTime.UtcNow, author, author, true);
            PopsicleInventories.Add(newPopsicle);
            return newPopsicle;
        }

    }
}
