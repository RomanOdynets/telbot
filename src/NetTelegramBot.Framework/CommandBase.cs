﻿using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NetTelegramBot.Framework.Abstractions;
using NetTelegramBotApi.Types;

namespace NetTelegramBot.Framework
{
    public abstract class CommandBase<TCommandArgs> : ICommand<TCommandArgs>
        where TCommandArgs : ICommandArgs, new()
    {
        /// <summary>
        /// Command name without leading '/'
        /// </summary>
        public string Name { get; }

        public IBot Bot { get; set; }

        protected CommandBase(string name)
        {
            Name = name;
        }

        public virtual bool CanHandle(Update update)
        {
            var canHandle = false;
            if (!string.IsNullOrEmpty(update.Message.Text))
            {
                canHandle = Regex.IsMatch(update.Message.Text,
                    $@"^/{Name}(?:@{Bot.BotUserInfo.Username})?\s*", RegexOptions.IgnoreCase);
            }
            return canHandle;
        }

        public virtual TCommandArgs ParseInput(Update update)
        {
            return new TCommandArgs { RawInput = update.Message.Text };
        }

        public virtual async Task<UpdateHandlingResult> HandleUpdateAsync(Update update)
        {
            var args = ParseInput(update);
            return await HandleCommand(update, args);
        }

        public abstract Task<UpdateHandlingResult> HandleCommand(Update update, TCommandArgs args);
    }
}
