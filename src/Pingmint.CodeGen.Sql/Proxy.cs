using System.Data;
using Microsoft.Data.SqlClient;

namespace Pingmint.CodeGen.Sql;

public static partial class Proxy
{
    private static SqlParameter CreateParameter(String parameterName, Object? value, SqlDbType sqlDbType, Int32 size = -1, ParameterDirection direction = ParameterDirection.Input) => new SqlParameter(parameterName, value ?? DBNull.Value)
    {
        Size = size,
        Direction = direction,
        SqlDbType = sqlDbType,
    };

    private static SqlParameter CreateParameter(String parameterName, Object? value, SqlDbType sqlDbType, String typeName, Int32 size = -1, ParameterDirection direction = ParameterDirection.Input) => new SqlParameter(parameterName, value ?? DBNull.Value)
    {
        Size = size,
        Direction = direction,
        TypeName = typeName,
        SqlDbType = sqlDbType,
    };

    private static T? GetField<T>(SqlDataReader reader, int ordinal) where T : class => reader.IsDBNull(ordinal) ? null : reader.GetFieldValue<T>(ordinal);
    private static T? GetFieldValue<T>(SqlDataReader reader, int ordinal) where T : struct => reader.IsDBNull(ordinal) ? null : reader.GetFieldValue<T>(ordinal);
    private static T GetNonNullField<T>(SqlDataReader reader, int ordinal) where T : class => reader.IsDBNull(ordinal) ? throw new NullReferenceException() : reader.GetFieldValue<T>(ordinal);
    private static T GetNonNullFieldValue<T>(SqlDataReader reader, int ordinal) where T : struct => reader.IsDBNull(ordinal) ? throw new NullReferenceException() : reader.GetFieldValue<T>(ordinal);

    private static SqlCommand CreateStatement(SqlConnection connection, String text) => new() { Connection = connection, CommandType = CommandType.Text, CommandText = text, };
    private static SqlCommand CreateStoredProcedure(SqlConnection connection, String text) => new() { Connection = connection, CommandType = CommandType.StoredProcedure, CommandText = text, };

	public static async Task<List<TempDb.echo_1Row>> echo_1Async(SqlConnection connection)
	{
		using SqlCommand cmd = CreateStoredProcedure(connection, "pingmint.echo_1");

		var result = new List<TempDb.echo_1Row>();
		using var reader = await cmd.ExecuteReaderAsync();
		if (await reader.ReadAsync())
		{
			int ordOne = reader.GetOrdinal("one");

			do
			{
				result.Add(new TempDb.echo_1Row
				{
					One = GetNonNullFieldValue<Int32>(reader, ordOne),
				});
			} while (await reader.ReadAsync());
		}
		return result;
	}

	public static async Task<List<TempDb.echo_textRow>> echo_textAsync(SqlConnection connection, String text)
	{
		using SqlCommand cmd = CreateStoredProcedure(connection, "pingmint.echo_text");

		cmd.Parameters.Add(CreateParameter("@text", text, SqlDbType.VarChar));

		var result = new List<TempDb.echo_textRow>();
		using var reader = await cmd.ExecuteReaderAsync();
		if (await reader.ReadAsync())
		{
			int ordText = reader.GetOrdinal("text");

			do
			{
				result.Add(new TempDb.echo_textRow
				{
					Text = GetField<String>(reader, ordText),
				});
			} while (await reader.ReadAsync());
		}
		return result;
	}

	public static async Task<List<TempDb.GetSysTypesRow>> GetSysTypesAsync(SqlConnection connection)
	{
		using SqlCommand cmd = CreateStatement(connection, "SELECT name FROM sys.types");

		var result = new List<TempDb.GetSysTypesRow>();
		using var reader = await cmd.ExecuteReaderAsync();
		if (await reader.ReadAsync())
		{
			int ordName = reader.GetOrdinal("name");

			do
			{
				result.Add(new TempDb.GetSysTypesRow
				{
					Name = GetNonNullField<String>(reader, ordName),
				});
			} while (await reader.ReadAsync());
		}
		return result;
	}

	public static async Task<List<TempDb.GetTableTypesRow>> GetTableTypesAsync(SqlConnection connection)
	{
		using SqlCommand cmd = CreateStatement(connection, "SELECT T.name, T.type_table_object_id, S.name as [Schema_Name] FROM sys.table_types AS T INNER JOIN sys.schemas as S ON (T.schema_id = S.schema_id) ORDER BY S.name, T.name");

		var result = new List<TempDb.GetTableTypesRow>();
		using var reader = await cmd.ExecuteReaderAsync();
		if (await reader.ReadAsync())
		{
			int ordName = reader.GetOrdinal("name");
			int ordTypeTableObjectId = reader.GetOrdinal("type_table_object_id");
			int ordSchemaName = reader.GetOrdinal("Schema_Name");

			do
			{
				result.Add(new TempDb.GetTableTypesRow
				{
					Name = GetNonNullField<String>(reader, ordName),
					TypeTableObjectId = GetNonNullFieldValue<Int32>(reader, ordTypeTableObjectId),
					SchemaName = GetNonNullField<String>(reader, ordSchemaName),
				});
			} while (await reader.ReadAsync());
		}
		return result;
	}

	public static async Task<List<TempDb.GetObsoleteProceduresRow>> GetObsoleteProceduresAsync(SqlConnection connection)
	{
		using SqlCommand cmd = CreateStatement(connection, "SELECT E.name, CAST(E.value as VARCHAR(MAX)) AS [value], E.major_id FROM sys.procedures AS P INNER JOIN sys.extended_properties AS E ON (P.object_id = E.major_id) WHERE E.name = 'Obsolete'");

		var result = new List<TempDb.GetObsoleteProceduresRow>();
		using var reader = await cmd.ExecuteReaderAsync();
		if (await reader.ReadAsync())
		{
			int ordName = reader.GetOrdinal("name");
			int ordValue = reader.GetOrdinal("value");
			int ordMajorId = reader.GetOrdinal("major_id");

			do
			{
				result.Add(new TempDb.GetObsoleteProceduresRow
				{
					Name = GetNonNullField<String>(reader, ordName),
					Value = GetField<String>(reader, ordValue),
					MajorId = GetNonNullFieldValue<Int32>(reader, ordMajorId),
				});
			} while (await reader.ReadAsync());
		}
		return result;
	}

	public static async Task<List<TempDb.GetProcGenHintProceduresRow>> GetProcGenHintProceduresAsync(SqlConnection connection, String hintName)
	{
		using SqlCommand cmd = CreateStatement(connection, "SELECT CAST(E.value as VARCHAR(MAX)) AS [value], E.major_id FROM sys.procedures AS P INNER JOIN sys.extended_properties AS E ON (P.object_id = E.major_id) WHERE E.name = @hint_name");

		cmd.Parameters.Add(CreateParameter("@hint_name", hintName, SqlDbType.VarChar));

		var result = new List<TempDb.GetProcGenHintProceduresRow>();
		using var reader = await cmd.ExecuteReaderAsync();
		if (await reader.ReadAsync())
		{
			int ordValue = reader.GetOrdinal("value");
			int ordMajorId = reader.GetOrdinal("major_id");

			do
			{
				result.Add(new TempDb.GetProcGenHintProceduresRow
				{
					Value = GetField<String>(reader, ordValue),
					MajorId = GetNonNullFieldValue<Int32>(reader, ordMajorId),
				});
			} while (await reader.ReadAsync());
		}
		return result;
	}

	public static async Task<List<TempDb.GetExtendedPropertiesForProcedureRow>> GetExtendedPropertiesForProcedureAsync(SqlConnection connection, String schema, String proc)
	{
		using SqlCommand cmd = CreateStatement(connection, "SELECT CAST(e.value as varchar(max)) as [value] FROM sys.fn_listextendedproperty('ProcGen_ReinterpretColumns', 'SCHEMA', @schema, 'PROCEDURE', @proc, NULL, NULL) AS E");

		cmd.Parameters.Add(CreateParameter("@schema", schema, SqlDbType.VarChar));
		cmd.Parameters.Add(CreateParameter("@proc", proc, SqlDbType.VarChar));

		var result = new List<TempDb.GetExtendedPropertiesForProcedureRow>();
		using var reader = await cmd.ExecuteReaderAsync();
		if (await reader.ReadAsync())
		{
			int ordValue = reader.GetOrdinal("value");

			do
			{
				result.Add(new TempDb.GetExtendedPropertiesForProcedureRow
				{
					Value = GetField<String>(reader, ordValue),
				});
			} while (await reader.ReadAsync());
		}
		return result;
	}

	public static async Task<List<TempDb.GetProcedureForSchemaRow>> GetProcedureForSchemaAsync(SqlConnection connection, String schema, String proc)
	{
		using SqlCommand cmd = CreateStatement(connection, "SELECT P.name, P.object_id, S.name as [Schema_Name] FROM sys.procedures AS P INNER JOIN sys.schemas as S ON (P.schema_id = S.schema_id) WHERE S.name = @schema ANd P.name = @proc ORDER BY P.name");

		cmd.Parameters.Add(CreateParameter("@schema", schema, SqlDbType.VarChar));
		cmd.Parameters.Add(CreateParameter("@proc", proc, SqlDbType.VarChar));

		var result = new List<TempDb.GetProcedureForSchemaRow>();
		using var reader = await cmd.ExecuteReaderAsync();
		if (await reader.ReadAsync())
		{
			int ordName = reader.GetOrdinal("name");
			int ordObjectId = reader.GetOrdinal("object_id");
			int ordSchemaName = reader.GetOrdinal("Schema_Name");

			do
			{
				result.Add(new TempDb.GetProcedureForSchemaRow
				{
					Name = GetNonNullField<String>(reader, ordName),
					ObjectId = GetNonNullFieldValue<Int32>(reader, ordObjectId),
					SchemaName = GetNonNullField<String>(reader, ordSchemaName),
				});
			} while (await reader.ReadAsync());
		}
		return result;
	}

	public static async Task<List<TempDb.GetProceduresForSchemaRow>> GetProceduresForSchemaAsync(SqlConnection connection, String schema)
	{
		using SqlCommand cmd = CreateStatement(connection, "SELECT P.name, P.object_id, S.name as [Schema_Name] FROM sys.procedures AS P INNER JOIN sys.schemas as S ON (P.schema_id = S.schema_id) WHERE S.name = @schema ORDER BY P.name");

		cmd.Parameters.Add(CreateParameter("@schema", schema, SqlDbType.VarChar));

		var result = new List<TempDb.GetProceduresForSchemaRow>();
		using var reader = await cmd.ExecuteReaderAsync();
		if (await reader.ReadAsync())
		{
			int ordName = reader.GetOrdinal("name");
			int ordObjectId = reader.GetOrdinal("object_id");
			int ordSchemaName = reader.GetOrdinal("Schema_Name");

			do
			{
				result.Add(new TempDb.GetProceduresForSchemaRow
				{
					Name = GetNonNullField<String>(reader, ordName),
					ObjectId = GetNonNullFieldValue<Int32>(reader, ordObjectId),
					SchemaName = GetNonNullField<String>(reader, ordSchemaName),
				});
			} while (await reader.ReadAsync());
		}
		return result;
	}

	public static async Task<List<TempDb.DmDescribeFirstResultSetForObjectRow>> DmDescribeFirstResultSetForObjectAsync(SqlConnection connection, Int32 objectid)
	{
		using SqlCommand cmd = CreateStatement(connection, "SELECT D.name, D.system_type_id, D.is_nullable, D.column_ordinal, T.name as [Type_Name] FROM sys.dm_exec_describe_first_result_set_for_object(@objectid, NULL) AS D JOIN sys.types AS T ON (D.system_type_id = T.system_type_id AND T.user_type_id = D.system_type_id) ORDER BY D.column_ordinal");

		cmd.Parameters.Add(CreateParameter("@objectid", objectid, SqlDbType.Int));

		var result = new List<TempDb.DmDescribeFirstResultSetForObjectRow>();
		using var reader = await cmd.ExecuteReaderAsync();
		if (await reader.ReadAsync())
		{
			int ordName = reader.GetOrdinal("name");
			int ordSystemTypeId = reader.GetOrdinal("system_type_id");
			int ordIsNullable = reader.GetOrdinal("is_nullable");
			int ordColumnOrdinal = reader.GetOrdinal("column_ordinal");
			int ordTypeName = reader.GetOrdinal("Type_Name");

			do
			{
				result.Add(new TempDb.DmDescribeFirstResultSetForObjectRow
				{
					Name = GetField<String>(reader, ordName),
					SystemTypeId = GetFieldValue<Int32>(reader, ordSystemTypeId),
					IsNullable = GetFieldValue<Boolean>(reader, ordIsNullable),
					ColumnOrdinal = GetFieldValue<Int32>(reader, ordColumnOrdinal),
					TypeName = GetNonNullField<String>(reader, ordTypeName),
				});
			} while (await reader.ReadAsync());
		}
		return result;
	}

	public static async Task<List<TempDb.DmDescribeFirstResultSetRow>> DmDescribeFirstResultSetAsync(SqlConnection connection, String text)
	{
		using SqlCommand cmd = CreateStatement(connection, "SELECT D.name, D.system_type_id, D.is_nullable, D.column_ordinal, T.name as [type_name] FROM sys.dm_exec_describe_first_result_set(@text, NULL, NULL) AS D JOIN sys.types AS T ON (D.system_type_id = T.system_type_id AND T.user_type_id = D.system_type_id) ORDER BY D.column_ordinal");

		cmd.Parameters.Add(CreateParameter("@text", text, SqlDbType.VarChar));

		var result = new List<TempDb.DmDescribeFirstResultSetRow>();
		using var reader = await cmd.ExecuteReaderAsync();
		if (await reader.ReadAsync())
		{
			int ordName = reader.GetOrdinal("name");
			int ordSystemTypeId = reader.GetOrdinal("system_type_id");
			int ordIsNullable = reader.GetOrdinal("is_nullable");
			int ordColumnOrdinal = reader.GetOrdinal("column_ordinal");
			int ordTypeName = reader.GetOrdinal("type_name");

			do
			{
				result.Add(new TempDb.DmDescribeFirstResultSetRow
				{
					Name = GetField<String>(reader, ordName),
					SystemTypeId = GetFieldValue<Int32>(reader, ordSystemTypeId),
					IsNullable = GetFieldValue<Boolean>(reader, ordIsNullable),
					ColumnOrdinal = GetFieldValue<Int32>(reader, ordColumnOrdinal),
					TypeName = GetNonNullField<String>(reader, ordTypeName),
				});
			} while (await reader.ReadAsync());
		}
		return result;
	}

	public static async Task<List<TempDb.GetSysTypeRow>> GetSysTypeAsync(SqlConnection connection, Int32 id)
	{
		using SqlCommand cmd = CreateStatement(connection, "SELECT system_type_id, is_table_type, name FROM sys.types where system_type_id = @id");

		cmd.Parameters.Add(CreateParameter("@id", id, SqlDbType.Int));

		var result = new List<TempDb.GetSysTypeRow>();
		using var reader = await cmd.ExecuteReaderAsync();
		if (await reader.ReadAsync())
		{
			int ordSystemTypeId = reader.GetOrdinal("system_type_id");
			int ordIsTableType = reader.GetOrdinal("is_table_type");
			int ordName = reader.GetOrdinal("name");

			do
			{
				result.Add(new TempDb.GetSysTypeRow
				{
					SystemTypeId = GetNonNullFieldValue<Byte>(reader, ordSystemTypeId),
					IsTableType = GetNonNullFieldValue<Boolean>(reader, ordIsTableType),
					Name = GetNonNullField<String>(reader, ordName),
				});
			} while (await reader.ReadAsync());
		}
		return result;
	}

	public static async Task<List<TempDb.GetParametersForObjectRow>> GetParametersForObjectAsync(SqlConnection connection, Int32 id)
	{
		using SqlCommand cmd = CreateStatement(connection, "SELECT P.parameter_id, P.system_type_id, P.name, P.is_output, P.max_length, T.is_table_type, T.name as [Type_Name] FROM sys.parameters AS P JOIN sys.types AS T ON (P.system_type_id = T.system_type_id AND P.user_type_id = T.user_type_id) WHERE P.object_id = @id");

		cmd.Parameters.Add(CreateParameter("@id", id, SqlDbType.Int));

		var result = new List<TempDb.GetParametersForObjectRow>();
		using var reader = await cmd.ExecuteReaderAsync();
		if (await reader.ReadAsync())
		{
			int ordParameterId = reader.GetOrdinal("parameter_id");
			int ordSystemTypeId = reader.GetOrdinal("system_type_id");
			int ordName = reader.GetOrdinal("name");
			int ordIsOutput = reader.GetOrdinal("is_output");
			int ordMaxLength = reader.GetOrdinal("max_length");
			int ordIsTableType = reader.GetOrdinal("is_table_type");
			int ordTypeName = reader.GetOrdinal("Type_Name");

			do
			{
				result.Add(new TempDb.GetParametersForObjectRow
				{
					ParameterId = GetNonNullFieldValue<Int32>(reader, ordParameterId),
					SystemTypeId = GetNonNullFieldValue<Byte>(reader, ordSystemTypeId),
					Name = GetField<String>(reader, ordName),
					IsOutput = GetNonNullFieldValue<Boolean>(reader, ordIsOutput),
					MaxLength = GetNonNullFieldValue<Int16>(reader, ordMaxLength),
					IsTableType = GetNonNullFieldValue<Boolean>(reader, ordIsTableType),
					TypeName = GetNonNullField<String>(reader, ordTypeName),
				});
			} while (await reader.ReadAsync());
		}
		return result;
	}

	public static async Task<List<TempDb.GetTableTypeColumnsRow>> GetTableTypeColumnsAsync(SqlConnection connection, Int32 id)
	{
		using SqlCommand cmd = CreateStatement(connection, "SELECT C.is_nullable, C.max_length, C.name, t.name as [Type_Name] from sys.columns as C join sys.types T ON (C.system_type_id = T.system_type_id) where C.object_id = @id and t.name <> 'sysname' order by c.column_id");

		cmd.Parameters.Add(CreateParameter("@id", id, SqlDbType.Int));

		var result = new List<TempDb.GetTableTypeColumnsRow>();
		using var reader = await cmd.ExecuteReaderAsync();
		if (await reader.ReadAsync())
		{
			int ordIsNullable = reader.GetOrdinal("is_nullable");
			int ordMaxLength = reader.GetOrdinal("max_length");
			int ordName = reader.GetOrdinal("name");
			int ordTypeName = reader.GetOrdinal("Type_Name");

			do
			{
				result.Add(new TempDb.GetTableTypeColumnsRow
				{
					IsNullable = GetFieldValue<Boolean>(reader, ordIsNullable),
					MaxLength = GetNonNullFieldValue<Int16>(reader, ordMaxLength),
					Name = GetField<String>(reader, ordName),
					TypeName = GetNonNullField<String>(reader, ordTypeName),
				});
			} while (await reader.ReadAsync());
		}
		return result;
	}

	public partial class TempDb
	{
		public partial class DmDescribeFirstResultSetForObjectRow
		{
			public String? Name { get; set; }
			public Int32? SystemTypeId { get; set; }
			public Boolean? IsNullable { get; set; }
			public Int32? ColumnOrdinal { get; set; }
			public String TypeName { get; set; }
		}

		public partial class DmDescribeFirstResultSetRow
		{
			public String? Name { get; set; }
			public Int32? SystemTypeId { get; set; }
			public Boolean? IsNullable { get; set; }
			public Int32? ColumnOrdinal { get; set; }
			public String TypeName { get; set; }
		}

		public partial class echo_1Row
		{
			public Int32 One { get; set; }
		}

		public partial class echo_textRow
		{
			public String? Text { get; set; }
		}

		public partial class GetExtendedPropertiesForProcedureRow
		{
			public String? Value { get; set; }
		}

		public partial class GetObsoleteProceduresRow
		{
			public String Name { get; set; }
			public String? Value { get; set; }
			public Int32 MajorId { get; set; }
		}

		public partial class GetParametersForObjectRow
		{
			public Int32 ParameterId { get; set; }
			public Byte SystemTypeId { get; set; }
			public String? Name { get; set; }
			public Boolean IsOutput { get; set; }
			public Int16 MaxLength { get; set; }
			public Boolean IsTableType { get; set; }
			public String TypeName { get; set; }
		}

		public partial class GetProcedureForSchemaRow
		{
			public String Name { get; set; }
			public Int32 ObjectId { get; set; }
			public String SchemaName { get; set; }
		}

		public partial class GetProceduresForSchemaRow
		{
			public String Name { get; set; }
			public Int32 ObjectId { get; set; }
			public String SchemaName { get; set; }
		}

		public partial class GetProcGenHintProceduresRow
		{
			public String? Value { get; set; }
			public Int32 MajorId { get; set; }
		}

		public partial class GetSysTypeRow
		{
			public Byte SystemTypeId { get; set; }
			public Boolean IsTableType { get; set; }
			public String Name { get; set; }
		}

		public partial class GetSysTypesRow
		{
			public String Name { get; set; }
		}

		public partial class GetTableTypeColumnsRow
		{
			public Boolean? IsNullable { get; set; }
			public Int16 MaxLength { get; set; }
			public String? Name { get; set; }
			public String TypeName { get; set; }
		}

		public partial class GetTableTypesRow
		{
			public String Name { get; set; }
			public Int32 TypeTableObjectId { get; set; }
			public String SchemaName { get; set; }
		}

	}
}
