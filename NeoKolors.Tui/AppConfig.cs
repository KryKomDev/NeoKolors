//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui;

/// <summary>
/// structure for configuring the application 
/// </summary>
public struct AppConfig {
    
    /// <summary>
    /// determines whether the Ctrl + C combination force quits the application,
    /// when true just makes the System.Console.TreatControlCAsInput false
    /// </summary>
    public bool CtrlCForceQuits {
        get;
        set {
            System.Console.TreatControlCAsInput = !value;

            field = value;
        }
    } = true;

    /// <summary>
    /// the combination that interrupts the application
    /// </summary>
    public ConsoleKeyInfo InterruptCombination { get; set; } = new('c', key: ConsoleKey.C, false, false, true);

    /// <summary>
    /// if true rerenders the application only on a key press 
    /// </summary>
    public bool LazyRender { get; set; } = true;

    /// <summary>
    /// maximum number of updates / renders of the application per second 
    /// </summary>
    /// <exception cref="ArgumentException">set value was less than 1</exception>
    public int MaxUpdatesPerSecond {
        get;
        set {
            if (value < 1) {
                throw new ArgumentException(
                    $"Tried to set MaxUpdatesPerSecond to {value}. Value must be greater than 0.");
            }

            field = value;
        }
    } = 20;


    /// <param name="ctrlCForceQuits">determines whether the Ctrl + C combination force quits the application</param>
    /// <param name="interruptCombination">the combination that interrupts the application</param>
    /// <param name="lazyRender">if true rerenders the application only on a key press </param>
    /// <param name="maxUpdatesPerSecond">maximum number of updates / renders of the application per second</param>
    public AppConfig(
        bool ctrlCForceQuits = true,
        ConsoleKeyInfo interruptCombination = default,
        bool lazyRender = true,
        int maxUpdatesPerSecond = 20)
    {
        CtrlCForceQuits = ctrlCForceQuits;
        InterruptCombination = interruptCombination == default 
            ? new ConsoleKeyInfo('c', key: ConsoleKey.C, false, false, true)
            : interruptCombination;
        LazyRender = lazyRender;
        MaxUpdatesPerSecond = maxUpdatesPerSecond;
    }

    /// <summary>
    /// constructor with default settings
    /// </summary>
    public AppConfig() {
        CtrlCForceQuits = true;
        InterruptCombination = new('c', key: ConsoleKey.C, false, false, true);
        LazyRender = true;
        MaxUpdatesPerSecond = 20;
    }
}