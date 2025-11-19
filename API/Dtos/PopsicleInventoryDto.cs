using API.Enums;
using System.ComponentModel.DataAnnotations;

namespace API.Dtos;

public class PopsicleInventoryDto: MetaDataDto
{
    public int PopsicleId { get; set; }

    public uint Quantity { get; set; }

    public PopsicleFlavor PopsicleFlavor;

    [Required]
    public string Plu { get; set; }

    public PopsicleInventoryDto(int popsicleId, uint quantity, PopsicleFlavor popsicleFlavor, string plu, 
        DateTime dateCreated, DateTime? dateModified, string creator, string modifier, bool? enabled = null)
        : base(dateCreated, dateModified, creator, modifier, enabled)
    {
        PopsicleId = popsicleId;
        Quantity = quantity;
        PopsicleFlavor = popsicleFlavor;
        Plu = plu;
    }
}
