using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// This file contains few classes referred by SerializableClass, Track and Pattern.
// We don't need the full implementation as the converter doesn't need them, just enough
// definition for the project to compile.

class TrackV1
{
    public const string kVersion = "1";
}

class TrackV2
{
    public const string kVersion = "2";
}

public enum Judgement
{
    Nothing
}