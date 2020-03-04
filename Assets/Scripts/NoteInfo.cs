using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;

public class NoteInfo{

    public List<NoteInfo> _nextNotes = new List<NoteInfo>();
    public MidiTimeSpan _timeSpan;
    public MidiTimeSpan _startTime;
    public MidiTimeSpan _endTime;
    public NoteName _noteName;
    public int _octave;

    public struct ConcurrentNote {
        public NoteName _noteName;
        public int _octave;
    }
    public List<ConcurrentNote> _concurrentNotes = new List<ConcurrentNote>();
    public List<MidiTimeSpan> _pauses = new List<MidiTimeSpan>();
    public static int idSequence = 0;
    public int id = 0;

    public NoteInfo(NoteName noteName, int octave, MidiTimeSpan startTime, MidiTimeSpan endTime, MidiTimeSpan span) {
        _noteName = noteName;
        _octave = octave;
        _startTime = startTime;
        _endTime = endTime;
        _timeSpan = span;
        id = idSequence;
        idSequence++;
    }

    public NoteName GetNoteName() {
        return _noteName;
    }

    public void AddConcurrentNote(NoteName noteName, int octave) {
        ConcurrentNote newConcurrentNote;
        newConcurrentNote._noteName = noteName;
        newConcurrentNote._octave = octave;
        _concurrentNotes.Add(newConcurrentNote);
    }

    public MidiTimeSpan GetTimeSpan() {
        return _timeSpan;
    }

    public void AddPause(MidiTimeSpan duration) {
        _pauses.Add(duration);
    }

    public void AddNextNote(NoteInfo note) {
        _nextNotes.Add(note);
    }

    private List<int> randomOutputs = new List<int>();
    public NoteInfo GetRandomNextNote() {
        if (_nextNotes.Count == 0){
            return null;
        }
        if (randomOutputs.Count == _nextNotes.Count) randomOutputs.Clear();
        Random.InitState(System.DateTime.Now.Millisecond);
        int r = Random.Range(0, _nextNotes.Count);
        while (randomOutputs.Contains(r)) {
            r = Random.Range(0, _nextNotes.Count);
        }
        Debug.Log(r.ToString() + "/" + _nextNotes.Count.ToString());
        randomOutputs.Add(r);
        return _nextNotes[r];
    }

    private List<int> randomPauseOutputs = new List<int>();
    public MidiTimeSpan GetRandomPause() {
        if (_pauses.Count == 0) return null;
        if (randomPauseOutputs.Count == _pauses.Count) randomPauseOutputs.Clear();
        Random.InitState(System.DateTime.Now.Millisecond);
        int r = Random.Range(0, _pauses.Count);
        while (randomPauseOutputs.Contains(r)) {
            r = Random.Range(0, _pauses.Count);
        }
        randomPauseOutputs.Add(r);
        return _pauses[r];
    }
}
