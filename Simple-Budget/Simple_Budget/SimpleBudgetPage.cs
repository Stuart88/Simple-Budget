using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace Simple_Budget
{
    class SimpleBudgetPage : ContentPage
    {


        //Initialise UI points
        DatePicker startDate = new DatePicker
        {
            //Format = "D",
            Date = DateTime.Now,
            HorizontalOptions = LayoutOptions.StartAndExpand,
            VerticalOptions = LayoutOptions.StartAndExpand,
            TextColor = Color.Black,
            IsEnabled = false,
        };
        public DatePicker endDate = new DatePicker
        {
            //Format = "D",
            Date = DateTime.Now.AddMonths(3),
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.StartAndExpand,
            TextColor = Color.Black,
        };
        Entry budgetEntry = new Entry
        {
            Placeholder = "Enter amount",
            Keyboard = Keyboard.Numeric,
            HorizontalOptions = LayoutOptions.End,
            BackgroundColor = Color.White,
            TextColor = Color.Black,
            PlaceholderColor = Color.Red,
        };
        Entry expenseEntry = new Entry
        {
            Placeholder = "Add Funds/Expense",
            Keyboard = Keyboard.Numeric,
            HorizontalOptions = LayoutOptions.Start,
            BackgroundColor = Color.White,
            TextColor = Color.Black,
            PlaceholderColor = Color.Red,
            IsEnabled = false,
            IsVisible = false,
        };
        Picker currencyPicker = new Picker
        {
            Title = "Select Currency",
            TextColor = Color.Black,
            BackgroundColor = Color.White,
            HorizontalOptions = LayoutOptions.End,
        };
        Button budgetCalculateButton, addExpenseButton, addFundsButton, updateButton, infoButton, resetButton;

        //Initialise data and layout
        List<string> currencies = new List<string>
        {
            "\u0024", "£", "\u20AC", "\u00A5", "\u20B9", "\u20A9" , "\u20AD", "\u20BA", "\u0E3F", "元", "¤",
           //dollar, pound, euro,     Yen,     Rupee,     Won,        Kip,    Lira,     Baht,     Yuan, generic
        };
        double dailyAvailable;
        StackLayout budgetList = new StackLayout();
        StackLayout mainLayout;
        Label dailyAvailableLabel = new Label
        {
            TextColor = Color.Black,
            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
        };
        Label remainingBudget = new Label
        {
            TextColor = Color.Black,
            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
        };
        double spentToday = 0;
        Label DailySpent = new Label
        {
            IsVisible = false,
            FontAttributes = FontAttributes.Italic,
            TextColor = Color.Black,
            BackgroundColor = Color.White,
            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
        };
        double remaining = 0;
        Label Remaining = new Label
        {
            IsVisible = false,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.Black,
            BackgroundColor = Color.White,
            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
        };
        DateTime dateSave;

        

        // CONSTRUCTOR
        public SimpleBudgetPage()
        {            
            //Buttons and clicks
            budgetCalculateButton = new Button
            {
                Text = "Calculate Budget",
                TextColor = Color.Red,
                BackgroundColor = Color.Black,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center,
                BorderColor = Color.Red,
                BorderWidth = 2,
                IsVisible = true,
                IsEnabled = true,
            };
            budgetCalculateButton.Clicked += OnBudgetButtonClicked;

            addExpenseButton = new Button
            {
                Text = " - ",
                TextColor = Color.White,
                BackgroundColor = Color.Red,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Start,
                IsEnabled = false,
                IsVisible = false,
            };
            addExpenseButton.Clicked += OnAddExpenseButtonClicked;

            addFundsButton = new Button
            {
                Text = " + ",
                TextColor = Color.White,
                BackgroundColor = Color.LightGreen,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Start,
                IsEnabled = false,
                IsVisible = false,
            };
            addFundsButton.Clicked += OnAddFundsButtonClicked;

            updateButton = new Button
            {
                Text = "Update",
                BackgroundColor = Color.Yellow,
                TextColor = Color.Black,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                HorizontalOptions = LayoutOptions.End,
                IsEnabled = false,
                IsVisible = false,
            };
            updateButton.Clicked += OnUpdateButtonClicked;

            infoButton = new Button
            {
                Text = "info",
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = Color.DarkBlue,
                FontAttributes = FontAttributes.Italic,
                BackgroundColor = Color.White,
                HorizontalOptions = LayoutOptions.End,

            };
            infoButton.Clicked += OnInfoButtonPressed;

            resetButton = new Button
            {
                IsVisible = false,
                IsEnabled = false,
                Text = "Reset",
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = Color.Yellow,
                BackgroundColor = Color.Red,
                HorizontalOptions = LayoutOptions.End,
            };
            resetButton.Clicked += OnResetButton;

            //Initialise stored info
            for (int i = 0; i < currencies.Count; i++)
            {
                currencyPicker.Items.Add(currencies[i]);
            }
            currencyPicker.SelectedIndex = 1;
            
            BackgroundColor = Color.Black;
            mainLayout = new StackLayout
            {
                Padding = new Thickness(5, Device.OnPlatform(25, 5, 5), 5, 5),
                Spacing = 0,
                // MAIN SCREEN LAYOUT
                Children =
                {
                    // 0 Title
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            new Label
                            {
                                Text = "Simple Budget",
                                TextColor = Color.Yellow,
                                FontSize = Device.GetNamedSize(NamedSize.Large,typeof(Label)),
                                FontAttributes = FontAttributes.Bold,
                                HorizontalOptions = LayoutOptions.CenterAndExpand,
                                VerticalOptions = LayoutOptions.Center,
                            },
                            infoButton,

                        }
                    },
                    
                    // 1 Start/End date labels
                    new StackLayout
                    {

                        Orientation = StackOrientation.Horizontal,
                        VerticalOptions = LayoutOptions.Start,
                        BackgroundColor = Color.White,
                        Children =
                        {
                            new Label
                            {
                                Text = "Start Date (today)",
                                HorizontalOptions = LayoutOptions.StartAndExpand,
                                TextColor = Color.Black,

                            },
                            new Label
                            {
                                Text = "End Date",
                                HorizontalOptions = LayoutOptions.End,
                                TextColor = Color.Black,
                            },
                        }
                    },
                    // 2 DatePicker boxes
                    new StackLayout
                    {

                        Orientation = StackOrientation.Horizontal,
                        VerticalOptions = LayoutOptions.Start,
                        BackgroundColor = Color.White,
                        Children =
                        {
                            startDate,
                            endDate,
                        }
                    },
                    // 3 Budget label and input
                    new StackLayout
                    {

                        Orientation = StackOrientation.Horizontal,
                        VerticalOptions = LayoutOptions.Start,
                        BackgroundColor = Color.White,
                        Children =
                        {
                            new Label
                            {
                                Text = "Budget: ",
                                TextColor = Color.Black,
                                FontSize = Device.GetNamedSize(NamedSize.Large,typeof(Label)),
                                HorizontalOptions = LayoutOptions.StartAndExpand,
                            },
                            budgetEntry,

                        }
                    },
                    // 4 Current selection
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        BackgroundColor = Color.White,
                        Children =
                        {
                            new Label
                            {
                                Text = "Select Currency: ",
                                TextColor = Color.Black,
                                BackgroundColor = Color.White,
                                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                                HorizontalOptions = LayoutOptions.StartAndExpand,
                            },
                            currencyPicker,
                           
                        }
                    },
                    // 5 Budget calulation button
                    new StackLayout
                    {
                        Children =
                        {
                            budgetCalculateButton,
                            updateButton,
                        }
                    },
                    // 6 Budget output
                    new ScrollView
                    {
                        Content = budgetList,
                        VerticalOptions = LayoutOptions.StartAndExpand,
                    },
                    // 7 Update and rest button
                    new StackLayout
                    {
                        Children =
                        {
                            resetButton,
                        }
                    },
                    
                    // 8 Daily Bugdet
                    new StackLayout
                    {
                        IsVisible = false,
                        BackgroundColor = Color.White,
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            new Label
                            {
                                Text = string.Format("Daily Budget: {0}",currencyPicker.SelectedItem),
                                TextColor = Color.Black,
                                FontSize = Device.GetNamedSize(NamedSize.Large,typeof(Label)),
                            },
                            dailyAvailableLabel,
                        },


                    },
                    // 9 Spent today
                    DailySpent,
                    // 10 Remaining today
                    Remaining,
                    // 11 Funds/Expense input buttons etc
                    new StackLayout
                    {
                        IsVisible = false,
                        HorizontalOptions = LayoutOptions.Start,
                        BackgroundColor = Color.White,
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                        expenseEntry,
                        addExpenseButton,
                        addFundsButton,
                        }
                    },
                    new Label
                    {
                        BackgroundColor = Color.White,
                        TextColor = Color.Black,
                    }



        },
                
            };
            Content = mainLayout;

            //Load previous data
            Start();

        }



        void OnBudgetButtonClicked(object sender, EventArgs args)
        {
            BudgetButton();
            
            
        }
        void BudgetButton()
        {

            // Work out new budget
            double budget;
            double dateDiff = 1 + (endDate.Date - startDate.Date).TotalDays;


            if (dateDiff <= 0)
            {
                DisplayAlert("Alert", "Invalid dates", "OK");

            }
            else if (!double.TryParse(budgetEntry.Text, out budget))
            {
                DisplayAlert("Alert", "Enter a valid budget amount", "OK");
            }
            else
            {
                dailyAvailable = budget / dateDiff;


                expenseEntry.IsVisible = true;
                expenseEntry.IsEnabled = true;
                addExpenseButton.IsVisible = true;
                addExpenseButton.IsEnabled = true;
                budgetCalculateButton.IsVisible = false;
                budgetCalculateButton.IsEnabled = false;
                addFundsButton.IsVisible = true;
                addFundsButton.IsEnabled = true;
                updateButton.IsVisible = true;
                updateButton.IsEnabled = true;
                mainLayout.Children[8].IsVisible = true;
                mainLayout.Children[9].IsVisible = true;
                mainLayout.Children[10].IsVisible = true;
                mainLayout.Children[11].IsVisible = true;
                resetButton.IsVisible = true;
                resetButton.IsEnabled = true;

                spentToday = Math.Round(spentToday, 2);
                DailySpent.Text = string.Format("  Spent today: {1}{0}", spentToday, currencyPicker.SelectedItem);
                Remaining.Text = string.Format("   Remaining: {1}{0}", Math.Round(remaining, 2), currencyPicker.SelectedItem);
                remainingBudget.Text = Math.Round(double.Parse(budgetEntry.Text),2).ToString();
                budgetEntry.Text = Math.Round(double.Parse(budgetEntry.Text), 2).ToString();

                dailyAvailableLabel.Text = Math.Round(dailyAvailable, 2).ToString();
                
            }

            Update();
        }

        void OnAddExpenseButtonClicked(object sender, EventArgs args)
        {
            double budget;
            double dateDiff = 1+ (endDate.Date - DateTime.Now.Date).TotalDays;

            // Check expense entry box has valid data
            if (!double.TryParse(expenseEntry.Text, out budget))
            {
                DisplayAlert("Alert", "Enter a valid amount", "OK");
            }
            else if (!double.TryParse(budgetEntry.Text, out budget))
            {
                DisplayAlert("Alert", "Budget value is null", "OK");
            }
            else
            {
                // Calculate new values
                double newExpense = double.Parse(expenseEntry.Text);
                budget = double.Parse(budgetEntry.Text);
                budget -= newExpense;
                budgetEntry.Text = budget.ToString();
                budgetEntry.Text = Math.Round(double.Parse(budgetEntry.Text), 2).ToString();
                dailyAvailable = budget / dateDiff;

                //Add expense to ScrollView
                budgetList.Children.Insert(0, new Label
                {
                    Text = DateTime.Now.ToString("dd/MM/yyyy") + ":     - "
                            + currencyPicker.SelectedItem + " " + Math.Round(newExpense, 2).ToString(),
                    FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                    TextColor = Color.Red,
                });

                // Update 'daily spent' amount
                spentToday += newExpense;
                spentToday = Math.Round(spentToday, 2);
                DailySpent.Text = string.Format("  Spent today: {1} {0}", spentToday, currencyPicker.SelectedItem);
                if (spentToday > dailyAvailable)
                    DailySpent.TextColor = Color.Red;
                else
                    DailySpent.TextColor = Color.Black;

                // Update 'remaining' amount
                remaining = dailyAvailable - spentToday;
                Remaining.Text = string.Format("   Remaining today: {1} {0}", Math.Round(remaining, 2), currencyPicker.SelectedItem);
                if (remaining < 0)
                    Remaining.TextColor = Color.Red;
                else
                    Remaining.TextColor = Color.Black;

                //update labels showing budget amounts
                dailyAvailableLabel.Text = Math.Round(dailyAvailable, 2).ToString();
                remainingBudget.Text = Math.Round(double.Parse(budgetEntry.Text),2).ToString();

                

            }


            Update();
        }

        void OnAddFundsButtonClicked(object sender, EventArgs args)
        {
            double budget;
            double dateDiff = 1+(endDate.Date - DateTime.Now.Date).TotalDays;

            // Check expense entry box has valid data
            if (!double.TryParse(expenseEntry.Text, out budget))
            {
                DisplayAlert("Alert", "Enter a valid amount", "OK");
            }
            else if(!double.TryParse(budgetEntry.Text, out budget))
            {
                DisplayAlert("Alert", "Budget value is null", "OK");
            }
            else
            {
                
                // Calculate new values
                double newFunds = double.Parse(expenseEntry.Text);
                budget = double.Parse(budgetEntry.Text);
                budget += newFunds;
                budgetEntry.Text = budget.ToString();
                budgetEntry.Text = Math.Round(double.Parse(budgetEntry.Text), 2).ToString();
                dailyAvailable = budget / dateDiff;

                //Add new funds to ScrollView
                budgetList.Children.Insert(0, new Label
                {
                    Text = DateTime.Now.ToString("dd/MM/yyyy") + ":     + "
                            + currencyPicker.SelectedItem + " " + Math.Round(newFunds, 2).ToString(),
                    FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                    TextColor = Color.Green,
                });

                // Update 'daily spent' amount
                spentToday -= newFunds;
                spentToday = Math.Round(spentToday, 2);
                spentToday = Math.Round(spentToday, 2);
                DailySpent.Text = string.Format("  Spent today: {1} {0}", spentToday, currencyPicker.SelectedItem);
                if (spentToday > dailyAvailable)
                    DailySpent.TextColor = Color.Red;
                else
                    DailySpent.TextColor = Color.Black;

                // Update 'remaining' amount
                remaining = dailyAvailable - spentToday;
                Remaining.Text = string.Format("   Remaining today: {1} {0}", Math.Round(remaining,2), currencyPicker.SelectedItem);
                if (remaining < 0)
                    Remaining.TextColor = Color.Red;
                else
                    Remaining.TextColor = Color.Black;

                //update labels showing budget amounts
                dailyAvailableLabel.Text = Math.Round(dailyAvailable, 2).ToString();
                remainingBudget.Text = Math.Round(double.Parse(budgetEntry.Text),2).ToString();

                
            }


            Update();
        }

        void OnUpdateButtonClicked(object sender, EventArgs args)
        {
            Update();  
        }

        void Update()
        {
            double budget;
            if (double.TryParse(budgetEntry.Text, out budget))
            {
                double dateDiff = (endDate.Date - DateTime.Now.Date).TotalDays;
                dailyAvailable = budget / dateDiff;
                spentToday = Math.Round(spentToday, 2);
                dailyAvailableLabel.Text = Math.Round(dailyAvailable, 2).ToString();
                remaining = dailyAvailable - spentToday;
                remainingBudget.Text = Math.Round(double.Parse(budgetEntry.Text), 2).ToString();
                ((Label)(((StackLayout)(mainLayout.Children[8])).Children[0])).Text = string.Format("Daily Budget: {0}", currencyPicker.SelectedItem);
                Remaining.Text = string.Format("   Remaining today: {1} {0}", Math.Round(remaining, 2), currencyPicker.SelectedItem);
                DailySpent.Text = string.Format("  Spent today: {1} {0}", spentToday, currencyPicker.SelectedItem);
                if (remaining < 0)
                    Remaining.TextColor = Color.Red;
                else
                    Remaining.TextColor = Color.Black;
                if (spentToday > dailyAvailable)
                    DailySpent.TextColor = Color.Red;
                else
                    DailySpent.TextColor = Color.Black;

               

            }
            Save();
        }

        void OnInfoButtonPressed(object sender, EventArgs args)
        {
            DisplayAlert("Simple Bugdet", "Enter primary budget information at top of screen.\n\n" +
                "Enter new funds and expenses at bottom of screen.\n\n" +
                "To change primary budget information, alter details at top of screen then press update button." +
                "\n\nBy Stuart Aitken", "OK");


            Update();

        }

        public void Save()
        {
            App app = Application.Current as App;

            app.Properties.Remove("endDate");
            app.Properties.Add("endDate", endDate.Date);
            app.Properties.Remove("budget");
            app.Properties.Add("budget", budgetEntry.Text);
            app.Properties.Remove("currency");
            app.Properties.Add("currency", currencyPicker.SelectedItem);


            app.Properties.Remove("calculateButton1");
            app.Properties.Add("calculateButton1", budgetCalculateButton.IsEnabled);
            app.Properties.Remove("calculateButton2");
            app.Properties.Add("calculateButton2", budgetCalculateButton.IsVisible);

            app.Properties.Remove("updateButton1");
            app.Properties.Add("updateButton1", updateButton.IsEnabled);
            app.Properties.Remove("updateButton2");
            app.Properties.Add("updateButton2", updateButton.IsVisible);

            app.Properties.Remove("budgetRemaining1");
            app.Properties.Add("budgetRemaining1", remainingBudget.Text);
            app.Properties.Remove("budgetRemaining2");
            app.Properties.Add("budgetRemaining2", remainingBudget.IsVisible);

            app.Properties.Remove("dailyBudget");
            app.Properties.Add("dailyBudget", dailyAvailableLabel.IsVisible);
            app.Properties.Remove("dailyBudgetAmount");
            app.Properties.Add("dailyBudgetAmount", dailyAvailable);

            app.Properties.Remove("expenseEntry1");
            app.Properties.Add("expenseEntry1", expenseEntry.IsEnabled);
            app.Properties.Remove("expenseEntry2");
            app.Properties.Add("expenseEntry2", expenseEntry.IsVisible);

            app.Properties.Remove("expenseButton1");
            app.Properties.Add("expenseButton1", addExpenseButton.IsEnabled);
            app.Properties.Remove("expenseButton2");
            app.Properties.Add("expenseButton2", addExpenseButton.IsVisible);

            app.Properties.Remove("fundsButton1");
            app.Properties.Add("fundsButton1", addFundsButton.IsEnabled);
            app.Properties.Remove("fundsButton2");
            app.Properties.Add("fundsButton2", addFundsButton.IsVisible);

            app.Properties.Remove("remaining");
            app.Properties.Add("remaining", remaining);
            app.Properties.Remove("spent");
            app.Properties.Add("spent", spentToday);
            app.Properties.Remove("remainButton");
            app.Properties.Add("remainButton", Remaining.IsVisible);
            app.Properties.Remove("spentButton");
            app.Properties.Add("spentButton", DailySpent.IsVisible);

            app.Properties.Remove("reset1");
            app.Properties.Add("reset1", resetButton.IsEnabled);
            app.Properties.Remove("reset2");
            app.Properties.Add("reset2", resetButton.IsVisible);

            dateSave = DateTime.Now.Date;
            app.Properties.Remove("date");
            app.Properties.Add("date", dateSave);

            
        }

        void Start()
        {
            App app = Application.Current as App;

            if (app.Properties.ContainsKey("remaining"))
                remaining = (double)app.Properties["remaining"];
            if (app.Properties.ContainsKey("remainButton"))
                Remaining.IsVisible = (bool)app.Properties["remainButton"];

            if (app.Properties.ContainsKey("spent"))
                spentToday = (double)app.Properties["spent"];
            if (app.Properties.ContainsKey("spentButton"))
                DailySpent.IsVisible = (bool)app.Properties["spentButton"];

            if (app.Properties.ContainsKey("endDate"))
                endDate.Date = (DateTime)app.Properties["endDate"];

            if (app.Properties.ContainsKey("date"))
                dateSave = (DateTime)app.Properties["date"];
            //Check if it's a new day or not.
            //If so, reset the 'spent today' field
            if ((dateSave.Date.DayOfWeek != DateTime.Now.Date.DayOfWeek))
            {
                spentToday = 0;
            }
           
            if (app.Properties.ContainsKey("budget"))
                budgetEntry.Text = (string)app.Properties["budget"];

            if (app.Properties.ContainsKey("currency"))
                currencyPicker.SelectedItem = (object)app.Properties["currency"];


            if (app.Properties.ContainsKey("calculateButton1"))
                budgetCalculateButton.IsEnabled = (bool)app.Properties["calculateButton1"];
            if (app.Properties.ContainsKey("calculateButton2"))
                budgetCalculateButton.IsVisible = (bool)app.Properties["calculateButton2"];

            if (app.Properties.ContainsKey("reset1"))
                resetButton.IsEnabled = (bool)app.Properties["reset1"];
            if (app.Properties.ContainsKey("reset2"))
                resetButton.IsVisible = (bool)app.Properties["reset2"];
            

            if (!budgetCalculateButton.IsVisible)
                BudgetButton();

            if (app.Properties.ContainsKey("updateButton1"))
                updateButton.IsEnabled = (bool)app.Properties["updateButton1"];
            if (app.Properties.ContainsKey("updateButton2"))
                updateButton.IsVisible = (bool)app.Properties["updateButton2"];

            if (app.Properties.ContainsKey("budgetRemaining1"))
                remainingBudget.Text = (string)app.Properties["budgetRemaining1"];
            if (app.Properties.ContainsKey("budgetRemaining2"))
                remainingBudget.IsVisible = (bool)app.Properties["budgetRemaining2"];

            if (app.Properties.ContainsKey("dailtyBudget"))
                dailyAvailableLabel.IsVisible = (bool)app.Properties["dailyBudget"];

            if (app.Properties.ContainsKey("dailyBudgetAmount"))
                dailyAvailable = (double)app.Properties["dailyBudgetAmount"];

            if (app.Properties.ContainsKey("expenseEntry1"))
                expenseEntry.IsEnabled = (bool)app.Properties["expenseEntry1"];
            if (app.Properties.ContainsKey("expenseEntry2"))
                expenseEntry.IsVisible = (bool)app.Properties["expenseEntry2"];

            if (app.Properties.ContainsKey("expenseButton1"))
                addExpenseButton.IsEnabled = (bool)app.Properties["expenseButton1"];
            if (app.Properties.ContainsKey("expenseButton2"))
                addExpenseButton.IsVisible = (bool)app.Properties["expenseButton2"];

            if (app.Properties.ContainsKey("fundsButton1"))
                addFundsButton.IsEnabled = (bool)app.Properties["fundsButton1"];
            if (app.Properties.ContainsKey("fundsButton2"))
                addFundsButton.IsVisible = (bool)app.Properties["fundsButton2"];

            if (app.Properties.ContainsKey("budgetScreen"))
                budgetList = (StackLayout)app.Properties["budgetScreen"];

            

            
            




            Update();
        }

        void OnResetButton(object sender, EventArgs args)
        {
            Reset();
        }

        async void Reset()
        {
            string action = await DisplayActionSheet("Reset all values - Are you sure?", null, null, "     Yes", "     No");
            switch (action)
            {
                case "     Yes":
                    Application.Current.Properties.Clear();
                    remaining = 0;
                    dailyAvailable = 0;
                    spentToday = 0;
                    budgetEntry.Text = "0";
                    Start();
                    break;
                case "     No":
                    break;
            }
            
        }

    }

}
