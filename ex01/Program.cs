using System;
using System.IO;
using System.Text.RegularExpressions;

string[] nameList = LoadNameList();

Console.WriteLine(">Enter name:");
string name = Console.ReadLine().ToLower();
if (!ValidateName(name))
	return;
if (IsNameInList(name, nameList))
	Console.WriteLine($"Hello, {NameRepresentation(name)}!");
else
{
	for (int i = 0; i < nameList.Length; i++)
	{
		string option = nameList[i];
		if (GetLevenshteinDistance(name, option) <= 3)
		{
			while (true)
			{
				Console.WriteLine($">Did you mean \"{NameRepresentation(option)}\"? Y/N");
				string answer = Console.ReadLine().ToUpper();
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

bool ValidateName(string name)
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
	Regex mask = new Regex("[^a-zA-Z -]");
	if (mask.IsMatch(name))
	{
		Console.WriteLine("The name can only contain letters, spaces and dashes..");
		return (false);
	}
	return (true);
}

string NameRepresentation(string name)
{
	return (String.Concat(name[0].ToString().ToUpper(), name.Substring(1)));
}

string[] LoadNameList()
{
	string[] nameList = File.ReadAllLines("us.txt");
	for (int i = 0; i < nameList.Length; i++)
	{
		nameList[i] = nameList[i].ToLower();
	}
	return (nameList);
}

// Да, понимаю что это не ASCII упорядоченное расстяное, но я болею :p
int GetLevenshteinDistance(string name, string option)
{
	int n = name.Length;
	int m = option.Length;
	int[,] matrix = new int[n + 1, m + 1];

	if (n == 0)
		return m;
	if (m == 0)
		return n;

	for (int i = 0; i <= n; matrix[i, 0] = i++) {}
	for (int j = 0; j <= m; matrix[0, j] = j++) {}

	for (int i = 1; i <= n; i++)
	{
		for (int j = 1; j <= m; j++)
		{
			int cost = (option[j - 1] == name[i - 1]) ? 0 : 1;

			matrix[i, j] = Math.Min(
				Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
				matrix[i - 1, j - 1] + cost);
		}
	}
	return matrix[n, m];
}

bool IsNameInList(string name, string[] nameList)
{
	foreach (string line in nameList)
	{
		if (name == line)
			return (true);
	}
	return (false);
}
