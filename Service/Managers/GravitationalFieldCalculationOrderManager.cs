using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using OSDC.DotnetLibraries.General.DataManagement;
using Microsoft.Data.Sqlite;
using System.Text.Json;
using NORCE.Drilling.GravitationalField.Model;

namespace NORCE.Drilling.GravitationalField.Service.Managers
{

    /// <summary>
    /// A manager for GravitationalFieldCalculationOrder. The manager implements the singleton pattern as defined by 
    /// Gamma, Erich, et al. "Design patterns: Abstraction and reuse of object-oriented design." 
    /// European Conference on Object-Oriented Programming. Springer, Berlin, Heidelberg, 1993.
    /// </summary>
    public class GravitationalFieldCalculationOrderManager
    {
        private static GravitationalFieldCalculationOrderManager? _instance = null;
        private readonly ILogger<GravitationalFieldCalculationOrderManager> _logger;
        private readonly SqlConnectionManager _connectionManager;

        private GravitationalFieldCalculationOrderManager(ILogger<GravitationalFieldCalculationOrderManager> logger, SqlConnectionManager connectionManager)
        {
            _logger = logger;
            _connectionManager = connectionManager;
        }

        public static GravitationalFieldCalculationOrderManager GetInstance(ILogger<GravitationalFieldCalculationOrderManager> logger, SqlConnectionManager connectionManager)
        {
            _instance ??= new GravitationalFieldCalculationOrderManager(logger, connectionManager);
            return _instance;
        }

        public int Count
        {
            get
            {
                int count = 0;
                var connection = _connectionManager.GetConnection();
                if (connection != null)
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT COUNT(*) FROM GravitationalFieldCalculationOrderTable";
                    try
                    {
                        using SqliteDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            count = (int)reader.GetInt64(0);
                        }
                    }
                    catch (SqliteException ex)
                    {
                        _logger.LogError(ex, "Impossible to count records in the GravitationalFieldCalculationOrderTable");
                    }
                }
                else
                {
                    _logger.LogWarning("Impossible to access the SQLite database");
                }
                return count;
            }
        }

        public bool Clear()
        {
            var connection = _connectionManager.GetConnection();
            if (connection != null)
            {
                bool success = false;
                using var transaction = connection.BeginTransaction();
                try
                {
                    //empty GravitationalFieldCalculationOrderTable
                    var command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM GravitationalFieldCalculationOrderTable";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    success = true;
                }
                catch (SqliteException ex)
                {
                    transaction.Rollback();
                    _logger.LogError(ex, "Impossible to clear the GravitationalFieldCalculationOrderTable");
                }
                return success;
            }
            else
            {
                _logger.LogWarning("Impossible to access the SQLite database");
                return false;
            }
        }

        public bool Contains(Guid guid)
        {
            int count = 0;
            var connection = _connectionManager.GetConnection();
            if (connection != null)
            {
                var command = connection.CreateCommand();
                command.CommandText = $"SELECT COUNT(*) FROM GravitationalFieldCalculationOrderTable WHERE ID = '{guid}'";
                try
                {
                    using SqliteDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        count = (int)reader.GetInt64(0);
                    }
                }
                catch (SqliteException ex)
                {
                    _logger.LogError(ex, "Impossible to count rows from GravitationalFieldCalculationOrderTable");
                }
            }
            else
            {
                _logger.LogWarning("Impossible to access the SQLite database");
            }
            return count >= 1;
        }
        private static Model.GravitationalFieldCalculationOrderLight CreateDataLightInstance(Model.GravitationalFieldCalculationOrder gravitationalFieldCalculationOrder)
        {
            return new Model.GravitationalFieldCalculationOrderLight()
                {
                    MetaInfo = gravitationalFieldCalculationOrder.MetaInfo,
                    Name = gravitationalFieldCalculationOrder.Name,
                    Description = gravitationalFieldCalculationOrder.Description,
                    CreationDate = gravitationalFieldCalculationOrder.CreationDate,
                    LastModificationDate = gravitationalFieldCalculationOrder.LastModificationDate
                };
        }
        /// <summary>
        /// Returns the list of Guid of all GravitationalFieldCalculationOrder present in the microservice database 
        /// </summary>
        /// <returns>the list of Guid of all GravitationalFieldCalculationOrder present in the microservice database</returns>
        public List<Guid>? GetAllGravitationalFieldCalculationOrderId()
        {
            List<Guid> ids = [];
            var connection = _connectionManager.GetConnection();
            if (connection != null)
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT ID FROM GravitationalFieldCalculationOrderTable";
                try
                {
                    using var reader = command.ExecuteReader();
                    while (reader.Read() && !reader.IsDBNull(0))
                    {
                        Guid id = reader.GetGuid(0);
                        ids.Add(id);
                    }
                    _logger.LogInformation("Returning the list of ID of existing records from GravitationalFieldCalculationOrderTable");
                    return ids;
                }
                catch (SqliteException ex)
                {
                    _logger.LogError(ex, "Impossible to get IDs from GravitationalFieldCalculationOrderTable");
                }
            }
            else
            {
                _logger.LogWarning("Impossible to access the SQLite database");
            }
            return null;
        }

        /// <summary>
        /// Returns the list of MetaInfo of all GravitationalFieldCalculationOrder present in the microservice database 
        /// </summary>
        /// <returns>the list of MetaInfo of all GravitationalFieldCalculationOrder present in the microservice database</returns>
        public List<MetaInfo?>? GetAllGravitationalFieldCalculationOrderMetaInfo()
        {
            List<MetaInfo?> metaInfos = new();
            var connection = _connectionManager.GetConnection();
            if (connection != null)
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT MetaInfo FROM GravitationalFieldCalculationOrderTable";
                try
                {
                    using var reader = command.ExecuteReader();
                    while (reader.Read() && !reader.IsDBNull(0))
                    {
                        string mInfo = reader.GetString(0);
                        MetaInfo? metaInfo = JsonSerializer.Deserialize<MetaInfo>(mInfo, JsonSettings.Options);
                        metaInfos.Add(metaInfo);
                    }
                    _logger.LogInformation("Returning the list of MetaInfo of existing records from GravitationalFieldCalculationOrderTable");
                    return metaInfos;
                }
                catch (SqliteException ex)
                {
                    _logger.LogError(ex, "Impossible to get IDs from GravitationalFieldCalculationOrderTable");
                }
            }
            else
            {
                _logger.LogWarning("Impossible to access the SQLite database");
            }
            return null;
        }

        /// <summary>
        /// Returns the GravitationalFieldCalculationOrder identified by its Guid from the microservice database 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>the GravitationalFieldCalculationOrder identified by its Guid from the microservice database</returns>
        public Model.GravitationalFieldCalculationOrder? GetGravitationalFieldCalculationOrderById(Guid guid)
        {
            if (!guid.Equals(Guid.Empty))
            {
                var connection = _connectionManager.GetConnection();
                if (connection != null)
                {
                    Model.GravitationalFieldCalculationOrder? gravitationalFieldCalculationOrder;
                    var command = connection.CreateCommand();
                    command.CommandText = $"SELECT GravitationalFieldCalculationOrder FROM GravitationalFieldCalculationOrderTable WHERE ID = '{guid}'";
                    try
                    {
                        using var reader = command.ExecuteReader();
                        if (reader.Read() && !reader.IsDBNull(0))
                        {
                            string data = reader.GetString(0);
                            gravitationalFieldCalculationOrder = JsonSerializer.Deserialize<Model.GravitationalFieldCalculationOrder>(data, JsonSettings.Options);
                            if (gravitationalFieldCalculationOrder != null && gravitationalFieldCalculationOrder.MetaInfo != null && !gravitationalFieldCalculationOrder.MetaInfo.ID.Equals(guid))
                                throw new SqliteException("SQLite database corrupted: returned GravitationalFieldCalculationOrder is null or has been jsonified with the wrong ID.", 1);
                        }
                        else
                        {
                            _logger.LogInformation("No GravitationalFieldCalculationOrder of given ID in the database");
                            return null;
                        }
                    }
                    catch (SqliteException ex)
                    {
                        _logger.LogError(ex, "Impossible to get the GravitationalFieldCalculationOrder with the given ID from GravitationalFieldCalculationOrderTable");
                        return null;
                    }
                    _logger.LogInformation("Returning the GravitationalFieldCalculationOrder of given ID from GravitationalFieldCalculationOrderTable");
                    return gravitationalFieldCalculationOrder;
                }
                else
                {
                    _logger.LogWarning("Impossible to access the SQLite database");
                }
            }
            else
            {
                _logger.LogWarning("The given GravitationalFieldCalculationOrder ID is null or empty");
            }
            return null;
        }

        /// <summary>
        /// Returns the list of all GravitationalFieldCalculationOrder present in the microservice database 
        /// </summary>
        /// <returns>the list of all GravitationalFieldCalculationOrder present in the microservice database</returns>
        public List<Model.GravitationalFieldCalculationOrder?>? GetAllGravitationalFieldCalculationOrder()
        {
            List<Model.GravitationalFieldCalculationOrder?> vals = [];
            var connection = _connectionManager.GetConnection();
            if (connection != null)
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT GravitationalFieldCalculationOrder FROM GravitationalFieldCalculationOrderTable";
                try
                {
                    using var reader = command.ExecuteReader();
                    while (reader.Read() && !reader.IsDBNull(0))
                    {
                        string data = reader.GetString(0);
                        Model.GravitationalFieldCalculationOrder? gravitationalFieldCalculationOrder = JsonSerializer.Deserialize<Model.GravitationalFieldCalculationOrder>(data, JsonSettings.Options);
                        vals.Add(gravitationalFieldCalculationOrder);
                    }
                    _logger.LogInformation("Returning the list of existing GravitationalFieldCalculationOrder from GravitationalFieldCalculationOrderTable");
                    return vals;
                }
                catch (SqliteException ex)
                {
                    _logger.LogError(ex, "Impossible to get GravitationalFieldCalculationOrder from GravitationalFieldCalculationOrderTable");
                }
            }
            else
            {
                _logger.LogWarning("Impossible to access the SQLite database");
            }
            return null;
        }

        /// <summary>
        /// Returns the list of all GravitationalFieldCalculationOrderLight present in the microservice database 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>the list of GravitationalFieldCalculationOrderLight present in the microservice database</returns>
        public List<Model.GravitationalFieldCalculationOrderLight>? GetAllGravitationalFieldCalculationOrderLight()
        {
            List<Model.GravitationalFieldCalculationOrderLight>? gravitationalFieldCalculationOrderLightList = [];
            var connection = _connectionManager.GetConnection();
            if (connection != null)
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT MetaInfo, GravitationalFieldCalculationOrderLight FROM GravitationalFieldCalculationOrderTable";
                try
                {
                    using var reader = command.ExecuteReader();
                    while (reader.Read() && !reader.IsDBNull(0))
                    {
                        string metaInfoStr = reader.GetString(0);
                        MetaInfo? metaInfo = JsonSerializer.Deserialize<MetaInfo>(metaInfoStr, JsonSettings.Options);
                        Model.GravitationalFieldCalculationOrderLight? gravitationalFieldCalculationOrderLight = JsonSerializer.Deserialize<Model.GravitationalFieldCalculationOrderLight>(reader.GetString(1), JsonSettings.Options);
                        if (gravitationalFieldCalculationOrderLight != null)
                        {
                            gravitationalFieldCalculationOrderLightList.Add(gravitationalFieldCalculationOrderLight);                            
                        }
                    }
                    _logger.LogInformation("Returning the list of existing GravitationalFieldCalculationOrderLight from GravitationalFieldCalculationOrderTable");
                    return gravitationalFieldCalculationOrderLightList;
                }
                catch (SqliteException ex)
                {
                    _logger.LogError(ex, "Impossible to get light datas from GravitationalFieldCalculationOrderTable");
                }
            }
            else
            {
                _logger.LogWarning("Impossible to access the SQLite database");
            }
            return null;
        }
        private bool AddCompletedGravitationalField(
            SqliteConnection connection,
            GravitationalFieldCalculationOrder gravitationalFieldCalculationOrder)
        {
            bool success = false;
            try
            {
                if (gravitationalFieldCalculationOrder!.CompletedGravitationalField == null)
                {
                    success = false;
                    _logger.LogWarning("Impossible to insert the given GravitationalField into the GravitationalFieldTable");                             
                }
                else
                {  
                    Model.GravitationalField gravitationalField = gravitationalFieldCalculationOrder.CompletedGravitationalField;
                    //add the GravitationalField to the GravitationalFieldTable
                    string metaInfo = JsonSerializer.Serialize(gravitationalField.MetaInfo, JsonSettings.Options);
                    string data = JsonSerializer.Serialize(gravitationalField, JsonSettings.Options);
                    string type = gravitationalField.Type == GravitationalFieldType.Raw ? "Raw" : "Completed";
                    var command = connection.CreateCommand();
                    command.CommandText = "INSERT INTO GravitationalFieldTable (" +
                        "ID, " +
                        "MetaInfo, " +
                        "Type," +
                        "GravitationalField" +
                        ") VALUES (" +
                        $"'{gravitationalField.MetaInfo!.ID}', " +
                        $"'{metaInfo}', " +
                        $"'{type}', " +
                        $"'{data}'" +
                        ")";
                    int count = command.ExecuteNonQuery();
                    if (count != 1)
                    {
                        _logger.LogWarning("Impossible to insert the given GravitationalField into the GravitationalFieldTable");
                        success = false;
                    }    
                }                                                                                  
            }
            catch (SqliteException ex)
            {
                _logger.LogError(ex, "Impossible to add the given completed Gravitational Field  into GravitationalFieldTable");
                success = false;                            
            }
            
            return success;
        }
        /// <summary>
        /// Performs calculation on the given GravitationalFieldCalculationOrder and adds it to the microservice database
        /// </summary>
        /// <param name="gravitationalFieldCalculationOrder"></param>
        /// <returns>true if the given GravitationalFieldCalculationOrder has been added successfully to the microservice database</returns>
        public bool AddGravitationalFieldCalculationOrder(Model.GravitationalFieldCalculationOrder? gravitationalFieldCalculationOrder)
        {
            if (gravitationalFieldCalculationOrder != null && gravitationalFieldCalculationOrder.MetaInfo != null && gravitationalFieldCalculationOrder.MetaInfo.ID != Guid.Empty)
            {
                //calculate outputs
                if (!gravitationalFieldCalculationOrder.Calculate())
                {
                    _logger.LogWarning("Impossible to calculate outputs for the given GravitationalFieldCalculationOrder");
                    return false;
                }

                //if successful, check if another parent data with the same ID was calculated/added during the calculation time            
                Model.GravitationalFieldCalculationOrder? newGravitationalFieldCalculationOrder = GetGravitationalFieldCalculationOrderById(gravitationalFieldCalculationOrder.MetaInfo.ID);
                if (newGravitationalFieldCalculationOrder == null)
                {
                    //update GravitationalFieldCalculationOrderTable
                    var connection = _connectionManager.GetConnection();
                    if (connection != null)
                    {
                        using SqliteTransaction transaction = connection.BeginTransaction();
                        bool success = true;
                        try
                        {
                            //add the GravitationalFieldCalculationOrder to the GravitationalFieldCalculationOrderTable
                            string metaInfo = JsonSerializer.Serialize(gravitationalFieldCalculationOrder.MetaInfo, JsonSettings.Options);
                      
                            Model.GravitationalFieldCalculationOrderLight gravitationalFieldCalculationOrderLight = CreateDataLightInstance(gravitationalFieldCalculationOrder);
                            string dataLight = JsonSerializer.Serialize(gravitationalFieldCalculationOrderLight, JsonSettings.Options);                           

                            string? cDate = null;
                            if (gravitationalFieldCalculationOrder.CreationDate != null)
                                cDate = ((DateTimeOffset)gravitationalFieldCalculationOrder.CreationDate).ToString(SqlConnectionManager.DATE_TIME_FORMAT);
                            string? lDate = null;
                            if (gravitationalFieldCalculationOrder.LastModificationDate != null)
                                lDate = ((DateTimeOffset)gravitationalFieldCalculationOrder.LastModificationDate).ToString(SqlConnectionManager.DATE_TIME_FORMAT);
                            string data = JsonSerializer.Serialize(gravitationalFieldCalculationOrder, JsonSettings.Options);
                            
                            var command = connection.CreateCommand();
                            command.CommandText = "INSERT INTO GravitationalFieldCalculationOrderTable (" +
                                "ID, " +
                                "MetaInfo, " +
                                "GravitationalFieldCalculationOrderLight, " +                                
                                "CreationDate, " +
                                "LastModificationDate, " +
                                "GravitationalFieldCalculationOrder" +
                                ") VALUES (" +
                                $"'{gravitationalFieldCalculationOrder.MetaInfo.ID}', " +
                                $"'{metaInfo}', " +
                                $"'{dataLight}', " +
                                $"'{cDate}', " +
                                $"'{lDate}', " +
                                $"'{data}'" +
                                ")";
                            int count = command.ExecuteNonQuery();
                            if (count != 1)
                            {
                                _logger.LogWarning("Impossible to insert the given GravitationalFieldCalculationOrder into the GravitationalFieldCalculationOrderTable");
                                success = false;
                            }
                            AddCompletedGravitationalField(connection, gravitationalFieldCalculationOrder);
                        }
                        catch (SqliteException ex)
                        {
                            _logger.LogError(ex, "Impossible to add the given GravitationalFieldCalculationOrder into GravitationalFieldCalculationOrderTable");
                            success = false;
                        }
                        //finalizing SQL transaction
                        if (success)
                        {
                            transaction.Commit();
                            _logger.LogInformation("Added the given GravitationalFieldCalculationOrder of given ID into the GravitationalFieldCalculationOrderTable successfully");
                        }
                        else
                        {
                            transaction.Rollback();
                        }
                        return success;
                    }
                    else
                    {
                        _logger.LogWarning("Impossible to access the SQLite database");
                    }
                }
                else
                {
                    _logger.LogWarning("Impossible to post GravitationalFieldCalculationOrder. ID already found in database.");
                    return false;
                }

            }
            else
            {
                _logger.LogWarning("The GravitationalFieldCalculationOrder ID or the ID of its input are null or empty");
            }
            return false;
        }

        /// <summary>
        /// Performs calculation on the given GravitationalFieldCalculationOrder and updates it in the microservice database
        /// </summary>
        /// <param name="gravitationalFieldCalculationOrder"></param>
        /// <returns>true if the given GravitationalFieldCalculationOrder has been updated successfully</returns>
        public bool UpdateGravitationalFieldCalculationOrderById(Guid guid, Model.GravitationalFieldCalculationOrder? gravitationalFieldCalculationOrder)
        {
            bool success = true;
            if (guid != Guid.Empty && gravitationalFieldCalculationOrder != null && gravitationalFieldCalculationOrder.MetaInfo != null && gravitationalFieldCalculationOrder.MetaInfo.ID == guid)
            {
                //calculate outputs
                if (!gravitationalFieldCalculationOrder.Calculate())
                {
                    _logger.LogWarning("Impossible to calculate outputs of the given GravitationalFieldCalculationOrder");
                    return false;
                }
                //update GravitationalFieldCalculationOrderTable
                var connection = _connectionManager.GetConnection();
                if (connection != null)
                {
                    using SqliteTransaction transaction = connection.BeginTransaction();
                    //update fields in GravitationalFieldCalculationOrderTable
                    try
                    {
                        string metaInfo = JsonSerializer.Serialize(gravitationalFieldCalculationOrder.MetaInfo, JsonSettings.Options);
                        Model.GravitationalFieldCalculationOrderLight gravitationalFieldCalculationOrderLight = CreateDataLightInstance(gravitationalFieldCalculationOrder);
                        string dataLight = JsonSerializer.Serialize(gravitationalFieldCalculationOrderLight, JsonSettings.Options);                           
                        string? cDate = null;
                        if (gravitationalFieldCalculationOrder.CreationDate != null)
                            cDate = ((DateTimeOffset)gravitationalFieldCalculationOrder.CreationDate).ToString(SqlConnectionManager.DATE_TIME_FORMAT);
                        gravitationalFieldCalculationOrder.LastModificationDate = DateTimeOffset.UtcNow;
                        string? lDate = ((DateTimeOffset)gravitationalFieldCalculationOrder.LastModificationDate).ToString(SqlConnectionManager.DATE_TIME_FORMAT);
                        string data = JsonSerializer.Serialize(gravitationalFieldCalculationOrder, JsonSettings.Options);
                        var command = connection.CreateCommand();
                        command.CommandText = $"UPDATE GravitationalFieldCalculationOrderTable SET " +
                            $"MetaInfo = '{metaInfo}', " +
                            $"GravitationalFieldCalculationOrderLight = '{dataLight}', " +                              
                            $"CreationDate = '{cDate}', " +
                            $"LastModificationDate = '{lDate}', " +
                            $"GravitationalFieldCalculationOrder = '{data}' " +
                            $"WHERE ID = '{guid}'";
                        int count = command.ExecuteNonQuery();
                        if (count != 1)
                        {
                            _logger.LogWarning("Impossible to update the GravitationalFieldCalculationOrder");
                            success = false;
                        }
                        AddCompletedGravitationalField(connection, gravitationalFieldCalculationOrder);
                    }
                    catch (SqliteException ex)
                    {
                        _logger.LogError(ex, "Impossible to update the GravitationalFieldCalculationOrder");
                        success = false;
                    }

                    // Finalizing
                    if (success)
                    {
                        transaction.Commit();
                        _logger.LogInformation("Updated the given GravitationalFieldCalculationOrder successfully");
                        return true;
                    }
                    else
                    {
                        transaction.Rollback();
                    }
                }
                else
                {
                    _logger.LogWarning("Impossible to access the SQLite database");
                }
            }
            else
            {
                _logger.LogWarning("The GravitationalFieldCalculationOrder ID or the ID of some of its attributes are null or empty");
            }
            return false;
        }

        /// <summary>
        /// Deletes the GravitationalFieldCalculationOrder of given ID from the microservice database
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>true if the GravitationalFieldCalculationOrder was deleted from the microservice database</returns>
        public bool DeleteGravitationalFieldCalculationOrderById(Guid guid)
        {
            if (!guid.Equals(Guid.Empty))
            {
                var connection = _connectionManager.GetConnection();
                if (connection != null)
                {
                    using var transaction = connection.BeginTransaction();
                    bool success = true;
                    //delete GravitationalFieldCalculationOrder from GravitationalFieldCalculationOrderTable
                    try
                    {
                        var command = connection.CreateCommand();
                        command.CommandText = $"DELETE FROM GravitationalFieldCalculationOrderTable WHERE ID = '{guid}'";
                        int count = command.ExecuteNonQuery();
                        if (count < 0)
                        {
                            _logger.LogWarning("Impossible to delete the GravitationalFieldCalculationOrder of given ID from the GravitationalFieldCalculationOrderTable");
                            success = false;
                        }
                    }
                    catch (SqliteException ex)
                    {
                        _logger.LogError(ex, "Impossible to delete the GravitationalFieldCalculationOrder of given ID from GravitationalFieldCalculationOrderTable");
                        success = false;
                    }
                    if (success)
                    {
                        transaction.Commit();
                        _logger.LogInformation("Removed the GravitationalFieldCalculationOrder of given ID from the GravitationalFieldCalculationOrderTable successfully");
                    }
                    else
                    {
                        transaction.Rollback();
                    }
                    return success;
                }
                else
                {
                    _logger.LogWarning("Impossible to access the SQLite database");
                }
            }
            else
            {
                _logger.LogWarning("The GravitationalFieldCalculationOrder ID is null or empty");
            }
            return false;
        }
    }
}