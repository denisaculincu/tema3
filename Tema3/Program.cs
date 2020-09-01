using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Data;

namespace Tema3
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			int[,] harta = new int[10, 10] {
				{0,0,0,0,0,0,0,0,0,0 },
				{0,0,0,0,0,0,0,0,0,0 },
				{0,0,0,0,0,0,0,0,0,0 },
				{0,0,0,0,0,0,0,0,0,0 },
				{0,0,0,0,0,0,0,0,0,0 },
				{0,0,0,0,0,0,0,0,0,0 },
				{0,0,0,0,0,0,0,0,0,0 },
				{0,0,0,0,0,0,0,0,0,0 },
				{0,0,0,0,0,0,0,0,0,0 },
				{0,0,0,0,0,0,0,0,0,0 },
			};

			OleDbConnection con = new OleDbConnection("Provider = Microsoft.ACE.OLEDB.12.0; Data Source = \"D:\\Denisa\\programare idk\\Tema3\\Tema3\\Database2.accdb\"");

			while (true)
			{
				AfiseazaHarta(harta, con);
				Console.WriteLine("Move (WASD), E to exit");
				var key=Console.ReadLine();
				if(key!="w" && key!="a" && key != "s" && key != "d" && key != "W" && key != "A" && key != "S" && key != "D")
				{
					if(key=="e" || key=="E")
					{
						Environment.Exit(0);
					}
					else
					{
						Console.WriteLine("Key is not valid");
					}
				}
				else
				{
					Move(key, con, harta);
				}
			}
		}

		public static List<int> GetObjectPosition(OleDbConnection con)
		{
			List<int> poz = new List<int>();

			string queryString = "SELECT X,Y FROM [OBJECT] WHERE [ID]=1";
			OleDbCommand command = new OleDbCommand(queryString, con);
			con.Open();

			OleDbDataReader reader = command.ExecuteReader();

			while (reader.Read())
			{
				poz.Add(reader.GetInt32(0));
				poz.Add(reader.GetInt32(1));
			}
			reader.Close();

			con.Close();

			return poz;
		}

		public static void AfiseazaHarta(int[,] harta, OleDbConnection con)
		{
			int x, y;
			List<int> poz = GetObjectPosition(con);
			x = poz[0];
			y = poz[1];

			for(int i = 0; i < harta.GetLength(1); i++)
			{
				for(int j = 0; j < harta.GetLength(0); j++)
				{
					if (i == y && j == x)
						Console.Write(1+" ");
					else
						Console.Write(harta[i,j]+" ");
				}
				Console.WriteLine("");
			}
		}

		public static void Move(string key, OleDbConnection con, int[,] harta)
		{
			int x, y;
			List<int> poz = GetObjectPosition(con);
			x = poz[0];
			y = poz[1];

			con.Open();
			switch (key)
			{
				case "W":
				case "w":
					if (y - 1 < 0)
					{
						Console.WriteLine("Invalid move");
						con.Close();
					}
					else
					{
						int newY = y - 1;
						string updateString = "UPDATE [object] SET y=? WHERE ID=1;";
						OleDbCommand commandUpdate = new OleDbCommand(updateString, con);
						commandUpdate.CommandType = CommandType.Text;
						commandUpdate.Parameters.AddWithValue("y", newY);
						commandUpdate.ExecuteNonQuery();
						con.Close();
						addMoveToDb(x, newY, con);
					}
					break;
				case "S":
				case "s":
					if (y + 1 >= harta.GetLength(1))
					{
						Console.WriteLine("Invalid move");
						con.Close();
					}
					else
					{
						int newY = y + 1;
						string updateString = "UPDATE [object] SET y=?;";
						OleDbCommand commandUpdate = new OleDbCommand(updateString, con);
						commandUpdate.CommandType = CommandType.Text;
						commandUpdate.Parameters.AddWithValue("y", newY);
						commandUpdate.ExecuteNonQuery();
						con.Close();
						addMoveToDb(x, newY, con);
					}
					break;
				case "A":
				case "a":
					if (x-1<0)
					{
						Console.WriteLine("Invalid move");
						con.Close();
					}
					else
					{
						int newX = x-1;
						string updateString = "UPDATE [object] SET x=?;";
						OleDbCommand commandUpdate = new OleDbCommand(updateString, con);
						commandUpdate.CommandType = CommandType.Text;
						commandUpdate.Parameters.AddWithValue("x", newX);
						commandUpdate.ExecuteNonQuery();
						con.Close();
						addMoveToDb(newX, y, con);
					}
					break;
				case "D":
				case "d":
					if (x + 1 >= harta.GetLength(0))
					{
						Console.WriteLine("Invalid move");
						con.Close();
					}
					else
					{
						int newX = x + 1;
						string updateString = "UPDATE [object] SET x=?;";
						OleDbCommand commandUpdate = new OleDbCommand(updateString, con);
						commandUpdate.CommandType = CommandType.Text;
						commandUpdate.Parameters.AddWithValue("x", newX);
						commandUpdate.ExecuteNonQuery();
						con.Close();
						addMoveToDb(newX, y, con);
					}
					break;
				default:
					Console.WriteLine("Something went wrong");
					con.Close();
					break;
			}		
		}

		public static void addMoveToDb(int x, int y, OleDbConnection con)
		{
			string insertString = "Insert into Mutare (X,Y) Values(?,?)";
			OleDbCommand command = new OleDbCommand(insertString, con);
			command.CommandType = CommandType.Text;
			command.Parameters.AddWithValue("X", x);
			command.Parameters.AddWithValue("Y", y);

			con.Open();

			command.ExecuteNonQuery();

			con.Close();
		}
	}
}
