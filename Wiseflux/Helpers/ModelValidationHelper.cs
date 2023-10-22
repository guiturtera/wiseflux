using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Wiseflux.Helpers
{
    public static class ModelValidationHelper
    {
        public static bool ValidModel(ModelStateDictionary modelState, out string[] errors)
        {
            if (!modelState.IsValid)
            {
                errors = (string[])modelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage));
                return false;
            }

            errors = null;
            return true;
        }
    }
}
