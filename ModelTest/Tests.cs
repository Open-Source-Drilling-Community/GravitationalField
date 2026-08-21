using OSDC.DotnetLibraries.Drilling.DrillingProperties;
using OSDC.DotnetLibraries.General.DataManagement;
using OSDC.DotnetLibraries.General.Statistics;
using NORCE.Drilling.GravitationalField.Model;
using GeographicLib;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace NORCE.Drilling.GravitationalField.ModelTest
{
    public class Tests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
        }

        [Test]
        public void Calculate_ConvertsPublicRadiansToGeographicLibDegrees()
        {
            Guid guid = Guid.NewGuid();
            MetaInfo metaInfo = new() { ID = guid };
            DateTimeOffset creationDate = DateTimeOffset.UtcNow;

            Guid guid2 = Guid.NewGuid();
            MetaInfo metaInfo2 = new() { ID = guid2 };
            DateTimeOffset creationDate2 = DateTimeOffset.UtcNow;
            List<GravitationalData> table = new List<GravitationalData>
            {
                new GravitationalData
                {
                    Latitude = 50.0 * Math.PI / 180.0,
                    Longitude = 30.0 * Math.PI / 180.0,
                    Depth = 100.0
                }
            };
            Model.GravitationalField gravitationalField = new()
            {
                MetaInfo = metaInfo2,
                Name = "My test GravitationalField name",
                Description = "My test GravitationalField for POST",
                CreationDate = creationDate,
                LastModificationDate = creationDate2,
                Type = GravitationalFieldType.Raw,
                GravitationalDataTable = table
               
            };
            Model.GravitationalFieldCalculationOrder gravitationalFieldCalculationOrder = new()
            {
                MetaInfo = metaInfo,
                Name = "My test GravitationalFieldCalculationOrder",
                Description = "My test GravitationalFieldCalculationOrder",
                CreationDate = creationDate,
                LastModificationDate = creationDate,
                RawGravitationalField = gravitationalField,
            };

            bool success = gravitationalFieldCalculationOrder.Calculate();
            Assert.That(success, Is.True);
            GravitationalData result = gravitationalFieldCalculationOrder.CompletedGravitationalField!.GravitationalDataTable!.Single();
            GravityModel model = new("egm96", Path.Combine(FindSolutionDirectory(), "GravityModelFiles"));
            (_, double expectedX, double expectedY, double expectedZ) = model.Gravity(50.0, 30.0, -100.0);
            Assert.Multiple(() =>
            {
                Assert.That(result.Latitude, Is.EqualTo(table[0].Latitude));
                Assert.That(result.Longitude, Is.EqualTo(table[0].Longitude));
                Assert.That(result.Depth, Is.EqualTo(100.0));
                Assert.That(result.GravityIntensityX, Is.EqualTo(expectedX).Within(1e-12));
                Assert.That(result.GravityIntensityY, Is.EqualTo(expectedY).Within(1e-12));
                Assert.That(result.GravityIntensityZ, Is.EqualTo(expectedZ).Within(1e-12));
            });
        }

        [Test]
        public void GravitationalData_RejectsDegreeValuesInPublicAngleContract()
        {
            GravitationalData data = new() { Latitude = 50.0, Longitude = 30.0, Depth = 0.0 };
            List<ValidationResult> results = [];

            bool valid = Validator.TryValidateObject(data, new ValidationContext(data), results, validateAllProperties: true);

            Assert.That(valid, Is.False);
            Assert.That(results, Has.Count.EqualTo(2));
        }

        [Test]
        public void GravitationalData_UsesCorrectGravityIntensityJsonNames()
        {
            string json = JsonSerializer.Serialize(new GravitationalData
            {
                Latitude = 0.1,
                Longitude = 0.2,
                Depth = 3.0,
                GravityIntensityX = 1.0,
                GravityIntensityY = 2.0,
                GravityIntensityZ = 3.0
            });

            Assert.That(json, Does.Contain("\"GravityIntensityX\""));
            Assert.That(json, Does.Not.Contain("Gravitaty"));
        }

        private static string FindSolutionDirectory()
        {
            DirectoryInfo? directory = new(TestContext.CurrentContext.TestDirectory);
            while (directory != null && directory.GetFiles("*.sln").Length == 0)
            {
                directory = directory.Parent;
            }

            return directory?.FullName ?? throw new DirectoryNotFoundException("Could not find the solution directory.");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
        }
    }
}
