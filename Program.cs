using Spectre.Console;
using Expr = MathNet.Symbolics.SymbolicExpression;

int a;
int b;
int acc;
double e;
int i = 1;
bool isRunning = true;
Func<double, double> f;
Func<double, double> firstDiffF; // первая производная
Func<double, double> secondDiffF; // вторая производная

while (isRunning)
{
    Console.Clear();
    string userInput = AnsiConsole.Prompt(new TextPrompt<string>("Введите ваше [blue]уравнение[/] -> ").Validate(userInput =>
    {
        ValidationResult result = ValidationResult.Success();
        try
        {
            var textF = Expr.Parse(userInput).Compile("x");
        }
        catch (Exception)
        {
            result = ValidationResult.Error($"[red]Не могу обработать уравнение[/]");
        }
        return result;
    }));
    Expr funcX = Expr.Parse(userInput);
    f = funcX.Compile("x");
    AnsiConsole.Status()
        .Start("Вычисляем производные...", ctx => {
            ctx.SpinnerStyle(Style.Parse("blue"));
            var firstDiffFExpr = funcX.Differentiate("x");
            firstDiffFExpr.ToString();
            firstDiffF = firstDiffFExpr.Compile("x");
            secondDiffF = firstDiffFExpr.Differentiate("x").Compile("x");
        });

    a = AnsiConsole.Ask<int>("Введи [red]a[/] -> ");
    b = AnsiConsole.Ask<int>("Введи [red]b[/] -> ");
    acc = AnsiConsole.Prompt(new TextPrompt<int>("Введи до какого знака после запятой округлять -> ").DefaultValue(4).Validate(acc =>
    {
        ValidationResult result;
        if (acc > 15)
        {
            result = ValidationResult.Error("Не больше 15");
        }
        else if (acc < 0)
        {
            result = ValidationResult.Error("Не меньше 1");
        }
        else
        {
            result = ValidationResult.Success();
        }
        return result;
    }));
    e = AnsiConsole.Prompt(new TextPrompt<double>("Введи требуемую точность (e) -> ").DefaultValue(0.001));

    if (canUse(a, b))
    {
        var method = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("Какой метод хотите использовать?")
            .PageSize(5)
            .AddChoices(new[] {
            "Половинного деления", "Хорд", "Касательных", "Ньютона"
            }));

        switch (method)
        {
            case "Половинного деления":
                bi(a, "a", b, "b", e, i);
                break;
            case "Хорд":
                secant(a, "a", b, "b", e, b, i);
                break;
            case "Касательных":
                Console.WriteLine("Пока нет");
                //kasa(a, "a", b, "b", e, b, i);
                break;
            case "Ньютона":
                Console.WriteLine("Пока нет");
                break;
        }
    }

    bool restart = AnsiConsole.Confirm("\nПовторить?");
    if (!restart)
    {
        isRunning = false;
    }
}

bool canUse(double a, double b)
{
    double result = f(a) * f(b);
    Console.Write($"\nf(a) * f(b) ");
    if (result >= 0)
    {
        Console.WriteLine(" > 0 - нельзя применить метод, либо выбран неправильный отрезок\n");
        return false;
    }
    else
    {
        Console.WriteLine(" < 0 - можно применить метод\n");
        return true;
    }
}

void bi(double a, string aName, double b, string bName, double e, int i)
{
    string newAName = "";
    string newBName = "";
    double c = Math.Round((a + b) / 2, acc);
    string cName = $"c{i}";
    Console.WriteLine($"{cName} = {aName}+{bName} / 2 = {a}+{b} / 2 = {c}");
    double fX = Math.Round(f(c), acc);
    Console.Write($"f({cName}) = f({c}) = {fX}");
    if (fX > 0)
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
    Console.Write($"\t |{nextRangeDesc}| = |{nextRange[1]} - {nextRange[0]}| = {Math.Round(nextRange[1] - nextRange[0], 5)}");
    if (Math.Round(nextRange[1] - nextRange[0], 5) >= e) // проверка на выполнение заданной точности
    {
        Console.WriteLine($" >= {e}");
        Console.WriteLine("Заданная точность не достигнута\n");
        i++;
        bi(nextRange[0], newAName, nextRange[1], newBName, e, i);
    }
    else if (Math.Round(nextRange[1] - nextRange[0], 5) < e)
    {
        Console.WriteLine($" < {e}");
        Console.WriteLine($"x = {c}");
    }
}

void secant(double a, string aName, double b, string bName, double e, double prevX, int i)
{
    string newAName = "";
    string newBName = "";
    string xName = $"x{i}";
    double x = a - (((b - a) * f(a)) / (f(b) - f(a)));
    x = Math.Round(x, acc);
    double fX = Math.Round(f(x), acc);
    Console.WriteLine($"{xName} = {x}");
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
        newBName = xName; // не меняется
    }
    else if (condition1 == 0)
    {
        Console.WriteLine($" < 0 - условие условно выполняется на [{range1[0]}, {range1[1]}]");
        nextRange = range1;
        nextRangeDesc = $"{aName} - {xName}";
        newAName = aName;
        newBName = xName; // не меняется
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
        newBName = bName; // не меняется
    }
    else if (condition2 == 0)
    {
        Console.WriteLine($" < 0 - условие условно выполняется на [{range2[0]}, {range2[1]}]");
        nextRange = range2;
        nextRangeDesc = $"{xName} - {bName}";
        newAName = xName;
        newBName = bName; // не меняется
    }
    else
    {
        Console.WriteLine($" > 0 - условие не выполняется на [{range2[0]}, {range2[1]}]");
    }

    Console.WriteLine($"Рассмотрим [{nextRange[0]}, {nextRange[1]}]");
    double deltaX = Math.Round(Math.Abs(x - prevX), 5);
    Console.Write($"|x{i} - x{i - 1}| = |{x} - {prevX}| = {deltaX}");
    if (deltaX >= e) // проверка на выполнение заданной точности
    {
        Console.WriteLine($" >= {e}");
        Console.WriteLine("Заданная точность не достигнута\n");
        i++;
        secant(nextRange[0], newAName, nextRange[1], newBName, e, x, i);
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
    string xName = $"x{i}";
    double prevXDer = Math.Round(firstDiffF(prevX), acc);
    double x = Math.Round(prevX - (f(prevX) / prevXDer), acc);
    double fX = f(x);

    Console.WriteLine($"{xName} = {x}");
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
    double deltaX = Math.Round(Math.Abs(x - prevX), 5);
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