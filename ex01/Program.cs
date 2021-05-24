using System;
using System.IO;


string[] nameList = LoadNameList();

Console.WriteLine(">Enter name:");
string name = Console.ReadLine().ToLower();
if (!ValidateName(name))
	return;
if (IsNameInList(name, nameList))
	Console.WriteLine($"Hello, {NameRepresentation(name)}!");
else
{
	string option;
	string answer;

	for (int i = 0; i < nameList.Length; i++)
	{
		option = nameList[i];
		if (DistanceLevenshtein(option, name) <= 2)
		{
			while (true)
			{
				Console.WriteLine($">Did you mean \"{NameRepresentation(option)}\"? Y/N");
				answer = Console.ReadLine().ToUpper();
				if (answer == "Y")
				{
					Console.WriteLine($"Hello, {NameRepresentation(option)}!");
					return;
				}
				else if (answer == "N")
					break;
			}
		}
	}
	Console.WriteLine("Name not found.");
}

static bool ValidateName(string name)
{
	if (name == null)
		return (false);
	else if (name.Length == 0)
		return (false);
	else if (name.ToUpper() == "X AE A-12")
	{
		Console.WriteLine("There was a quick, unscheduled disassembly of the prototype.");
		return (false);
	}
	// TODO regx
	return (true);
}

static string NameRepresentation(string name)
{
	return (String.Concat(name[0].ToString().ToUpper(), name.Substring(1)));
}

static string[] LoadNameList()
{
	string[] nameList = File.ReadAllLines("us.txt");
	for (int i = 0; i < nameList.Length; i++)
	{
		nameList[i] = nameList[i].ToLower();
	}
	return (nameList);
}

static int GetLevenshteinDistance(string name, string option)
{
	int n = name.Length;
	int m = option.Length;
	int[,] d = new int[n + 1, m + 1];

	// Step 1
	if (n == 0)
	{
		return m;
	}

	if (m == 0)
	{
		return n;
	}

	// Step 2
	for (int i = 0; i <= n; d[i, 0] = i++)
	{
	}

	for (int j = 0; j <= m; d[0, j] = j++)
	{
	}

	// Step 3
	for (int i = 1; i <= n; i++)
	{
		//Step 4
		for (int j = 1; j <= m; j++)
		{
			// Step 5
			int cost = (option[j - 1] == name[i - 1]) ? 0 : 1;

			// Step 6
			d[i, j] = Math.Min(
				Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
				d[i - 1, j - 1] + cost);
		}
	}
	// Step 7
	return d[n, m];
}

static int MinAmongFreeNumbers(int x, int y, int z)
{
	if (x > y)
		x = y;
	if (x < z)
		return (x);
	return (z);
}

static int DistanceLevenshtein(string namecons, string namedict)
{
	var n = namecons.Length + 1;
	var m = namedict.Length + 1;
	var matrixDistLev = new int[n, m];

	for (var i = 0; i < n; i++)
		matrixDistLev[i, 0] = i;

	for (var j = 0; j < m; j++)
		matrixDistLev[0, j] = j;

	for (var i = 1; i < n; i++)
	{
		for (var j = 1; j < m; j++)
		{
			var changeel = 0;
			if (namecons[i - 1] == namedict[j - 1])
				changeel = 0;
			else
				changeel = 1;

			matrixDistLev[i, j] = MinAmongFreeNumbers(matrixDistLev[i - 1, j] + 1,
				matrixDistLev[i, j - 1] + 1,
				matrixDistLev[i - 1, j - 1] + changeel);
		}
	}
	return matrixDistLev[n - 1, m - 1];
}

static bool IsNameInList(string name, string[] nameList)
{
	foreach (string line in nameList)
	{
		if (name == line)
			return (true);
	}
	return (false);
}
