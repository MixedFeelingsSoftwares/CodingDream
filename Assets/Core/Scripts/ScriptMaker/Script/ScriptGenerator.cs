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

    private readonly CommandType cType;

    #endregion Private Fields

    #region Public Constructors

    // This is a positional argument
    /// <summary>
    /// Sets the Command and adds it to list of commands
    /// </summary>
    /// <param name="Command">Name of the command.</param>
    public CommandName(string Command, CommandType type)
    {
        this.Command = Command;
        this.cType = type;
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

    public string CommandText
    {
        get { return Command; }
    }

    public CommandType getCommandType
    {
        get { return cType; }
    }

    #endregion Public Properties
}

public class Commands
{
    #region Public Methods

    public static bool CommandExists(string Command)
    {
        if (string.IsNullOrEmpty(Command)) { return false; }

        IEnumerable<MethodInfo> action = typeof(CommandMethods).GetMethods().
           Where(x => x.GetCustomAttributes(false).OfType<CommandName>().Count() > 0)
           .Where(x => x.GetCustomAttributes(false).OfType<CommandName>().First().CommandText == Command);

        return action != null && action.Count() > 0 ? true : false;
    }

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

        [CommandName("myHealth", CommandName.CommandType.Return)]
        public int getHealth()
        {
            Debug.Log(100);
            return 100;
        }

        [CommandName("Shoot", CommandName.CommandType.Action)]
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

    #endregion Public Properties

    #region Private Methods

    private void Awake()
    {
        Instance = this;
        field = GameObject.FindGameObjectWithTag("textCode").GetComponent<TMP_InputField>();
    }

    #endregion Private Methods

    #region Public Methods

    public void RefreshCommands()
    {
        if (field == null) { return; }

        if (field.text.Contains('\n'))
        {
            string[] fields = field.text.Split('\n');

            if (fields != null && fields.Length > 0 && fields.Length <= 25)
            {
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < fields.Length; i++)
                {
                    string line = fields[i];

                    bool cSrp = line.EndsWith("();");
                    string cmd = line.Split('(')[0];

                    if (Commands.CommandExists(cmd) && cSrp)
                    {
                        Debug.Log($"Command Exists: {cmd}");

                        string cCMD = line.Insert(0, "<b>")
                            .Insert(line.Length, "</b>");

                        builder.AppendLine(cCMD);

                        Commands.RunActionByName(cmd);
                    }
                    else
                    {
                        builder.AppendLine(line);
                    }
                }

                if (builder.ToString().Length > 0)
                {
                    field.text = builder.ToString();
                    field.selectionAnchorPosition = builder.Length;
                }
            }
        }
        else if (field != null && field.text.Length > 0)
        {
            string line = field.text;
            bool cSrp = line.EndsWith("();");
            string cmd = line.Split('(')[0];
            if (Commands.CommandExists(cmd) && cSrp)
            {
                Debug.Log($"Command Exists: {cmd}");
                Commands.RunActionByName(cmd);
            }
        }
    }

    #endregion Public Methods
}