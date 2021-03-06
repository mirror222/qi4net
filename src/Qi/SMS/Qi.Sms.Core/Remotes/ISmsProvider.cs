﻿using Qi.Sms.Protocol.SendCommands;

namespace Qi.Sms.Remotes
{
    public interface ISmsProvider
    {
        
        void Send(string mobile, string content, SmsFormat type);
        ReceiveSms GetSms(int index);
        void Delete(int smsIndex);
    }
}