namespace fujifilmmx.generatecurp.Models
{
    public class ResponseModel
    {
        public string CURP { get; set; }
        public bool hasError { get; set; }
        public string ErrorMessage { get; set; }
    }
}
