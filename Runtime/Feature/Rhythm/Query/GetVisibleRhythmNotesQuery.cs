using System.Collections.Generic;
using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public sealed class GetVisibleRhythmNotesQuery :
        Query<IReadOnlyList<KeyValuePair<RhythmNoteId, RhythmNoteViewData>>>
    {
        private readonly IReadOnlyRhythmNoteModel _noteModel;

        public GetVisibleRhythmNotesQuery(
            IReadOnlyRhythmNoteModel noteModel)
        {
            _noteModel = noteModel;
        }

        protected override IReadOnlyList<KeyValuePair<RhythmNoteId, RhythmNoteViewData>> OnExecute()
        {
            return _noteModel.Snapshot();
        }
    }
}
