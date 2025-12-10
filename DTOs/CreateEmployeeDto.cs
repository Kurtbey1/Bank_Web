namespace Bank_Project.DTOs
{
    public class CreateEmployeeDto
    {
        public int EmpID{get;set;}
        public string FirstName {get;set;}=string.Empty;
        public string SecondName {get;set;}=string.Empty; 
        public string LastName {get;set;}=string.Empty; 

        public string Gender {get;set;}= string.Empty;

        public int Salary {get;set;}

        public string PhoneNumber { get; set; } = string.Empty;

        public string Position { get; set; } = string.Empty;

        public int? SupervisorID { get; set; }  


        public int BranchId{get;set;}

      
    }
}