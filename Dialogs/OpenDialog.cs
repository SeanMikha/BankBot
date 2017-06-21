namespace LuisBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.FormFlow;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class OpenDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            //context.Fail(new NotImplementedException("This Dialog is not implemented and is instead being used to show context.Fail"));

            await context.PostAsync("Okay, let's open a new account.");

            var OpenFormDialog = FormDialog.FromForm(this.BuildOpenForm, FormOptions.PromptInStart);

            context.Call(OpenFormDialog, this.ResumeAfterOpenFormDialog);
        }

        private IForm<OpenQuery> BuildOpenForm()
        {
            OnCompletionAsyncDelegate<OpenQuery> processOpenSearch = async (context, state) =>
            {
                await context.PostAsync($"Ok. Opening a {state.AccountType} account for {state.Name}. Our agent is doing the prep work to make this smooth. We'll call you at {state.PhoneNum} when we're ready.");
            };

            return new FormBuilder<OpenQuery>()
                .Field(nameof(OpenQuery.Name))
                .Message("Setting up a {AccountType} for {Name}...")
                .AddRemainingFields()
                .OnCompletion(processOpenSearch)
                .Build();
        }

        private async Task ResumeAfterOpenFormDialog(IDialogContext context, IAwaitable<OpenQuery> result)
        {
            try
            {
                var searchQuery = await result;
                var rates = await GetRatesAsync(searchQuery);

                await context.PostAsync($"I found a total of {rates.Count()} Institutions for your account:");

                var resultMessage = context.MakeMessage();
                resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                resultMessage.Attachments = new List<Attachment>();

                foreach (var rate in rates)
                {
                    HeroCard heroCard = new HeroCard()
                    {
                        Title = rate.Name,
                        Subtitle = $"{rate.Rating} starts. {rate.NumberOfReviews} reviews. From %{rate.PriceStarting} per year APR.",
                        Images = new List<CardImage>()
                        {
                            new CardImage() { Url = rate.Image }
                        },
                        Buttons = new List<CardAction>()
                        {
                            new CardAction()
                            {
                                Title = "More details",
                                Type = ActionTypes.OpenUrl,
                                Value = $"https://www.bing.com/search?q=institutions+for+" + HttpUtility.UrlEncode(rate.Name)
                            }
                        }
                    };

                    resultMessage.Attachments.Add(heroCard.ToAttachment());
                }

                await context.PostAsync(resultMessage);
            }
            catch (FormCanceledException ex)
            {
                string reply;

                if (ex.InnerException == null)
                {
                    reply = "You have canceled the operation. Quitting from the HotelsDialog";
                }
                else
                {
                    reply = $"Oops! Something went wrong :( Technical Details: {ex.InnerException.Message}";
                }

                await context.PostAsync(reply);
            }
            finally
            {
                context.Done<object>(null);
            }
        }

        private async Task<IEnumerable<Rate>> GetRatesAsync(OpenQuery searchQuery)
        {
            var rates = new List<Rate>();

            // Filling the institutions results manually just for demo purposes

            //list of institituions
            List<string> institutions = new List<string>();
            institutions.Add("The Brothers Institution");
            institutions.Add("The Vulchers Institution");
            institutions.Add("The Kids Institution");
            institutions.Add("The Monopoly Institution");
            institutions.Add("The Crazy Institution");

            for (int i = 0; i < 5; i++)
            {
                var random = new Random(i);
                Rate rate = new Rate()
                {
                    Name = institutions[i],
                    //Location = searchQuery.Destination,
                    Rating = random.Next(1, 5),
                    NumberOfReviews = random.Next(0, 5000),
                    PriceStarting = random.Next(0, 100) / 100,
                    Image = $"https://placeholdit.imgix.net/~text?txtsize=35&txt=Hotel+{i}&w=500&h=260"
                };

                rates.Add(rate);
            }

            rates.Sort((h1, h2) => h1.PriceStarting.CompareTo(h2.PriceStarting));

            return rates;
        }

    }

}