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

    [HttpGet(Name = "GetPopsicleInventory")]
    public IActionResult GetPopsicleInventory(string? flavor, string? plu, bool? enabled = true)
    {
        string resultMessage;
        
        if (!IsValidPopsicleInventoryRequest(flavor, plu, out resultMessage, enabled))
            return BadRequest(resultMessage);

        if (!IsValidCriteriaMatch(flavor, plu, out resultMessage, enabled))
            return Conflict(resultMessage);

        var popsicleInventory = Sql.CommonMethods.RetrievePopsicleInventory(flavor, plu);
        if (popsicleInventory is null)
            // request was properly formatted & submitted, no need for an error
            return Ok(string.Format(ErrorMessages[ErrorDescription.Does_Not_Exist], flavor ?? "None", plu ?? "None"));

        return Ok(new PopsicleInventory(popsicleInventory));
    }

    [HttpPut(Name = "AddPopsicleInventory")]
    public IActionResult AddPopsicleInventory(string? flavor, string? plu, uint quantity, string author)
    {
        string resultMessage;

        if (!IsValidPopsicleInventoryRequest(flavor, plu, out resultMessage))
            return BadRequest(resultMessage);

        if (!IsValidAuthor(author))
            return BadRequest(ErrorMessages[ErrorDescription.Invalid_Author]);

        if (!IsValidCriteriaMatch(flavor, plu, out resultMessage))
            return Conflict(resultMessage);

        var popsicleInventory = Sql.CommonMethods.CreatePopsicleInventory(flavor, plu, quantity, author);
        if (popsicleInventory is null)
            return Problem(ErrorMessages[ErrorDescription.Contact_Support]);

        return Ok(new PopsicleInventory(popsicleInventory));
    }

    [HttpPut(Name = "ReplacePopsicleInventory")]
    public IActionResult ReplacePopsicleInventory(string? flavor, string? plu, string? newFlavor, string? newPlu, uint? quantity, string author, bool? enabled = null)
    {
        string resultMessage;

        if (!IsValidPopsicleInventoryRequest(flavor, plu, out resultMessage))
            return BadRequest("Target Popsicle Invalid - " + resultMessage);

        if (!IsValidPopsicleInventoryUpdateRequest(flavor, plu, newFlavor, newPlu, quantity, out resultMessage, enabled))
            return BadRequest("Update Values Invalid - " + resultMessage);

        if (!IsValidAuthor(author))
            return BadRequest(ErrorMessages[ErrorDescription.Invalid_Author]);

        if (!IsValidCriteriaMatch(flavor, plu, out resultMessage))
            return Conflict(resultMessage);

        var popsicleInventory = Sql.CommonMethods.UpdatePopsicleInventory(flavor, plu, newFlavor, newPlu,  quantity, author, enabled);
        if (popsicleInventory is null)
            return Problem(ErrorMessages[ErrorDescription.Contact_Support]);

        return Ok(new PopsicleInventory(popsicleInventory));
    }

    [HttpPut(Name = "UpdatePopsicleInventory")]
    public IActionResult UpdatePopsicleInventory(string? flavor, string? plu, string? newFlavor, string? newPlu, uint? quantity, string author, bool? enabled = null)
    {
        return ReplacePopsicleInventory(flavor, plu, newFlavor, newPlu, quantity, author, enabled);
    }

    [HttpPut(Name = "RemovePopsicleInventory")]
    public IActionResult RemovePopsicleInventory(string? flavor, string? plu, string author)
    {
        return ReplacePopsicleInventory(flavor, plu, null, null, null, author, false);
    }

    [HttpPut(Name = "SearchPopsicleInventory")]
    public IActionResult SearchPopsicleInventory(string? flavor, string? plu, bool? enabled = true)
    {
        if (!IsValidSearchRequest(flavor, plu, out string errorMessage))
            return BadRequest(errorMessage);

        return Ok(Sql.CommonMethods.RetrieveAnyPopsicleInventories(flavor, plu, enabled)
            .Select(p => new PopsicleInventory(p))
            .ToList());
    }
}
