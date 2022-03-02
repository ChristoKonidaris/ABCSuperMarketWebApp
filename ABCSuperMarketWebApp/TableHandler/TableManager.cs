using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table; 

namespace ABCSuperMarketWebApp.TableHandler
{
    public class TableManager
    {
        private CloudTable table; 

        //Check if table name is blank
        public TableManager(string _CloudTableName)
        {
            if(string.IsNullOrEmpty(_CloudTableName))
            {
                throw new ArgumentException("Table", " Table name cannot be empty");
            }
            try
            {

                //Get azure table storage connection string
                string ConnectionSting = "DefaultEndpointsProtocol=https;AccountName=abcsupermarketstorageac;AccountKey=Y0C3YTA1THH6XzJeLH6rCnvwwWSnT3PQpJb5DaoCzTV76uXa92lcM7ODXIJmPumPj6WajSrAYzo7cIEE3tRNlA==;EndpointSuffix=core.windows.net";
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionSting);

                //create table if it does no exist 
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                table = tableClient.GetTableReference(_CloudTableName);
                table.CreateIfNotExists();
            }
            catch(StorageException StorageExceptionObj)
            {
                throw StorageExceptionObj;
            }
            catch (Exception ExceptionObj)
            {
                throw ExceptionObj;
            }
        }

        //Retieve Items (Get > List)
        public List<T> RetrieveEntity<T>(String Query = null) where T : TableEntity, new()
        {
            try
            {
                TableQuery<T> DataTableQuery = new TableQuery<T>();
                if (!string.IsNullOrEmpty(Query))
                {
                    DataTableQuery = new TableQuery<T>().Where(Query);
                }
                IEnumerable<T> IDataList = table.ExecuteQuery(DataTableQuery);
                List<T> DataList = new List<T>();
                foreach (var singleItem in IDataList)
                    DataList.Add(singleItem);
                return DataList;
            }
            catch (Exception ExceptionObj)
            {
                throw ExceptionObj;
            }
        }

        //Insert Item

        public void InsertEntity<T>(T entity, bool forInsert = true) where T : TableEntity, new()
        {
            try
            {
                if (forInsert)
                {
                    var InsertOperation = TableOperation.Insert(entity);
                    table.Execute(InsertOperation);
                }
                else
                {
                    var InsertOrReplaceOperation = TableOperation.InsertOrReplace(entity);
                    table.Execute(InsertOrReplaceOperation);
                }
            }
            catch (Exception ExceptionObj)
            {
                throw ExceptionObj;
            }
        }

        //Delete Item

        public bool DeleteEntity<T>(T entity) where T : TableEntity, new()
        {
            try
            {
                var DeleteOperation = TableOperation.Delete(entity);
                table.Execute(DeleteOperation);
                return true;
            }
            catch (Exception ExceptionObj)
            {
                throw ExceptionObj;
            }
        }
    }
}