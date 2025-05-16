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
        public ActionResult<object> GetAllMahasiswa(
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
                var response = new
                {
                    status = 200,
                    message = "Tidak ada mahasiswa yang cocok dengan kriteria pencarian.",
                    data = new List<Mahasiswa>()
                };
                return Ok(response);
            }
            var successResponse = new
            {
                status = 200,
                message = "Berhasil mengambil data mahasiswa.",
                data = result.ToList()
            };
            return Ok(successResponse);
        }

        // GET: api/mahasiswa/{nim} -> Mengambil mahasiswa berdasarkan NIM
        [HttpGet("{nim}")]
        public ActionResult<object> GetMahasiswaByNIM(string nim)
        {
            _logger.LogInformation($"Mencari mahasiswa dengan NIM: {nim}");
            var mahasiswa = _mahasiswaList.FirstOrDefault(m => m.NIM.Equals(nim, StringComparison.OrdinalIgnoreCase));
            if (mahasiswa == null)
            {
                _logger.LogWarning($"Mahasiswa dengan NIM: {nim} tidak ditemukan.");
                var response = new
                {
                    status = 404,
                    message = $"Mahasiswa dengan NIM {nim} tidak ditemukan.",
                    data = null as object
                };
                return NotFound(response);
            }
            var successResponse = new
            {
                status = 200,
                message = $"Berhasil mengambil data mahasiswa dengan NIM {nim}.",
                data = mahasiswa
            };
            return Ok(successResponse);
        }

        // POST: api/mahasiswa -> Menambah mahasiswa baru
        [HttpPost]
        public ActionResult<object> AddMahasiswa([FromBody] Mahasiswa mahasiswaBaru)
        {
            if (mahasiswaBaru == null || string.IsNullOrWhiteSpace(mahasiswaBaru.NIM) || string.IsNullOrWhiteSpace(mahasiswaBaru.Nama))
            {
                var errorResponse = new
                {
                    status = 400,
                    message = "Data mahasiswa tidak valid. NIM dan Nama tidak boleh kosong.",
                    data = null as object
                };
                return BadRequest(errorResponse);
            }
            // Cek duplikasi NIM
            if (_mahasiswaList.Any(m => m.NIM.Equals(mahasiswaBaru.NIM, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarning($"NIM {mahasiswaBaru.NIM} sudah ada.");
                // return Conflict($"Mahasiswa dengan NIM {mahasiswaBaru.NIM} sudah ada.");
                var conflictResponse = new
                {
                    status = 409,
                    message = $"Mahasiswa dengan NIM {mahasiswaBaru.NIM} sudah ada.",
                    data = new { nim = mahasiswaBaru.NIM }
                };
                return Conflict(conflictResponse);
            }
            _logger.LogInformation($"Menambahkan mahasiswa baru: {mahasiswaBaru.NIM} - {mahasiswaBaru.Nama}");
            _mahasiswaList.Add(mahasiswaBaru);
            // Return 201 Created dengan lokasi resource baru dan data yang ditambahkan
            var response = new
            {
                status = 201,
                message = "Berhasil menambah data mahasiswa.",
                data = mahasiswaBaru
            };
            return StatusCode(201, response);
        }

        // PUT: api/mahasiswa/{nim} -> Mengupdate data mahasiswa
        [HttpPut("{nim}")]
        public IActionResult UpdateMahasiswa(string nim, [FromBody] Mahasiswa mahasiswaUpdate)
        {
            if (mahasiswaUpdate == null || !nim.Equals(mahasiswaUpdate.NIM, StringComparison.OrdinalIgnoreCase))
            {
                var errorResponse = new
                {
                    status = 400,
                    message = "Data update tidak valid atau NIM pada body request tidak cocok dengan NIM pada URL.",
                    data = null as object
                };
                return BadRequest(errorResponse);
            }
            var index = _mahasiswaList.FindIndex(m => m.NIM.Equals(nim, StringComparison.OrdinalIgnoreCase));
            if (index == -1)
            {
                _logger.LogWarning($"Mahasiswa dengan NIM: {nim} tidak ditemukan untuk diupdate.");
                var notFoundResponse = new
                {
                    status = 404,
                    message = $"Mahasiswa dengan NIM {nim} tidak ditemukan untuk diupdate.",
                    data = null as object
                };
                return NotFound(notFoundResponse);
            }
            _logger.LogInformation($"Mengupdate mahasiswa: {nim}");
            _mahasiswaList[index] = mahasiswaUpdate;
            var successResponse = new
            {
                status = 200,
                message = $"Berhasil mengupdate data mahasiswa dengan NIM {nim}.",
                data = mahasiswaUpdate
            };
            return Ok(successResponse);
        }

        // DELETE: api/mahasiswa/{nim} -> Menghapus data mahasiswa
        [HttpDelete("{nim}")]
        public IActionResult DeleteMahasiswa(string nim)
        {
            var mahasiswa = _mahasiswaList.FirstOrDefault(m => m.NIM.Equals(nim, StringComparison.OrdinalIgnoreCase));
            if (mahasiswa == null)
            {
                _logger.LogWarning($"Mahasiswa dengan NIM: {nim} tidak ditemukan untuk dihapus.");
                var notFoundResponse = new
                {
                    status = 404,
                    message = $"Mahasiswa dengan NIM {nim} tidak ditemukan untuk dihapus.",
                    data = null as object
                };
                return NotFound(notFoundResponse);
            }
            _logger.LogInformation($"Menghapus mahasiswa: {nim}");
            _mahasiswaList.Remove(mahasiswa);
            var successResponse = new
            {
                status = 200,
                message = $"Berhasil menghapus data mahasiswa dengan NIM {nim}.",
                data = null as object
            };
            return Ok(successResponse);
        }
    }
}
