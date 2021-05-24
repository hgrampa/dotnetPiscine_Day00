using System;

double sum; // Сумма кредита, р
double rate; // Годовая процентная ставка, %
double ratePerMonth;  // Процентная ставка по займу в месяц
int term; // Количество месяцев кредита
int selectedMonth; // Номер месяца кредита, в котором вносится досрочный платёж
double payment; // Сумма досрочного платежа, р

if (args.Length == 5
	&& Double.TryParse(args[0], out sum)
	&& Double.TryParse(args[1], out rate)
	&& Int32.TryParse(args[2], out term)
	&& Int32.TryParse(args[3], out selectedMonth)
	&& Double.TryParse(args[4], out payment)
	&& ValidateArgs())
{
	ratePerMonth = rate / 12.0f / 100.0f;
	DateTime firstPaymentDate = GetFirstPaymentDate(); // дата первого платежа (1 число след. месяца)
	double debtSumm = sum; // Остаток долга
	double overpaidSumm = 0; // Сумма переплат (до Decrease mounth)

	// Выплачиваем месяцы до Decrease mounth
	DateTime mouth = firstPaymentDate;
	double monthlyPaySumm = GetMonthlyPaySumm(sum, term); // Сумма ежемесячного платежа
	for (int i = 1; i <= selectedMonth; i++)
	{
		ApplyMonthlyPayment(ref overpaidSumm, ref debtSumm, mouth, monthlyPaySumm);
		mouth = mouth.AddMonths(1);
	}
	

	double summDecreaseOverpaid = GetSummDecreaseOverpaid(overpaidSumm, debtSumm, mouth); // Сумма переплат при погашении суммы
	double periodDecreaseOverpaid = GetPeriodDecreaseOverpaid(overpaidSumm, debtSumm, mouth, monthlyPaySumm); // Сумма переплат при погашении строка
	PrintBetterOverpaid(summDecreaseOverpaid, periodDecreaseOverpaid);
}
else
	Console.WriteLine("Ошибка ввода. Проверьте входные данные и повторите запрос.");

void PrintBetterOverpaid(double summDecreaseOverpaid, double periodDecreaseOverpaid)
{
	
	if (summDecreaseOverpaid > periodDecreaseOverpaid)
		Console.WriteLine($"Уменьшение срока выгоднее уменьшения платежа на {summDecreaseOverpaid - periodDecreaseOverpaid:f2} р.");
	else if (summDecreaseOverpaid < periodDecreaseOverpaid)
		Console.WriteLine($"Уменьшение платежа выгоднее уменьшения срока на {periodDecreaseOverpaid - summDecreaseOverpaid:f2} р.");
	else
		Console.WriteLine("Переплата одинакова в обоих вариантах.");
}

double GetPeriodDecreaseOverpaid(double overpaidSumm, double debtSumm, DateTime month, double monthlyPaySumm)
{
	debtSumm -= payment;
	int newTerm = (int)Math.Log(monthlyPaySumm / (monthlyPaySumm - ratePerMonth * debtSumm), ratePerMonth + 1);
	for (int i = 0; i < newTerm; i++)
	{
		ApplyMonthlyPayment(ref overpaidSumm, ref debtSumm, month, monthlyPaySumm);
		month = month.AddMonths(1);
	}
	return (overpaidSumm);
}

double GetSummDecreaseOverpaid(double overpaidSumm, double debtSumm, DateTime month)
{
	debtSumm -= payment;
	double monthlyPaySumm = GetMonthlyPaySumm(debtSumm, term - selectedMonth);
	for (int i = selectedMonth + 1; i <= term; i++)
	{
		ApplyMonthlyPayment(ref overpaidSumm, ref debtSumm, month, monthlyPaySumm);
		month = month.AddMonths(1);
	}
	return (overpaidSumm);
}

bool ValidateArgs()
{
	if (!(Double.IsNormal(sum) && Double.IsNormal(rate) && Double.IsNormal(payment)))
		return (false);
	if (sum < 0 || rate < 0 || payment < 0)
		return (false);
	if (selectedMonth < 1 && selectedMonth < term)
		return (false);
	return (true);
}

DateTime GetFirstPaymentDate()
{
	DateTime now = DateTime.Now.AddMonths(1);
	return (new DateTime(now.Year, now.Month, 1));
}

double GetMonthlyPaySumm(double sum, int term)
{
	double factor = Math.Pow((1 + ratePerMonth), (double)term); // (1 + i)^n
	return (sum * ratePerMonth * factor / (factor - 1));
}

void ApplyMonthlyPayment(ref double overpaidSumm, ref double debtSumm, DateTime month, double monthlyPaySumm)
{
	double interestSum = GetInterestInMonth(debtSumm, month);
	overpaidSumm += interestSum;
	debtSumm -= (monthlyPaySumm - interestSum);
}

int DaysInYear(DateTime date)
{
	return DateTime.IsLeapYear(date.Year) ? 366 : 365;
}

double GetInterestInMonth(double debtSumm, DateTime month)
{
	DateTime lastmonth = month.AddMonths(-1);

	return (debtSumm * rate * (DateTime.DaysInMonth(lastmonth.Year, lastmonth.Month))) / (100 * DaysInYear(lastmonth));
}
