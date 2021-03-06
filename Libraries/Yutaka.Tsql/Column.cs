﻿using System;

namespace Yutaka.Data
{
	public class Column
	{
		#region Fields
		public string TableCatalog;
		public string TableSchema;
		public string TableName;
		public string ColumnName;
		public int OrdinalPosition;
		public string ColumnDefault;
		public bool IsNullable;
		public bool IsIdentity;
		public bool IsComputed;
		public string DataType;
		public string DataTypeFull
		{
			get {
				switch (DataType) {
					#region cases "decimal" & "numeric":
					case "decimal":
					case "numeric":
						return String.Format("{0}({1},{2})", DataType, NumericPrecision, NumericScale);
					#endregion
					#region cases "nvarchar" & "varchar":
					case "nvarchar":
					case "varchar":
						if (CharacterMaximumLength > 0)
							return String.Format("{0}({1})", DataType, CharacterMaximumLength);
						return String.Format("{0}(MAX)", DataType);
					#endregion
					default:
						return DataType;
				}
			}
		}
		public int CharacterMaximumLength;
		public int CharacterOctetLength;
		public int NumericPrecision;
		public int NumericPrecisionRadix;
		public int NumericScale;
		public int DatetimePrecision;
		public string CharacterSetCatalog;
		public string CharacterSetSchema;
		public string CharacterSetName;
		public string CollationCatalog;
		public string CollationSchema;
		public string CollationName;
		public string DomainCatalog;
		public string DomainSchema;
		public string DomainName;
		#endregion Fields

		#region Public Methods
		public void DumpToConsole()
		{
			Console.Write("\n");
			Console.Write("\n          TableCatalog: {0}", TableCatalog);
			Console.Write("\n           TableSchema: {0}", TableSchema);
			Console.Write("\n             TableName: {0}", TableName);
			Console.Write("\n            ColumnName: {0}", ColumnName);
			Console.Write("\n       OrdinalPosition: {0}", OrdinalPosition);
			Console.Write("\n         ColumnDefault: {0}", ColumnDefault);
			Console.Write("\n            IsNullable: {0}", IsNullable);
			Console.Write("\n            IsIdentity: {0}", IsIdentity);
			Console.Write("\n            IsComputed: {0}", IsComputed);
			Console.Write("\n              DataType: {0}", DataType);
			Console.Write("\n          DataTypeFull: {0}", DataTypeFull);
			Console.Write("\nCharacterMaximumLength: {0}", CharacterMaximumLength);
			Console.Write("\n  CharacterOctetLength: {0}", CharacterOctetLength);
			Console.Write("\n      NumericPrecision: {0}", NumericPrecision);
			Console.Write("\n NumericPrecisionRadix: {0}", NumericPrecisionRadix);
			Console.Write("\n          NumericScale: {0}", NumericScale);
			Console.Write("\n     DatetimePrecision: {0}", DatetimePrecision);
			Console.Write("\n   CharacterSetCatalog: {0}", CharacterSetCatalog);
			Console.Write("\n    CharacterSetSchema: {0}", CharacterSetSchema);
			Console.Write("\n      CharacterSetName: {0}", CharacterSetName);
			Console.Write("\n      CollationCatalog: {0}", CollationCatalog);
			Console.Write("\n       CollationSchema: {0}", CollationSchema);
			Console.Write("\n         CollationName: {0}", CollationName);
			Console.Write("\n         DomainCatalog: {0}", DomainCatalog);
			Console.Write("\n          DomainSchema: {0}", DomainSchema);
			Console.Write("\n            DomainName: {0}", DomainName);
			Console.Write("\n");
		}
		#endregion Public Methods
	}
}