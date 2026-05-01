using MyArchitecture.Core;
using UnityEngine;

namespace MyArchitecture.Feature.Rhythm
{
    public abstract class RhythmNotePresenter<TView> :
        EntityPresenter<RhythmNoteId, RhythmNoteViewData, TView>
        where TView : Component, IEntityView<RhythmNoteId>
    {
    }
}
