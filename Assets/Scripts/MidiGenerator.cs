using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Tools;
using Melanchall.DryWetMidi.MusicTheory;
using System.Linq;
using System.IO;

public class MidiGenerator : MonoBehaviour {

    public bool generateOnStart = false;
    public string fileName = "midifile.mid";
    private MidiReader midiReader = null;
    public int notesToOutput = 100; //Todo: Change to Metric Time
    public string outputName = "output.mid";
    public int bpm = 120;
    public int maxPauseLength = 500;

    void Start() {
        if (generateOnStart) {
            //GenerateMidi();
        }
    }

    public void GenerateMidi(string input = "", string output = "") {
        midiReader = new MidiReader(input);

        var midiFile = new MidiFile();

        var tempoMap = midiReader.tempoMap;
        var trackChunk1 = BuildTrackChunk(tempoMap);
        midiFile.Chunks.Add(trackChunk1);
        midiFile.ReplaceTempoMap(tempoMap);

        int fileNumber = 0;
        while (File.Exists(output + "/Generated Midi " + fileNumber.ToString("000") + ".mid")) {
            fileNumber++;
        }
        midiFile.Write(output + "/Generated Midi " + fileNumber.ToString("000") + ".mid");
    }

    private TrackChunk BuildTrackChunk(TempoMap tempoMap) {

            var patternBuilder = new PatternBuilder();

            NoteInfo currentNoteInfo = midiReader.GetRandomNote();
            DurationInfo currentDurationInfo = midiReader.GetRandomDuration();

            int currentNoteCount = 0;
            while (currentNoteCount < notesToOutput) {
                BuildChord(currentNoteInfo, currentDurationInfo, patternBuilder);
                BuildPause(currentDurationInfo, patternBuilder);
                currentNoteInfo = currentNoteInfo.GetRandomNextNote();
                currentDurationInfo = currentDurationInfo.GetRandomNextDuration();
            if (currentNoteInfo == null) {
                Debug.LogWarning("No consecutive Note found!");
                break;
            }
            currentNoteCount++;
            }


        // Build pattern into an instance of the Pattern class
        TrackChunk trackChunk = patternBuilder.Build().ToTrackChunk(tempoMap);

        return trackChunk;
    }

    private void BuildChord(NoteInfo noteInfo, DurationInfo durationInfo, PatternBuilder patternBuilder) {
        MidiTimeSpan timeSpan = durationInfo.GetTimeSpan();
        patternBuilder.SetNoteLength(timeSpan);
        patternBuilder.SetOctave(Octave.Get(noteInfo._octave));
        patternBuilder.Note(noteInfo._noteName);
        foreach (NoteInfo.ConcurrentNote cn in noteInfo._concurrentNotes) {
            patternBuilder.StepBack(timeSpan);
            patternBuilder.SetOctave(Octave.Get(cn._octave));
            patternBuilder.Note(cn._noteName);
        }
    }

    private void BuildPause(DurationInfo durationInfo, PatternBuilder patternBuilder) {
        MidiTimeSpan pause = durationInfo.GetRandomPause();
        if (pause != null && pause.TimeSpan < maxPauseLength) {
            patternBuilder.StepForward(pause);
        }
    }
}
