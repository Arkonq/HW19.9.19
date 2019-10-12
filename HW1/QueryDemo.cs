using System;
using System.Reflection;
using System.IO;
using System.Threading;

namespace Homework
{
	class QueryDemo
	{
		static string path;									// путь к проекту
		static string projectName;          // Название проекта (пригодится в дальнейшей работе)
		static void GetObjects()
		{
			int count = 0;
			for (int i = 0; i < path.Length; i++)
			{         // Расчет позиции названия проекта
				if (path[i] == '\\')
				{
					count = i;
				}
			}
			projectName = path.Remove(0, count + 1);        // Достаем название проекта, чтобы в дальнейшем 
			projectName = projectName.Replace(".exe", "");  // стереть его из нашего SQL запроса
			Assembly assembly = Assembly.LoadFile(path);
			using (StreamWriter streamWriter = new StreamWriter("Data.txt"))
			{
				foreach (var type in assembly.GetTypes())
				{
					//Console.WriteLine($"{type.BaseType} | {type.FullName}");
					streamWriter.WriteLine($"{type.BaseType} | {type.FullName}");
					int num = 0;
					foreach (var memberInfo in type.GetMembers())
					{
						num++;
						if (num > 5)
						{                           // Игнорирование системных данных, в которых нет необходимости для sql таблицы
																				//Console.WriteLine(memberInfo.ToString());
							streamWriter.WriteLine(memberInfo.ToString());
						}
					}
				}
			}
			Console.WriteLine("Data Analyzed!");
		}
		static void CreateQuery()
		{
			string[] lines = new string[1000];
			int dataSize = 0;
			string line;
			using (StreamReader sr = new StreamReader("Data.txt"))
			{
				while ((line = sr.ReadLine()) != null)
				{
					lines[dataSize] = line;
					dataSize++;
				}
			}
			using (StreamWriter streamWriter = new StreamWriter("create_db.sql"))
			{
				streamWriter.WriteLine("CREATE DATABASE Very_Unique_Database_Name; -- Дроп датабазы не получится, так как она 'используется в данный момент'");
				streamWriter.WriteLine("GO");
				streamWriter.WriteLine("USE Very_Unique_Database_Name;");
				bool firstrow = true;
				for (int i = 0; i < dataSize; i++)
				{
					if (lines[i].Contains("System.Object |"))
					{ // Если текстовый документ содержит строку
						if (i < 1)
						{ // Первый объект - class Program, поэтому пропускаем эту строку
							continue;
						}
						else
						{
							if (firstrow)
							{ // Первая строка на Create не нуждается в закрывающей скобке перед ней
								firstrow = false;
							}
							else
							{
								streamWriter.WriteLine(")");
							}
							lines[i] = lines[i].Replace("System.Object |", "CREATE TABLE"); // Замена кода на SQL
							lines[i] = lines[i].Remove("CREATE TABLE".Length + 1, projectName.Length + 1); // Затирание названия проекта, т.е. "CREATE TABLE  'Название проекта'.'Название таблицы'"
							streamWriter.WriteLine((lines[i].Replace("CREATE TABLE", "DROP TABLE IF EXISTS") + "s;"));        //											    Вот этой части   ^^^^^^^^^^^^									
							lines[i] += "s";
							streamWriter.WriteLine(lines[i]);
							streamWriter.WriteLine("(");
						}
					}
					else if (lines[i].Contains("Int32"))
					{ // Замена типа данных на SQL'овский
						string temp = lines[i].Remove(0, "Int32".Length + 1); // укорачивание строки от 0го элемента до "Int32".Length + 1 (пробел)
						temp += " int NOT NULL,"; // Добавление к строке ее типа (по синтаксису сиквела)
						streamWriter.WriteLine(temp); // Запись в файл
					}
					else if (lines[i].Contains("System.String"))
					{ // Замена типа данных на SQL'овский
						string temp = lines[i].Remove(0, "System.String".Length + 1); // укорачивание строки от 0го элемента до "System.String".Length + 1 (пробел)
						temp += " nvarchar(50) NOT NULL,"; // Добавление к строке ее типа (по синтаксису сиквела)
						streamWriter.WriteLine(temp); // Запись в файл
					}
					else if (lines[i].Contains("System.DateTime"))
					{ // Замена типа данных на SQL'овский
						string temp = lines[i].Remove(0, "System.DateTime".Length + 1); // укорачивание строки от 0го элемента до "System.DateTime".Length + 1 (пробел)
						temp += " datetime NOT NULL,"; // Добавление к строке ее типа (по синтаксису сиквела)
						streamWriter.WriteLine(temp); // Запись в файл
					}
					else if (lines[i].Contains("Double"))
					{ // Замена типа данных на SQL'овский
						string temp = lines[i].Remove(0, "Double".Length + 1); // укорачивание строки от 0го элемента до "Double".Length + 1 (пробел)
						temp += " decimal(10,8) NOT NULL,"; // Добавление к строке ее типа (по синтаксису сиквела)
						streamWriter.WriteLine(temp); // Запись в файл
					}
					else if (lines[i].Contains("Single"))
					{ // Замена типа данных на SQL'овский
						string temp = lines[i].Remove(0, "Single".Length + 1); // укорачивание строки от 0го элемента до "Single".Length + 1 (пробел)
						temp += " float NOT NULL,"; // Добавление к строке ее типа (по синтаксису сиквела)
						streamWriter.WriteLine(temp); // Запись в файл
					}
					else if (lines[i].Contains("Int64"))
					{ // Замена типа данных на SQL'овский
						string temp = lines[i].Remove(0, "Int64".Length + 1); // укорачивание строки от 0го элемента до "Int64".Length + 1 (пробел)
						temp += " bigint NOT NULL,"; // Добавление к строке ее типа (по синтаксису сиквела)
						streamWriter.WriteLine(temp); // Запись в файл
					}
					else if (lines[i].Contains("Char"))
					{ // Замена типа данных на SQL'овский
						string temp = lines[i].Remove(0, "Char".Length + 1); // укорачивание строки от 0го элемента до "Char".Length + 1 (пробел)
						temp += " char NOT NULL,"; // Добавление к строке ее типа (по синтаксису сиквела)
						streamWriter.WriteLine(temp); // Запись в файл
					}
					else
					{
						Console.WriteLine($"Не удалось создание SQL для строки '{lines[i]}'. Автор не реализовал код для вашего случая");
					}
				}
				streamWriter.WriteLine(")");
			}
			Console.WriteLine("Query To Create Created!");
		}
		static void AddData()
		{
			string[] lines = new string[1000];
			int dataSize = 0;
			string line;
			using (StreamReader sr = new StreamReader("create_db.sql"))
			{
				while ((line = sr.ReadLine()) != null)
				{
					lines[dataSize] = line;
					dataSize++;
				}
			}
			using (StreamWriter streamWriter = new StreamWriter("add_data.sql"))	// запрос на вставку данных в таблицы
			{
				for (int i = 0; i < dataSize; i++)
				{
					if (lines[i].Contains("CREATE TABLE"))
					{
						streamWriter.WriteLine((lines[i].Replace("CREATE TABLE", "INSERT INTO") + " VALUES"));
					}
					else if (lines[i].Contains("int"))
					{
						if (lines[i + 1] == ")")
						{
							streamWriter.WriteLine($"'{RandomInt()}'");
						}
						else
						{
							streamWriter.WriteLine($"'{RandomInt()}',");
						}
					}
					else if (lines[i].Contains("nvarchar"))
					{
						if (lines[i + 1] == ")")
						{
							streamWriter.WriteLine($"'{RandomString()}'");
						}
						else
						{
							streamWriter.WriteLine($"'{RandomString()}',");
						}
					}
					else if (lines[i].Contains("datetime"))
					{
						if (lines[i + 1] == ")")
						{
							streamWriter.WriteLine($"'{RandomDate()}'");
						}
						else
						{
							streamWriter.WriteLine($"'{RandomDate()}',");
						}
					}
					else if (lines[i].Contains("decimal(10,8)"))
					{
						if (lines[i + 1] == ")")
						{
							streamWriter.WriteLine($"'{RandomDouble().ToString().Replace(',', '.')}'");
						}
						else
						{
							streamWriter.WriteLine($"'{RandomDouble().ToString().Replace(',', '.')}',");
						}
					}
					else if (lines[i].Contains("float"))
					{
						if (lines[i + 1] == ")")
						{
							streamWriter.WriteLine($"'{float.Parse(RandomDouble().ToString()).ToString().Replace(',', '.')}'");
						}
						else
						{
							streamWriter.WriteLine($"'{float.Parse(RandomDouble().ToString()).ToString().Replace(',','.')}',");
						}
					}
					else if (lines[i].Contains("bigint"))
					{
						if (lines[i + 1] == ")")
						{
							streamWriter.WriteLine($"'{RandomInt()}'");
						}
						else
						{
							streamWriter.WriteLine($"'{RandomInt()}',");
						}
					}
					else if (lines[i].Contains("char"))
					{
						if (lines[i + 1] == ")")
						{
							streamWriter.WriteLine($"'{RandomChar()}'");
						}
						else
						{
							streamWriter.WriteLine($"'{RandomChar()}',");
						}
					}
					else if (lines[i].Contains("CREATE") || lines[i].Contains("DROP") || lines[i].Contains("GO") || lines[i].Contains("USE"))
					{
						continue;
					}
					else
					{
						streamWriter.WriteLine(lines[i]);
					}
				}
			}
			Console.WriteLine("Query To Add Created!");
		}
		static int RandomInt()
		{
			int baseseed = (int)DateTime.Now.Ticks; // Для более случайных данных в рандоме
			Random rnd = new Random(baseseed);
			int num = rnd.Next();
			Thread.Sleep(1);  // если данные идут подряд - переход на новый тик baseseed
			return num;
		}
		static string RandomString()
		{
			var HighChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			var LowChars = "abcdefghijklmnopqrstuvwxyz";
			var stringChars = new char[(RandomInt() % 5) + 5];
			int baseseed = (int)DateTime.Now.Ticks;
			var random = new Random(baseseed);
			stringChars[0] = HighChars[random.Next(HighChars.Length)];
			for (int i = 1; i < stringChars.Length; i++)
			{
				stringChars[i] = LowChars[random.Next(LowChars.Length)];
			}
			var finalString = new String(stringChars);
			Thread.Sleep(1);
			return finalString;
		}
		static double RandomDouble()
		{
			int baseseed = (int)DateTime.Now.Ticks;
			Random rnd = new Random(baseseed);
			double num = rnd.NextDouble();
			Thread.Sleep(1);
			return num;
		}
		static char RandomChar()
		{
			var Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
			int baseseed = (int)DateTime.Now.Ticks;
			var random = new Random(baseseed);
			char Char = (char)Chars[random.Next(Chars.Length)];
			Thread.Sleep(1);
			return Char;
		}
		static DateTime RandomDate()
		{
			int baseseed = (int)DateTime.Now.Ticks;
			Random rnd = new Random(baseseed);
			DateTime start = new DateTime(1990, 1, 1);
			int range = (DateTime.Today - start).Days;
			Thread.Sleep(1);
			return start.AddDays(rnd.Next(range));
		}
		static public void Query()
		{
			Console.WriteLine("Введите путь:");
			path = Console.ReadLine();
			GetObjects();     // Переписывание всех полученных данных в Текстовый документ
			CreateQuery();    // Анализ текстового документа, и создание из него запроса SQL
			AddData();        // Вставка данных
			Console.WriteLine("End of Program!");
			Console.Read();
		}
	}
}
