using API.Enums;
using API.Validators;
using API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using static API.Validators.PopsicleInventoryValidator;

namespace API.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class PopsicleFactoryController : ControllerBase
{
    // Normally I'd use a logger for exceptions and *POSSIBLY* invalid requests sent, 
    // but for brevity of this exercise I'm choosing to omit.
    // WOULD NOT OMIT in all other cases
    private readonly ILogger<PopsicleFactoryController> _logger;

    public PopsicleFactoryController(ILogger<PopsicleFactoryController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetAllPopsicleInventory")]
    public IActionResult GetAllPopsicleInventory()
    {        
        var popsicleInventories = Sql.CommonMethods.RetrieveAllPopsicleInventories();
        
        return Ok(popsicleInventories.Select(pi => new PopsicleInventory(pi)).ToList());
    }

    [HttpGet(Name = "GetPopsicleInventory")]
    public IActionResult GetPopsicleInventory(string? flavor, string? plu)
    {
        string resultMessage;
        
        if (!IsValidPopsicleInventoryRequest(flavor, plu, out resultMessage))
            return BadRequest(resultMessage);

        var popsicleInventory = Sql.CommonMethods.RetrievePopsicleInventory(flavor, plu);
        if (popsicleInventory is null)
            // request was properly formatted & submitted, no need for an error
            return Ok(string.Format(ErrorMessages[ErrorDescription.Does_Not_Exist], flavor ?? "None", plu ?? "None"));

        return Ok(new PopsicleInventory(popsicleInventory));
    }

    [HttpPut(Name = "AddPopsicleInventory")]
    public IActionResult GetPopsicleInventory(string flavor, string plu, uint quantity, string author)
    {
        string resultMessage;

        if (!IsValidPopsicleInventoryRequest(flavor, plu, out resultMessage))
            return BadRequest(resultMessage);

        if (!IsValidAuthor(author))
            return BadRequest(ErrorMessages[ErrorDescription.Invalid_Author]);

        var popsicleInventory = Sql.CommonMethods.CreatePopsicleInventory(flavor, plu, quantity, author);
        if (popsicleInventory is null)
            return Problem(ErrorMessages[ErrorDescription.Contact_Support]);

        return Ok(new PopsicleInventory(popsicleInventory));
    }
}
