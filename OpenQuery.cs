namespace LuisBot
{
    using System;
    using Microsoft.Bot.Builder.FormFlow;

    [Serializable]
    public class OpenQuery
    {

        [Pattern("[A-Za-z]{1,50} +[A-Za-z]{1,50}")]
        [Prompt("What is your full name?")]
        public string Name { get; set; }

        [Pattern("deposits?|loans?")]
        [Prompt("What type of account would like to open (deposit, loan)?")]
        public string AccountType { get; set; }
        
        [Pattern("[0-9]{9,10}|[0-9]?[- ]*\\(*[0-9]{3}\\)*[- ]*[0-9]{3}[- ]*[0-9]{4}")]
        [Prompt("What is your phone number?")]
        public string PhoneNum { get; set; }
    }
}