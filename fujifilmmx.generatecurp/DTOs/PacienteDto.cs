namespace fujifilmmx.generatecurp.DTOs
{
    public class PacienteDto
    {
        public string idHIS { get; set; }
        public required string Nombre { get; set; }
        public required string ApellidoPaterno { get; set; }
        public required string ApellidoMaterno { get; set; }
        public required string Genero { get; set; }
        public required string EstadoNacimiento { get; set; }
        public required string FechaNacimiento { get; set; }
        public required string origen { get; set; }
    }
}
