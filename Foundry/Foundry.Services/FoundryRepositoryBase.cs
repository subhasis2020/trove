using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public class FoundryRepositoryBase<T> : IFoundryRepositoryBase<T> where T : class
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        /// <summary>
        /// Manager Qry Constructor.
        /// </summary>
        public IPartsQryGenerator<T> partsQryGenerator { private get; set; }
        /// <summary>
        /// Manager to worker with Identified Fields
        /// </summary>
        public IIdentityInspector<T> identityInspector { private get; set; }

        /// <summary>
        /// Idetenfier parameter (@) to SqlServer (:) to Oracle
        /// </summary>
        public char ParameterIdentified { get; set; }
        protected string qrySelect { get; set; }
        protected string qryInsert { get; set; }
        private string identityField;


        /// <summary>
        /// Create a Generic FoundryRepository
        /// </summary>
        /// <param name="conn">Connection</param>
        /// <param name="parameterIdentified">Idetenfier parameter (@) to SqlServer (:) to Oracle</param>
        public FoundryRepositoryBase(IDatabaseConnectionFactory dbConnections, char parameterIdentified = '@')
        {
            _databaseConnectionFactory = dbConnections;
            ParameterIdentified = parameterIdentified;
            partsQryGenerator = new PartsQryGenerator<T>(ParameterIdentified);

            identityInspector = new IdentityInspector<T>(_databaseConnectionFactory.CreateConnection());
        }

        #region Create Queries

        /// <summary>
        /// Query to create select statement for SQL
        /// </summary>
        protected virtual void CreateSelectQry()
        {
            if (string.IsNullOrWhiteSpace(qrySelect))
            {
                qrySelect = partsQryGenerator.GenerateSelect();
            }
        }

        protected virtual void CreateSelectMaxQry()
        {
            if (string.IsNullOrWhiteSpace(qrySelect))
            {
                identityField = identityInspector.GetColumnsIdentityForType();
                qrySelect = partsQryGenerator.GenerateMaxSelect(identityField);
            }
        }

        /// <summary>
        /// Query to create insert statement for SQL
        /// </summary>
        protected virtual void CreateInsertQry()
        {
            if (string.IsNullOrWhiteSpace(qryInsert))
            {
                identityField = identityInspector.GetColumnsIdentityForType();

                qryInsert = partsQryGenerator.GeneratePartInsert(identityField);
            }
        }

        #endregion

        #region All/Async


        /// <summary>
        /// Get all entities of a table
        /// </summary>
        /// <returns>All entities of a table</returns>
        public async Task<IEnumerable<T>> AllAsync()
        {
            CreateSelectQry();
            IEnumerable<T> result = null;
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    result = await sqlConnection.QueryAsync<T>(qrySelect);

                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
        }


        #endregion

        #region GetData/Async

        /// <summary>
        /// Get entities from a database based on query and condition  
        /// </summary>
        /// <param name="query">SQL query which needs to be executed</param>
        /// <param name="filterParameters">parameter to match the condition</param>
        /// <returns>entities which matches the condition and query</returns>
        public async Task<IEnumerable<T>> GetDataAsync(string query, object filterParameters)
        {
            ParameterValidator.ValidateString(query, nameof(query));
            ParameterValidator.ValidateObject(filterParameters, nameof(filterParameters));
            IEnumerable<T> result = null;
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    result = await sqlConnection.QueryAsync<T>(query, filterParameters);

                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
        }


        /// <summary>
        /// Get entities which matches the condition based on filter parameters
        /// </summary>
        /// <param name="filterParameters">parameter to match the condition</param>
        /// <returns>entities which matches the condition based on parameter</returns>
        public async Task<IEnumerable<T>> GetDataAsync(object filterParameters)
        {
            ParameterValidator.ValidateObject(filterParameters, nameof(filterParameters));

            var selectQry = partsQryGenerator.GenerateSelect(filterParameters);
            IEnumerable<T> result = null;
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    result = await sqlConnection.QueryAsync<T>(selectQry, filterParameters);

                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
        }

        /// <summary>
        /// Get the single entity based on parameter
        /// </summary>
        /// <param name="filterParameters">parameter to match the condition</param>
        /// <returns>entity which matches the condition based on parameter</returns>
        public async Task<T> GetDataByIdAsync(object filterParameters)
        {
            ParameterValidator.ValidateObject(filterParameters, nameof(filterParameters));

            var selectQry = partsQryGenerator.GenerateSelect(filterParameters);
            T result = null;
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    result = await sqlConnection.QueryFirstOrDefaultAsync<T>(selectQry, filterParameters);

                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
        }

        public async Task<T> GetSingleDataByConditionAsync(object filterParameters)
        {
            ParameterValidator.ValidateObject(filterParameters, nameof(filterParameters));
            T result = null;
            var selectQry = partsQryGenerator.GenerateSelect(filterParameters);
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    result = await sqlConnection.QuerySingleOrDefaultAsync<T>(selectQry, filterParameters);

                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
        }

        public async Task<T> GetSingleDataAsync(string query, object filterParameters)
        {
            ParameterValidator.ValidateString(query, nameof(query));
            ParameterValidator.ValidateObject(filterParameters, nameof(filterParameters));
            T result = null;
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    result = await sqlConnection.QuerySingleOrDefaultAsync<T>(query, filterParameters);

                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
        }

        public async Task<IEnumerable<T>> GetMultipleDataByConditionAsync(object filterParameters)
        {
            ParameterValidator.ValidateObject(filterParameters, nameof(filterParameters));
            IEnumerable<T> result = null;
            var selectQry = partsQryGenerator.GenerateSelect(filterParameters);
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    result = await sqlConnection.QueryAsync<T>(selectQry, filterParameters);

                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }

        }
        #endregion

        #region Find/Async

        /// <summary>
        /// Finding an entity in given table based on condition
        /// </summary>
        /// <param name="filterParameters">parameter to match the condition</param>
        /// <returns>entity which matches the condition</returns>
        public async Task<T> FindAsync(object filterParameters)
        {
            ParameterValidator.ValidateObject(filterParameters, nameof(filterParameters));
            T result = null;
            var selectQry = partsQryGenerator.GenerateSelect(filterParameters);
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    result = (await sqlConnection.QuerySingleOrDefaultAsync<T>(selectQry, filterParameters));

                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }

            }

        }

        #endregion

        #region Add/Async
        /// <summary>
        /// Add Async a new Entity in DB
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>number of changes in DB</returns>
        public async Task<int> AddAsync(T entity)
        {
            CreateInsertQry();
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = await sqlConnection.QueryFirstOrDefaultAsync<int>(qryInsert, entity);
                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }

            }
        }


        /// <summary>
        /// Add Async a Sequence of items in BulkInsert (DP)
        /// </summary>
        /// <param name="entities">Sequence of entities to insert</param>
        /// <returns>number of changes in DB</returns>
        public async Task<int> AddAsync(IEnumerable<T> entities)
        {
            ParameterValidator.ValidateEnumerable(entities, nameof(entities));
            CreateInsertQry();
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = await sqlConnection.ExecuteAsync(qryInsert, entities);
                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
        }

        #endregion

        #region Remove/Async

        /// <summary>
        /// Removing(Soft deletion) an entity based on provided parameter
        /// </summary>
        /// <param name="filterParameters">parameter to match the condition</param>
        /// <returns>number of entities removed</returns>
        public async Task<int> RemoveAsync(object filterParameters)
        {
            ParameterValidator.ValidateObject(filterParameters, nameof(filterParameters));
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var deleteQry = partsQryGenerator.GenerateDelete(filterParameters);
                    var result = await sqlConnection.QueryFirstOrDefaultAsync<int>(deleteQry, filterParameters);
                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }

            }
        }
        public async Task<int> DeleteEntityAsync(object filterParameters)
        {
            ParameterValidator.ValidateObject(filterParameters, nameof(filterParameters));
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var deleteQry = partsQryGenerator.GenerateDeleteEntity(filterParameters);
                    var result = await sqlConnection.ExecuteAsync(deleteQry, filterParameters);
                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }

            }
        }

        #endregion

        #region Update/Async
        /// <summary>
        /// Updating an entity for given table and entity value based on parameter.
        /// </summary>
        /// <param name="entity">table entity to update</param>
        /// <param name="filterParameters">parameter to match the condition</param>
        /// <returns>number of entities updated</returns>
        public async Task<int> UpdateAsync(T entity, object filterParameters)
        {
            ParameterValidator.ValidateObject(entity, nameof(entity));
            ParameterValidator.ValidateObject(filterParameters, nameof(filterParameters));

            var updateQry = partsQryGenerator.GenerateUpdate(filterParameters);
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = await sqlConnection.QueryFirstOrDefaultAsync<int>(updateQry, entity);
                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
        }


        /// <summary>
        /// Updating multiple entities for given table and entity value based on parameter.
        /// </summary>
        /// <param name="entity">table entity to update</param>
        /// <param name="filterParameters">parameter to match the condition</param>
        /// <returns>number of entities updated</returns>
        public async Task<int> UpdateMultipleAsync(string query, object filterParameters)
        {
            ParameterValidator.ValidateObject(query, nameof(query));
            ParameterValidator.ValidateObject(filterParameters, nameof(filterParameters));

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = await sqlConnection.ExecuteAsync(query, filterParameters);
                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
        }

        #endregion

        #region InsertOrUpdate/Async

        /// <summary>
        /// Query to insert or update an entity based on condition check
        /// </summary>result = await AddAsync(entity)
        /// <param name="entity">entity to be updated</param>
        /// <param name="filterParameters">parameter to match the specified condition</param>
        /// <returns></returns>
        public async Task<int> InsertOrUpdateAsync(T entity, object filterParameters)
        {
            ParameterValidator.ValidateObject(entity, nameof(entity));
            ParameterValidator.ValidateObject(filterParameters, nameof(filterParameters));
            int result = 0;
            var entityInTable = await FindAsync(filterParameters).ConfigureAwait(false);
            if (entityInTable == null)
            {
                result = await AddAsync(entity).ConfigureAwait(false);
            }
            else
            {
                result = await UpdateAsync(entity, filterParameters).ConfigureAwait(false);
            }
            return result;
        }

        /// <summary>
        /// Finding an entity in given table based on condition
        /// </summary>
        /// <param name="filterParameters">parameter to match the condition</param>
        /// <returns>entity which matches the condition</returns>
        public async Task<int> GetPrimaryMaxAsync()
        {
            CreateSelectMaxQry();
            int result = 0;
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    result = (await sqlConnection.QuerySingleOrDefaultAsync<int>(qrySelect));
                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }

            }

        }

        #endregion

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
