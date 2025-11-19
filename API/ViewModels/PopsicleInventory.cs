using API.Dtos;
using API.Enums;

namespace API.ViewModels
{
    public class PopsicleInventory
    {
        public PopsicleFlavor Flavor { get; }
        public uint Quantity { get; }
        public string Plu { get; }
        
        public PopsicleInventory(PopsicleInventoryDto inventory)
        {
            Flavor = inventory.PopsicleFlavor;
            Quantity = inventory.Quantity;
            Plu = inventory.Plu;
        }
    }
}
