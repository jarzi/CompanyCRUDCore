using FluentNHibernate.Mapping;

namespace CompanyWebApplication.Models
{
    public class EmployeeMap : ClassMap<Employee>
    {
        public EmployeeMap()
        {
            Id(x => x.EmployeeId);
            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.DateOfBirth);
            Map(x => x.JobTitle);
            References(x => x.Company)
                .Column("CompanyId");
            Table("Employees");
        }
    }
}
