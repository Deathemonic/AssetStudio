﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AssetStudio
{
    public static class TypeTreeHelper
    {
        private static readonly JsonSerializerOptions JsonOptions;
        static TypeTreeHelper()
        {
            JsonOptions = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                IncludeFields = true,
            };
        }

        public static string ReadTypeString(TypeTree m_Type, ObjectReader reader)
        {
            reader.Reset();
            var readed = 0L;
            var sb = new StringBuilder();
            var m_Nodes = m_Type.m_Nodes;
            try
            {
                for (var i = 0; i < m_Nodes.Count; i++)
                {
                    ReadStringValue(sb, m_Nodes, reader, ref i);
                    readed = reader.Position - reader.byteStart;
                }
            }
            catch (Exception)
            {
                //Ignore
            }
            if (readed != reader.byteSize)
            {
                Logger.Info($"Failed to read type, read {readed} bytes but expected {reader.byteSize} bytes");
            }

            return sb.ToString();
        }

        private static void ReadStringValue(StringBuilder sb, List<TypeTreeNode> m_Nodes, BinaryReader reader, ref int i)
        {
            var m_Node = m_Nodes[i];
            var level = m_Node.m_Level;
            var varTypeStr = m_Node.m_Type;
            var varNameStr = m_Node.m_Name;
            object value = null;
            var append = true;
            var align = (m_Node.m_MetaFlag & 0x4000) != 0;
            switch (varTypeStr)
            {
                case "SInt8":
                    value = reader.ReadSByte();
                    break;
                case "UInt8":
                    value = reader.ReadByte();
                    break;
                case "char":
                    value = BitConverter.ToChar(reader.ReadBytes(2), 0);
                    break;
                case "short":
                case "SInt16":
                    value = reader.ReadInt16();
                    break;
                case "UInt16":
                case "unsigned short":
                    value = reader.ReadUInt16();
                    break;
                case "int":
                case "SInt32":
                    value = reader.ReadInt32();
                    break;
                case "UInt32":
                case "unsigned int":
                case "Type*":
                    value = reader.ReadUInt32();
                    break;
                case "long long":
                case "SInt64":
                    value = reader.ReadInt64();
                    break;
                case "UInt64":
                case "unsigned long long":
                case "FileSize":
                    value = reader.ReadUInt64();
                    break;
                case "float":
                    value = reader.ReadSingle();
                    break;
                case "double":
                    value = reader.ReadDouble();
                    break;
                case "bool":
                    value = reader.ReadBoolean();
                    break;
                case "string":
                    append = false;
                    var str = reader.ReadAlignedString();
                    sb.AppendFormat("{0}{1} {2} = \"{3}\"\r\n", (new string('\t', level)), varTypeStr, varNameStr, str);
                    var toSkip = GetNodes(m_Nodes, i);
                    i += toSkip.Count - 1;
                    break;
                case "map":
                    {
                        if ((m_Nodes[i + 1].m_MetaFlag & 0x4000) != 0)
                            align = true;
                        append = false;
                        sb.AppendFormat("{0}{1} {2}\r\n", (new string('\t', level)), varTypeStr, varNameStr);
                        sb.AppendFormat("{0}{1} {2}\r\n", (new string('\t', level + 1)), "Array", "Array");
                        var size = reader.ReadInt32();
                        sb.AppendFormat("{0}{1} {2} = {3}\r\n", (new string('\t', level + 1)), "int", "size", size);
                        var map = GetNodes(m_Nodes, i);
                        i += map.Count - 1;
                        var first = GetNodes(map, 4);
                        var next = 4 + first.Count;
                        var second = GetNodes(map, next);
                        for (int j = 0; j < size; j++)
                        {
                            sb.AppendFormat("{0}[{1}]\r\n", (new string('\t', level + 2)), j);
                            sb.AppendFormat("{0}{1} {2}\r\n", (new string('\t', level + 2)), "pair", "data");
                            int tmp1 = 0;
                            int tmp2 = 0;
                            ReadStringValue(sb, first, reader, ref tmp1);
                            ReadStringValue(sb, second, reader, ref tmp2);
                        }
                        break;
                    }
                case "TypelessData":
                    {
                        append = false;
                        var size = reader.ReadInt32();
                        reader.BaseStream.Position += size;
                        i += 2;
                        sb.AppendFormat("{0}{1} {2}\r\n", (new string('\t', level)), varTypeStr, varNameStr);
                        sb.AppendFormat("{0}{1} {2} = {3}\r\n", (new string('\t', level)), "int", "size", size);
                        break;
                    }
                default:
                    {
                        if (i < m_Nodes.Count - 1 && m_Nodes[i + 1].m_Type == "Array") //Array
                        {
                            if ((m_Nodes[i + 1].m_MetaFlag & 0x4000) != 0)
                                align = true;
                            append = false;
                            sb.AppendFormat("{0}{1} {2}\r\n", (new string('\t', level)), varTypeStr, varNameStr);
                            sb.AppendFormat("{0}{1} {2}\r\n", (new string('\t', level + 1)), "Array", "Array");
                            var size = reader.ReadInt32();
                            sb.AppendFormat("{0}{1} {2} = {3}\r\n", (new string('\t', level + 1)), "int", "size", size);
                            var vector = GetNodes(m_Nodes, i);
                            i += vector.Count - 1;
                            for (int j = 0; j < size; j++)
                            {
                                sb.AppendFormat("{0}[{1}]\r\n", (new string('\t', level + 2)), j);
                                int tmp = 3;
                                ReadStringValue(sb, vector, reader, ref tmp);
                            }
                            break;
                        }
                        else //Class
                        {
                            append = false;
                            sb.AppendFormat("{0}{1} {2}\r\n", (new string('\t', level)), varTypeStr, varNameStr);
                            var @class = GetNodes(m_Nodes, i);
                            i += @class.Count - 1;
                            for (int j = 1; j < @class.Count; j++)
                            {
                                ReadStringValue(sb, @class, reader, ref j);
                            }
                            break;
                        }
                    }
            }
            if (append)
                sb.AppendFormat("{0}{1} {2} = {3}\r\n", (new string('\t', level)), varTypeStr, varNameStr, value);
            if (align)
                reader.AlignStream();
        }

        public static byte[] ReadTypeByteArray(TypeTree m_Types, ObjectReader reader)
        {
            var type = ReadType(m_Types, reader);
            var bytes = JsonSerializer.SerializeToUtf8Bytes(type, JsonOptions);
            type.Clear();
            return bytes;
        }

        public static OrderedDictionary ReadType(TypeTree m_Types, ObjectReader reader)
        {
            reader.Reset();
            var obj = new OrderedDictionary();
            var m_Nodes = m_Types.m_Nodes;
            var readed = 0L;
            try
            {
                for (int i = 1; i < m_Nodes.Count; i++)
                {
                    var m_Node = m_Nodes[i];
                    var varNameStr = m_Node.m_Name;
                    obj[varNameStr] = ReadValue(m_Nodes, reader, ref i);
                    readed = reader.Position - reader.byteStart;
                }
            }
            catch (Exception)
            {
                //Ignore
            }
            if (readed != reader.byteSize)
            {
                Logger.Info($"Failed to read type, read {readed} bytes but expected {reader.byteSize} bytes");
            }
            return obj;
        }

        private static object ReadValue(List<TypeTreeNode> m_Nodes, BinaryReader reader, ref int i)
        {
            var m_Node = m_Nodes[i];
            var varTypeStr = m_Node.m_Type;
            object value;
            var align = (m_Node.m_MetaFlag & 0x4000) != 0;
            switch (varTypeStr)
            {
                case "SInt8":
                    value = reader.ReadSByte();
                    break;
                case "UInt8":
                    value = reader.ReadByte();
                    break;
                case "char":
                    value = BitConverter.ToChar(reader.ReadBytes(2), 0);
                    break;
                case "short":
                case "SInt16":
                    value = reader.ReadInt16();
                    break;
                case "UInt16":
                case "unsigned short":
                    value = reader.ReadUInt16();
                    break;
                case "int":
                case "SInt32":
                    value = reader.ReadInt32();
                    break;
                case "UInt32":
                case "unsigned int":
                case "Type*":
                    value = reader.ReadUInt32();
                    break;
                case "long long":
                case "SInt64":
                    value = reader.ReadInt64();
                    break;
                case "UInt64":
                case "unsigned long long":
                case "FileSize":
                    value = reader.ReadUInt64();
                    break;
                case "float":
                    value = reader.ReadSingle();
                    break;
                case "double":
                    value = reader.ReadDouble();
                    break;
                case "bool":
                    value = reader.ReadBoolean();
                    break;
                case "string":
                    value = reader.ReadAlignedString();
                    var toSkip = GetNodes(m_Nodes, i);
                    i += toSkip.Count - 1;
                    break;
                case "map":
                    {
                        if ((m_Nodes[i + 1].m_MetaFlag & 0x4000) != 0)
                            align = true;
                        var map = GetNodes(m_Nodes, i);
                        i += map.Count - 1;
                        var first = GetNodes(map, 4);
                        var next = 4 + first.Count;
                        var second = GetNodes(map, next);
                        var size = reader.ReadInt32();
                        var dic = new List<KeyValuePair<object, object>>(size);
                        for (int j = 0; j < size; j++)
                        {
                            int tmp1 = 0;
                            int tmp2 = 0;
                            dic.Add(new KeyValuePair<object, object>(ReadValue(first, reader, ref tmp1), ReadValue(second, reader, ref tmp2)));
                        }
                        value = dic;
                        break;
                    }
                case "TypelessData":
                    {
                        var size = reader.ReadInt32();
                        var dic = new OrderedDictionary
                        {
                            { "Offset", reader.BaseStream.Position },
                            { "Size", size }
                        };
                        value = dic;
                        reader.BaseStream.Position += size;
                        i += 2;
                        break;
                    }
                default:
                    {
                        if (i < m_Nodes.Count - 1 && m_Nodes[i + 1].m_Type == "Array") //Array
                        {
                            if ((m_Nodes[i + 1].m_MetaFlag & 0x4000) != 0)
                                align = true;
                            var vector = GetNodes(m_Nodes, i);
                            i += vector.Count - 1;
                            var size = reader.ReadInt32();
                            var array = new object[size];
                            for (int j = 0; j < size; j++)
                            {
                                int tmp = 3;
                                array[j] = ReadValue(vector, reader, ref tmp);
                            }
                            value = array;
                            break;
                        }
                        else //Class
                        {
                            var @class = GetNodes(m_Nodes, i);
                            i += @class.Count - 1;
                            var obj = new OrderedDictionary();
                            for (int j = 1; j < @class.Count; j++)
                            {
                                var classmember = @class[j];
                                var name = classmember.m_Name;
                                obj[name] = ReadValue(@class, reader, ref j);
                            }
                            value = obj;
                            break;
                        }
                    }
            }
            if (align)
                reader.AlignStream();
            return value;
        }

        private static List<TypeTreeNode> GetNodes(List<TypeTreeNode> m_Nodes, int index)
        {
            var nodes = new List<TypeTreeNode>();
            nodes.Add(m_Nodes[index]);
            var level = m_Nodes[index].m_Level;
            for (int i = index + 1; i < m_Nodes.Count; i++)
            {
                var member = m_Nodes[i];
                var level2 = member.m_Level;
                if (level2 <= level)
                {
                    return nodes;
                }
                nodes.Add(member);
            }
            return nodes;
        }
    }
}
