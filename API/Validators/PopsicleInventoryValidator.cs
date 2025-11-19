using API.Common;
using API.Enums;
using System.Globalization;
using System.Text.RegularExpressions;
using static API.Common.CommonMethods;

namespace API.Validators
{
    public static class PopsicleInventoryValidator
    {
        private static TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        private static Regex PluFormatRegex = new Regex(@"^\d{6}$");

        public enum ErrorDescription
        {
            Invalid_Flavor,// Would normally do a regex here for format validation
            Invalid_Plu,
            Does_Not_Exist,
            Contact_Support,
            Invalid_Author,
            Multiple_Matches,
            Invalid_Match,
            Invalid_Quantity,
            Avoiding_Duplicate_Entry,
        }

        public static Dictionary<ErrorDescription, string> ErrorMessages = new()
        {
            { ErrorDescription.Invalid_Flavor, "Invalid Flavor {0}" },
            { ErrorDescription.Invalid_Plu, "Invalid PLU Format" },
            { ErrorDescription.Does_Not_Exist, "Popsicle Inventory Does Not Exist for Flavor {0} Plu {1}" },
            { ErrorDescription.Contact_Support, "Please Contact Support" },
            { ErrorDescription.Invalid_Author, "Please Supply An Author" },
            { ErrorDescription.Multiple_Matches, "Your request returned multiple values" },
            { ErrorDescription.Invalid_Match, "Your request returned no or multiple values" },
            { ErrorDescription.Invalid_Quantity, "Invalid Quantity Specified {0}" },
            { ErrorDescription.Avoiding_Duplicate_Entry, "Avoiding Duplicate Entry for Flavor {0} Plu {1}" },
        };


        public static bool IsValidPopsicleInventoryRequest(string? flavor, string? plu, out string errorDescription, bool? enabled = null)
        {
            errorDescription = "";

            bool isValidFlavor = IsValidFlavor(flavor);
            bool isValidPlu = IsValidPlu(plu);

            if (!isValidFlavor && !isValidPlu)
            {
                if(!isValidFlavor)
                    errorDescription += string.Format(ErrorMessages[ErrorDescription.Invalid_Flavor], flavor ?? "None;");
                if(!isValidPlu)
                    errorDescription += string.Format(ErrorMessages[ErrorDescription.Invalid_Plu], plu ?? "None;");
            }

            if(isValidFlavor && isValidPlu)
            {
                if (IsMultipleMatches(flavor, plu, enabled))
                    errorDescription = ErrorMessages[ErrorDescription.Multiple_Matches];
            }

            if(! (isValidFlavor && isValidPlu) && !IsEmptyString(flavor) && !IsEmptyString(plu))
            {
                if(!IsEmptyString(flavor))
                    errorDescription += string.Format(ErrorMessages[ErrorDescription.Invalid_Flavor], flavor ?? "None;");
                else if (!IsEmptyString(plu))
                    errorDescription += string.Format(ErrorMessages[ErrorDescription.Invalid_Plu], plu ?? "None;");
            }

            if (errorDescription.Length > 0)
            return false;

            return true;
        }

        public static bool IsValidFlavor(string? flavor)
        {
            if (IsEmptyString(flavor))
                return false;
            
            var popsicleFlavor = GetPopsicleFlavorFromString(flavor);
            if (popsicleFlavor is null)
                return false;

            return true;
        }

        public static bool IsValidPlu(string? plu)
        {
            if (IsEmptyString(plu))
                return false;

            if (!PluFormatRegex.IsMatch(plu))
                return false;            

            return true;
        }

        public static bool IsValidAuthor(string author)
        {
            // IsEmptyString already unit tested, add tests if this method changes
            return !IsEmptyString(author);
        }

        public static bool IsValidCriteriaMatch(string? flavor, string? plu, out string errorMessage, bool? enabled = null)
        {
            errorMessage = "";

            if (IsEmptyString(plu) && IsEmptyString(flavor))
                return false;

            // The flavor and PLU must match a single popsicle for a single popsicle,
            // otherwise we would run into a conflict
            var popsicles = Sql.CommonMethods.RetrieveAllPopsicleInventories(enabled: null);
            var test = popsicles.Where(p =>
                p.PopsicleFlavor.Equals(GetPopsicleFlavorFromString(flavor))
                && (IsEmptyString(plu) || p.Plu.Equals(plu))
                && (enabled is null || p.Enabled == enabled))
            .ToList();
            test = test.Union(
                popsicles.Where(p =>
                    p.Plu.Equals(plu)
                    && (IsEmptyString(flavor) || p.PopsicleFlavor.Equals(GetPopsicleFlavorFromString(flavor)))
                    && (enabled is null || p.Enabled == enabled)
                )                
            ).ToList().Distinct().ToList();

            if(test.Count != 1)
            {
                errorMessage += ErrorMessages[ErrorDescription.Invalid_Match];
                return false;
            }

            return true;
        }

        // Different use case from IsValidCriteriaMatch, as no matches would still be valid
        public static bool IsMultipleMatches(string flavor, string plu, bool? enabled = null)
        {
            PopsicleFlavor popsicleFlavor = (PopsicleFlavor)GetPopsicleFlavorFromString(flavor);

            // Make sure to use .ToList because there are destructive operations on Enumerables :(
            return Sql.CommonMethods.RetrievePopsicleInventories(null, null, enabled: null).ToList() // custom matching, so return em all
                .Where(p => 
                    (p.PopsicleFlavor.Equals(popsicleFlavor) || p.Plu.Equals(plu))
                    && (enabled is null || p.Enabled == enabled)
                ).ToList()
                .Count() > 1;
        }

        public static PopsicleFlavor? GetPopsicleFlavorFromString(string? flavor)
        {
            if (IsEmptyString(flavor))
                return null;

            var success = PopsicleFlavor.TryParse(textInfo.ToTitleCase(flavor), out PopsicleFlavor popsicleFlavor);
            if (!success)
                return null;
            return popsicleFlavor;
        }        

        public static bool IsValidPopsicleInventoryUpdateRequest(string? flavor, string? plu, string? newFlavor, string? newPlu, uint? quantity, out string errorMessage)
        {
            errorMessage = "";

            bool newFlavorEmpty = IsEmptyString(newFlavor);
            bool newPluEmpty = IsEmptyString(newPlu);

            if(!IsValidPopsicleInventoryRequest(flavor, plu, out string preValidationMessage))
            {
                errorMessage = preValidationMessage;
                return false;
            }

            if (quantity is not uint && !IsValidPopsicleInventoryRequest(newFlavor, newPlu, out string innerErrorMessage))
            {
                if (quantity is not uint)
                    errorMessage += innerErrorMessage + string.Format(ErrorMessages[ErrorDescription.Invalid_Quantity], quantity is null ? "None;" : quantity);
                return false;
            }

            if(!newFlavorEmpty && !newPluEmpty)
            {
                if(!IsValidPopsicleInventoryRequest(newFlavor, newPlu, out string invalidMatchMessage))
                {
                    errorMessage = invalidMatchMessage;
                    return false;
                }
            }

            var originalPopsicle = Sql.CommonMethods.RetrievePopsicleInventory(flavor, plu);
            if(originalPopsicle is null)
            {
                errorMessage = String.Format(ErrorMessages[ErrorDescription.Does_Not_Exist], flavor, plu);
                return false;
            }
            
            // Only updating quantity
            if (newFlavorEmpty && newPluEmpty)
                return true;

            var newPopsicle = Sql.CommonMethods.RetrievePopsicleInventory(newFlavor, newPlu);
            if(newPopsicle is not null && newPopsicle != originalPopsicle)
            {
                errorMessage = string.Format(ErrorMessages[ErrorDescription.Avoiding_Duplicate_Entry], newFlavor, newPlu);
                return false;
            }

            return true;
        }
    }
}
