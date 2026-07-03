using OSDC.DotnetLibraries.Drilling.DrillingProperties;
using OSDC.DotnetLibraries.General.DataManagement;
using OSDC.DotnetLibraries.General.Statistics;
using NORCE.Drilling.GravitationalField.Model;

namespace NORCE.Drilling.GravitationalField.ModelTest
{
    public class Tests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
        }

        [Test]
        public void Test_Calculus()
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
                    Latitude = 0,
                    Longitude = 0,
                    Depth = 0
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
            Assert.That(success == true);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
        }
    }
}
