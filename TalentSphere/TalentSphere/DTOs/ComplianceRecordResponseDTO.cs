public class ComplianceRecordResponseDTO
{
    public int ComplianceID { get; set; }
    public int EmployeeID { get; set; }
    public string? EmployeeName { get; set; }
    public string RecordType { get; set; }
    public string? Result { get; set; }      // ADD
    public DateTime? Date { get; set; }      // ADD
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}