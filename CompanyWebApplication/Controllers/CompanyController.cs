using System;
using System.Collections.Generic;
using CompanyWebApplication.Models;
using CompanyWebApplication.Security;
using Microsoft.AspNetCore.Mvc;

namespace CompanyWebApplication.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        readonly Repository _repository = Repository.Instance;

        [HttpGet]
        public string Index()
        {
            return "Running...";
        }

        [BasicAuthorize]
        [HttpPost]
        public object Create(Company company)
        {
            var id = _repository.CreateCompany(company);
            return new {Id = id};
        }

        [BasicAuthorize]
        [HttpPost]
        public IEnumerable<Company> Search(dynamic data)
        {
            DateTime? dateFrom = null;
            bool successFrom = DateTime.TryParse(data.EmplyeeDateOfBirthFrom, out DateTime @from);
            if (successFrom) dateFrom = from;

            DateTime? dateTo = null;
            bool successTo = DateTime.TryParse(data.EmplyeeDateOfBirthTo, out DateTime @to);
            if (successTo) dateTo = to;

            var jobTitles = new List<JobTitle>();
            foreach (var empJob in data.EmployeeJobTitles)
            {
                string job = empJob.ToString();
                if (Enum.TryParse(job.TrimStart('{').TrimEnd('}'), out JobTitle jobTitle)) 
                    jobTitles.Add(jobTitle);
            }

            return _repository.SearchCompany(jobTitles.ToArray(), dateFrom, dateTo, data.Keyword.ToString());
        }

        [HttpGet]
        public IEnumerable<Company> Get()
        {
            return _repository.GetAllCompanies();
        }

        [HttpPut]
        public IActionResult Update(long id, Company company)
        {
            if (_repository.UpdateCompany(id, company))
                return Ok();
            return NotFound();
        }

        [BasicAuthorize]
        [HttpDelete]
        public IActionResult Delete(long id)
        {
            if(_repository.DeleteCompany(id))
                return Ok();
            return NotFound();
        }
    }
}