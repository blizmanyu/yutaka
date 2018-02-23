﻿using System;
using System.Data;
using System.Data.SqlClient;

namespace Yutaka.Data
{
	public static class SqlUtil
	{
		#region Constants
		public const CommandType STORED_PROCEDURE = CommandType.StoredProcedure;
		public const CommandType TABLE_DIRECT = CommandType.TableDirect;
		public const CommandType TEXT_COMM_TYPE = CommandType.Text;
		#endregion

		#region Methods
		public static void ExecuteNonQuery(string connectionString, string commandText, CommandType commandType, params SqlParameter[] parameters)
		{
			using (var conn = new SqlConnection(connectionString)) {
				using (var cmd = new SqlCommand(commandText, conn)) {
					cmd.CommandType = commandType;
					try {
						cmd.Parameters.AddRange(parameters);
						conn.Open();
						cmd.ExecuteNonQuery();
					}

					catch (Exception ex) {
						throw ex;
					}
				}
			}
		}

		// This method should never actually be called. It is provided as an example only since the reader will get destroyed before returning. You
		// may want to use the GetData method instead.
		public static void ExecuteReader(string connectionString, string commandText, CommandType commandType, params SqlParameter[] parameters)
		{
			using (var conn = new SqlConnection(connectionString)) {
				using (var cmd = new SqlCommand(commandText, conn)) {
					cmd.CommandType = commandType;
					try {
						cmd.Parameters.AddRange(parameters);
						conn.Open();
						using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection)) {
							while (reader.Read())
								Console.WriteLine(String.Format("{0}", reader[0]));
						}
					}

					catch (Exception ex) {
						throw ex;
					}
				}
			}
		}

		public static void ExecuteScalar(string connectionString, string commandText, CommandType commandType, params SqlParameter[] parameters)
		{
			using (var conn = new SqlConnection(connectionString)) {
				using (var cmd = new SqlCommand(commandText, conn)) {
					cmd.CommandType = commandType;
					try {
						cmd.Parameters.AddRange(parameters);
						conn.Open();
						cmd.ExecuteScalar();
					}

					catch (Exception ex) {
						throw ex;
					}
				}
			}
		}

		public static DataSet GetData(string connectionString, string commandText, CommandType commandType, params SqlParameter[] parameters)
		{
			var ds = new DataSet();

			using (var conn = new SqlConnection(connectionString)) {
				using (var cmd = new SqlCommand(commandText, conn)) {
					cmd.CommandType = commandType;
					try {
						cmd.Parameters.AddRange(parameters);
						using (var adapter = new SqlDataAdapter(cmd)) {
							adapter.Fill(ds);
						}
						return ds;
					}

					catch (Exception) {
						return null;
					}
				}
			}
		}
		#endregion
	}
}