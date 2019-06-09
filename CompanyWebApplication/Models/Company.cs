using System.Collections.Generic;

namespace CompanyWebApplication.Models
{
    public class Company
    {
        public virtual long CompanyId { get; set; }
        public virtual string Name { get; set; }
        public virtual int EstablishmentYear { get; set; }
        public virtual IList<Employee> Employees { get; set; }
    }
}
