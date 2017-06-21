namespace LuisBot
{
    using System;
    using Microsoft.Bot.Builder.FormFlow;

    [Serializable]
    public class OpenQuery
    {
        [Prompt("What is your full name?")]
        public string Name { get; set; }

        [Prompt("What type of account would like to open (checking, savings)?")]
        public string AccountType { get; set; }

        [Numeric(1, int.MaxValue)]
        [Prompt("What is your phone number?")]
        public int PhoneNum { get; set; }
    }
}