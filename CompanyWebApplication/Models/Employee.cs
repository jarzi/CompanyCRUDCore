using System;
using Newtonsoft.Json;

namespace CompanyWebApplication.Models
{
    public class Employee
    {
        public virtual long EmployeeId { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual DateTime DateOfBirth { get; set; }
        public virtual JobTitle JobTitle { get; set; }
        [JsonIgnore]
        public virtual Company Company { get; set; }
    }

    public enum JobTitle
    {
        Administrator,
        Developer,
        Architect,
        Manager
    }

}