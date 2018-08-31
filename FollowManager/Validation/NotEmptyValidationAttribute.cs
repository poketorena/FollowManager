using System.ComponentModel.DataAnnotations;

namespace FollowManager.Validation
{
    public class NotEmptyValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is string str)
            {
                if (string.IsNullOrWhiteSpace(str))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
