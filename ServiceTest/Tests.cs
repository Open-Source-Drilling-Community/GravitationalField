using System.Net.Http.Headers;
using NORCE.Drilling.GravitationalField.ModelShared;

namespace ServiceTest
{
    public class Tests
    {
        // testing outside Visual Studio requires using http port (https faces authentication issues both in console and on github)
        private static string host = "http://localhost:8080/";
        //private static string host = "https://localhost:5001/";
        //private static string host = "https://localhost:44368/";
        //private static string host = "http://localhost:54949/";
        private static HttpClient httpClient;
        private static Client nSwagClient;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }; // temporary workaround for testing purposes: bypass certificate validation (not recommended for production environments due to security risks)
            httpClient = new HttpClient(handler);
            httpClient.BaseAddress = new Uri(host + "GravitationalField/api/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            nSwagClient = new Client(httpClient.BaseAddress.ToString(), httpClient);
        }

        [Test]
        public async Task Test_GravitationalFieldCalculationOrder_GET()
        {
            #region post a GravitationalFieldCalculationOrder
            // Create instance of GravitationalFieldCalculationOrder
            GravitationalFieldCalculationOrder gravitationalFieldCalculationOrder = PseudoConstructors.ConstructGravitationalFieldCalculationOrder();

            //Extract metainfo
            MetaInfo metaInfo = gravitationalFieldCalculationOrder.MetaInfo;
            Guid guid = metaInfo.ID;
            try
            {
                await nSwagClient.PostGravitationalFieldCalculationOrderAsync(gravitationalFieldCalculationOrder);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to POST given GravitationalFieldCalculationOrder\n" + ex.Message);
            }
            #endregion

            #region GetAllGravitationalFieldCalculationOrderId
            List<Guid> idList = [];
            try
            {
                idList = (List<Guid>)await nSwagClient.GetAllGravitationalFieldCalculationOrderIdAsync();
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to GET all GravitationalFieldCalculationOrder ids\n" + ex.Message);
            }
            Assert.That(idList, Is.Not.Null);
            Assert.That(idList, Does.Contain(guid));
            #endregion

            #region GetAllGravitationalFieldCalculationOrderMetaInfo
            List<MetaInfo> metaInfoList = [];
            try
            {
                metaInfoList = (List<MetaInfo>)await nSwagClient.GetAllGravitationalFieldCalculationOrderMetaInfoAsync();
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to GET all GravitationalFieldCalculationOrder metainfos\n" + ex.Message);
            }
            Assert.That(metaInfoList, Is.Not.Null);
            IEnumerable<MetaInfo> metaInfoList2 =
                from elt in metaInfoList
                where elt.ID == guid
                select elt;
            Assert.That(metaInfoList2, Is.Not.Null);
            Assert.That(metaInfoList2, Is.Not.Empty);
            #endregion

            #region GetAllGravitationalFieldCalculationOrderById
            GravitationalFieldCalculationOrder? gravitationalFieldCalculationOrder2 = null;
            try
            {
                gravitationalFieldCalculationOrder2 = await nSwagClient.GetGravitationalFieldCalculationOrderByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to GET GravitationalFieldCalculationOrder of given Id\n" + ex.Message);
            }
            Assert.That(gravitationalFieldCalculationOrder2, Is.Not.Null);
            Assert.That(gravitationalFieldCalculationOrder2.Name, Is.EqualTo(gravitationalFieldCalculationOrder.Name));
            #endregion

            #region GetAllGravitationalFieldCalculationOrderLight
            List<GravitationalFieldCalculationOrderLight> gravitationalFieldCalculationOrderLightList = [];
            try
            {
                gravitationalFieldCalculationOrderLightList = (List<GravitationalFieldCalculationOrderLight>)await nSwagClient.GetAllGravitationalFieldCalculationOrderLightAsync();
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to GET the list of GravitationalFieldCalculationOrderLight\n" + ex.Message);
            }
            Assert.That(gravitationalFieldCalculationOrderLightList, Is.Not.Null);
            Assert.That(gravitationalFieldCalculationOrderLightList, Is.Not.Empty);
            IEnumerable<GravitationalFieldCalculationOrderLight> gravitationalFieldCalculationOrderLightList2 =
                from elt in gravitationalFieldCalculationOrderLightList
                where elt.Name == gravitationalFieldCalculationOrder.Name
                select elt;
            Assert.That(gravitationalFieldCalculationOrderLightList2, Is.Not.Null);
            Assert.That(gravitationalFieldCalculationOrderLightList2, Is.Not.Empty);
            #endregion

            #region GetAllGravitationalFieldCalculationOrder
            List<GravitationalFieldCalculationOrder> gravitationalFieldCalculationOrderList = new();
            try
            {
                gravitationalFieldCalculationOrderList = (List<GravitationalFieldCalculationOrder>)await nSwagClient.GetAllGravitationalFieldCalculationOrderAsync();
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to GET the list of GravitationalFieldCalculationOrder\n" + ex.Message);
            }
            Assert.That(gravitationalFieldCalculationOrderList, Is.Not.Null);
            IEnumerable<GravitationalFieldCalculationOrder> gravitationalFieldCalculationOrderList2 =
                from elt in gravitationalFieldCalculationOrderList
                where elt.Name == gravitationalFieldCalculationOrder.Name
                select elt;
            Assert.That(gravitationalFieldCalculationOrderList2, Is.Not.Null);
            Assert.That(gravitationalFieldCalculationOrderList2, Is.Not.Empty);
            #endregion

            #region finally delete the new ID
            gravitationalFieldCalculationOrder2 = null;
            try
            {
                await nSwagClient.DeleteGravitationalFieldCalculationOrderByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to DELETE GravitationalFieldCalculationOrder of given Id\n" + ex.Message);
            }
            try
            {
                gravitationalFieldCalculationOrder2 = await nSwagClient.GetGravitationalFieldCalculationOrderByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(404));
                TestContext.WriteLine("Impossible to GET GravitationalFieldCalculationOrder of given Id\n" + ex.Message);
            }
            Assert.That(gravitationalFieldCalculationOrder2, Is.Null);
            #endregion
        }

        [Test]
        public async Task Test_GravitationalFieldCalculationOrder_POST()
        {
            #region trying to post an empty guid
            // Create instance of gravitationalFieldCalculationOrder
            GravitationalFieldCalculationOrder gravitationalFieldCalculationOrder = PseudoConstructors.ConstructGravitationalFieldCalculationOrder();
            gravitationalFieldCalculationOrder.MetaInfo.ID = Guid.Empty;
            //Extract metainfo
            MetaInfo metaInfo = gravitationalFieldCalculationOrder.MetaInfo;
            GravitationalFieldCalculationOrder? gravitationalFieldCalculationOrder2 = null;
            try
            {
                await nSwagClient.PostGravitationalFieldCalculationOrderAsync(gravitationalFieldCalculationOrder);
            }
            catch (ApiException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(400));
                TestContext.WriteLine("Impossible to POST GravitationalFieldCalculationOrder with empty Guid\n" + ex.Message);
            }
            try
            {
                gravitationalFieldCalculationOrder2 = await nSwagClient.GetGravitationalFieldCalculationOrderByIdAsync(Guid.Empty);
            }
            catch (ApiException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(400));
                TestContext.WriteLine("Impossible to GET GravitationalFieldCalculationOrder identified by an empty Guid\n" + ex.Message);
            }
            Assert.That(gravitationalFieldCalculationOrder2, Is.Null);
            #endregion

            #region post some corrupted data
            // post data with missing input that fails the calculation process
            #endregion

            #region posting a new ID in a valid state
            Guid guid = Guid.NewGuid();
            metaInfo = new() { ID = guid };
            gravitationalFieldCalculationOrder.MetaInfo = metaInfo;
            try
            {
                await nSwagClient.PostGravitationalFieldCalculationOrderAsync(gravitationalFieldCalculationOrder);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to POST GravitationalFieldCalculationOrder although it is in a valid state\n" + ex.Message);
            }
            try
            {
                gravitationalFieldCalculationOrder2 = await nSwagClient.GetGravitationalFieldCalculationOrderByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to GET the GravitationalFieldCalculationOrder of given Id\n" + ex.Message);
            }
            Assert.That(gravitationalFieldCalculationOrder2, Is.Not.Null);
            Assert.That(gravitationalFieldCalculationOrder2.MetaInfo, Is.Not.Null);
            Assert.That(gravitationalFieldCalculationOrder2.MetaInfo.ID, Is.EqualTo(guid));
            Assert.That(gravitationalFieldCalculationOrder2.Name, Is.EqualTo(gravitationalFieldCalculationOrder.Name));
            #endregion

            #region trying to repost the same ID
            bool conflict = false;
            try
            {
                await nSwagClient.PostGravitationalFieldCalculationOrderAsync(gravitationalFieldCalculationOrder);
            }
            catch (ApiException ex)
            {
                conflict = true;
                Assert.That(ex.StatusCode, Is.EqualTo(409));
                TestContext.WriteLine("Impossible to POST existing GravitationalFieldCalculationOrder\n" + ex.Message);
            }
            Assert.That(conflict, Is.True);
            #endregion

            #region finally delete the new ID
            gravitationalFieldCalculationOrder2 = null;
            try
            {
                await nSwagClient.DeleteGravitationalFieldCalculationOrderByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to DELETE GravitationalFieldCalculationOrder of given Id\n" + ex.Message);
            }
            try
            {
                gravitationalFieldCalculationOrder2 = await nSwagClient.GetGravitationalFieldCalculationOrderByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(404));
                TestContext.WriteLine("Impossible to GET deleted GravitationalFieldCalculationOrder of given Id\n" + ex.Message);
            }
            Assert.That(gravitationalFieldCalculationOrder2, Is.Null);
            #endregion
        }

        [Test]
        public async Task Test_GravitationalFieldCalculationOrder_PUT()
        {
            #region posting a new ID
            // Create instance of gravitationalFieldCalculationOrder
            GravitationalFieldCalculationOrder gravitationalFieldCalculationOrder = PseudoConstructors.ConstructGravitationalFieldCalculationOrder();
            //Extract metainfo
            MetaInfo metaInfo = gravitationalFieldCalculationOrder.MetaInfo;
            Guid guid = metaInfo.ID;
            GravitationalFieldCalculationOrder? gravitationalFieldCalculationOrder2 = null;
            try
            {
                await nSwagClient.PostGravitationalFieldCalculationOrderAsync(gravitationalFieldCalculationOrder);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to POST GravitationalFieldCalculationOrder\n" + ex.Message);
            }
            try
            {
                gravitationalFieldCalculationOrder2 = await nSwagClient.GetGravitationalFieldCalculationOrderByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to GET the GravitationalFieldCalculationOrder of given Id\n" + ex.Message);
            }
            Assert.That(gravitationalFieldCalculationOrder2, Is.Not.Null);
            Assert.That(gravitationalFieldCalculationOrder2.MetaInfo, Is.Not.Null);
            Assert.That(gravitationalFieldCalculationOrder2.MetaInfo.ID, Is.EqualTo(guid));
            Assert.That(gravitationalFieldCalculationOrder2.Name, Is.EqualTo(gravitationalFieldCalculationOrder.Name));
            #endregion

            #region updating the new Id
            gravitationalFieldCalculationOrder.Name = "My test GravitationalFieldCalculationOrder with modified name";
            gravitationalFieldCalculationOrder.LastModificationDate = DateTimeOffset.UtcNow;
            try
            {
                await nSwagClient.PutGravitationalFieldCalculationOrderByIdAsync(gravitationalFieldCalculationOrder.MetaInfo.ID, gravitationalFieldCalculationOrder);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to PUT GravitationalFieldCalculationOrder of given Id\n" + ex.Message);
            }
            try
            {
                gravitationalFieldCalculationOrder2 = await nSwagClient.GetGravitationalFieldCalculationOrderByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to GET the updated GravitationalFieldCalculationOrder of given Id\n" + ex.Message);
            }
            Assert.That(gravitationalFieldCalculationOrder2, Is.Not.Null);
            Assert.That(gravitationalFieldCalculationOrder2.MetaInfo, Is.Not.Null);
            Assert.That(gravitationalFieldCalculationOrder2.MetaInfo.ID, Is.EqualTo(gravitationalFieldCalculationOrder.MetaInfo.ID));
            Assert.That(gravitationalFieldCalculationOrder2.Name, Is.EqualTo(gravitationalFieldCalculationOrder.Name));
            #endregion

            #region finally delete the new ID
            gravitationalFieldCalculationOrder2 = null;
            try
            {
                await nSwagClient.DeleteGravitationalFieldCalculationOrderByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to DELETE GravitationalFieldCalculationOrder of given Id\n" + ex.Message);
            }
            try
            {
                gravitationalFieldCalculationOrder2 = await nSwagClient.GetGravitationalFieldCalculationOrderByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(404));
                TestContext.WriteLine("Impossible to GET deleted GravitationalFieldCalculationOrder of given Id\n" + ex.Message);
            }
            Assert.That(gravitationalFieldCalculationOrder2, Is.Null);
            #endregion
        }

        [Test]
        public async Task Test_GravitationalFieldCalculationOrder_DELETE()
        {
            #region posting a new ID
            // Create instance of gravitationalFieldCalculationOrder
            GravitationalFieldCalculationOrder gravitationalFieldCalculationOrder = PseudoConstructors.ConstructGravitationalFieldCalculationOrder();
            //Extract metainfo
            MetaInfo metaInfo = gravitationalFieldCalculationOrder.MetaInfo;
            Guid guid = metaInfo.ID;
            GravitationalFieldCalculationOrder? gravitationalFieldCalculationOrder2 = null;
            try
            {
                await nSwagClient.PostGravitationalFieldCalculationOrderAsync(gravitationalFieldCalculationOrder);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to POST GravitationalFieldCalculationOrder\n" + ex.Message);
            }
            try
            {
                gravitationalFieldCalculationOrder2 = await nSwagClient.GetGravitationalFieldCalculationOrderByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to GET the GravitationalFieldCalculationOrder of given Id\n" + ex.Message);
            }
            Assert.That(gravitationalFieldCalculationOrder2, Is.Not.Null);
            Assert.That(gravitationalFieldCalculationOrder2.MetaInfo, Is.Not.Null);
            Assert.That(gravitationalFieldCalculationOrder2.MetaInfo.ID, Is.EqualTo(gravitationalFieldCalculationOrder.MetaInfo.ID));
            Assert.That(gravitationalFieldCalculationOrder2.Name, Is.EqualTo(gravitationalFieldCalculationOrder.Name));
            #endregion

            #region finally delete the new ID
            gravitationalFieldCalculationOrder2 = null;
            try
            {
                await nSwagClient.DeleteGravitationalFieldCalculationOrderByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to DELETE GravitationalFieldCalculationOrder of given Id\n" + ex.Message);
            }
            try
            {
                gravitationalFieldCalculationOrder2 = await nSwagClient.GetGravitationalFieldCalculationOrderByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(404));
                TestContext.WriteLine("Impossible to GET deleted GravitationalFieldCalculationOrder of given Id\n" + ex.Message);
            }
            Assert.That(gravitationalFieldCalculationOrder2, Is.Null);
            #endregion
        }

        [Test]
        public async Task Test_GravitationalField_GET()
        {
            #region post a GravitationalField
            // Create instance of gravitationalField
            GravitationalField gravitationalField = PseudoConstructors.ConstructGravitationalField();
            MetaInfo metaInfo = gravitationalField.MetaInfo;
            Guid guid = metaInfo.ID;

            try
            {
                await nSwagClient.PostGravitationalFieldAsync(gravitationalField);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to POST given GravitationalField\n" + ex.Message);
            }
            #endregion

            #region GetAllGravitationalFieldId
            List<Guid?> idList = [];
            try
            {
                idList = (List<Guid?>)await nSwagClient.GetAllGravitationalFieldIdAsync();
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to GET all GravitationalField ids\n" + ex.Message);
            }
            Assert.That(idList, Is.Not.Null);
            Assert.That(idList, Does.Contain(guid));
            #endregion

            #region GetAllGravitationalFieldMetaInfo
            List<MetaInfo> metaInfoList = [];
            try
            {
                metaInfoList = (List<MetaInfo>)await nSwagClient.GetAllGravitationalFieldMetaInfoAsync();
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to GET all GravitationalField metainfos\n" + ex.Message);
            }
            Assert.That(metaInfoList, Is.Not.Null);
            IEnumerable<MetaInfo> metaInfoList2 =
                from elt in metaInfoList
                where elt.ID == guid
                select elt;
            Assert.That(metaInfoList2, Is.Not.Null);
            Assert.That(metaInfoList2, Is.Not.Empty);
            #endregion

            #region GetAllGravitationalFieldById
            GravitationalField? gravitationalField2 = null;
            try
            {
                gravitationalField2 = await nSwagClient.GetGravitationalFieldByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to GET GravitationalField of given Id\n" + ex.Message);
            }
            Assert.That(gravitationalField2, Is.Not.Null);
            Assert.That(gravitationalField2.MetaInfo.ID, Is.EqualTo(guid));
            Assert.That(gravitationalField2.Name, Is.EqualTo(gravitationalField.Name));
            #endregion

            #region GetAllGravitationalField
            List<GravitationalField> gravitationalFieldList = [];
            try
            {
                gravitationalFieldList = (List<GravitationalField>)await nSwagClient.GetAllGravitationalFieldAsync();
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to GET the list of GravitationalField\n" + ex.Message);
            }
            Assert.That(gravitationalFieldList, Is.Not.Null);
            IEnumerable<GravitationalField> gravitationalFieldList2 =
                from elt in gravitationalFieldList
                where elt.Name == gravitationalField.Name
                select elt;
            Assert.That(gravitationalFieldList2, Is.Not.Null);
            Assert.That(gravitationalFieldList2, Is.Not.Empty);
            #endregion

            #region finally delete the new ID
            gravitationalField2 = null;
            try
            {
                await nSwagClient.DeleteGravitationalFieldByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to DELETE GravitationalField of given Id\n" + ex.Message);
            }
            try
            {
                gravitationalField2 = await nSwagClient.GetGravitationalFieldByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(404));
                TestContext.WriteLine("Impossible to GET GravitationalField of given Id\n" + ex.Message);
            }
            Assert.That(gravitationalField2, Is.Null);
            #endregion
        }

        [Test]
        public async Task Test_GravitationalField_POST()
        {
            #region trying to post an empty guid
            // Create instance of gravitationalField
            GravitationalField gravitationalField = PseudoConstructors.ConstructGravitationalField();
            MetaInfo metaInfo = gravitationalField.MetaInfo;

            GravitationalField? gravitationalField2 = null;
            try
            {
                await nSwagClient.PostGravitationalFieldAsync(gravitationalField);
            }
            catch (ApiException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(400));
                TestContext.WriteLine("Impossible to POST GravitationalField with empty Guid\n" + ex.Message);
            }
            try
            {
                gravitationalField2 = await nSwagClient.GetGravitationalFieldByIdAsync(Guid.Empty);
            }
            catch (ApiException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(400));
                TestContext.WriteLine("Impossible to GET GravitationalField identified by an empty Guid\n" + ex.Message);
            }
            Assert.That(gravitationalField2, Is.Null);
            #endregion

            #region posting a new ID in a valid state
            Guid guid = Guid.NewGuid();
            metaInfo = new() { ID = guid };
            gravitationalField.MetaInfo = metaInfo;
            try
            {
                await nSwagClient.PostGravitationalFieldAsync(gravitationalField);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to POST GravitationalField although it is in a valid state\n" + ex.Message);
            }
            try
            {
                gravitationalField2 = await nSwagClient.GetGravitationalFieldByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to GET the GravitationalField of given Id\n" + ex.Message);
            }
            Assert.That(gravitationalField2, Is.Not.Null);
            Assert.That(gravitationalField2.MetaInfo, Is.Not.Null);
            Assert.That(gravitationalField2.MetaInfo.ID, Is.EqualTo(guid));
            Assert.That(gravitationalField2.Name, Is.EqualTo(gravitationalField.Name));
            #endregion

            #region trying to repost the same ID
            bool conflict = false;
            try
            {
                await nSwagClient.PostGravitationalFieldAsync(gravitationalField);
            }
            catch (ApiException ex)
            {
                conflict = true;
                Assert.That(ex.StatusCode, Is.EqualTo(409));
                TestContext.WriteLine("Impossible to POST existing GravitationalField\n" + ex.Message);
            }
            Assert.That(conflict, Is.True);
            #endregion

            #region finally delete the new ID
            gravitationalField2 = null;
            try
            {
                await nSwagClient.DeleteGravitationalFieldByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to DELETE GravitationalField of given Id\n" + ex.Message);
            }
            try
            {
                gravitationalField2 = await nSwagClient.GetGravitationalFieldByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(404));
                TestContext.WriteLine("Impossible to GET deleted GravitationalField of given Id\n" + ex.Message);
            }
            Assert.That(gravitationalField2, Is.Null);
            #endregion
        }

        [Test]
        public async Task Test_GravitationalField_PUT()
        {
            #region posting a new ID
            // Create instance of gravitationalField
            GravitationalField gravitationalField = PseudoConstructors.ConstructGravitationalField();
            MetaInfo metaInfo = gravitationalField.MetaInfo;
            Guid guid = metaInfo.ID;

            GravitationalField? gravitationalField2 = null;
            try
            {
                await nSwagClient.PostGravitationalFieldAsync(gravitationalField);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to POST GravitationalField\n" + ex.Message);
            }
            try
            {
                gravitationalField2 = await nSwagClient.GetGravitationalFieldByIdAsync(gravitationalField.MetaInfo.ID);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to GET the GravitationalField of given Id\n" + ex.Message);
            }
            Assert.That(gravitationalField2, Is.Not.Null);
            Assert.That(gravitationalField2.MetaInfo, Is.Not.Null);
            Assert.That(gravitationalField2.MetaInfo.ID, Is.EqualTo(gravitationalField.MetaInfo.ID));
            Assert.That(gravitationalField2.Name, Is.EqualTo(gravitationalField.Name));
            #endregion

            #region updating the new Id
            gravitationalField.Name = "My test GravitationalField with modified name";
            gravitationalField.LastModificationDate = DateTimeOffset.UtcNow;
            try
            {
                await nSwagClient.PutGravitationalFieldByIdAsync(gravitationalField.MetaInfo.ID, gravitationalField);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to PUT GravitationalField of given Id\n" + ex.Message);
            }
            try
            {
                gravitationalField2 = await nSwagClient.GetGravitationalFieldByIdAsync(gravitationalField.MetaInfo.ID);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to GET the updated GravitationalField of given Id\n" + ex.Message);
            }
            Assert.That(gravitationalField2, Is.Not.Null);
            Assert.That(gravitationalField2.MetaInfo, Is.Not.Null);
            Assert.That(gravitationalField2.MetaInfo.ID, Is.EqualTo(gravitationalField.MetaInfo.ID));
            Assert.That(gravitationalField2.Name, Is.EqualTo(gravitationalField.Name));
            #endregion

            #region finally delete the new ID
            gravitationalField2 = null;
            try
            {
                await nSwagClient.DeleteGravitationalFieldByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to DELETE GravitationalField of given Id\n" + ex.Message);
            }
            try
            {
                gravitationalField2 = await nSwagClient.GetGravitationalFieldByIdAsync(gravitationalField.MetaInfo.ID);
            }
            catch (ApiException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(404));
                TestContext.WriteLine("Impossible to GET deleted GravitationalField of given Id\n" + ex.Message);
            }
            Assert.That(gravitationalField2, Is.Null);
            #endregion
        }

        [Test]
        public async Task Test_GravitationalField_DELETE()
        {
            #region posting a new ID
            // Create instance of gravitationalField
            GravitationalField gravitationalField = PseudoConstructors.ConstructGravitationalField();
            MetaInfo metaInfo = gravitationalField.MetaInfo;
            Guid guid = metaInfo.ID;

            GravitationalField? gravitationalField2 = null;
            try
            {
                await nSwagClient.PostGravitationalFieldAsync(gravitationalField);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to POST GravitationalField\n" + ex.Message);
            }
            try
            {
                gravitationalField2 = await nSwagClient.GetGravitationalFieldByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to GET the GravitationalField of given Id\n" + ex.Message);
            }
            Assert.That(gravitationalField2, Is.Not.Null);
            Assert.That(gravitationalField2.MetaInfo, Is.Not.Null);
            Assert.That(gravitationalField2.MetaInfo.ID, Is.EqualTo(guid));
            Assert.That(gravitationalField2.Name, Is.EqualTo(gravitationalField.Name));
            #endregion

            #region finally delete the new ID
            gravitationalField2 = null;
            try
            {
                await nSwagClient.DeleteGravitationalFieldByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to DELETE GravitationalField of given Id\n" + ex.Message);
            }
            try
            {
                gravitationalField2 = await nSwagClient.GetGravitationalFieldByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(404));
                TestContext.WriteLine("Impossible to GET deleted GravitationalField of given Id\n" + ex.Message);
            }
            Assert.That(gravitationalField2, Is.Null);
            #endregion
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            httpClient?.Dispose();
        }
    }
}