using FluentNHibernate.Mapping;

namespace CompanyWebApplication.Models
{
    public class CompanyMap : ClassMap<Company>
    {
        public CompanyMap()
        {
            Id(x => x.CompanyId);
            Map(x => x.Name);
            Map(x => x.EstablishmentYear);
            HasMany(x => x.Employees)
                .Inverse()
                .Cascade.All()
                .KeyColumn("CompanyId");
            Table("Companies");
        }
    }
}
