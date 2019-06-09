# CompanyCRUDCore
Simple CRUD WebApi .Net Core

API examples:

Basic auth:
login: user
pass: pass

POST:

http://localhost:54039/api/company/create

{
    "Name": "Firma test 1",
    "EstablishmentYear": 1988,
    "Employees": [
        {
            "FirstName": "Michał",
            "LastName": "Jarzyna",
            "DateOfBirth": "1988.01.01",
            "JobTitle": "Developer"
        }
    ]
}

POST:

http://localhost:54039/api/company/search

{
    "Keyword": "jarzyna",
    "EmployeeDateOfBirthFrom": "1900.01.01",
    "EmployeeDateOfBirthTo": "2000.01.01",
    "EmployeeJobTitles": [
        "Developer",
        "Administrator"
    ]
}

PUT:

http://localhost:54039/api/company/update?id=1

{
    "Name": "Firma test 2",
    "EstablishmentYear": 1380,
    "Employees": [
        {
            "FirstName": "Wojciech",
            "LastName": "Jarzyna",
            "DateOfBirth": "1988.01.01",
            "JobTitle": "Developer"
        }
    ]
}

DELETE:

http://localhost:54039/api/company/delete?id=1
