using System;
using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotGDev
{  
    public class Program
    {
        static ReplyKeyboardMarkup replyKeyboard;
        static ReplyKeyboardRemove replyKeyboardRemove;
        static InlineKeyboardMarkup inlineKeyboard;

        static ITelegramBotClient botClient;
        static ConcurrentDictionary<long, User> usersDictionary;

        public static void Main(string[] args)
        {
            replyKeyboard = new[] { Texts.ACCEPT, Texts.CANCEL };
            replyKeyboardRemove = new ReplyKeyboardRemove();

            replyKeyboard.OneTimeKeyboard = true;
            replyKeyboard.ResizeKeyboard = true;

            botClient = new TelegramBotClient("---BOT API HERE---");
            usersDictionary = new ConcurrentDictionary<long, User>();

            var me = botClient.GetMeAsync().Result;
            Console.WriteLine("Connected: " + me.FirstName);

            botClient.OnMessage += Bot_OnMessage;
            botClient.StartReceiving();

            while (true)
            {
                foreach (User user in usersDictionary.Values)
                {
                    user.CheckFinishedBlock();
                }
            }
        }


        #region EVENT_METHODS
        static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                long chatId = e.Message.Chat.Id;

                //If the user has not used the bot yet, we try to add it to the dictionary
                if (!usersDictionary.ContainsKey(chatId))
                {
                    if (!TryAddNewUser(chatId))
                    {
                        return;
                    }
                }

                User user = usersDictionary[chatId];

                //We check if the message if a command message
                if (!user.IsConfirming)
                {
                    if (CommandMessage(chatId, e))
                    {
                        return;
                    }
                }

                //We check if the daily limit is reached
                if (UserDailyLimitReached(chatId, user))
                {
                    return;
                }

                //If a new publication is just sended
                if (!user.IsConfirming)
                {
                    SendMessageWithKeyboardAndReply(chatId, e.Message.MessageId, replyKeyboard, Texts.CONFIRM_PUBLICATION);
                    user.CreateNewPublication(e);
                }
                else //Is the user is confirming, he/she has to accept the publication
                {
                    UserPublicationConfirmation(chatId, user, e);
                }
            }
        }
        #endregion

        #region USER_METHODS
        static bool TryAddNewUser(long chatId)
        {
            User newUser = new User(chatId);

            //If we couldnt add the user to the concurrent dictionary, a message error is shown an nothing more happens
            if (!usersDictionary.TryAdd(chatId, newUser))
            {
                SendMessage(chatId, Texts.ADD_USER_PROBLEM);
                return false;
            }

            return true;
        }

        static bool UserDailyLimitReached(long chatId, User user)
        {
            if (user.CheckReachedDailyUses())
            {
                int minutesToWait = user.GetMinutesToWait();
                SendMessage(chatId, Texts.MaxPublicationsReachedText(minutesToWait));
                return true;
            }

            return false;
        }

        static void UserPublicationConfirmation(long chatId, User user, MessageEventArgs message)
        {
            switch (message.Message.Text.ToUpper())
            {
                case "ACEPTAR":
                    user.PublicationSended();
                    SendMessage(chatId, Texts.RemainingPublicationsText(user.GetRemainingPublications(), true));
                    SendMessage("--CHANNEL WHERE THE BOT SEND THE MESSAGE HERE--", user.GetLastPublication().Message.Text);
                    break;
                case "CANCELAR":
                    SendMessage(chatId, Texts.RemainingPublicationsText(user.GetRemainingPublications(), false));
                    user.IsConfirming = false;
                    break;
                default:
                    SendMessageWithKeyboardAndReply(chatId, message.Message.MessageId, replyKeyboard, Texts.NOT_UNDERSTOOD);
                    break;
            }
        }
        #endregion

        static bool CommandMessage(long chatId, MessageEventArgs message)
        {
            switch (message.Message.Text)
            {
                case "/start":
                    SendMessage(chatId, Texts.WELCOME);
                    return true;

                case "/ayuda":
                    SendMessage(chatId, Texts.HELP);
                    return true;

                case "Cancelar":
                    return true;

                case "Aceptar":
                    return true;

                default:
                    return false;
            }
        }

        #region SEND_MESSAGES_METHODS

        static async void SendMessage(long chatId, string messageText)
        {
            await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    text: messageText + "\n"
                );
        }

        static async void SendMessage(string chatId, string messageText)
        {
            await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    text: messageText + "\n"
                );
        }

        static async void SendMessageWithReply(long chatId, int messageId, string messageText)
        {
            await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyToMessageId: messageId,
                    text: messageText + "\n"
                );
        }

        static async void SendMessageWithKeyboardAndReply(long chatId, int messageId, ReplyKeyboardMarkup replyKeyboard, string messageText)
        {
            await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyToMessageId: messageId,
                    text: messageText + "\n",
                    replyMarkup: replyKeyboard
                );
        }
        #endregion
    }
}
