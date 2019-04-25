using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

public class ScriptGenerator : MonoBehaviour
{
    public TMP_InputField field = null;

    private void Awake()
    {
        field = GameObject.FindGameObjectWithTag("textCode").GetComponent<TMP_InputField>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RefreshCommands()
    {
        if (field == null) { return; }

        if (field.text.Contains('\n'))
        {
            string[] fields = field.text.Split('\n');

            if (fields != null && fields.Length > 0 && fields.Length <= 25)
            {
                foreach (string line in fields)
                {
                    if (Commands.CommandExists(line))
                    {
                        Debug.Log($"Command Exists: {line}");
                    }
                }
            }
        }
        else if (field != null && field.text.Length > 0)
        {
            string line = field.text;
            if (Commands.CommandExists(line))
            {
                Debug.Log($"Command Exists: {line}");
            }
        }
    }
}


public class Commands
{
    public class CommandMethods
    {
        [CommandName("Shoot", CommandName.CommandType.Action)]
        public void Shoot()
        {
            Debug.Log("Shooting..");
        }

        [CommandName("myHealth", CommandName.CommandType.Return)]
        public int getHealth()
        {
            Debug.Log(100);
            return 100;
        }
    }

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

        action.Invoke(new CommandMethods(), null);
    }
}



[System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
public sealed class CommandName : Attribute
{
    // See the attribute guidelines at 
    //  http://go.microsoft.com/fwlink/?LinkId=85236

    readonly string Command;
    
    readonly CommandType cType;

    public enum CommandType
    {
        Return = 0,
        Action = 1
    }

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

    public string CommandText
    {
        get { return Command; }
    }

    public CommandType getCommandType
    {
        get { return cType; }
    }
}
