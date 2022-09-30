using Spectre.Console;
using Expr = MathNet.Symbolics.SymbolicExpression;

int a;
int b;
int acc;
double e;
int i = 1;
bool isRunning = true;
Expr fExpr = Expr.Parse("x");
Func<double, double> f = Expr.Parse("x").Compile("x"); // заглушки
Func<double, double> firstDiffF = Expr.Parse("x").Compile("x");
Func<double, double> secondDiffF = Expr.Parse("x").Compile("x");

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
        .Start("Вычисляем производные...", ctx =>
        {
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
            "Половинного деления", "Хорд", "Касательных (Ньютона)", "Комбинированный"
            }));

        switch (method)
        {
            case "Половинного деления":
                main(Methods.Bisection, a, "a", b, "b", 0, e, i); // 0 потому что не используется
                break;
            case "Хорд":
                main(Methods.Secant, a, "a", b, "b", b, e, i);
                break;
            case "Касательных (Ньютона)":
                double x0 = selectX0(a, b);
                main(Methods.Newton, a, "a", b, "b", x0, e, i);
                break;
            case "Комбинированный":
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

double selectX0(double a, double b)
{
    var aResult = f(a) * secondDiffF(a);
    if (aResult > 0)
    {
        return a;
    }
    var bResult = f(b) * secondDiffF(b);
    if (bResult > 0)
    {
        return b;
    }
    else
    {
        throw new Exception("Невозможно определить начальную точку");
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

double calcX(Methods method, double aValue, double bValue, double prevXValue, double i, string aName, string bName)
{
    string xName = $"x{i}";
    string prevXName = $"x{i-1}";
    double x;
    double fA = Math.Round(f(aValue), acc);
    double fB = Math.Round(f(bValue), acc);
    switch (method)
    {
        case Methods.Bisection:
            x = (aValue + bValue) / 2;
            x = Math.Round(x, acc);
            Console.WriteLine($"{xName} = {aName}+{bName} / 2 = {aValue}+{bValue} / 2 = {x}");
            break;
        case Methods.Secant:
            x = aValue - (((bValue - aValue) * fA / (fB - fA)));
            x = Math.Round(x, acc);
            Console.WriteLine($"{xName} = {aValue} - (({bValue} - {aValue}) * f({aValue} / (f({bValue}) - f({aValue}) )) = {aValue} - ({Math.Round(bValue - aValue, acc)} * {fA} / ({fB} - {fA})) = {x}");
            break;
        case Methods.Newton:
            x = prevXValue - (f(prevXValue) / firstDiffF(prevXValue));
            x = Math.Round(x, acc);
            Console.WriteLine($"{xName} = {prevXValue} - (f({prevXValue}) / f'({prevXValue})) = {prevXValue} - ({f(prevXValue)} / {firstDiffF(prevXValue)}) = {x}");
            break;
        case Methods.Mixed:
            x = 0;
            break;
        default:
            x = 0;
            break;
    }
    return x;
}

void main(Methods method, double a, string aName, double b, string bName, double prevX, double e, int i)
{
    string newAName = "";
    string newBName = "";
    string xName = $"x{i}";
    string nextRangeDesc = "";

    double x = calcX(method, a, b, prevX, i, aName, bName);

    double fX = Math.Round(f(x), acc);
    Console.Write($"f({xName}) = f({x}) = {fX}");
    if (fX > 0) Console.WriteLine(" > 0");
    else Console.WriteLine(" < 0");
    double[] range1 = new double[2] { a, x }; // первый отрезок изоляция
    double[] range2 = new double[2] { x, b }; // второй отрезок изоляции
    double[] nextRange = Array.Empty<double>();
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
    else
    {
        Console.WriteLine($" > 0 - условие не выполняется на [{range2[0]}, {range2[1]}]");
    }
    Console.WriteLine($"Рассмотрим [{nextRange[0]}, {nextRange[1]}]");
    bool isX = false;
    switch (method)
    {
        case Methods.Bisection:
            isX = Math.Round(nextRange[1] - nextRange[0], 5) < e;
            Console.Write($"\t |{nextRangeDesc}| = |{nextRange[1]} - {nextRange[0]}| = {Math.Round(nextRange[1] - nextRange[0], 5)}");
            break;
        case Methods.Secant:
        case Methods.Newton:
            isX = Math.Round(Math.Abs(x - prevX), 5) < e;
            Console.Write($"|x{i} - x{i - 1}| = |{x} - {prevX}| = {Math.Round(Math.Abs(x - prevX), 5)}");
            break;
        case Methods.Mixed:
            break;
    }

    if (!isX) // проверка на выполнение заданной точности
    {
        Console.WriteLine($" >= {e}");
        Console.WriteLine("Заданная точность не достигнута\n");
        i++;
        main(method, nextRange[0], newAName, nextRange[1], newBName, x, e, i);
    }
    else if (isX)
    {
        Console.WriteLine($" < {e}");
        Console.WriteLine($"x = {x}");
    }
}

enum Methods
{
    Bisection,
    Secant,
    Newton,
    Mixed
}
