﻿// Octopus MFS is an integrated suite for managing a Micro Finance Institution: 
// clients, contracts, accounting, reporting and risk
// Copyright © 2006,2007 OCTO Technology & OXUS Development Network
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
//
// Website: http://www.opencbs.com
// Contact: contact@opencbs.com

using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using OpenCBS.Shared;

namespace OpenCBS.Manager
{
    public class OpenCbsReader : IDisposable
    {
        public readonly SqlDataReader Reader;
        private bool _disposed;

        public OpenCbsReader(SqlDataReader reader)
        {
            Reader = reader;
        }

        public bool Empty
        {
            get
            {
                return null == Reader || !Reader.HasRows;
            }
        }

        public bool Read()
        {
            Debug.Assert(Reader != null, "Reader is null");
            return Reader.Read();
        }

        public bool NextResult()
        {
            return Reader.NextResult();
        }

        public int GetInt(int num)
        {
            return Reader.GetInt32(num);
        }

        public byte GetByte(int num)
        {
            return Reader.GetByte(num);
        }

        public byte GetByte(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Column name should not be empty");
            int ordinal = Reader.GetOrdinal(name);
            return Reader.GetByte(ordinal);
        }

        private bool IsNull(string name)
        {
            var r = Reader.GetValue(Reader.GetOrdinal(name));
            return DBNull.Value == Reader.GetValue(Reader.GetOrdinal(name));
        }

        public string GetString(int num)
        {
            return Reader.GetString(num);
        }

        public byte[] GetBytes(int num)
        {
            return (byte[])Reader[0];
        }

        public byte[] GetBytes(string name)
        {
            var i = Reader.GetOrdinal(name);
            return (byte[])Reader[i];
        }

        public long GetBytes(int i, long startIndex, byte[] buffer, int p, int bufferSize)
        {
            return Reader.GetBytes(i, startIndex, buffer, p, bufferSize);
        }

        public string GetString(string name)
        {
            return IsNull(name) 
                ? null 
                : Reader.GetString(Reader.GetOrdinal(name));
        }

        public char GetChar(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Column name should not be empty");
            return Reader.GetString(Reader.GetOrdinal(name))[0];
        }

        public int GetInt(string name)
        {
            return Reader.GetInt32(Reader.GetOrdinal(name));
        }

        public int? GetNullInt(string name)
        {
            if (IsNull(name))
                return null;
            return GetInt(name);
        }

        public int? GetNullSmallInt(string name)
        {
            if (IsNull(name))
                return null;
            return GetSmallInt(name);
        }

        public int GetSmallInt(string name)
        {
            return Reader.GetInt16(Reader.GetOrdinal(name));
        }

        public bool GetBool(string name)
        {
            return Reader.GetBoolean(Reader.GetOrdinal(name));
        }

        public bool? GetNullBool(string name)
        {
            if (IsNull(name))
                return null;
            return GetBool(name);
        }

        public double GetDouble(string name)
        {
            return Reader.GetDouble(Reader.GetOrdinal(name));
        }

        public double? GetNullDouble(string name)
        {
            if (IsNull(name))
                return null;
            return GetDouble(name);
        }

        public DateTime GetDateTime(string name)
        {
            return Reader.GetDateTime(Reader.GetOrdinal(name));
        }

        public DateTime? GetNullDateTime(string name)
        {
            if (IsNull(name))
                return null;
            return GetDateTime(name);
        }

        public OCurrency GetMoney(string name)
        {
            if (IsNull(name))
                return null;
            return GetDecimal(name);
        }

        public decimal GetDecimal(string name)
        {
            return Reader.GetDecimal(Reader.GetOrdinal(name));
        }

        public string GetName(int number)
        {
            return Reader.GetName(number);
        }

        public object GetValue(int number)
        {
            return Reader.GetValue(number);
        }

        public decimal? GetNullDecimal(string name)
        {
            if (IsNull(name))
                return null;
            return GetDecimal(name);
        }

        public bool HasColumn(string pColName)
        {
            for (var i = 0; i < Reader.FieldCount; i++)
                if (Reader.GetName(i).Equals(pColName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            return false;
        }

        public int FieldCount
        {
            get { return Reader.FieldCount; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                Reader.Dispose();
            }
            _disposed = true;
        }

        public void Close()
        {
            Reader.Close();
        }
    }

    public class OpenCbsCommand : IDisposable
    {
        private readonly SqlCommand _cmd;

        public OpenCbsCommand()
        {
            _cmd = new SqlCommand();
        }

        public OpenCbsCommand(string query, SqlConnection conn)
        {
            _cmd = new SqlCommand(query, conn) {CommandTimeout = conn.ConnectionTimeout};
        }

        public OpenCbsCommand(string query, SqlConnection conn, SqlTransaction transaction)
        {
            _cmd = new SqlCommand(query, conn, transaction) {CommandTimeout = conn.ConnectionTimeout};
        }

        public void AddParam(string name, object value)
        {
            Debug.Assert(_cmd != null, "Command is null");

            if (value is OCurrency)
            {
                OCurrency val = (OCurrency) value;
                if (val.HasValue)
                {
                    _cmd.Parameters.Add(name, SqlDbType.Money);
                    _cmd.Parameters[name].Value = val.Value;
                    return;
                }
                value = null;
            }

            if(value == null)
            {
                _cmd.Parameters.Add(name, SqlDbType.NVarChar);
                _cmd.Parameters[name].Value = DBNull.Value;
            }
            else
                _cmd.Parameters.AddWithValue(name, value);
        }

        public void ResetParams()
        {
            _cmd.Parameters.Clear();
        }

        public void ExecuteAsStoredProcedure()
        {
            _cmd.CommandType = CommandType.StoredProcedure;
        }

        public OpenCbsReader ExecuteReader()
        {
            return new OpenCbsReader(_cmd.ExecuteReader());
        }

        public OpenCbsReader ExecuteReader(CommandType commandType)
        {
            _cmd.CommandType = commandType;
            return new OpenCbsReader(_cmd.ExecuteReader());
        }

        public OpenCbsReader ExecuteReader(CommandBehavior commandBehavior)
        {
            return new OpenCbsReader(_cmd.ExecuteReader(commandBehavior));
        }

        public int ExecuteNonQuery()
        {
            return _cmd.ExecuteNonQuery();
        }

        public object ExecuteScalar()
        {
            return _cmd.ExecuteScalar();
        }

        public int FieldCount
        {
            get { return FieldCount; }
        }

        public string CommandText
        {
            get
            {
                return _cmd.CommandText;
            }

            set
            {
                _cmd.CommandText = value;
            }
        }

        public int CommandTimeout
        {
            get
            {
                return _cmd.CommandTimeout;
            }
            set
            {
                _cmd.CommandTimeout = value;
            }
        }

        public SqlConnection Connection
        {
            get
            {
                return _cmd.Connection;
            }

            set
            {
                _cmd.Connection = value;
            }
        }

        public SqlTransaction Transaction
        {
            get
            {
                return _cmd.Transaction;
            }
            set
            {
                _cmd.Transaction = value;
            }
        }

        public void Dispose()
        {
            _cmd.Dispose();
        }

        public OpenCbsCommand AsStoredProcedure()
        {
            _cmd.CommandType = CommandType.StoredProcedure;
            return this;
        }

        public OpenCbsCommand With(string paramName, object paramValue)
        {
            AddParam(paramName, paramValue);
            return this;
        }

        public OpenCbsCommand WithTimeout(int timeout)
        {
            _cmd.CommandTimeout = timeout;
            return this;
        }
    }
}
