using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Args;

namespace TelegramBotGDev
{
    class User
    {
        public static readonly int MAX_HOURLY_PUBLICATIONS = 3;
        public static readonly int UNBLOCK_WAIT_MILLISECONDS = 3600000;

        long userChatId;
        bool isConfirming;
        int hourly;
        MessageEventArgs lastPublication;
        DateTime blockedTime;

        public bool IsConfirming { get => isConfirming; set => isConfirming = value; }

        public User(long userChatId)
        {
            this.userChatId = userChatId;
            isConfirming = false;
            hourly = 0;
        }

        public bool CheckReachedDailyUses()
        {
            bool reached = hourly < MAX_HOURLY_PUBLICATIONS ? false : true;
            return reached;
        }

        public int GetMinutesToWait()
        {
            TimeSpan diff = DateTime.Now - blockedTime;
            return (UNBLOCK_WAIT_MILLISECONDS / 60_000) - (int)diff.TotalMinutes;
        }

        public void PublicationSended()
        {
            IsConfirming = false;
            hourly++;

            if (hourly >= MAX_HOURLY_PUBLICATIONS)
            {
                blockedTime = DateTime.Now;
            }
        }

        public void CheckFinishedBlock()
        {
            if (blockedTime != null && hourly >= MAX_HOURLY_PUBLICATIONS)
            {
                TimeSpan diff = DateTime.Now - blockedTime;

                if (diff.TotalMilliseconds >= UNBLOCK_WAIT_MILLISECONDS)
                    hourly = 0;
            }
        }

        public int GetRemainingPublications()
        {
            return MAX_HOURLY_PUBLICATIONS - hourly;
        }

        public void CreateNewPublication(MessageEventArgs publication)
        {
            lastPublication = publication;
            isConfirming = true;
        }

        public MessageEventArgs GetLastPublication()
        {
            return lastPublication;
        }
    }
}
