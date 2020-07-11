﻿using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Yutaka.Core.CSharp
{
	public class Method
	{
		#region Fields
		private static readonly string Space = " ";
		protected static readonly Regex Tab = new Regex("\t", RegexOptions.Compiled);
		public string CurrentIndentation = "";
		public string AccessLevel;
		public string Modifier;
		public string ReturnType;
		public string Name;
		public string Parameters;
		public string Body;
		#endregion Fields

		public Method(string accessLevel = null, string modifier = null, string returnType = null, string name = null, string parameters = null, string body = null)
		{
			if (String.IsNullOrWhiteSpace(accessLevel))
				AccessLevel = "public";
			else
				AccessLevel = accessLevel.Trim().ToLower();

			if (String.IsNullOrWhiteSpace(modifier))
				Modifier = null;
			else
				Modifier = modifier.Trim().ToLower();

			if (String.IsNullOrWhiteSpace(returnType))
				ReturnType = "void";
			else
				ReturnType = returnType.Trim().ToLower();

			if (String.IsNullOrWhiteSpace(name))
				Name = null;
			else
				Name = name.Trim();

			if (String.IsNullOrWhiteSpace(parameters))
				Parameters = null;
			else
				Parameters = parameters.Trim();

			if (String.IsNullOrWhiteSpace(body))
				Body = null;
			else
				Body = body.Trim();
		}

		#region Non-Public Methods
		protected void DecreaseIndent()
		{
			if (String.IsNullOrEmpty(CurrentIndentation))
				CurrentIndentation = "";
			else
				CurrentIndentation = Tab.Replace(CurrentIndentation, "", 1);
		}

		protected void IncreaseIndent()
		{
			CurrentIndentation = String.Format("{0}\t", CurrentIndentation);
		}
		#endregion Non-Public Methods

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append(Tab).Append(Tab).Append(AccessLevel).Append(Space);

			if (!String.IsNullOrWhiteSpace(Modifier))
				sb.Append(Modifier).Append(Space);

			sb.Append(ReturnType).Append(Space);
			sb.Append(Name).Append("(");
			sb.Append(Parameters).AppendLine(")");
			sb.Append(Tab).Append(Tab).AppendLine("{");
			sb.Append(Body);
			sb.Append(Tab).Append(Tab).AppendLine("}");

			return sb.ToString();
		}
	}
}