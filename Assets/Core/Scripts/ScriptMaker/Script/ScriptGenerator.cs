using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;

[System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
public sealed class CommandName : Attribute
{
    // See the attribute guidelines at http://go.microsoft.com/fwlink/?LinkId=85236

    #region Private Fields

    private readonly string Command;

    private readonly int Cost;

    private readonly CommandType cType;

    #endregion Private Fields

    #region Public Constructors

    // This is a positional argument
    /// <summary>
    /// Sets the Command and adds it to list of commands
    /// </summary>
    /// <param name="Command">Name of the command.</param>
    public CommandName(string Command, CommandType type, int Cost)
    {
        this.Command = Command;
        this.cType = type;
        this.Cost = Cost;
    }

    #endregion Public Constructors

    #region Public Enums

    public enum CommandType
    {
        Return = 0,

        Action = 1
    }

    #endregion Public Enums

    #region Public Properties

    /// <summary>
    /// Command of the move
    /// </summary>
    public string CommandText
    {
        get { return Command; }
    }

    /// <summary>
    /// Energy Cost of Move
    /// </summary>
    public int EnergyCost
    {
        get { return Cost; }
    }

    /// <summary>
    /// Command Type of move
    /// </summary>
    public CommandType getCommandType
    {
        get { return cType; }
    }

    #endregion Public Properties
}

public class Commands
{
    #region Public Methods

    public static Tuple<bool, MethodInfo> CommandExists(string Command)
    {
        if (string.IsNullOrEmpty(Command)) { return new Tuple<bool, MethodInfo>(false, null); }

        IEnumerable<MethodInfo> action = typeof(CommandMethods).GetMethods().
           Where(x => x.GetCustomAttributes(false).OfType<CommandName>().Count() > 0)
           .Where(x => x.GetCustomAttributes(false).OfType<CommandName>().First().CommandText == Command);

        return action != null && action.Count() > 0 ? new Tuple<bool, MethodInfo>(true, action.First()) : new Tuple<bool, MethodInfo>(false, null);
    }

    #region ActionCostOverload

    /// <summary>
    /// Gets Action Energy Cost by Command Name
    /// </summary>
    /// <param name="Name">Name of Command</param>
    /// <returns>Energy Cost</returns>
    public static int getActionCost(string Name)
    {
        int cost = -1;
        MethodInfo action = typeof(CommandMethods).GetMethods().
            Where(x => x.GetCustomAttributes(false).OfType<CommandName>().Count() > 0)
            .Where(x => x.GetCustomAttributes(false).OfType<CommandName>().First().CommandText == Name)
            .First();
        if (action != null)
        {
            CommandName cmdName = action.GetCustomAttributes(false).OfType<CommandName>().First();
            cost = cmdName.EnergyCost;
        }

        return cost;
    }

    /// <summary>
    /// Gets Action Energy Cost by Action
    /// </summary>
    /// <param name="action">Command action</param>
    /// <returns>Energy Cost</returns>
    public static int getActionCost(MethodInfo action)
    {
        int cost = -1;
        if (action != null)
        {
            CommandName cmdName = action.GetCustomAttributes(false).OfType<CommandName>().First();
            cost = cmdName.EnergyCost;
        }

        return cost;
    }

    #endregion ActionCostOverload

    public static void RunActionByName(string Command)
    {
        MethodInfo action = typeof(CommandMethods).GetMethods().
            Where(x => x.GetCustomAttributes(false).OfType<CommandName>().Count() > 0)
            .Where(x => x.GetCustomAttributes(false).OfType<CommandName>().First().CommandText == Command)
            .First();

        if (action != null)
        {
            action.Invoke(new CommandMethods(), null);
        }
    }

    #endregion Public Methods

    #region Public Classes

    public class CommandMethods
    {
        #region Public Methods

        [CommandName("myHealth", CommandName.CommandType.Return, 1)]
        public int getHealth()
        {
            Debug.Log(100);
            return 100;
        }

        [CommandName("Move", CommandName.CommandType.Action, 2)]
        public void Move()
        {
            if (CharacterCore.Instance != null)
            {
                CharacterCore.Instance.Move();
            }
        }

        [CommandName("Shoot", CommandName.CommandType.Action, 3)]
        public void Shoot()
        {
            Debug.Log("Shooting..");
        }

        #endregion Public Methods
    }

    #endregion Public Classes
}

public class ScriptGenerator : MonoBehaviour
{
    #region Public Properties

    public static ScriptGenerator Instance { get; private set; }

    public TMP_InputField field { get; private set; }

    public TextMeshProUGUI txtEnergyCost { get; private set; }

    #endregion Public Properties

    #region Private Methods

    private void Awake()
    {
        Instance = this;
        field = GameObject.FindGameObjectWithTag("textCode").GetComponent<TMP_InputField>();
        txtEnergyCost = GameObject.FindGameObjectWithTag("txtEnergyCost").GetComponent<TextMeshProUGUI>();
    }

    #endregion Private Methods

    #region Public Methods

    public void RefreshCommands()
    {
        if (field == null) { return; }

        if (field.text.Contains('\n'))
        {
            // Multiline - Checks all commands
            string[] fields = field.text.Split('\n');

            if (fields != null && fields.Length > 0 && fields.Length <= 25)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    string line = fields[i];

                    // C# formatting
                    bool cSrp = line.EndsWith("();");
                    string cmd = line.Split('(')[0];

                    // Ignore all spaces
                    cmd = cmd.Replace(" ", string.Empty);
                    Tuple<bool, MethodInfo> act = Commands.CommandExists(cmd);
                    if (act.Item1 && cSrp)
                    {
                        // Command Exists do something
                        int cost = Commands.getActionCost(act.Item2);
                        Debug.Log($"Command Exists: {cmd}:{cost}");
                    }
                }
            }
        }
        else if (field != null && field.text.Length > 0)
        {
            // If only one line
            string line = field.text;
            bool cSrp = line.EndsWith("();");
            string cmd = line.Split('(')[0];

            // Ignore all spaces
            cmd = cmd.Replace(" ", string.Empty);

            Tuple<bool, MethodInfo> act = Commands.CommandExists(cmd);
            if (act.Item1 && cSrp)
            {
                int cost = Commands.getActionCost(act.Item2);
                Debug.Log($"Command Exists: {cmd}:{cost}");
            }
        }
    }

    public void RunCommands()
    {
        if (field == null) { return; }

        if (field.text.Contains('\n'))
        {
            // Multiline - Checks all commands
            string[] fields = field.text.Split('\n');

            if (fields != null && fields.Length > 0 && fields.Length <= 25)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    string line = fields[i];

                    // C# formatting
                    bool cSrp = line.EndsWith("();");
                    string cmd = line.Split('(')[0];

                    // Ignore all spaces
                    cmd = cmd.Replace(" ", string.Empty);

                    Tuple<bool, MethodInfo> act = Commands.CommandExists(cmd);
                    if (act.Item1 && cSrp)
                    {
                        // Command Exists do something
                        int cost = Commands.getActionCost(act.Item2);
                        Debug.Log($"Command Exists: {cmd}:{cost}");

                        Commands.RunActionByName(cmd);
                    }
                    else
                    {
                        // Command does not exist (ERROR)
                    }
                }
            }
        }
        else if (field != null && field.text.Length > 0)
        {
            // If only one line
            string line = field.text;
            bool cSrp = line.EndsWith("();");
            string cmd = line.Split('(')[0];

            // Ignore all spaces
            cmd = cmd.Replace(" ", string.Empty);

            Tuple<bool, MethodInfo> act = Commands.CommandExists(cmd);
            if (act.Item1 && cSrp)
            {
                int cost = Commands.getActionCost(act.Item2);
                Debug.Log($"Command Exists: {cmd}:{cost}");

                Commands.RunActionByName(cmd);
            }
        }
    }

    #endregion Public Methods
}