using ConsoleRpg.Helpers;
using ConsoleRpgEntities.Data;
using ConsoleRpgEntities.Models.Attributes;
using ConsoleRpgEntities.Models.Characters;
using ConsoleRpgEntities.Models.Characters.Monsters;
using Spectre.Console;

namespace ConsoleRpg.Services;

public class GameEngine
{
    private readonly GameContext _context;
    private readonly MenuManager _menuManager;
    private readonly MapManager _mapManager;
    private readonly PlayerService _playerService;
    private readonly OutputManager _outputManager;
    private Table _logTable;
    private Panel _mapPanel;

    private Player _player;
    private IMonster _goblin;

    public GameEngine(GameContext context, MenuManager menuManager, MapManager mapManager, PlayerService playerService, OutputManager outputManager)
    {
        _menuManager = menuManager;
        _mapManager = mapManager;
        _playerService = playerService;
        _outputManager = outputManager;
        _context = context;
    }

    public void Run()
    {
        if (_menuManager.ShowMainMenu())
        {
            SetupGame();
        }
    }

    private void GameLoop()
    {
        while (true)
        {
            _outputManager.AddLogEntry("1. Attack");
            _outputManager.AddLogEntry("2. Manage Characters");
            _outputManager.AddLogEntry("3. Quit");
            var input = _outputManager.GetUserInput("Choose an action:");


            switch (input)
            {
                case "1":
                    AttackCharacter();
                    break;
                case "2":
                    Console.WriteLine("manage characters");
                    break;
                case "3":
                    _outputManager.AddLogEntry("Exiting game...");
                    Environment.Exit(0);
                    break;
                default:
                    _outputManager.AddLogEntry("Invalid selection. Please choose 1.");
                    break;
            }
        }
    }

    private void AttackCharacter()
    {
        if (_goblin is ITargetable targetableGoblin)
        {
            _playerService.Attack(_player, targetableGoblin);
            _playerService.UseAbility(_player, _player.Abilities.First(), targetableGoblin);
        }
    }

    private void SetupGame()
    {
        _player = _context.Players.FirstOrDefault();
        _outputManager.AddLogEntry($"{_player.Name} has entered the game.");

        // Load monsters into random rooms 
        LoadMonsters();

        // Load map
        _mapManager.LoadInitialRoom(1);
        _mapManager.DisplayMap();

        // Pause before starting the game loop
        Thread.Sleep(500);
        GameLoop();
    }

    private void LoadMonsters()
    {
        _goblin = _context.Monsters.OfType<Goblin>().FirstOrDefault();
    }

    private void ManageCharacters()
    {
        _outputManager.AddLogEntry("List created characters");
        _outputManager.AddLogEntry("Create new character");
        _outputManager.AddLogEntry("Edit created characters");
        _outputManager.AddLogEntry("Add ability to created character");
        _outputManager.AddLogEntry("Display character abilities");

        var input = _outputManager.GetUserInput("Choose an action:");

        switch (input)
        {
            case "1":
                DisplayCharacters();
                break;
            case "2":
                SearchCharacters();
                break;
            case "3":
                AddCharacter();
                break;
            case "4":
                EditCharacter();
                break;
            case "5":
                AddAbility();
                break;
            case "6":
                break;
            default:
                _outputManager.AddLogEntry("Invalid selection. Please choose a valid action.");
                break;
        }
    }

    private void DisplayCharacters()
    {
        foreach (Player player in _context.Players)
        {
            Console.WriteLine(player);
        }
    }
}
