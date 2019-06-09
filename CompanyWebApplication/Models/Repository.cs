using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Tool.hbm2ddl;

namespace CompanyWebApplication.Models
{
    public class Repository
    {
        ISessionFactory _sessionFactory;
        ISession _session;
        private readonly NLog.Logger _logger;

        private Repository()
        {
            _logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            InitializeSession();
        }

        public static Repository Instance { get; } = new Repository();

        void InitializeSession()
        {
            try
            {
                _sessionFactory = Fluently.Configure()
                    .Database(MsSqlConfiguration.MsSql2012
                        .ConnectionString("Server=.\\SQLEXPRESS; Database=Company; Integrated Security=SSPI;"))
                    .Mappings(m => m.FluentMappings.AddFromAssemblyOf<CompanyMap>())
                    .Mappings(m => m.FluentMappings.AddFromAssemblyOf<EmployeeMap>())
                    .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(true, true))
                    .BuildSessionFactory();
                _session = _sessionFactory.OpenSession();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        public IEnumerable<Company> GetAllCompanies()
        {
            try
            {
                return _session.QueryOver<Company>().List();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public long CreateCompany(Company company)
        {
            using (var transaction = _session.BeginTransaction())
            {
                long id = 0, companyId = 0;
                try
                {
                    id = (long) _session.Save(company);

                    for (var i = 0; i < company.Employees.Count; ++i)
                    {
                        var employee = company.Employees.ElementAt(i);
                        employee.Company = company;
                        _session.Save(employee);
                    }
                    transaction.Commit();
                }
                catch (StaleObjectStateException ex)
                {
                    try
                    {
                        var entity = _session.Merge(company);
                        companyId = entity.CompanyId;
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        _logger.Error(ex.Message);
                        throw;
                    }
                }
                return id == 0 ? companyId : id;
            }
        }

        public IEnumerable<Company> SearchCompany(JobTitle[] jobTitle, DateTime? dateFrom, DateTime? dateTo, string keyword = "")
        {
            using (var transaction = _session.BeginTransaction())
            {
                dateFrom = dateFrom ?? DateTime.MinValue;
                dateTo = dateTo ?? DateTime.MaxValue;

                try
                {
                    Company company = null;
                    Employee employee = null;
                    var results = _session
                        .QueryOver(() => company)
                        .JoinQueryOver(() => company.Employees, () => employee)
                        .Where(() => employee.DateOfBirth >= dateFrom && employee.DateOfBirth <= dateTo
                                     && (company.Name.IsLike($"%{keyword}%")
                                     || employee.FirstName.IsLike($"%{keyword}%")
                                     || employee.LastName.IsLike($"%{keyword}%")))
                        .And(() => employee.JobTitle.IsIn(jobTitle))
                        .List();
                    
                    return results;
                }
                catch (StaleObjectStateException ex)
                {
                    transaction.Rollback();
                    _logger.Error(ex.Message);
                    throw;
                }
            }
        }

        public bool DeleteCompany(long id)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var company = _session.Get<Company>(id);
                    if (company != null)
                    {
                        _session.Delete(company);
                        transaction.Commit();
                        return true;
                    }
                    else
                        return false;
                }
                catch (StaleObjectStateException ex)
                {
                    transaction.Rollback();
                    _logger.Error(ex.Message);
                    throw;
                }
            }
        }

        public bool UpdateCompany(long id, Company company)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var tmpCompany = _session.Get<Company>(id);
                    if (tmpCompany != null)
                    {
                        _session.Flush();
                        _session.Clear();
                        company.CompanyId = id;
                        foreach (var emp in company.Employees)
                            emp.Company = company;
                        _session.Update(company);
                        transaction.Commit();
                        return true;
                    }
                    else
                        return false;
                }
                catch (StaleObjectStateException ex)
                {
                    transaction.Rollback();
                    _logger.Error(ex.Message);
                    throw;
                }
            }
        }

    }
}
