﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yutaka.Core.CSharp;
using Yutaka.Data;

namespace Yutaka.Code
{
	public class Scripter
	{
		#region C#
		/// <summary>
		/// Calls all code generating methods in this class and returns them as one string.
		/// </summary>
		/// <param name="columns">The list of all columns from a table.</param>
		/// <returns></returns>
		public static string GenerateAll(IList<Column> columns)
		{
			if (columns == null || columns.Count < 1)
				return "";

			var table = columns[0].TableName;
			var sb = new StringBuilder();
			sb.Append(String.Format("\t\t#region {0}{1}", table, Environment.NewLine));
			sb.Append(GenerateGetById(columns)).Append(Environment.NewLine);
			//sb.Append(GenerateSearch(columns)).Append(Environment.NewLine);
			//sb.Append(GenerateSearch(columns)).Append(Environment.NewLine);
			sb.Append(GenerateSearch(columns));
			sb.Append(String.Format("\t\t#endregion {0}{1}", table, Environment.NewLine));
			return sb.ToString();
		}

		/// <summary>
		/// Generates method for Getting the entity by Id.
		/// </summary>
		/// <param name="table">The name of the table.</param>
		/// <returns></returns>
		public static string GenerateGetById(IList<Column> columns)
		{
			var table = "";
			var idCol = "";
			var dataType = "";

			foreach (var col in columns) {
				if (col.ColumnName.Equals("Id") || col.ColumnName.Equals("UniqueId")) {
					table = col.TableName;
					idCol = col.ColumnName;
					dataType = col.DataType;
					break;
				}
			}

			var alias = table.Replace("_", "").Substring(0, 1).ToLower();

			if (dataType.Equals("int")) {
				return String.Format(
					"\t\tpublic {0} Get{0}ById(int id){3}" +
					"\t\t{{{3}" +
					"\t\t\treturn (from {1} in dbcontext.{0}s{3}" +
					"\t\t\t\t\twhere id == {1}.{2}{3}" +
					"\t\t\t\t\tselect {1}).FirstOrDefault();{3}" +
					"\t\t}}{3}", table, alias, idCol, Environment.NewLine);
			}

			return String.Format(
				"\t\tpublic {0} Get{0}ById(string id){3}" +
				"\t\t{{{3}" +
				"\t\t\treturn (from {1} in dbcontext.{0}s{3}" +
				"\t\t\t\t\twhere id.Equals({1}.{2}){3}" +
				"\t\t\t\t\tselect {1}).FirstOrDefault();{3}" +
				"\t\t}}{3}", table, alias, idCol, Environment.NewLine);
		}

		/// <summary>
		/// Generates method for searching the entity.
		/// </summary>
		/// This is WIP.
		/// <param name="table">The name of the table.</param>
		/// <returns></returns>
		public static string GenerateSearch(IList<Column> columns)
		{
			var table = "";
			var idCol = "";
			var dataType = "";

			foreach (var col in columns) {
				table = col.TableName;
				idCol = col.ColumnName;
				dataType = col.DataType;
				break;
			}

			var alias = table.Replace("_", "").Substring(0, 1).ToLower();

			return String.Format(
				"\t\tpublic IList<{0}List> Search{0}(){3}" +
				"\t\t{{{3}" +
				"\t\t\tvar query = from {1} in dbcontext.{0}Lists{3}" +
				"\t\t\t\t\t\tselect {1};{3}" +
				"{3}" +
				"\t\t\treturn query.ToList();{3}" +
				"\t\t}}{3}", table, alias, idCol, Environment.NewLine);
		}

		/// <summary>
		/// WIP: Do not use yet! Generates method for Updating.
		/// </summary>
		/// <param name="columns">The list of Columns Information.</param>
		/// <returns></returns>
		public static string GenerateTryUpdate(IList<Column> columns)
		{
			var database = "";
			var table = "";
			var alias = "";
			var idCol = "";
			var dataType = "";
			var parameters = new StringBuilder();
			var isFirstCol = true;
			var findID = true;

			foreach (var col in columns) {
				if (isFirstCol) {
					database = col.TableCatalog;
					table = col.TableName;

					if (table.Substring(0, 1).Equals("_"))
						table = table.Substring(1);

					alias = String.Format("{0}{1}", table.Substring(0, 1).ToLower(), table.Substring(1));
					isFirstCol = false;
				}

				if (findID && (col.ColumnName.Equals("Id") || col.ColumnName.Equals("UniqueId"))) {
					idCol = col.ColumnName;
					dataType = col.DataType;
					findID = false;
				}

				parameters.Append(String.Format("\t\t\t\t\tnew SqlParameter(\"@{0}\", {1}.{0}", col.ColumnName, alias));

				if (col.IsNullable)
					parameters.Append(" ?? null");

				parameters.Append(String.Format("),{0}", Environment.NewLine));
			}

			var sb = new StringBuilder();
			sb.Append(String.Format("\t\tpublic bool TryUpdate{0}({0} {1}, out string response)", table, alias)).Append(Environment.NewLine);
			sb.Append("\t\t{").Append(Environment.NewLine);
			sb.Append("\t\t\t#region Input Check").Append(Environment.NewLine);
			sb.Append("\t\t\tresponse = \"\";").Append(Environment.NewLine);
			sb.Append(Environment.NewLine);
			sb.Append(String.Format("\t\t\tif ({0} == null)", alias)).Append(Environment.NewLine);
			sb.Append(String.Format("\t\t\t\tresponse = String.Format(\"{{0}}<{0}> is null.{{1}}\", response, Environment.NewLine);", alias)).Append(Environment.NewLine);

			if (dataType.Equals("int")) {
				sb.Append(String.Format("\t\t\telse if ({0}.{1} < 1)", alias, idCol)).Append(Environment.NewLine);
				sb.Append(String.Format("\t\t\t\tresponse = String.Format(\"{{0}}{0}.{1} is invalid.{{2}}\", response, Environment.NewLine);", alias, idCol)).Append(Environment.NewLine);
			}
			else {
				sb.Append(String.Format("\t\t\telse if (String.IsNullOrWhiteSpace({0}.{1}))", alias, idCol)).Append(Environment.NewLine);
				sb.Append(String.Format("\t\t\t\tresponse = String.Format(\"{{0}}{0}.{1} is NULL or whitespace.{{2}}\", response, Environment.NewLine);", alias, idCol)).Append(Environment.NewLine);
			}

			sb.Append(Environment.NewLine);
			sb.Append("\t\t\tif (!String.IsNullOrWhiteSpace(response)) {").Append(Environment.NewLine);
			sb.Append(String.Format("\t\t\t\tresponse = String.Format(\"{{0}}Exception thrown in {0}Service.TryUpdate{1}({1} {2}, out string response).{{1}}{{1}}\", response, Environment.NewLine);", database, table, alias)).Append(Environment.NewLine);
			sb.Append("\t\t\t\treturn false;").Append(Environment.NewLine);
			sb.Append("\t\t\t}").Append(Environment.NewLine);
			sb.Append("\t\t\t#endregion Input Check").Append(Environment.NewLine);
			sb.Append(Environment.NewLine);
			sb.Append("\t\t\ttry {").Append(Environment.NewLine);
			sb.Append(String.Format("\t\t\t\tvar storProc = \"[dbo].[{0}Update]\";", table)).Append(Environment.NewLine);
			sb.Append("\t\t\t\tSqlParameter[] parameters = {").Append(Environment.NewLine);
			sb.Append(parameters);
			sb.Append("\t\t\t\t};").Append(Environment.NewLine);
			sb.Append("\t\t\t\tExecuteNonQuery(storProc, STORED_PROCEDURE, parameters);").Append(Environment.NewLine);
			sb.Append("\t\t\t\treturn true;").Append(Environment.NewLine);
			sb.Append("\t\t\t}").Append(Environment.NewLine);
			sb.Append(Environment.NewLine);
			sb.Append("\t\t\tcatch (Exception ex) {").Append(Environment.NewLine);
			sb.Append("\t\t\t\t#region Log").Append(Environment.NewLine);
			sb.Append("\t\t\t\tif (ex.InnerException == null)").Append(Environment.NewLine);
			sb.Append(String.Format("\t\t\t\t\tresponse = String.Format(\"{{0}}{{2}}Exception thrown in {0}Service.TryUpdateChatter(Chatter chatter='{{3}}', out string response).{{2}}{{1}}{{2}}{{2}}\", ex.Message, ex.ToString(), Environment.NewLine, chatter.Id);", database)).Append(Environment.NewLine);
			sb.Append("\t\t\t\telse").Append(Environment.NewLine);
			sb.Append(String.Format("\t\t\t\t\tresponse = String.Format(\"{{0}}{{2}}Exception thrown in INNER EXCEPTION of {0}Service.TryUpdateChatter(Chatter chatter='{{3}}', out string response).{{2}}{{1}}{{2}}{{2}}\", ex.InnerException.Message, ex.InnerException.ToString(), Environment.NewLine, chatter.Id);", database)).Append(Environment.NewLine);
			sb.Append(Environment.NewLine);
			sb.Append("\t\t\t\tLog(response);").Append(Environment.NewLine);
			sb.Append("\t\t\t\t#endregion Log").Append(Environment.NewLine);
			sb.Append(Environment.NewLine);
			sb.Append("\t\t\t\treturn false;").Append(Environment.NewLine);
			sb.Append("\t\t\t}").Append(Environment.NewLine);
			sb.Append("\t\t}").Append(Environment.NewLine);
			return sb.ToString();
		}
		#endregion C#

		/// <summary>
		/// Separates all columns by table, then runs all Script methods on each. Wraps each table in a #region for organization.
		/// </summary>
		/// <param name="columns">The columns.</param>
		/// <returns></returns>
		public string ScriptAll(IList<Column> columns)
		{
			#region Input Check
			var log = "";

			if (columns == null)
				log = String.Format("{0}<columns> is null.{1}", log, Environment.NewLine);
			else if (columns.Count == 0)
				log = String.Format("{0}<columns> is empty.{1}", log, Environment.NewLine);

			if (!String.IsNullOrWhiteSpace(log)) {
				log = String.Format("{0}Exception thrown in Scripter.ScriptAll(IList<Column> columns).{1}{1}", log, Environment.NewLine);
				return log;
			}
			#endregion

			var finalScript = new StringBuilder();
			var table = "";

			columns = columns.OrderBy(x => x.TableSchema).ThenBy(x => x.TableName).ThenBy(x => x.OrdinalPosition).ToList();
			var grouped = columns.GroupBy(x => new { x.TableCatalog, x.TableSchema, x.TableName }).ToList();
			var last = grouped.Last();

			foreach (var tables in grouped) {
				table = tables.Key.TableName;
				finalScript.AppendLine(String.Format("\t\t#region {0}", table));
				//finalScript.Append(ScriptTryInsertMethod(tables.ToList()));
				finalScript.Append(ScriptIndex());
				finalScript.AppendLine();
				finalScript.Append(ScriptListModel(tables.ToList()));
				finalScript.AppendLine();
				finalScript.Append(ScriptModel(tables.ToList()));
				finalScript.AppendLine(String.Format("\t\t#endregion {0}", table));

				if (!last.Equals(tables))
					finalScript.AppendLine();
			}

			return finalScript.ToString();
		}

		/// <summary>
		/// Creates script for a TryInsert method.
		/// </summary>
		/// <param name="columns">The columns information for each table.</param>
		/// <returns></returns>
		public string ScriptTryInsertMethod(IList<Column> columns)
		{
			#region Input Check
			var log = "";

			if (columns == null)
				log = String.Format("{0}<columns> is null.{1}", log, Environment.NewLine);
			else if (columns.Count == 0)
				log = String.Format("{0}<columns> is empty.{1}", log, Environment.NewLine);

			if (!String.IsNullOrWhiteSpace(log)) {
				log = String.Format("{0}Exception thrown in Scripter.ScriptTryInsertMethod(IList<Column> columns).{1}{1}", log, Environment.NewLine);
				return log;
			}
			#endregion

			var finalScript = new StringBuilder();
			var checkInputBlock = new StringBuilder();
			var tryBlock = new StringBuilder();
			var catchBlock = new StringBuilder();
			Method method = null;
			var database = "";
			var schema = "";
			var table = "";
			var alias = "";

			columns = columns.OrderBy(x => x.TableSchema).ThenBy(x => x.TableName).ThenBy(x => x.OrdinalPosition).ToList();

			foreach (var tables in columns.GroupBy(x => new { x.TableCatalog, x.TableSchema, x.TableName })) {
				database = tables.Key.TableCatalog;
				schema = tables.Key.TableSchema;
				table = tables.Key.TableName.Replace(".", "_");
				alias = table.Replace("_", "").Substring(0, 2).ToLower();

				// check for C# keywords //
				if (alias.Equals("in"))
					alias = table.Replace("_", "").Substring(0, 3).ToLower();

				#region Check Input Block
				checkInputBlock = new StringBuilder();
				checkInputBlock.AppendLine("\t\t\t#region Check Input");
				checkInputBlock.AppendLine("\t\t\tresponse = \"\";");
				checkInputBlock.AppendLine();
				checkInputBlock.AppendLine(String.Format("\t\t\tif ({0} == null)", alias));
				checkInputBlock.AppendLine(String.Format("\t\t\t\tresponse = String.Format(\"{{0}}<{0}> is required.{{1}}\", response, Environment.NewLine);", alias));
				checkInputBlock.AppendLine();
				checkInputBlock.AppendLine("\t\t\tif (!String.IsNullOrWhiteSpace(response)) {");
				checkInputBlock.AppendLine(String.Format("\t\t\t\tresponse = String.Format(\"{{0}}Exception thrown in {0}Service.TryInsert{1}({1} {2}, out string response).{{1}}\", response, Environment.NewLine); ", database, table, alias));
				checkInputBlock.AppendLine("\t\t\t\treturn false;");
				checkInputBlock.AppendLine("\t\t\t}");
				checkInputBlock.AppendLine("\t\t\t#endregion");
				#endregion Check Input Block

				#region Open Try Block
				tryBlock = new StringBuilder();
				tryBlock.AppendLine("\t\t\ttry {");
				tryBlock.AppendLine(String.Format("\t\t\t\tvar storProc = \"[dbo].[{0}Insert]\";", table));
				tryBlock.AppendLine("\t\t\t\tSqlParameter[] parameters = {");
				#endregion Open Try Block

				foreach (var col in tables) {
					if (!col.IsIdentity && !col.IsComputed) {
						tryBlock.Append(String.Format("\t\t\t\t\tnew SqlParameter(\"@{0}\", {1}.{0}", col.ColumnName, alias));

						if (col.IsNullable) {
							if (col.ColumnName.StartsWith("Delete"))
								tryBlock.AppendLine(" ?? null),");
							else if (col.DataType.Equals("bit") || col.DataType.Equals("datetime"))
								tryBlock.AppendLine(" ?? null),");
							else if (col.DataType.Equals("int") || col.DataType.Equals("decimal") || col.DataType.Equals("numeric"))
								tryBlock.AppendLine(" ?? -1),");
							else
								tryBlock.AppendLine(" ?? \"\"),");
						}

						else if (col.DataType.Equals("varchar") || col.DataType.Equals("nvarchar"))
							tryBlock.AppendLine(" ?? \"\"),");
						else
							tryBlock.AppendLine("),");
					}
				}

				#region Close Try Block
				tryBlock.AppendLine("\t\t\t\t};");
				tryBlock.AppendLine("\t\t\t\tExecuteStoredProcedure(storProc, parameters);");
				tryBlock.AppendLine("\t\t\t\treturn true;");
				tryBlock.AppendLine("\t\t\t}");
				#endregion Close Try Block

				#region Catch Block
				catchBlock = new StringBuilder();
				catchBlock.AppendLine("\t\t\tcatch (Exception ex) {");
				catchBlock.AppendLine("\t\t\t\t#region Log");
				catchBlock.AppendLine("\t\t\t\tif (ex.InnerException == null)");
				catchBlock.AppendLine(String.Format("\t\t\t\t\tresponse = String.Format(\"{{0}}{{2}}Exception thrown in {0}Service.TryInsert{1}({1} {2}, out string response).{{2}}{{1}}{{2}}{{2}}\", ex.Message, ex.ToString(), Environment.NewLine);", database, table, alias));
				catchBlock.AppendLine("\t\t\t\telse");
				catchBlock.AppendLine(String.Format("\t\t\t\t\tresponse = String.Format(\"{{0}}{{2}}Exception thrown in INNER EXCEPTION of {0}Service.TryInsert{1}({1} {2}, out string response).{{2}}{{1}}{{2}}{{2}}\", ex.InnerException.Message, ex.InnerException.ToString(), Environment.NewLine);", database, table, alias));
				catchBlock.AppendLine("\t\t\t\t#endregion Log");
				catchBlock.AppendLine();
				catchBlock.AppendLine("\t\t\t\treturn false;");
				catchBlock.AppendLine("\t\t\t}");
				#endregion Catch Block

				method = new Method {
					AccessLevel = "public",
					Body = "",
					Modifier = null,
					Name = String.Format("TryInsert{0}", table),
					Parameters = String.Format("{0} {1}, out string response", table, alias),
					ReturnType = "bool",
				};

				if (!String.IsNullOrWhiteSpace(checkInputBlock.ToString()))
					method.Body = String.Format("{0}{1}{2}", method.Body, checkInputBlock, Environment.NewLine);
				if (!String.IsNullOrWhiteSpace(tryBlock.ToString()))
					method.Body = String.Format("{0}{1}{2}", method.Body, tryBlock, Environment.NewLine);
				if (!String.IsNullOrWhiteSpace(catchBlock.ToString()))
					method.Body = String.Format("{0}{1}", method.Body, catchBlock);

				finalScript.Append(method.ToString());
			}

			return finalScript.ToString();
		}

		#region NopCommerce
		public string ScriptListModel(IList<Column> columns)
		{
			#region Input Check
			var log = "";

			if (columns == null)
				log = String.Format("{0}<columns> is null.{1}", log, Environment.NewLine);
			else if (columns.Count == 0)
				log = String.Format("{0}<columns> is empty.{1}", log, Environment.NewLine);

			if (!String.IsNullOrWhiteSpace(log)) {
				log = String.Format("{0}Exception thrown in Scripter.ScriptListModel(IList<Column> columns).{1}{1}", log, Environment.NewLine);
				return log;
			}
			#endregion

			var finalScript = new StringBuilder();
			Class cl = null;
			Field field = null;
			var table = "";

			columns = columns.OrderBy(x => x.TableSchema).ThenBy(x => x.TableName).ThenBy(x => x.OrdinalPosition).ToList();

			foreach (var tables in columns.GroupBy(x => new { x.TableCatalog, x.TableSchema, x.TableName })) {
				table = tables.Key.TableName.Replace(".", "_");

				cl = new Class {
					AccessLevel = "public",
					BaseClass = null,
					Fields = new List<Field>(),
					Methods = new List<Method>(),
					Modifier = null,
					Name = String.Format("{0}ListModel", table),
					Namespace = String.Format("Yutaka.Models.{0}s", table),
					Usings = new List<string>(),
				};

				foreach (var col in tables) {
					field = new Field {
						AccessLevel = "public",
						DisplayName = col.ColumnName,
						Getter = null,
						IsAutoImplemented = true,
						Modifier = null,
						Name = String.Format("Search{0}", col.ColumnName),
						Setter = null,
						Type = null,
						UIHint = null,
					};

					switch (col.DataType) {
						#region case "bit":
						case "bit":
							if (col.IsNullable)
								field.Type = "bool?";
							else
								field.Type = "bool";
							break;
						#endregion case "bit":
						#region case "datetime":
						case "datetime":
							if (col.IsNullable) {
								field.Type = "DateTime?";
								field.UIHint = "DateTimeNullable";
							}
							else
								field.Type = "DateTime";
							break;
						#endregion case "datetime":
						#region case "decimal" & "numeric":
						case "decimal":
						case "numeric":
							if (col.IsNullable)
								field.Type = "decimal?";
							else
								field.Type = "decimal";
							break;
						#endregion case "decimal" & "numeric":
						#region case "int":
						case "int":
							if (col.IsNullable)
								field.Type = "int?";
							else
								field.Type = "int";
							break;
						#endregion case "int":
						#region case "nvarchar" & "varchar":
						case "nvarchar":
						case "varchar":
							field.Type = "string";
							break;
						#endregion case "nvarchar" & "varchar":
						default:
							if (col.IsNullable)
								field.Type = String.Format("{0}?", col.DataType);
							else
								field.Type = col.DataType;
							break;
					}

					cl.Fields.Add(field);
				}

				finalScript.Append(cl.ToString());
			}

			return finalScript.ToString();
		}

		/// <summary>
		/// Creates script for an MVC Model.
		/// </summary>
		/// <param name="columns">The columns information for each table.</param>
		/// <returns></returns>
		public string ScriptModel(IList<Column> columns)
		{
			#region Input Check
			var log = "";

			if (columns == null)
				log = String.Format("{0}<columns> is null.{1}", log, Environment.NewLine);
			else if (columns.Count == 0)
				log = String.Format("{0}<columns> is empty.{1}", log, Environment.NewLine);

			if (!String.IsNullOrWhiteSpace(log)) {
				log = String.Format("{0}Exception thrown in Scripter.ScriptModel(IList<Column> columns).{1}{1}", log, Environment.NewLine);
				return log;
			}
			#endregion

			var finalScript = new StringBuilder();
			Class cl;
			Field field, field2;
			var table = "";

			columns = columns.OrderBy(x => x.TableSchema).ThenBy(x => x.TableName).ThenBy(x => x.OrdinalPosition).ToList();

			foreach (var tables in columns.GroupBy(x => new { x.TableCatalog, x.TableSchema, x.TableName })) {
				table = tables.Key.TableName.Replace(".", "_");

				cl = new Class {
					AccessLevel = "public",
					BaseClass = null,
					Fields = new List<Field>(),
					Methods = new List<Method>(),
					Modifier = null,
					Name = String.Format("{0}Model", table),
					Namespace = String.Format("Yutaka.Models.{0}s", table),
					Usings = new List<string>(),
				};

				foreach (var col in tables) {
					field2 = null;
					field = new Field {
						AccessLevel = "public",
						DisplayName = col.ColumnName,
						Getter = null,
						IsAutoImplemented = true,
						Modifier = null,
						Name = col.ColumnName,
						Setter = null,
						Type = null,
						UIHint = null,
					};

					switch (col.DataType) {
						#region case "bit":
						case "bit":
							if (col.IsNullable)
								field.Type = "bool?";
							else
								field.Type = "bool";
							break;
						#endregion case "bit":
						#region case "datetime" & "datetime2":
						case "datetime":
						case "datetime2":
							if (col.IsNullable) {
								field.Type = "DateTime?";
								field.UIHint = "DateTimeNullable";
							}
							else
								field.Type = "DateTime";

							if (col.ColumnName.StartsWith("CreatedOn") || col.ColumnName.StartsWith("UpdatedOn") || col.ColumnName.StartsWith("ModifiedOn") || col.ColumnName.StartsWith("DeletedOn")) {
								field.DisplayName = null;
								field2 = new Field {
									AccessLevel = "public",
									DisplayName = null,
									Getter = null,
									IsAutoImplemented = true,
									Modifier = null,
									Name = String.Format("{0}Str", col.ColumnName.Replace("Utc", "")),
									Setter = null,
									Type = "string",
									UIHint = null,
								};
							}
							break;
						#endregion case "datetime" & "datetime2":
						#region case "decimal" & "numeric":
						case "decimal":
						case "numeric":
							if (col.IsNullable)
								field.Type = "decimal?";
							else
								field.Type = "decimal";
							break;
						#endregion case "decimal" & "numeric":
						#region case "int":
						case "int":
							if (col.IsNullable)
								field.Type = "int?";
							else
								field.Type = "int";
							break;
						#endregion case "int":
						#region case "nvarchar" & "varchar":
						case "nvarchar":
						case "varchar":
							field.Type = "string";
							break;
						#endregion case "nvarchar" & "varchar":
						default:
							if (col.IsNullable)
								field.Type = String.Format("{0}?", col.DataType);
							else
								field.Type = col.DataType;
							break;
					}

					cl.Fields.Add(field);

					if (field2 != null)
						cl.Fields.Add(field2);
				}

				finalScript.Append(cl.ToString());
			}

			return finalScript.ToString();
		}

		#region Controller
		public string ScriptIndex(string currentIndentation = "\t\t")
		{
			var body = new StringBuilder();
			var method = new Method {
				AccessLevel = "public",
				Body = null,
				CurrentIndentation = currentIndentation,
				Modifier = null,
				Name = "Index",
				Parameters = null,
				ReturnType = "ActionResult",
			};

			method.IncreaseIndent();
			body.AppendFormat("{0}if (!_workContext.CurrentCustomer.IsRegistered()){1}", method.CurrentIndentation, Environment.NewLine);
			method.IncreaseIndent();
			body.AppendFormat("{0}return new HttpUnauthorizedResult();{1}", method.CurrentIndentation, Environment.NewLine);
			method.DecreaseIndent();
			body.AppendLine();
			body.AppendFormat("{0}SetSessionVariables();{1}", method.CurrentIndentation, Environment.NewLine);
			body.AppendFormat("{0}var customerId = _workContext.CurrentCustomer.Id;{1}", method.CurrentIndentation, Environment.NewLine);
			body.AppendFormat("{0}var storeId = _intranetDataService.GetStoreIdByCustomerId(customerId);{1}", method.CurrentIndentation, Environment.NewLine);
			body.AppendFormat("{0}SetStoreIdSessions(customerId, storeId);{1}", method.CurrentIndentation, Environment.NewLine);
			body.AppendFormat("{0}var currentStoreId = Session[\"CurrentStoreId\"] == null ? -1 : (int) Session[\"CurrentStoreId\"];{1}", method.CurrentIndentation, Environment.NewLine);
			body.AppendFormat("{0}ViewBag.CurrentStoreId = currentStoreId;{1}", method.CurrentIndentation, Environment.NewLine);
			body.AppendLine();
			body.AppendFormat("{0}return View();{1}", method.CurrentIndentation, Environment.NewLine);
			method.DecreaseIndent();

			method.Body = body.ToString();
			return method.ToString();
		}
		#endregion Controller
		#endregion NopCommerce
	}
}