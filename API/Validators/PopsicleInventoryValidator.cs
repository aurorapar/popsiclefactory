using API.Common;
using API.Enums;
using System.Globalization;
using System.Text.RegularExpressions;

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
        }

        public static Dictionary<ErrorDescription, string> ErrorMessages = new()
        {
            { ErrorDescription.Invalid_Flavor, "Invalid Flavor {0}" },
            { ErrorDescription.Invalid_Plu, "Invalid PLU Format" },
            { ErrorDescription.Does_Not_Exist, "Popsicle Inventory Does Not Exist for Flavor {0} Plu {1}" },
            { ErrorDescription.Contact_Support, "Please Contact Support" },
            { ErrorDescription.Invalid_Author, "Please Supply An Author" },
        };


        public static bool IsValidPopsicleInventoryRequest(string? flavor, string? plu, out string errorDescription)
        {
            errorDescription = "";

            bool isValidFlavor = IsValidFlavor(flavor);
            bool isValidPlu = IsValidPlu(plu);

            // I don't like the way this looks, and I don't think theres a much better way to format it. Cie La Vie
            if (
                (
                    !isValidFlavor && !isValidPlu
                ) 
                    || 
                (
                    (!string.IsNullOrEmpty(flavor) && ! string.IsNullOrEmpty(plu))
                    && (!isValidFlavor || !isValidPlu)
                )
            )
            {
                if(!isValidFlavor)
                    errorDescription += string.Format(ErrorMessages[ErrorDescription.Invalid_Flavor], flavor ?? "None;");
                if(!isValidPlu)
                    errorDescription += string.Format(ErrorMessages[ErrorDescription.Invalid_Plu], plu ?? "None;");
            }

            if (errorDescription.Length > 0)
                return false;

            return true;
        }

        public static bool IsValidFlavor(string? flavor)
        {
            if (string.IsNullOrEmpty(flavor))
                return false;

            var popsicleFlavor = GetPopsicleFlavorFromString(flavor);
            if (popsicleFlavor is null)
                return false;

            return true;
        }

        public static bool IsValidPlu(string? plu)
        {
            if (string.IsNullOrEmpty(plu))
                return false;

            if (!PluFormatRegex.IsMatch(plu))
                return false;            

            return true;
        }

        public static bool IsValidAuthor(string author)
        {
            return !string.IsNullOrEmpty(CommonMethods.RemoveWhitespace(author));
        }

        public static PopsicleFlavor? GetPopsicleFlavorFromString(string? flavor)
        {            
            var success = PopsicleFlavor.TryParse(textInfo.ToTitleCase(flavor), out PopsicleFlavor popsicleFlavor);
            if (!success)
                return null;
            return popsicleFlavor;
        }        
    }
}
