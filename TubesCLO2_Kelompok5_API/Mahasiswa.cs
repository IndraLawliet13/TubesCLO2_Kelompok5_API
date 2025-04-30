namespace TubesCLO2_Kelompok5_API
{
    public class Mahasiswa
    {
        // Pakai string untuk NIM biar fleksibel (misal ada huruf)
        public required string NIM { get; set; }
        public required string Nama { get; set; }
        public string? Jurusan { get; set; } // Boleh kosong (nullable)
        public double IPK { get; set; }
    }
}
