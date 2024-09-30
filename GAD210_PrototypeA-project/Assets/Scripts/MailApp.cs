using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Mail App Instance", menuName = "Terminal Applications/Mail App", order = 0)]
public class MailApp : TerminalApp
{
    [SerializeField] private Mail[] inbox;

    public override bool Display(TerminalHUD hudInstance)
    {
        return hudInstance.DisplayApp(this);
    }

    public override bool StartApp(Terminal terminalInstance)
    {
        //startup program
        return false;
    }

    public bool SelectMail(int index, out Mail? mail)
    {
        mail = null;
        if(index >= 0 && index < inbox.Length)
        {
            mail = inbox[index];
            return true;
        }
        return false;
    }
}

[System.Serializable]
public struct Mail
{
    [SerializeField] private string fromAddress, toAddress, subjectLine;
    [SerializeField] [TextArea] private string message;

    public string FromAddress { get { return fromAddress; } }
    public string ToAddress { get { return toAddress; } }
    public string SubjectLine { get { return SubjectLine; } }
    public string Message { get { return message; } }
    public string CompiledMessage
    {
        get
        {
            return $"To: {toAddress}\nFrom: {fromAddress}\nSubject: {subjectLine}\n\nMessage: {message}";
        }
    }
}