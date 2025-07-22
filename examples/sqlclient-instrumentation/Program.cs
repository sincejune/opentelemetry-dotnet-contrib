// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

using System.Data.SqlClient;
using OpenTelemetry;
using OpenTelemetry.Trace;

internal class Program
    {
        private static readonly string connectionString = 
            "Server=mssql-server;Database=AdventureWorks;User Id=SA;Password=password123;Encrypt=true;TrustServerCertificate=true;";
        
        // List of SQL queries to run
        private static readonly List<string> queries = new List<string>
        {
            "SELECT * FROM Person.Address",
            "SELECT * FROM Person.Person JOIN Person.BusinessEntity ON Person.Person.BusinessEntityID = Person.BusinessEntity.BusinessEntityId JOIN Person.BusinessEntityAddress ON Person.BusinessEntityAddress.BusinessEntityID = Person.Person.BusinessEntityID",
            "SELECT * FROM Production.Product",
            "SELECT * FROM Sales.SalesOrderDetail s INNER JOIN Production.Product p ON s.ProductID = p.ProductID"
        };

        public static void Main(string[] args)
        {
            using var tracerProvider = Sdk.CreateTracerProviderBuilder()
                .AddSqlClientInstrumentation()
                .AddConsoleExporter()
                .Build();
            int cycleCount = 1;

            int sleepInterval = Convert.ToInt32(Environment.GetEnvironmentVariable("INTERVAL") ?? "5000");
            while (true)
            {
                Console.WriteLine($"Running query set for the {cycleCount} time.");
                RunQueries();
                Console.WriteLine($"Query set executed successfully for the {cycleCount} time. Waiting for {sleepInterval} milliseconds...");
                cycleCount++;
                Thread.Sleep(sleepInterval);
            }
        }

        // Function to execute SQL queries
        private static void RunQueries()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Connection to SQL Server successful.");

                    foreach (var query in queries)
                    {
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                int rowCount = 0;
                                while (reader.Read())
                                {
                                    rowCount++;
                                }
                                Console.WriteLine($"Query completed successfully. {rowCount} rows affected.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred while connecting or querying SQL Server: {ex.Message}");
                }
                finally
                {
                    connection.Close();
                    Console.WriteLine("Connection closed.");
                }
            }
        }
    }

