/*
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
*/

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Odbc;

class Arguments {
	public string ConnectionString {get;set;}
	public DateTime StartingTime {get;set;}
}

class Hammer {
	private static Arguments obj = new Arguments();

	public static void Main(string[] args)
	{
		int numThreads = int.Parse(args[0]);
		int numIterations  = int.Parse(args[1]);
		obj.ConnectionString = args[2]; //Pass the ODBC connection string by enclosing it in quotes, e.g. "DSN=My data source"

		for(int j = 1; j <= numIterations; j++) {
			Console.WriteLine(System.DateTime.Now + "\tSTART ITERATION " + j);
			obj.StartingTime = System.DateTime.Now;
			
			var tasks = new Task[numThreads];
			for (int i = 0; i < numThreads; i++)
			{
				try {
			    		tasks[i] = Task.Factory.StartNew(SqlRunner);
			    	} catch (Exception ex)
			    	{
			    		Console.WriteLine("Error running thread " + i);
			    		Console.WriteLine(ex.Message);
				}
			}
            		Task.WaitAll(tasks);
		}
		//Console.WriteLine(System.DateTime.Now);
	}


	public static void SqlRunner()
	{
	  try
	  {
	  	//Thread thread = Thread.CurrentThread;
	  	string connectionString = obj.ConnectionString;
		
		int randomVal = GetRandomNumber(0,10001); // creates a number between 0 and 10000
		
		string queryString = string.Format("select distinct {0} QUERY_ID, {1} RANDOM_VAL, COLUMN_A FROM TABLE_A", Task.CurrentId, randomVal);
		
		OdbcCommand command = new OdbcCommand(queryString);

		using (OdbcConnection conn = new OdbcConnection(connectionString))
		{
			conn.ConnectionTimeout = 120; //default is 15 seconds
			command.Connection = conn;
			conn.Open();
			using (OdbcDataReader reader = command.ExecuteReader())
			{
				while (reader.Read())
				{
					var dbVal = reader.GetValue(0);
					//string word = reader.GetString(0);
					//Console.WriteLine(word);
				}
			}
		}
		
		//Console.WriteLine(System.DateTime.Now + "\t" + thread.ManagedThreadId);
		TimeSpan ts = System.DateTime.Now - obj.StartingTime;
		Console.WriteLine(Task.CurrentId + "\t" + obj.StartingTime + "\t" + System.DateTime.Now + "\t" + ts.TotalMilliseconds);
	  }
	  catch (Exception ex)
	  {
		Console.Write(ex.Message);
	  }
	}
	
	//Function to get random number
	private static readonly Random getrandom = new Random();
	private static readonly object syncLock = new object();
	public static int GetRandomNumber(int min, int max)
	{
	    lock(syncLock) { // synchronize
	        return getrandom .Next(min, max);
	    }
	}
}

