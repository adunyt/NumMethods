//Console.WriteLine("Отрезок изоляции - a,b >> ");
//var isolateRange = Console.ReadLine();
//var test = isolateRange.Split(",");

int a = 1;
int b = 2;
double e = 0.001;
int i = 1;
int acc = 4;
MidpointRounding mid = MidpointRounding.ToEven;

bi(a, "a", b, "b", e, i);


double f(double x)
{
    //double result = Math.Round(Math.Pow(x, 3) - x - 1, acc, mid); // стандартный a = 1, b = 2
    //double result = Math.Round(Math.Pow(x, 3) + 8 * x + 4, 5); // матвей
    //double result = Math.Round(Math.Pow(x, 3) + 3 * x - 7, acc); // андрей
    //double result = Math.Round(Math.Pow(x, 3) + 2 * x - 7, acc, mid); // моё
    //double result = Math.Round(Math.Pow(x, 3) + 15 * x + 1, acc, mid); // максон
    //double result = Math.Round(Math.Pow(x, 3) - 4 * x - 2, acc, mid); // софа
    double result = Math.Round(Math.Pow(x, 3) + x - 3.5, acc, mid); // ??
    return result;
}

double fi(double x) 
{
    double y = Math.Round(3 * Math.Pow(x,2) + 2, acc, mid);
    return y;
}

void bi(double a, string aName, double b, string bName, double e, int i)
{
    string newAName = "";
    string newBName = "";
    double c = Math.Round((a + b) / 2, acc, mid);
    string cName = $"c{i}";
    Console.WriteLine($"{cName} = {aName}+{bName} / 2 = {a}+{b} / 2 = {c}");
    double f1 = f(c);
    Console.Write($"f({cName}) = f({c}) = {c}^3 + {c} + 3.5  = {f1}");
    if (f1 > 0)
    {
        Console.WriteLine(" > 0");
    }
    else
    {
        Console.WriteLine(" < 0");
    }
    double[] range1 = new double[2] { a, c }; // первый отрезок изоляция
    double[] range2 = new double[2] { c, b }; // второй отрезок изоляции
    double[] nextRange = Array.Empty<double>();
    string nextRangeDesc = "";
    Console.WriteLine($"[{range1[0]}, {range1[1]}] и [{range2[0]}, {range2[1]}]");
    double condition1 = f(range1[0]) * f(range1[1]);
    Console.Write($"f({aName}) f({cName})");
    if (condition1 < 0) // проверка на выполнения условия первым отрезком изоляции
    {
        Console.WriteLine($" < 0 - условие выполняется на [{range1[0]}, {range1[1]}]");
        nextRange = range1;
        nextRangeDesc = $"{aName} - {cName}";
        newAName = aName;
        newBName = cName;
    }
    else
    {
        Console.WriteLine($" > 0 - условие не выполняется на [{range1[0]}, {range1[1]}]");
    }
    double condition2 = f(range2[0]) * f(range2[1]);
    Console.Write($"f({cName}) f({bName})");
    if (condition2 < 0) // проверка на выполнения условия вторым отрезком изоляции
    {
        Console.WriteLine($" < 0 - условие выполняется на [{range2[0]}, {range2[1]}]");
        nextRange = range2;
        nextRangeDesc = $"{cName} - {bName}";
        newAName = cName;
        newBName = bName;
    }
    else
    {
        Console.WriteLine($" > 0 - условие не выполняется на [{range2[0]}, {range2[1]}]");
    }
    Console.WriteLine($"Рассмотрим [{nextRange[0]}, {nextRange[1]}]");
    Console.Write($"\t |{nextRangeDesc}| = |{nextRange[1]} - {nextRange[0]}| = {Math.Round(nextRange[1] - nextRange[0], 5, mid)}");
    if (Math.Round(nextRange[1] - nextRange[0], 5, mid) >= e) // проверка на выполнение заданной точности
    {
        Console.WriteLine($" >= {e}");
        Console.WriteLine("Заданная точность не достигнута\n");
        i++;
        bi(nextRange[0], newAName, nextRange[1], newBName, e, i);
    }
    else if (Math.Round(nextRange[1] - nextRange[0], 5, mid) < e)
    {
        Console.WriteLine($" < {e}");
        Console.WriteLine($"x = {c}");
    }
}

void chord(double a, string aName, double b, string bName, double e, double prevX, int i)
{
    string newAName = "";
    string newBName = "";
    double x = a - (((b - a) * f(a)) / (f(b) - f(a)));
    x = Math.Round(x, acc, mid);
    string xName = $"x{i}";
    Console.WriteLine($"{xName} = {x}");
    double fX = f(x);
    Console.Write($"f({xName}) = f({x}) = {fX}");
    if (fX > 0)
    {
        Console.WriteLine(" > 0");
    }
    else
    {
        Console.WriteLine(" < 0");
    }

    double[] range1 = new double[2] { a, x }; // первый отрезок изоляция
    double[] range2 = new double[2] { x, b }; // второй отрезок изоляции
    double[] nextRange = Array.Empty<double>();
    string nextRangeDesc = "";
    Console.WriteLine($"[{range1[0]}, {range1[1]}] и [{range2[0]}, {range2[1]}]");

    double condition1 = f(range1[0]) * f(range1[1]);
    Console.Write($"f({aName}) f({xName})");
    if (condition1 < 0) // проверка на выполнения условия первым отрезком изоляции
    {
        Console.WriteLine($" < 0 - условие выполняется на [{range1[0]}, {range1[1]}]");
        nextRange = range1;
        nextRangeDesc = $"{aName} - {xName}";
        newAName = aName;
        newBName = xName;
    }
    else if (condition1 == 0)
    {
        Console.WriteLine($" < 0 - условие условно выполняется на [{range1[0]}, {range1[1]}]");
        nextRange = range1;
        nextRangeDesc = $"{aName} - {xName}";
        newAName = aName;
        newBName = xName;
    }
    else
    {
        Console.WriteLine($" > 0 - условие не выполняется на [{range1[0]}, {range1[1]}]");
    }

    double condition2 = f(range2[0]) * f(range2[1]);
    Console.Write($"f({xName}) f({bName})");
    if (condition2 < 0) // проверка на выполнения условия вторым отрезком изоляции
    {
        Console.WriteLine($" < 0 - условие выполняется на [{range2[0]}, {range2[1]}]");
        nextRange = range2;
        nextRangeDesc = $"{xName} - {bName}";
        newAName = xName;
        newBName = bName;
    }
    else if (condition2 == 0)
    {
        Console.WriteLine($" < 0 - условие условно выполняется на [{range2[0]}, {range2[1]}]");
        nextRange = range2;
        nextRangeDesc = $"{xName} - {bName}";
        newAName = xName;
        newBName = bName;
    }
    else
    {
        Console.WriteLine($" > 0 - условие не выполняется на [{range2[0]}, {range2[1]}]");
    }

    Console.WriteLine($"Рассмотрим [{nextRange[0]}, {nextRange[1]}]");
    double deltaX = Math.Round(Math.Abs(x - prevX), 5, mid);
    Console.Write($"|x{i} - x{i - 1}| = |{x} - {prevX}| = {deltaX}");
    if (deltaX >= e) // проверка на выполнение заданной точности
    {
        Console.WriteLine($" >= {e}");
        Console.WriteLine("Заданная точность не достигнута\n");
        i++;
        chord(nextRange[0], newAName, nextRange[1], newBName, e, x, i);
    }
    else if (deltaX < e)
    {
        Console.WriteLine($" < {e}");
        Console.WriteLine($"x = {x}");
    }
}

void kasa(double a, string aName, double b, string bName, double e, double prevX, int i)
{
    string newAName = "";
    string newBName = "";
    double prevXDer = fi(prevX);
    prevXDer = Math.Round(prevXDer, acc, mid);
    double x = prevX - (f(prevX) / prevXDer);
    x = Math.Round(x, acc, mid);
    string xName = $"x{i}";
    Console.WriteLine($"{xName} = {x}");
    double fX = f(x);
    Console.Write($"f({xName}) = f({x}) = {fX}");
    if (fX > 0)
    {
        Console.WriteLine(" > 0");
    }
    else
    {
        Console.WriteLine(" < 0");
    }

    double[] range1 = new double[2] { a, x }; // первый отрезок изоляция
    double[] range2 = new double[2] { x, b }; // второй отрезок изоляции
    double[] nextRange = Array.Empty<double>();
    string nextRangeDesc = "";
    Console.WriteLine($"[{range1[0]}, {range1[1]}] и [{range2[0]}, {range2[1]}]");

    double condition1 = f(range1[0]) * f(range1[1]);
    Console.Write($"f({aName}) f({xName})");
    if (condition1 < 0) // проверка на выполнения условия первым отрезком изоляции
    {
        Console.WriteLine($" < 0 - условие выполняется на [{range1[0]}, {range1[1]}]");
        nextRange = range1;
        nextRangeDesc = $"{aName} - {xName}";
        newAName = aName;
        newBName = xName;
    }
    else if (condition1 == 0)
    {
        Console.WriteLine($" < 0 - условие условно выполняется на [{range1[0]}, {range1[1]}]");
        nextRange = range1;
        nextRangeDesc = $"{aName} - {xName}";
        newAName = aName;
        newBName = xName;
    }
    else
    {
        Console.WriteLine($" > 0 - условие не выполняется на [{range1[0]}, {range1[1]}]");
    }

    double condition2 = f(range2[0]) * f(range2[1]);
    Console.Write($"f({xName}) f({bName})");
    if (condition2 < 0) // проверка на выполнения условия вторым отрезком изоляции
    {
        Console.WriteLine($" < 0 - условие выполняется на [{range2[0]}, {range2[1]}]");
        nextRange = range2;
        nextRangeDesc = $"{xName} - {bName}";
        newAName = xName;
        newBName = bName;
    }
    else if (condition2 == 0)
    {
        Console.WriteLine($" < 0 - условие условно выполняется на [{range2[0]}, {range2[1]}]");
        nextRange = range2;
        nextRangeDesc = $"{xName} - {bName}";
        newAName = xName;
        newBName = bName;
    }
    else
    {
        Console.WriteLine($" > 0 - условие не выполняется на [{range2[0]}, {range2[1]}]");
    }

    Console.WriteLine($"Рассмотрим [{nextRange[0]}, {nextRange[1]}]");
    double deltaX = Math.Round(Math.Abs(x - prevX), 5, mid);
    Console.Write($"|x{i} - x{i - 1}| = |{x} - {prevX}| = {deltaX}");
    if (deltaX >= e) // проверка на выполнение заданной точности
    {
        Console.WriteLine($" >= {e}");
        Console.WriteLine("Заданная точность не достигнута\n");
        i++;
        kasa(nextRange[0], newAName, nextRange[1], newBName, e, x, i);
    }
    else if (deltaX < e)
    {
        Console.WriteLine($" < {e}");
        Console.WriteLine($"x = {x}");
    }
}