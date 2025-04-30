using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace TubesCLO2_Kelompok5_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // ... /api/mahasiswa
    public class MahasiswaController : ControllerBase
    {
        // Data disimpan di list statis, akan hilang saat aplikasi stop, Sementara Kami pakai data statis terlebih dahulu
        private static List<Mahasiswa> _mahasiswaList = new List<Mahasiswa>()
        {
            new Mahasiswa { NIM = "103022300017", Nama = "Indra Yuda", Jurusan = "RPL", IPK = 3.8 },
            new Mahasiswa { NIM = "103022300023", Nama = "Azwa Radya", Jurusan = "RPL", IPK = 3.7 },
            new Mahasiswa { NIM = "103022300027", Nama = "April Hardinata", Jurusan = "RPL", IPK = 3.9 }
        };
        private readonly ILogger<MahasiswaController> _logger;
        public MahasiswaController(ILogger<MahasiswaController> logger)
        {
            _logger = logger;
        }

        // GET: api/mahasiswa -> Mengambil semua data mahasiswa
        [HttpGet]
        public ActionResult<IEnumerable<Mahasiswa>> GetAllMahasiswa(
            [FromQuery] string? nim,
            [FromQuery] string? nama
            )
        {
            _logger.LogInformation("Mencari data mahasiswa...");
            IEnumerable<Mahasiswa> result = _mahasiswaList;

            // Filter berdasarkan NIM jika ada
            if (!string.IsNullOrWhiteSpace(nim))
            {
                _logger.LogInformation($"Filter berdasarkan NIM: {nim}");
                // Cari yang NIM-nya sama persis (ignore case)
                result = result.Where(m => m.NIM.Equals(nim, StringComparison.OrdinalIgnoreCase));
            }

            // Filter berdasarkan Nama jika ada
            if (!string.IsNullOrWhiteSpace(nama))
            {
                _logger.LogInformation($"Filter berdasarkan Nama: {nama}");
                // Cari yang namanya mengandung string pencarian (ignore case)
                result = result.Where(m => m.Nama.Contains(nama, StringComparison.OrdinalIgnoreCase));
            }

            // Cek jika hasil filter kosong
            if (!result.Any())
            {
                _logger.LogWarning("Tidak ada mahasiswa yang cocok dengan kriteria pencarian.");
                return Ok(new List<Mahasiswa>()); // Return list kosong (200 OK)
            }

            return Ok(result.ToList()); // Return 200 OK dengan list hasil filter
        }

        // GET: api/mahasiswa/{nim} -> Mengambil mahasiswa berdasarkan NIM
        [HttpGet("{nim}")]
        public ActionResult<Mahasiswa> GetMahasiswaByNIM(string nim)
        {
            _logger.LogInformation($"Mencari mahasiswa dengan NIM: {nim}");
            var mahasiswa = _mahasiswaList.FirstOrDefault(m => m.NIM.Equals(nim, StringComparison.OrdinalIgnoreCase));
            if (mahasiswa == null)
            {
                _logger.LogWarning($"Mahasiswa dengan NIM: {nim} tidak ditemukan.");
                return NotFound(); // Return 404 Not Found jika tidak ada
            }
            return Ok(mahasiswa); // Return 200 OK dengan data mahasiswa
        }

        // POST: api/mahasiswa -> Menambah mahasiswa baru
        [HttpPost]
        public ActionResult<Mahasiswa> AddMahasiswa([FromBody] Mahasiswa mahasiswaBaru)
        {
            if (mahasiswaBaru == null || string.IsNullOrWhiteSpace(mahasiswaBaru.NIM) || string.IsNullOrWhiteSpace(mahasiswaBaru.Nama))
            {
                return BadRequest("Data mahasiswa tidak valid."); // Return 400 Bad Request
            }
            // Cek duplikasi NIM
            if (_mahasiswaList.Any(m => m.NIM.Equals(mahasiswaBaru.NIM, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarning($"NIM {mahasiswaBaru.NIM} sudah ada.");
                return Conflict($"Mahasiswa dengan NIM {mahasiswaBaru.NIM} sudah ada."); // Return 409 Conflict
            }
            _logger.LogInformation($"Menambahkan mahasiswa baru: {mahasiswaBaru.NIM} - {mahasiswaBaru.Nama}");
            _mahasiswaList.Add(mahasiswaBaru);
            // Return 201 Created dengan lokasi resource baru dan data yang ditambahkan
            return CreatedAtAction(nameof(GetMahasiswaByNIM), new { nim = mahasiswaBaru.NIM }, mahasiswaBaru);
        }

        // PUT: api/mahasiswa/{nim} -> Mengupdate data mahasiswa
        [HttpPut("{nim}")]
        public IActionResult UpdateMahasiswa(string nim, [FromBody] Mahasiswa mahasiswaUpdate)
        {
            if (mahasiswaUpdate == null || !nim.Equals(mahasiswaUpdate.NIM, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Data update tidak valid atau NIM tidak cocok."); // Return 400
            }
            var index = _mahasiswaList.FindIndex(m => m.NIM.Equals(nim, StringComparison.OrdinalIgnoreCase));
            if (index == -1)
            {
                _logger.LogWarning($"Mahasiswa dengan NIM: {nim} tidak ditemukan untuk diupdate.");
                return NotFound(); // Return 404
            }
            _logger.LogInformation($"Mengupdate mahasiswa: {nim}");
            _mahasiswaList[index] = mahasiswaUpdate; // Mengganti data lama dengan data yang baru
            return NoContent(); // Return 204 No Content (sukses tanpa body response)
        }

        // DELETE: api/mahasiswa/{nim} -> Menghapus data mahasiswa
        [HttpDelete("{nim}")]
        public IActionResult DeleteMahasiswa(string nim)
        {
            var mahasiswa = _mahasiswaList.FirstOrDefault(m => m.NIM.Equals(nim, StringComparison.OrdinalIgnoreCase));
            if (mahasiswa == null)
            {
                _logger.LogWarning($"Mahasiswa dengan NIM: {nim} tidak ditemukan untuk dihapus.");
                return NotFound(); // Return 404
            }
            _logger.LogInformation($"Menghapus mahasiswa: {nim}");
            _mahasiswaList.Remove(mahasiswa);
            return NoContent(); // Return 204 No Content
        }
    }
}
