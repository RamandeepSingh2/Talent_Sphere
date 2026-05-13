namespace TalentSphere.DTOs
{
    public class UpdateComplianceRecordDTO
    {
        public int? EmployeeID { get; set; }
        public string? RecordType { get; set; }
        public string? Description { get; set; }
        public string? Result { get; set; }      // ADD
        public DateTime? Date { get; set; }      // ADD
    }
}
