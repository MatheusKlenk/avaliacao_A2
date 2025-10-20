namespace Victor.models
{
    public class Consumo
    {
        public int Id { get; set; }
        public string Cpf { get; set; } = string.Empty;
        public int Mes { get; set; }
        public int Ano { get; set; }
        public double M3Consumidos { get; set; }
        public string Bandeira { get; set; } = string.Empty;
        public bool PossuiEsgoto { get; set; }

        // Campos calculados
        public double ConsumoFaturado { get; set; }
        public double Tarifa { get; set; }
        public double ValorAgua { get; set; }
        public double AdicionalBandeira { get; set; }
        public double TaxaEsgoto { get; set; }
        public double Total { get; set; }
    }
}