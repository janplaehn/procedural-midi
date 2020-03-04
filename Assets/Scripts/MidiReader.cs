using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.Linq;

public class MidiReader {
    private MidiFile _midiFile = null;
    [HideInInspector] public List<NoteInfo> _noteInfos = new List<NoteInfo>();
    [HideInInspector] public TempoMap tempoMap = null;

    public MidiReader(string path) {
        _midiFile = MidiFile.Read(path);
        ReadMidi();
    }

    public void ReadMidi() {
        tempoMap = _midiFile.GetTempoMap();
        var notes = _midiFile.GetNotes();
        foreach (Note note in notes) {
            ReadNote(note);
        }
        Debug.Break();
    }


    private NoteInfo _secondToLastNoteInfo = null;
    private NoteInfo _lastNoteInfo = null;
    public void ReadNote(Note note) {
        NoteInfo noteInfo = new NoteInfo(note.NoteName, note.Octave, note.TimeAs<MidiTimeSpan>(tempoMap), note.EndTimeAs<MidiTimeSpan>(tempoMap), note.LengthAs<MidiTimeSpan>(tempoMap));
        noteInfo._concurrentNotes = GetConcurrentNotes(noteInfo);
        if (_lastNoteInfo != null) {
            if (IsConcurrent(noteInfo, _lastNoteInfo)) {
                return;
            }
            MidiTimeSpan pause = GetPause(_lastNoteInfo, noteInfo);
            if (IsNoteInList(noteInfo, out NoteInfo clone)) {
                noteInfo = clone;
            }
            _lastNoteInfo.AddPause(pause);
            _lastNoteInfo.AddNextNote(noteInfo);
            _noteInfos.Add(_lastNoteInfo);

        }
        _lastNoteInfo = noteInfo;
        _secondToLastNoteInfo = _lastNoteInfo;
    }

    public List<NoteInfo.ConcurrentNote> GetConcurrentNotes(NoteInfo noteInfo) {
        List<NoteInfo.ConcurrentNote> concurrentNotes = new List<NoteInfo.ConcurrentNote>();
        var notesAtTime = _midiFile.GetNotes().AtTime(noteInfo._startTime, tempoMap);
        foreach (Note note in notesAtTime) {
            NoteInfo.ConcurrentNote concurrentNote;
            concurrentNote._noteName = note.NoteName;
            concurrentNote._octave = note.Octave;
            concurrentNotes.Add(concurrentNote);
        }
        return concurrentNotes;
    }

    public MidiTimeSpan GetPause(NoteInfo first, NoteInfo second) {
        MidiTimeSpan pause;
        if (second._startTime.CompareTo(first._endTime) > 0) {
            pause = (MidiTimeSpan)second._startTime.Subtract(first._endTime, TimeSpanMode.TimeTime);
        }
        else {
            pause = new MidiTimeSpan(0);
        }
        return pause;
    }

    public bool IsConcurrent(NoteInfo infoA, NoteInfo infoB) {
        if (Mathf.Abs(infoA._startTime.TimeSpan - infoB._startTime.TimeSpan) < 10) return true;
        return false;
    }

    bool IsNoteInList(NoteInfo info, out NoteInfo clone) {
        foreach (NoteInfo noteInfo in _noteInfos) {
            if (noteInfo != info
                && noteInfo.GetNoteName() == info.GetNoteName()
                && noteInfo._octave == info._octave
                && CompareListsScrambled<NoteInfo.ConcurrentNote>(noteInfo._concurrentNotes, info._concurrentNotes)) {
                clone = noteInfo;
                return true;
            }
        }
        clone = null;
        return false;
    }

    public NoteInfo GetRandomNote() {
        int r = UnityEngine.Random.Range(0,_noteInfos.Count);
        return _noteInfos[r];
    }

    public static bool CompareListsScrambled<T>(List<T> aListA, List<T> aListB) {
        if (aListA == null || aListB == null || aListA.Count != aListB.Count)
            return false;
        if (aListA.Count == 0)
            return true;
        Dictionary<T, int> lookUp = new Dictionary<T, int>();
        // create index for the first list
        for (int i = 0; i < aListA.Count; i++) {
            int count = 0;
            if (!lookUp.TryGetValue(aListA[i], out count)) {
                lookUp.Add(aListA[i], 1);
                continue;
            }
            lookUp[aListA[i]] = count + 1;
        }
        for (int i = 0; i < aListB.Count; i++) {
            int count = 0;
            if (!lookUp.TryGetValue(aListB[i], out count)) {
                // early exit as the current value in B doesn't exist in the lookUp (and not in ListA)
                return false;
            }
            count--;
            if (count <= 0)
                lookUp.Remove(aListB[i]);
            else
                lookUp[aListB[i]] = count;
        }
        // if there are remaining elements in the lookUp, that means ListA contains elements that do not exist in ListB
        return lookUp.Count == 0;
    }
}
