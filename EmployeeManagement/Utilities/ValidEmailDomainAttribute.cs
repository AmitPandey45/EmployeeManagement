using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EmployeeManagement.Utilities
{
    public class ValidEmailDomainAttribute : ValidationAttribute
    {
        private readonly string allowedDomain;
        private readonly string[] allowedDomains;

        public ValidEmailDomainAttribute(string allowedDomain)
        {
            this.allowedDomain = allowedDomain;
        }

        public ValidEmailDomainAttribute(string[] allowedDomains)
        {
            this.allowedDomains = allowedDomains;
        }

        public override bool IsValid(object value)
        {
            string domainName = value.ToString().Split('@')[1];
            bool isValid = false;

            if (!string.IsNullOrEmpty(allowedDomain))
            {
                isValid = domainName.ToLower().Equals(allowedDomain.ToLower());
            }
            else
            {
                isValid = allowedDomains.Any(s => s.Split('@')[1].ToLower().Equals(domainName.ToLower()));
            }

            return isValid;
        }
    }
}
