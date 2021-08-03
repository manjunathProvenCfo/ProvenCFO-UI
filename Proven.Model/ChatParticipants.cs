﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public class ChatParticipants
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfileImage { get; set; }
        public string Email { get; set; }
        public string TwilioUserId { get; set; }
        public string[] AgencyNames { get; set; }
        public string ConversationId { get; set; }
        public string[] ParticipantIds { get; set; }
        public string ChannelId { get; set; }
        public string ChannelUniqueName { get; set; }
    }
}