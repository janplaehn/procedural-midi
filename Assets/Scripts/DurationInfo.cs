using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;

public class DurationInfo {

    public List<DurationInfo> _nextDurations = new List<DurationInfo>();
    public List<MidiTimeSpan> _pauses = new List<MidiTimeSpan>();
    public MidiTimeSpan _span;
    public MidiTimeSpan _startTime;
    public MidiTimeSpan _endTime;

    public DurationInfo(MidiTimeSpan start, MidiTimeSpan end, MidiTimeSpan span) {
        _startTime = start;
        _endTime = end;
        _span = span;
    }

    public MidiTimeSpan GetTimeSpan() {
        return _span;
    }

    public void AddNextDuration(DurationInfo duration) {
        _nextDurations.Add(duration);
    }

    private List<int> randomOutputs = new List<int>();
    public DurationInfo GetRandomNextDuration() {
        if (_nextDurations.Count == 0) {
            return null;
        }
        if (randomOutputs.Count == _nextDurations.Count) randomOutputs.Clear();
        Random.InitState(System.DateTime.Now.Millisecond);
        int r = Random.Range(0, _nextDurations.Count);
        while (randomOutputs.Contains(r)) {
            r = Random.Range(0, _nextDurations.Count);
        }
        Debug.Log(r.ToString() + "/" + _nextDurations.Count.ToString());
        randomOutputs.Add(r);
        return _nextDurations[r];
    }

    public void AddPause(MidiTimeSpan duration) {
        _pauses.Add(duration);
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
