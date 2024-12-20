﻿using Nebula.Core.Data.Chunks.MFAChunks;
using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.AppChunks
{
    public class Extensions : Chunk
    {
        public Dictionary<int, Extension> Exts = new();

        public Extensions()
        {
            ChunkName = "Extensions";
            ChunkID = 0x2234;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Exts = new();
            ushort Count = reader.ReadUShort();
            ushort MaxHandle = reader.ReadUShort();

            for (int i = 0; i < Count; i++)
            {
                Extension ext = new Extension();
                ext.ReadCCN(reader);
                Exts.Add(ext.Handle, ext);
            }

            NebulaCore.PackageData.Extensions = this;
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Exts = new();
            uint Count = reader.ReadUInt();
            for (int i = 0; i < Count; i++)
            {
                Extension ext = new Extension();
                ext.ReadMFA(reader);
                Exts.Add(ext.Handle, ext);
            }
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(Exts.Count);
            foreach (Extension ext in Exts.Values)
                ext.WriteMFA(writer);
        }
    }
}
