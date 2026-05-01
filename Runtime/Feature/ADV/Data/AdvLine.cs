using System;
using System.Collections.Generic;
using System.Linq;

namespace MyArchitecture.Feature.ADV
{
    public sealed class AdvLine
    {
        private readonly IReadOnlyList<string> _tags;

        public AdvLine(
            string speakerId,
            string speakerName,
            string body,
            string textKey = null,
            string voiceKey = null,
            IEnumerable<string> tags = null)
        {
            SpeakerId = speakerId;
            SpeakerName = speakerName;
            Body = body;
            TextKey = textKey;
            VoiceKey = voiceKey;
            _tags = tags?.ToArray() ?? Array.Empty<string>();
        }

        public string SpeakerId { get; }
        public string SpeakerName { get; }
        public string Body { get; }
        public string TextKey { get; }
        public string VoiceKey { get; }
        public IReadOnlyList<string> Tags => _tags;
    }
}