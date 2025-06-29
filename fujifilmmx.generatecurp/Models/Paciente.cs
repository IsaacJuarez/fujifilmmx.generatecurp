namespace fujifilmmx.generatecurp.Models
{
    public class Paciente
    {
        public int Id { get; set; }
        public string idHIS { get; set; }
        public required string Nombre { get; set; }
        public required string ApellidoPaterno { get; set; }
        public required string ApellidoMaterno { get; set; }
        public required string Genero { get; set; }
        public required int EstadoNacimiento { get; set; }
        public required DateOnly FechaNacimiento { get; set; }
        public string CURP { get; set; }
        public DateTime datInserta { get; set; }
        public int intUser { get; set; }
        public bool bitActivo { get; set; }
        public int intVeces { get; set; }
        public required string origen { get; set; }
    }
}
