using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace Xunit
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class ExcelDataAttribute : DataAttribute
	{
		private readonly static String connectionTemplate =
				"Provider=Microsoft.ACE.OLEDB.12.0; Data Source={0}; Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';";

		public ExcelDataAttribute(String fileName, String queryString)
		{
			FileName = fileName;
			QueryString = queryString;
		}

		public String FileName { get; private set; }

		public String QueryString { get; private set; }

		public override IEnumerable<Object[]> GetData(MethodInfo testMethod)
		{
			if (testMethod == null) { throw new ArgumentNullException("testMethod"); }

			var pars = testMethod.GetParameters();
			return DataSource(FileName, QueryString, pars.Select(par => par.ParameterType).ToArray());
		}

		IEnumerable<Object[]> DataSource(String fileName, String selectString, Type[] parameterTypes)
		{
			var connectionString = String.Format(connectionTemplate, GetFullFilename(fileName));
			IDataAdapter adapter = new OleDbDataAdapter(selectString, connectionString);
			var dataSet = new DataSet();

			try
			{
				adapter.Fill(dataSet);

				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					yield return ConvertParameters(row.ItemArray, parameterTypes);
				}
			}
			finally
			{
				var disposable = adapter as IDisposable;
				disposable.Dispose();
			}
		}

		private static String GetFullFilename(String filename)
		{
			var executable = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
			return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(executable), filename));
		}

		private static Object[] ConvertParameters(Object[] values, Type[] parameterTypes)
		{
			var result = new Object[values.Length];

			for (int idx = 0; idx < values.Length; idx++)
			{
				result[idx] = ConvertParameter(values[idx], idx >= parameterTypes.Length ? null : parameterTypes[idx]);
			}

			return result;
		}

		/// <summary>Converts a parameter to its destination parameter type, if necessary.</summary>
		/// <param name="parameter">The parameter value</param>
		/// <param name="parameterType">The destination parameter type (null if not known)</param>
		/// <returns>The converted parameter value</returns>
		private static Object ConvertParameter(Object parameter, Type parameterType)
		{
			if ((parameter is Double || parameter is Single) &&
					(parameterType == typeof(Int32) || parameterType == typeof(Int32?)))
			{
				Int32 intValue;
				var floatValueAsString = parameter.ToString();

				if (Int32.TryParse(floatValueAsString, out intValue)) { return intValue; }
			}

			return parameter;
		}
	}
}