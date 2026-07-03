namespace NORCE.Drilling.GravitationalField.ModelShared
{
	public class PseudoConstructors
	{
		public static MetaInfo ConstructMetaInfo()
			{
				return new MetaInfo 
				{
					ID = Guid.NewGuid(),
					HttpHostName = "https://dev.digiwells.no/",
					HttpHostBasePath = "GravitationalField/api/",
					HttpEndPoint = "GravitationalFieldCalculationOrder/",
				};
			}

		public static MetaInfo ConstructMetaInfo(Guid id)
			{
				return new MetaInfo 
				{
					ID = id,
					HttpHostName = "https://dev.digiwells.no/",
					HttpHostBasePath = "GravitationalField/api/",
					HttpEndPoint = "GravitationalFieldCalculationOrder/",
				};
			}
		public static GravitationalData ConstructGravitationalData()
		{
			return new GravitationalData
			{
				Latitude = 0.0, 
				Longitude = 0.0, 
				Depth = 0.0, 
				GravitatyIntensityX = null, 
				GravitatyIntensityY = null, 
				GravitatyIntensityZ = null, 
			};
		}
		public static GravitationalField ConstructGravitationalField()
		{
			return new GravitationalField
			{
				MetaInfo = ConstructMetaInfo(),
				Name = "Default Name",
				Description = "Default Description",
				CreationDate = DateTimeOffset.UtcNow,
				LastModificationDate = DateTimeOffset.UtcNow,
				Type = (GravitationalFieldType)0,
				GravitationalDataTable = new List<GravitationalData>
					{
						ConstructGravitationalData(),
					},
			};
		}
		public static GravitationalFieldCalculationOrder ConstructGravitationalFieldCalculationOrder()
		{
			return new GravitationalFieldCalculationOrder
			{
				MetaInfo = ConstructMetaInfo(),
				Name = "Default Name",
				Description = "Default Description",
				CreationDate = DateTimeOffset.UtcNow,
				LastModificationDate = DateTimeOffset.UtcNow,
				RawGravitationalField = ConstructGravitationalField(),
				CompletedGravitationalField = ConstructGravitationalField(),
			};
		}
	}
}
