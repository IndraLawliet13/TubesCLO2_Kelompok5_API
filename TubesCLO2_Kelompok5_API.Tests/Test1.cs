using TubesCLO2_Kelompok5_API.Controllers;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace TubesCLO2_Kelompok5_API.Tests
{
    [TestClass]
    public sealed class Test1
    {
        private MahasiswaController? _controller;

        private void ResetInitialMahasiswaData()
        {
            var mahasiswaListField = typeof(MahasiswaController).GetField("_mahasiswaList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            if (mahasiswaListField != null)
            {
                var initialData = new List<Mahasiswa>() //
                {
                    new Mahasiswa { NIM = "103022300017", Nama = "Indra Yuda", Jurusan = "RPL", IPK = 3.8 }, //
                    new Mahasiswa { NIM = "103022300023", Nama = "Azwa Radya", Jurusan = "RPL", IPK = 3.7 }, //
                    new Mahasiswa { NIM = "103022300027", Nama = "April Hardinata", Jurusan = "RPL", IPK = 3.9 } //
                };
                mahasiswaListField.SetValue(null, initialData);
            }
            else
            {
                Assert.Fail("_mahasiswaList field not found in MahasiswaController. Check its definition.");
            }
        }

        [TestInitialize]
        public void Setup()
        {
            ResetInitialMahasiswaData();
            _controller = new MahasiswaController(NullLogger<MahasiswaController>.Instance);
        }
        private T? GetResultData<T>(ActionResult<object> actionResult) where T : class
        {
            if (actionResult.Result is OkObjectResult okResult)
            {
                return okResult.Value?.GetType().GetProperty("data")?.GetValue(okResult.Value) as T;
            }
            if (actionResult.Result is ObjectResult objectResult)
            {
                return objectResult.Value?.GetType().GetProperty("data")?.GetValue(objectResult.Value) as T;
            }
            return null;
        }
        private T? GetResultData<T>(IActionResult actionResult) where T : class
        {
            if (actionResult is OkObjectResult okResult)
            {
                return okResult.Value?.GetType().GetProperty("data")?.GetValue(okResult.Value) as T;
            }
            if (actionResult is ObjectResult objectResult)
            {
                return objectResult.Value?.GetType().GetProperty("data")?.GetValue(objectResult.Value) as T;
            }
            return null;
        }

        [TestMethod]
        public void GetMahasiswaByNIM_WhenNIMExists_ShouldReturnOkWithMahasiswa()
        {
            var existingNIM = "103022300017";

            var actionResult = _controller.GetMahasiswaByNIM(existingNIM);

            Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult), "Harusnya dapet OkObjectResult.");
            var mahasiswa = GetResultData<Mahasiswa>(actionResult);
            Assert.IsNotNull(mahasiswa, "Data mahasiswa gak boleh null.");
            Assert.AreEqual(existingNIM, mahasiswa.NIM, "NIM mahasiswa gak sesuai.");
        }
        [TestMethod]
        public void AddMahasiswa_WithValidData_ShouldReturnCreatedWithMahasiswa()
        {
            var newMahasiswa = new Mahasiswa { NIM = "103022399001", Nama = "Budi Santoso", Jurusan = "DKV", IPK = 3.9 };

            var actionResult = _controller.AddMahasiswa(newMahasiswa);

            Assert.IsInstanceOfType(actionResult.Result, typeof(ObjectResult), "Harusnya dapet ObjectResult untuk status 201.");
            var objectResult = actionResult.Result as ObjectResult;
            Assert.AreEqual(201, objectResult.StatusCode, "Status code harusnya 201 (Created).");

            var createdMahasiswa = GetResultData<Mahasiswa>(actionResult);
            Assert.IsNotNull(createdMahasiswa, "Data mahasiswa yang baru dibuat gak boleh null.");
            Assert.AreEqual(newMahasiswa.NIM, createdMahasiswa.NIM, "NIM mahasiswa yang baru dibuat gak sesuai.");
        }

        [TestMethod]
        public void AddMahasiswa_WhenNIMExists_ShouldReturnConflict()
        {
            var existingMahasiswa = new Mahasiswa { NIM = "103022300017", Nama = "Dobel Indra", Jurusan = "RPL", IPK = 3.0 };

            var actionResult = _controller.AddMahasiswa(existingMahasiswa); //

            Assert.IsInstanceOfType(actionResult.Result, typeof(ConflictObjectResult), "Harusnya dapet ConflictObjectResult.");
        }

        [TestMethod]
        public void UpdateMahasiswa_WithValidData_ShouldReturnOkWithUpdatedMahasiswa()
        {
            var nimToUpdate = "103022300023";
            var updatedData = new Mahasiswa { NIM = nimToUpdate, Nama = "Azwa Radya Updated", Jurusan = "RPL Updated", IPK = 3.75 };

            var actionResult = _controller.UpdateMahasiswa(nimToUpdate, updatedData);

            Assert.IsInstanceOfType(actionResult, typeof(OkObjectResult), "Harusnya dapet OkObjectResult.");
            var resultMahasiswa = GetResultData<Mahasiswa>(actionResult);
            Assert.IsNotNull(resultMahasiswa, "Data mahasiswa yang diupdate gak boleh null.");
            Assert.AreEqual(updatedData.Nama, resultMahasiswa.Nama, "Nama mahasiswa gak keupdate dengan benar.");
        }

        [TestMethod]
        public void DeleteMahasiswa_WhenNIMExists_ShouldReturnOkAndRemoveMahasiswa()
        {
            var nimToDelete = "103022300027";

            var actionResult = _controller.DeleteMahasiswa(nimToDelete);

            Assert.IsInstanceOfType(actionResult, typeof(OkObjectResult), "Harusnya dapet OkObjectResult setelah delete.");

            var checkResult = _controller.GetMahasiswaByNIM(nimToDelete);
            Assert.IsInstanceOfType(checkResult.Result, typeof(NotFoundObjectResult), "Mahasiswa harusnya gak ketemu setelah didelete.");
        }
    }
}
