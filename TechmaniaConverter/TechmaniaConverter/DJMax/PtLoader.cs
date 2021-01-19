using DJMaxEditor.DJMax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechmaniaConverter.DJMax
{
    class PtLoader
    {
        private static PlayerData Load(string filename)
        {
            UInt32 calcMinTick = ~0u;

            PlayerData playerData = new PlayerData();

            byte[] buff = File.ReadAllBytes(filename);
            if (buff == null) { throw new Exception("File is empty."); }

            DJMaxEditor.ByteArrayStream stream = new DJMaxEditor.ByteArrayStream(buff);

            string sign = stream.ReadString(4);

            if (sign != "PTFF")
            {
                throw new Exception("Did not find 'PTFF' in header.");
            }

            byte version = stream.ReadByte();
            byte un = stream.ReadByte();
            UInt16 tickPerMinute = stream.ReadUShort();
            float tempo2 = stream.ReadFloat();
            UInt16 tracksCount = stream.ReadUShort();
            UInt32 headerEndTick = stream.ReadUInt();
            float trackDuration = stream.ReadFloat();

            playerData.Version = version;
            playerData.TrackDuration = trackDuration;
            playerData.TickPerMinute = tickPerMinute;
            playerData.Tempo = tempo2;
            playerData.HeaderEndTick = headerEndTick;

            stream.Seek(0x16);
            ushort insCnt = stream.ReadUShort();

            stream.Seek(0x18);

            playerData.Clear();

            playerData.Instruments.Add(new InstrumentData()
            {
                InsNum = 0,
                Name = "none"
            });

            // read ogg instruments list
            for (int i = 0; i < insCnt; i++)
            {

                ushort insNo = 0;
                UInt16 unknown1 = 0;
                UInt16 unknown2 = 0;
                if (version == 1)
                {
                    insNo = stream.ReadUShort();
                    unknown1 = stream.ReadByte();
                    unknown2 = stream.ReadByte();
                }
                else
                {
                    insNo = stream.ReadByte();
                    unknown1 = stream.ReadByte();
                }

                if (insNo > 1000)
                {
                    // Is this really an issue?
                    // throw new Exception("Does not support more than 1000 instruments.");
                }

                string oggName = stream.ReadString(0x40);

                playerData.Instruments.Add(new InstrumentData()
                {
                    InsNum = insNo,
                    Name = oggName
                });
            }

            uint eventIndex = 0;
            uint trackIndex = 0;
            while (true)
            {

                if (stream.Available < 4)
                {
                    break;
                }

                uint eztr = stream.ReadUInt();
                if (eztr != 0x52545A45)
                {
                    throw new Exception("Did not find 'EZTR' in track header.");
                }

                stream.Skip(0x02);

                string trackName = stream.ReadString(0x40);

                uint endTick = stream.ReadUInt();
                int blockSize = stream.ReadInt();

                if (version == 1)
                {
                    ushort un1 = stream.ReadUShort();
                }

                int eventSize = version == 1 ? 0x10 : 0x0B;

                int eventsCount = (int)(double)(blockSize / eventSize);

                TrackData track = new TrackData(trackIndex)
                {
                    TrackName = trackName
                };

                playerData.Tracks.AddTrack(track);

                for (int i = 0; i < eventsCount; i++)
                {

                    int tick = stream.ReadInt();
                    byte id = stream.ReadByte();

                    if (version == 1)
                    {
                        byte[] extraData = stream.ReadBytes(0xB);


                        switch ((EventType)id)
                        {
                            case EventType.None:
                                break;
                            case EventType.Note:
                                {

                                    ushort insNo = BitConverter.ToUInt16(extraData, 3);
                                    byte vel = extraData[5];
                                    byte pan = extraData[6];

                                    byte attribute = extraData[7];
                                    ushort duration = BitConverter.ToUInt16(extraData, 8);

                                    InstrumentData inst =
                                        playerData.Instruments.SingleOrDefault(ins => ins.InsNum == insNo);

                                    EventData newEvent = new EventData()
                                    {
                                        TrackId = trackIndex,
                                        Tick = tick,
                                        Attribute = attribute,
                                        Duration = duration,
                                        EventType = EventType.Note,
                                        Instrument = inst,
                                        Vel = vel,
                                        Pan = pan
                                    };

                                    track.AddEvent(newEvent);

                                    eventIndex++;
                                    calcMinTick = (UInt32)Math.Min(calcMinTick, tick); // debug purpose

                                }
                                break;
                            case EventType.Volume:
                                {

                                    byte volume = extraData[3];

                                    EventData newEvent = new EventData()
                                    {
                                        TrackId = trackIndex,
                                        Tick = tick,
                                        EventType = EventType.Volume,
                                        Volume = volume
                                    };

                                    track.AddEvent(newEvent);

                                    eventIndex++;

                                }
                                break;
                            case EventType.Tempo:
                                {

                                    float tempo = BitConverter.ToSingle(extraData, 3);

                                    EventData newEvent = new EventData()
                                    {
                                        TrackId = trackIndex,
                                        Tick = tick,
                                        EventType = EventType.Tempo,
                                        Tempo = tempo
                                    };

                                    track.AddEvent(newEvent);

                                    eventIndex++;

                                }
                                break;
                            case EventType.Beat:
                                {

                                    ushort beat = BitConverter.ToUInt16(extraData, 3);

                                    EventData newEvent = new EventData()
                                    {
                                        TrackId = trackIndex,
                                        Tick = tick,
                                        EventType = EventType.Beat,
                                        Beat = beat
                                    };

                                    track.AddEvent(newEvent);

                                    eventIndex++;

                                }
                                break;
                            default:
                                break;
                        }


                    }
                    else
                    {

                        byte[] extraData = stream.ReadBytes(0x6);

                        switch ((EventType)id)
                        {
                            case EventType.None:
                                break;
                            case EventType.Note:
                                {

                                    byte insNo = extraData[0];
                                    byte vel = extraData[1];
                                    byte pan = extraData[2];

                                    byte attribute = extraData[3];
                                    ushort duration = BitConverter.ToUInt16(extraData, 4);

                                    InstrumentData inst =
                                        playerData.Instruments.FirstOrDefault(ins => ins != null && ins.InsNum == insNo);

                                    EventData newEvent = new EventData()
                                    {
                                        TrackId = trackIndex,
                                        Tick = tick,
                                        Attribute = attribute,
                                        Duration = duration,
                                        EventType = EventType.Note,
                                        Instrument = inst,
                                        Vel = vel,
                                        Pan = pan
                                    };

                                    track.AddEvent(newEvent);

                                    eventIndex++;

                                }
                                break;

                            case EventType.Volume:
                                {

                                    byte volume = extraData[0];

                                    EventData newEvent = new EventData()
                                    {
                                        //TrackId = trackIndex,
                                        Tick = tick,
                                        EventType = EventType.Volume,
                                        Volume = volume
                                    };

                                    track.AddEvent(newEvent);

                                    eventIndex++;

                                }
                                break;

                            case EventType.Tempo:
                                {

                                    float tempo = BitConverter.ToSingle(extraData, 0);

                                    EventData newEvent = new EventData()
                                    {
                                        TrackId = trackIndex,
                                        Tick = tick,
                                        EventType = EventType.Tempo,
                                        Tempo = tempo
                                    };

                                    track.AddEvent(newEvent);

                                    eventIndex++;

                                }
                                break;
                            case EventType.Beat:
                                {

                                    ushort beat = BitConverter.ToUInt16(extraData, 0);

                                    EventData newEvent = new EventData()
                                    {
                                        TrackId = trackIndex,
                                        Tick = tick,
                                        EventType = EventType.Beat,
                                        Beat = beat
                                    };

                                    track.AddEvent(newEvent);

                                    eventIndex++;

                                }
                                break;

                            default:
                                break;

                        }
                    }
                }

                trackIndex++;
            }

            stream.Dispose();

            foreach (TrackData track in playerData.Tracks)
            {

                int minTick = 0, maxTick = 0;
                if (track.Events.Count() > 0)
                {
                    minTick = track.Events.Min(evnt => evnt.Tick);
                    maxTick = track.Events.Max(evnt => evnt.Tick);
                }

                int len = maxTick - minTick;

            }

            return playerData;
        }

    }
}
