using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using OSDC.DotnetLibraries.General.DataManagement;
using NORCE.Drilling.GravitationalField.Model;

namespace NORCE.Drilling.GravitationalField.Service.Managers
{
    /// <summary>
    /// A manager for GravitationalField. The manager implements the singleton pattern as defined by 
    /// Gamma, Erich, et al. "Design patterns: Abstraction and reuse of object-oriented design." 
    /// European Conference on Object-Oriented Programming. Springer, Berlin, Heidelberg, 1993.
    /// </summary>
    public class GravitationalFieldManager
    {
        private static GravitationalFieldManager? _instance = null;
        private readonly ILogger<GravitationalFieldManager> _logger;
        private readonly SqlConnectionManager _connectionManager;

        private GravitationalFieldManager(ILogger<GravitationalFieldManager> logger, SqlConnectionManager connectionManager)
        {
            _logger = logger;
            _connectionManager = connectionManager;
        }

        public static GravitationalFieldManager GetInstance(ILogger<GravitationalFieldManager> logger, SqlConnectionManager connectionManager)
        {
            _instance ??= new GravitationalFieldManager(logger, connectionManager);
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
                    command.CommandText = "SELECT COUNT(*) FROM GravitationalFieldTable";
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
                        _logger.LogError(ex, "Impossible to count records in the GravitationalFieldTable");
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
                    //empty GravitationalFieldTable
                    var command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM GravitationalFieldTable";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    success = true;
                }
                catch (SqliteException ex)
                {
                    transaction.Rollback();
                    _logger.LogError(ex, "Impossible to clear the GravitationalFieldTable");
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
                command.CommandText = $"SELECT COUNT(*) FROM GravitationalFieldTable WHERE ID = ' {guid}'";
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
                    _logger.LogError(ex, "Impossible to count rows from GravitationalFieldTable");
                }
            }
            else
            {
                _logger.LogWarning("Impossible to access the SQLite database");
            }
            return count >= 1;
        }

        /// <summary>
        /// Returns the list of Guid of all GravitationalField present in the microservice database 
        /// </summary>
        /// <returns>the list of Guid of all GravitationalField present in the microservice database</returns>
        public List<Guid>? GetAllGravitationalFieldId()
        {
            List<Guid> ids = [];
            var connection = _connectionManager.GetConnection();
            if (connection != null)
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT ID FROM GravitationalFieldTable";
                try
                {
                    using var reader = command.ExecuteReader();
                    while (reader.Read() && !reader.IsDBNull(0))
                    {
                        Guid id = reader.GetGuid(0);
                        ids.Add(id);
                    }
                    _logger.LogInformation("Returning the list of ID of existing records from GravitationalFieldTable");
                    return ids;
                }
                catch (SqliteException ex)
                {
                    _logger.LogError(ex, "Impossible to get IDs from GravitationalFieldTable");
                }
            }
            else
            {
                _logger.LogWarning("Impossible to access the SQLite database");
            }
            return null;
        }

        /// <summary>
        /// Returns the list of MetaInfo of all GravitationalField present in the microservice database 
        /// </summary>
        /// <returns>the list of MetaInfo of all GravitationalField present in the microservice database</returns>
        public List<MetaInfo?>? GetAllGravitationalFieldMetaInfo()
        {
            List<MetaInfo?> metaInfos = [];
            var connection = _connectionManager.GetConnection();
            if (connection != null)
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT MetaInfo FROM GravitationalFieldTable";
                try
                {
                    using var reader = command.ExecuteReader();
                    while (reader.Read() && !reader.IsDBNull(0))
                    {
                        string mInfo = reader.GetString(0);
                        MetaInfo? metaInfo = JsonSerializer.Deserialize<MetaInfo>(mInfo, JsonSettings.Options);
                        metaInfos.Add(metaInfo);
                    }
                    _logger.LogInformation("Returning the list of MetaInfo of existing records from GravitationalFieldTable");
                    return metaInfos;
                }
                catch (SqliteException ex)
                {
                    _logger.LogError(ex, "Impossible to get IDs from GravitationalFieldTable");
                }
            }
            else
            {
                _logger.LogWarning("Impossible to access the SQLite database");
            }
            return null;
        }

        /// <summary>
        /// Returns a GravitationalField identified by its Guid from the microservice database 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>the GravitationalField retrieved from the database</returns>
        public Model.GravitationalField? GetGravitationalFieldById(Guid guid)
        {
            if (!guid.Equals(Guid.Empty))
            {
                var connection = _connectionManager.GetConnection();
                if (connection != null)
                {
                    Model.GravitationalField? gravitationalField = null;
                    var command = connection.CreateCommand();
                    command.CommandText = $"SELECT GravitationalField FROM GravitationalFieldTable WHERE ID = '{guid}'";
                    try
                    {
                        using var reader = command.ExecuteReader();
                        if (reader.Read() && !reader.IsDBNull(0))
                        {
                            string data = reader.GetString(0);
                            gravitationalField = JsonSerializer.Deserialize<Model.GravitationalField>(data, JsonSettings.Options);
                            if (gravitationalField != null && gravitationalField.MetaInfo != null && !gravitationalField.MetaInfo.ID.Equals(guid))
                                throw new SqliteException("SQLite database corrupted: retrieved GravitationalField is null or has been jsonified with the wrong ID.", 1);
                        }
                        else
                        {
                            _logger.LogInformation("No GravitationalField of given ID in the database");
                            return null;
                        }
                    }
                    catch (SqliteException ex)
                    {
                        _logger.LogError(ex, "Impossible to get the GravitationalField with the given ID from GravitationalFieldTable");
                        return null;
                    }

                    // Finalizing
                    _logger.LogInformation("Returning the GravitationalField of given ID from GravitationalFieldTable");
                    return gravitationalField;
                }
                else
                {
                    _logger.LogWarning("Impossible to access the SQLite database");
                }
            }
            else
            {
                _logger.LogWarning("The given GravitationalField ID is null or empty");
            }
            return null;
        }

        /// <summary>
        /// Returns the list of all GravitationalField present in the microservice database 
        /// </summary>
        /// <returns>the list of all GravitationalField present in the microservice database</returns>
        public List<Model.GravitationalField?>? GetAllGravitationalField()
        {
            List<Model.GravitationalField?> vals = [];
            var connection = _connectionManager.GetConnection();
            if (connection != null)
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT GravitationalField FROM GravitationalFieldTable";
                try
                {
                    using var reader = command.ExecuteReader();
                    while (reader.Read() && !reader.IsDBNull(0))
                    {
                        string data = reader.GetString(0);
                        Model.GravitationalField? gravitationalField = JsonSerializer.Deserialize<Model.GravitationalField>(data, JsonSettings.Options);
                        vals.Add(gravitationalField);
                    }
                    _logger.LogInformation("Returning the list of existing GravitationalField from GravitationalFieldTable");
                    return vals;
                }
                catch (SqliteException ex)
                {
                    _logger.LogError(ex, "Impossible to get GravitationalField from GravitationalFieldTable");
                }
            }
            else
            {
                _logger.LogWarning("Impossible to access the SQLite database");
            }
            return null;
        }

 /// <summary>
        /// Returns the list of all completed GravitationalField present in the microservice database 
        /// </summary>
        /// <returns>the list of all completed GravitationalField present in the microservice database</returns>
        public List<Model.GravitationalField?>? GetAllCompletedGravitationalField(bool completedBool)
        {
            List<Model.GravitationalField?> vals = [];
            var connection = _connectionManager.GetConnection();
            if (connection != null)
            {
                string dataType = completedBool ? "Completed" : "Raw";
                var command = connection.CreateCommand();
                command.CommandText = $"SELECT GravitationalField FROM GravitationalFieldTable WHERE Type = '{dataType}'";
                try
                {
                    using var reader = command.ExecuteReader();
                    while (reader.Read() && !reader.IsDBNull(0))
                    {
                        string data = reader.GetString(0);
                        Model.GravitationalField? gravitationalField = JsonSerializer.Deserialize<Model.GravitationalField>(data, JsonSettings.Options);
                        vals.Add(gravitationalField);
                    }
                    _logger.LogInformation("Returning the list of existing GravitationalField from GravitationalFieldTable");
                    return vals;
                }
                catch (SqliteException ex)
                {
                    _logger.LogError(ex, "Impossible to get GravitationalField from GravitationalFieldTable");
                }
            }
            else
            {
                _logger.LogWarning("Impossible to access the SQLite database");
            }
            return null;
        }
        /// <summary>
        /// Adds the given GravitationalField to the microservice database
        /// </summary>
        /// <param name="gravitationalField"></param>
        /// <returns>true if the given GravitationalField has been added successfully</returns>
        public bool AddGravitationalField(Model.GravitationalField? gravitationalField)
        {
            if (gravitationalField != null && gravitationalField.MetaInfo != null && gravitationalField.MetaInfo.ID != Guid.Empty)
            {
                //update GravitationalFieldTable
                var connection = _connectionManager.GetConnection();
                if (connection != null)
                {
                    using SqliteTransaction transaction = connection.BeginTransaction();
                    bool success = true;
                    try
                    {
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
                            $"'{gravitationalField.MetaInfo.ID}', " +
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
                    catch (SqliteException ex)
                    {
                        _logger.LogError(ex, "Impossible to add the given GravitationalField into GravitationalFieldTable");
                        success = false;
                    }
                    //finalizing
                    if (success)
                    {
                        transaction.Commit();
                        _logger.LogInformation("Added the given GravitationalField of given ID into the GravitationalFieldTable successfully");
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
                _logger.LogWarning("The GravitationalField ID or the ID of its input are null or empty");
            }
            return false;
        }

        /// <summary>
        /// Performs calculation on the given GravitationalField and updates it in the microservice database
        /// </summary>
        /// <param name="gravitationalField"></param>
        /// <returns>true if the given GravitationalField has been updated successfully</returns>
        public bool UpdateGravitationalFieldById(Guid guid, Model.GravitationalField? gravitationalField)
        {
            bool success = true;
            if (guid != Guid.Empty && gravitationalField != null && gravitationalField.MetaInfo != null && gravitationalField.MetaInfo.ID == guid)
            {
                //update GravitationalFieldTable
                var connection = _connectionManager.GetConnection();
                if (connection != null)
                {
                    using SqliteTransaction transaction = connection.BeginTransaction();
                    //update fields in GravitationalFieldTable
                    try
                    {
                        string metaInfo = JsonSerializer.Serialize(gravitationalField.MetaInfo, JsonSettings.Options);
                        gravitationalField.LastModificationDate = DateTimeOffset.UtcNow;
                        string data = JsonSerializer.Serialize(gravitationalField, JsonSettings.Options);
                        var command = connection.CreateCommand();
                        command.CommandText = $"UPDATE GravitationalFieldTable SET " +
                            $"MetaInfo = '{metaInfo}', " +
                            $"GravitationalField = '{data}' " +
                            $"WHERE ID = '{guid}'";
                        int count = command.ExecuteNonQuery();
                        if (count != 1)
                        {
                            _logger.LogWarning("Impossible to update the GravitationalField");
                            success = false;
                        }
                    }
                    catch (SqliteException ex)
                    {
                        _logger.LogError(ex, "Impossible to update the GravitationalField");
                        success = false;
                    }

                    // Finalizing
                    if (success)
                    {
                        transaction.Commit();
                        _logger.LogInformation("Updated the given GravitationalField successfully");
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
                _logger.LogWarning("The GravitationalField ID or the ID of some of its attributes are null or empty");
            }
            return false;
        }

        /// <summary>
        /// Deletes the GravitationalField of given ID from the microservice database
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>true if the GravitationalField was deleted from the microservice database</returns>
        public bool DeleteGravitationalFieldById(Guid guid)
        {
            if (!guid.Equals(Guid.Empty))
            {
                var connection = _connectionManager.GetConnection();
                if (connection != null)
                {
                    using var transaction = connection.BeginTransaction();
                    bool success = true;
                    //delete GravitationalField from GravitationalFieldTable
                    try
                    {
                        var command = connection.CreateCommand();
                        command.CommandText = $"DELETE FROM GravitationalFieldTable WHERE ID = '{guid}'";
                        int count = command.ExecuteNonQuery();
                        if (count < 0)
                        {
                            _logger.LogWarning("Impossible to delete the GravitationalField of given ID from the GravitationalFieldTable");
                            success = false;
                        }
                    }
                    catch (SqliteException ex)
                    {
                        _logger.LogError(ex, "Impossible to delete the GravitationalField of given ID from GravitationalFieldTable");
                        success = false;
                    }
                    if (success)
                    {
                        transaction.Commit();
                        _logger.LogInformation("Removed the GravitationalField of given ID from the GravitationalFieldTable successfully");
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
                _logger.LogWarning("The GravitationalField ID is null or empty");
            }
            return false;
        }
    }
}