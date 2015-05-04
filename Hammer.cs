The MIT License (MIT)

Copyright (c) 2015 Miguel Barrientos

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.


using System;
using System.IO;
using System.Threading;
using System.Data.Odbc;

class Arguments {
	public string ConnectionString {get;set;}
}

class Hammer {
	private static Arguments obj = new Arguments();

	public static void Main(string[] args)
	{
		int numThreads = int.Parse(args[0]);
		obj.ConnectionString = args[1]; //Pass the ODBC connection string by enclosing it in quotes, e.g. "DSN=My data source"
		
		Console.WriteLine(System.DateTime.Now + "\tSTART");
		
		for(int i=0; i < numThreads ; i++) {
			Thread thread = new Thread(new ThreadStart(SqlRunner));
			thread.Start();
		}
		
		//Console.WriteLine(System.DateTime.Now);
	}


	public static void SqlRunner()
	{
	  try
	  {
	  	Thread thread = Thread.CurrentThread;
	  	string connectionString = obj.ConnectionString;
		string queryString = string.Format("SELECT {0} QUERY_ID, COLUMN_A FROM TABLE_A", thread.ManagedThreadId);
		OdbcCommand command = new OdbcCommand(queryString);

		using (OdbcConnection conn = new OdbcConnection(connectionString))
		{
			//conn.ConnectionTimeout = 60; //default is 15 seconds
			command.Connection = conn;
			conn.Open();
			using (OdbcDataReader reader = command.ExecuteReader())
			{
				while (reader.Read())
				{
					string word = reader.GetString(0);
					//Console.WriteLine(word);
				}
			}
		}
		
		Console.WriteLine(System.DateTime.Now + "\t" + thread.ManagedThreadId);
	  }
	  catch (Exception ex)
	  {
		Console.Write(ex.Message);
	  }
	}

}

