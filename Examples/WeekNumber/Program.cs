using System.Globalization;
using NeoKolors.Common;
using NeoKolors.Console;

namespace WeekNumber;

public static class Program {
    public static void Main(string[] args) {
        NKDebug.ExceptionFormatting = true;
        
        switch (args) {
            case ["-a" or "--all"]:
                All();
                break;
            case ["-a" or "--all", _]:
                All(args[1]);
                break;
            case ["-v" or "--version", _]:
                Version(args[1]);
                break;
            case ["-h" or "--help"]:
                Help();
                break;
            case []:
                Default();
                break;
            default:
                Invalid(args);
                break;
        }
        
        Console.Write(EscapeCodes.FORMATTING_RESET);
    }

    private static void Default() {
        var now = DateTime.Now;
        var culture = CultureInfo.CurrentCulture;
        var calendar = culture.Calendar;
        int weekNumber = calendar.GetWeekOfYear(now, CalendarWeekRule.FirstDay, culture.DateTimeFormat.FirstDayOfWeek);
        
        Console.WriteLine($"Week number: <f-green><b>{weekNumber}</b></f-color>".ApplyColors().ApplyStyles());
    }

    private static void All(string v = "") {
        var now = DateTime.Now;
        var culture = CultureInfo.CurrentCulture;
        var calendar = culture.Calendar;
        int weekNumber = calendar.GetWeekOfYear(now, CalendarWeekRule.FirstDay, culture.DateTimeFormat.FirstDayOfWeek);
        
        Console.WriteLine($"Year: <f-green><b>{now.Year}</b></f-color>".ApplyColors().ApplyStyles());
        Console.WriteLine($"Month: <f-green><b>{now.Month}</b></f-color>".ApplyColors().ApplyStyles());
        Console.WriteLine($"Day: <f-green><b>{now.Day}. {now.DayOfWeek}</b></f-color>".ApplyColors().ApplyStyles());
        Console.WriteLine($"Week: <f-green><b>{weekNumber}</b></f-color>".ApplyColors().ApplyStyles());
        Console.WriteLine($"Version name: <f-green><b>{(now.Year + "")[2..]}w{weekNumber}{v}</b></f-color>".ApplyColors().ApplyStyles());
    }

    private static void Version(string version) {
        var now = DateTime.Now;
        var culture = CultureInfo.CurrentCulture;
        var calendar = culture.Calendar;
        int weekNumber = calendar.GetWeekOfYear(now, CalendarWeekRule.FirstDay, culture.DateTimeFormat.FirstDayOfWeek);
        
        Console.WriteLine($"Version name: <f-green><b>{(now.Year + "")[2..]}w{weekNumber}{version}</b></f-color>".ApplyColors().ApplyStyles());
    }

    private static void Invalid(string[] args) {
        throw new ArgumentException(
            $"Arguments {args.Aggregate((s1, s2) => s1 + ", " + s2)} are invalid. Use -h or --help for more info.");
    }

    private static void Help() {
        Console.Write(("flags: \n" +
                      "  <f-green>-a</f-color> or <f-green>--all</f-color> - <f-white>displays all info about today</f-color>\n" +
                      "  <f-green>-v</f-color> or <f-green>--version</f-color> - <f-white>when added another argument displays a version name</f-color>\n" +
                      "  <f-green>-h</f-color> or <f-green>--help</f-color> - <f-white>show this message</f-color>\n").ApplyColors().ApplyStyles());
    }
}