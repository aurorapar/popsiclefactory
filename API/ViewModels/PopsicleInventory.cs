using API.Dtos;
using API.Enums;
using System.Globalization;

namespace API.ViewModels
{
    public class PopsicleInventory
    {
        private static TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        public string Flavor { get; }
        public uint Quantity { get; }
        public string Plu { get; }
        
        public PopsicleInventory(PopsicleInventoryDto inventory)
        {
            Flavor = textInfo.ToTitleCase(inventory.PopsicleFlavor.ToString());
            Quantity = inventory.Quantity;
            Plu = inventory.Plu;
        }
    }
}
