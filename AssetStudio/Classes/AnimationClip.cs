﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace AssetStudio
{
    public class Keyframe<T>
    {
        public float time;
        public T value;
        public T inSlope;
        public T outSlope;
        public int weightedMode;
        public T inWeight;
        public T outWeight;

        public Keyframe() { }

        public Keyframe(ObjectReader reader, Func<T> readerFunc)
        {
            time = reader.ReadSingle();
            value = readerFunc();
            inSlope = readerFunc();
            outSlope = readerFunc();
            if (reader.version >= 2018) //2018 and up
            {
                weightedMode = reader.ReadInt32();
                inWeight = readerFunc();
                outWeight = readerFunc();
            }
        }
    }

    public class AnimationCurve<T>
    {
        public Keyframe<T>[] m_Curve;
        public int m_PreInfinity;
        public int m_PostInfinity;
        public int m_RotationOrder;

        public AnimationCurve() { }

        public AnimationCurve(ObjectReader reader, Func<T> readerFunc)
        {
            int numCurves = reader.ReadInt32();
            m_Curve = new Keyframe<T>[numCurves];
            for (int i = 0; i < numCurves; i++)
            {
                m_Curve[i] = new Keyframe<T>(reader, readerFunc);
            }

            m_PreInfinity = reader.ReadInt32();
            m_PostInfinity = reader.ReadInt32();
            if (reader.version >= (5, 3)) //5.3 and up
            {
                m_RotationOrder = reader.ReadInt32();
            }
        }
    }

    public class QuaternionCurve
    {
        public AnimationCurve<Quaternion> curve;
        public string path;

        public QuaternionCurve() { }

        public QuaternionCurve(ObjectReader reader)
        {
            curve = new AnimationCurve<Quaternion>(reader, reader.ReadQuaternion);
            path = reader.ReadAlignedString();
        }
    }

    public class PackedFloatVector
    {
        public uint m_NumItems;
        public float m_Range;
        public float m_Start;
        public byte[] m_Data;
        public byte m_BitSize;

        public PackedFloatVector() { }

        public PackedFloatVector(ObjectReader reader)
        {
            m_NumItems = reader.ReadUInt32();
            m_Range = reader.ReadSingle();
            m_Start = reader.ReadSingle();

            int numData = reader.ReadInt32();
            m_Data = reader.ReadBytes(numData);
            reader.AlignStream();

            m_BitSize = reader.ReadByte();
            reader.AlignStream();
        }

        public float[] UnpackFloats(int itemCountInChunk, int chunkStride, int start = 0, int numChunks = -1)
        {
            int bitPos = m_BitSize * start;
            int indexPos = bitPos / 8;
            bitPos %= 8;

            float scale = 1.0f / m_Range;
            if (numChunks == -1)
                numChunks = (int)m_NumItems / itemCountInChunk;
            var end = chunkStride * numChunks / 4;
            var data = new List<float>();
            for (var index = 0; index != end; index += chunkStride / 4)
            {
                for (int i = 0; i < itemCountInChunk; ++i)
                {
                    uint x = 0;

                    int bits = 0;
                    while (bits < m_BitSize)
                    {
                        x |= (uint)((m_Data[indexPos] >> bitPos) << bits);
                        int num = Math.Min(m_BitSize - bits, 8 - bitPos);
                        bitPos += num;
                        bits += num;
                        if (bitPos == 8)
                        {
                            indexPos++;
                            bitPos = 0;
                        }
                    }
                    x &= (uint)(1 << m_BitSize) - 1u;
                    data.Add(x / (scale * ((1 << m_BitSize) - 1)) + m_Start);
                }
            }

            return data.ToArray();
        }
    }

    public class PackedIntVector
    {
        public uint m_NumItems;
        public byte[] m_Data;
        public byte m_BitSize;

        public PackedIntVector() { }

        public PackedIntVector(ObjectReader reader)
        {
            m_NumItems = reader.ReadUInt32();

            int numData = reader.ReadInt32();
            m_Data = reader.ReadBytes(numData);
            reader.AlignStream();

            m_BitSize = reader.ReadByte();
            reader.AlignStream();
        }

        public int[] UnpackInts()
        {
            var data = new int[m_NumItems];
            int indexPos = 0;
            int bitPos = 0;
            for (int i = 0; i < m_NumItems; i++)
            {
                int bits = 0;
                data[i] = 0;
                while (bits < m_BitSize)
                {
                    data[i] |= (m_Data[indexPos] >> bitPos) << bits;
                    int num = Math.Min(m_BitSize - bits, 8 - bitPos);
                    bitPos += num;
                    bits += num;
                    if (bitPos == 8)
                    {
                        indexPos++;
                        bitPos = 0;
                    }
                }
                data[i] &= (1 << m_BitSize) - 1;
            }
            return data;
        }
    }

    public class PackedQuatVector
    {
        public uint m_NumItems;
        public byte[] m_Data;

        public PackedQuatVector() { }

        public PackedQuatVector(ObjectReader reader)
        {
            m_NumItems = reader.ReadUInt32();

            int numData = reader.ReadInt32();
            m_Data = reader.ReadBytes(numData);

            reader.AlignStream();
        }

        public Quaternion[] UnpackQuats()
        {
            var data = new Quaternion[m_NumItems];
            int indexPos = 0;
            int bitPos = 0;

            for (int i = 0; i < m_NumItems; i++)
            {
                uint flags = 0;

                int bits = 0;
                while (bits < 3)
                {
                    flags |= (uint)((m_Data[indexPos] >> bitPos) << bits);
                    int num = Math.Min(3 - bits, 8 - bitPos);
                    bitPos += num;
                    bits += num;
                    if (bitPos == 8)
                    {
                        indexPos++;
                        bitPos = 0;
                    }
                }
                flags &= 7;


                var q = new Quaternion();
                float sum = 0;
                for (int j = 0; j < 4; j++)
                {
                    if ((flags & 3) != j)
                    {
                        int bitSize = ((flags & 3) + 1) % 4 == j ? 9 : 10;
                        uint x = 0;

                        bits = 0;
                        while (bits < bitSize)
                        {
                            x |= (uint)((m_Data[indexPos] >> bitPos) << bits);
                            int num = Math.Min(bitSize - bits, 8 - bitPos);
                            bitPos += num;
                            bits += num;
                            if (bitPos == 8)
                            {
                                indexPos++;
                                bitPos = 0;
                            }
                        }
                        x &= (uint)((1 << bitSize) - 1);
                        q[j] = x / (0.5f * ((1 << bitSize) - 1)) - 1;
                        sum += q[j] * q[j];
                    }
                }

                int lastComponent = (int)(flags & 3);
                q[lastComponent] = MathF.Sqrt(1 - sum);
                if ((flags & 4) != 0u)
                    q[lastComponent] = -q[lastComponent];
                data[i] = q;
            }

            return data;
        }
    }

    public class CompressedAnimationCurve
    {
        public string m_Path;
        public PackedIntVector m_Times;
        public PackedQuatVector m_Values;
        public PackedFloatVector m_Slopes;
        public int m_PreInfinity;
        public int m_PostInfinity;

        public CompressedAnimationCurve() { }

        public CompressedAnimationCurve(ObjectReader reader)
        {
            m_Path = reader.ReadAlignedString();
            m_Times = new PackedIntVector(reader);
            m_Values = new PackedQuatVector(reader);
            m_Slopes = new PackedFloatVector(reader);
            m_PreInfinity = reader.ReadInt32();
            m_PostInfinity = reader.ReadInt32();
        }
    }

    public class Vector3Curve
    {
        public AnimationCurve<Vector3> curve;
        public string path;

        public Vector3Curve() { }

        public Vector3Curve(ObjectReader reader)
        {
            curve = new AnimationCurve<Vector3>(reader, reader.ReadVector3);
            path = reader.ReadAlignedString();
        }
    }

    public class FloatCurve
    {
        public AnimationCurve<float> curve;
        public string attribute;
        public string path;
        public ClassIDType classID;
        public PPtr<MonoScript> script;
        public int flags;

        public FloatCurve() { }

        public FloatCurve(ObjectReader reader)
        {
            curve = new AnimationCurve<float>(reader, reader.ReadSingle);
            attribute = reader.ReadAlignedString();
            path = reader.ReadAlignedString();
            classID = (ClassIDType)reader.ReadInt32();
            script = new PPtr<MonoScript>(reader);
            if (reader.version >= (2022, 2)) //2022.2 and up
            {
                flags = reader.ReadInt32();
            }
        }
    }

    public class PPtrKeyframe
    {
        public float time;
        public PPtr<Object> value;

        public PPtrKeyframe() { }

        public PPtrKeyframe(ObjectReader reader)
        {
            time = reader.ReadSingle();
            value = new PPtr<Object>(reader);
        }
    }

    public class PPtrCurve
    {
        public PPtrKeyframe[] curve;
        public string attribute;
        public string path;
        public int classID;
        public PPtr<MonoScript> script;
        public int flags;

        public PPtrCurve() { }

        public PPtrCurve(ObjectReader reader)
        {
            int numCurves = reader.ReadInt32();
            curve = new PPtrKeyframe[numCurves];
            for (int i = 0; i < numCurves; i++)
            {
                curve[i] = new PPtrKeyframe(reader);
            }

            attribute = reader.ReadAlignedString();
            path = reader.ReadAlignedString();
            classID = reader.ReadInt32();
            script = new PPtr<MonoScript>(reader);
            if (reader.version >= (2022, 2)) //2022.2 and up
            {
                flags = reader.ReadInt32();
            }
        }
    }

    public class AABB
    {
        public Vector3 m_Center;
        public Vector3 m_Extent;

        public AABB() { }

        public AABB(ObjectReader reader)
        {
            m_Center = reader.ReadVector3();
            m_Extent = reader.ReadVector3();
        }
    }

    public class xform
    {
        public Vector3 t;
        public Quaternion q;
        public Vector3 s;

        public xform() { }

        public xform(ObjectReader reader)
        {
            var version = reader.version;
            t = version >= (5, 4) ? reader.ReadVector3() : (Vector3)reader.ReadVector4();//5.4 and up
            q = reader.ReadQuaternion();
            s = version >= (5, 4) ? reader.ReadVector3() : (Vector3)reader.ReadVector4();//5.4 and up
        }
    }

    public class HandPose
    {
        public xform m_GrabX;
        public float[] m_DoFArray;
        public float m_Override;
        public float m_CloseOpen;
        public float m_InOut;
        public float m_Grab;

        public HandPose() { }

        public HandPose(ObjectReader reader)
        {
            m_GrabX = new xform(reader);
            m_DoFArray = reader.ReadSingleArray();
            m_Override = reader.ReadSingle();
            m_CloseOpen = reader.ReadSingle();
            m_InOut = reader.ReadSingle();
            m_Grab = reader.ReadSingle();
        }
    }

    public class HumanGoal
    {
        public xform m_X;
        public float m_WeightT;
        public float m_WeightR;
        public Vector3 m_HintT;
        public float m_HintWeightT;

        public HumanGoal() { }

        public HumanGoal(ObjectReader reader)
        {
            var version = reader.version;
            m_X = new xform(reader);
            m_WeightT = reader.ReadSingle();
            m_WeightR = reader.ReadSingle();
            if (version >= 5)//5.0 and up
            {
                m_HintT = version >= (5, 4) ? reader.ReadVector3() : (Vector3)reader.ReadVector4();//5.4 and up
                m_HintWeightT = reader.ReadSingle();
            }
        }
    }

    public class HumanPose
    {
        public xform m_RootX;
        public Vector3 m_LookAtPosition;
        public Vector4 m_LookAtWeight;
        public HumanGoal[] m_GoalArray;
        public HandPose m_LeftHandPose;
        public HandPose m_RightHandPose;
        public float[] m_DoFArray;
        public Vector3[] m_TDoFArray;

        public HumanPose() { }

        public HumanPose(ObjectReader reader)
        {
            var version = reader.version;
            m_RootX = new xform(reader);
            m_LookAtPosition = version >= (5, 4) ? reader.ReadVector3() : (Vector3)reader.ReadVector4();//5.4 and up
            m_LookAtWeight = reader.ReadVector4();

            int numGoals = reader.ReadInt32();
            m_GoalArray = new HumanGoal[numGoals];
            for (int i = 0; i < numGoals; i++)
            {
                m_GoalArray[i] = new HumanGoal(reader);
            }

            m_LeftHandPose = new HandPose(reader);
            m_RightHandPose = new HandPose(reader);

            m_DoFArray = reader.ReadSingleArray();

            if (version >= (5, 2))//5.2 and up
            {
                int numTDof = reader.ReadInt32();
                m_TDoFArray = new Vector3[numTDof];
                for (int i = 0; i < numTDof; i++)
                {
                    m_TDoFArray[i] = version >= (5, 4) ? reader.ReadVector3() : (Vector3)reader.ReadVector4();//5.4 and up
                }
            }
        }
    }

    public class StreamedClip
    {
        public uint[] data;
        public uint curveCount;

        public StreamedClip() { }

        public StreamedClip(ObjectReader reader)
        {
            var version = reader.version;
            data = reader.ReadUInt32Array();
            if (version.IsInRange((2022, 3, 19), 2023) //2022.3.19f1 to 2023
                || version >= (2023, 2, 8)) //2023.2.8f1 and up
            {
                curveCount = reader.ReadUInt16();
                var discreteCurveCount = reader.ReadUInt16();
            }
            else
            {
                curveCount = reader.ReadUInt32();
            }
        }

        public class StreamedCurveKey
        {
            public int index;
            public float[] coeff;

            public float value;
            public float outSlope;
            public float inSlope;

            public StreamedCurveKey() { }

            public StreamedCurveKey(BinaryReader reader)
            {
                index = reader.ReadInt32();
                coeff = reader.ReadSingleArray(4);

                outSlope = coeff[2];
                value = coeff[3];
            }

            public float CalculateNextInSlope(float dx, StreamedCurveKey rhs)
            {
                //Stepped
                if (coeff[0] == 0f && coeff[1] == 0f && coeff[2] == 0f)
                {
                    return float.PositiveInfinity;
                }

                dx = Math.Max(dx, 0.0001f);
                var dy = rhs.value - value;
                var length = 1.0f / (dx * dx);
                var d1 = outSlope * dx;
                var d2 = dy + dy + dy - d1 - d1 - coeff[1] / length;
                return d2 / dx;
            }
        }

        public class StreamedFrame
        {
            public float time;
            public StreamedCurveKey[] keyList;

            public StreamedFrame() { }

            public StreamedFrame(BinaryReader reader)
            {
                time = reader.ReadSingle();

                int numKeys = reader.ReadInt32();
                keyList = new StreamedCurveKey[numKeys];
                for (int i = 0; i < numKeys; i++)
                {
                    keyList[i] = new StreamedCurveKey(reader);
                }
            }
        }

        public List<StreamedFrame> ReadData()
        {
            var frameList = new List<StreamedFrame>();
            var buffer = new byte[data.Length * 4];
            Buffer.BlockCopy(data, 0, buffer, 0, buffer.Length);
            using (var reader = new BinaryReader(new MemoryStream(buffer)))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    frameList.Add(new StreamedFrame(reader));
                }
            }

            for (int frameIndex = 2; frameIndex < frameList.Count - 1; frameIndex++)
            {
                var frame = frameList[frameIndex];
                foreach (var curveKey in frame.keyList)
                {
                    for (int i = frameIndex - 1; i >= 0; i--)
                    {
                        var preFrame = frameList[i];
                        var preCurveKey = preFrame.keyList.FirstOrDefault(x => x.index == curveKey.index);
                        if (preCurveKey != null)
                        {
                            curveKey.inSlope = preCurveKey.CalculateNextInSlope(frame.time - preFrame.time, curveKey);
                            break;
                        }
                    }
                }
            }
            return frameList;
        }
    }

    public class DenseClip
    {
        public int m_FrameCount;
        public uint m_CurveCount;
        public float m_SampleRate;
        public float m_BeginTime;
        public float[] m_SampleArray;

        public DenseClip() { }

        public DenseClip(ObjectReader reader)
        {
            m_FrameCount = reader.ReadInt32();
            m_CurveCount = reader.ReadUInt32();
            m_SampleRate = reader.ReadSingle();
            m_BeginTime = reader.ReadSingle();
            m_SampleArray = reader.ReadSingleArray();
        }
    }

    public class ConstantClip
    {
        public float[] data;

        public ConstantClip() { }

        public ConstantClip(ObjectReader reader)
        {
            data = reader.ReadSingleArray();
        }
    }

    public class ValueConstant
    {
        public uint m_ID;
        public uint m_TypeID;
        public uint m_Type;
        public uint m_Index;

        public ValueConstant() { }

        public ValueConstant(ObjectReader reader)
        {
            m_ID = reader.ReadUInt32();
            if (reader.version < (5, 5)) //5.5 down
            {
                m_TypeID = reader.ReadUInt32();
            }
            m_Type = reader.ReadUInt32();
            m_Index = reader.ReadUInt32();
        }
    }

    public class ValueArrayConstant
    {
        public ValueConstant[] m_ValueArray;

        public ValueArrayConstant() { }

        public ValueArrayConstant(ObjectReader reader)
        {
            int numVals = reader.ReadInt32();
            m_ValueArray = new ValueConstant[numVals];
            for (int i = 0; i < numVals; i++)
            {
                m_ValueArray[i] = new ValueConstant(reader);
            }
        }
    }

    public class OffsetPtr
    {
        public Clip data;

        public OffsetPtr() { }

        public OffsetPtr(ObjectReader reader)
        {
            data = new Clip(reader);
        }
    }

    public class Clip
    {
        public StreamedClip m_StreamedClip;
        public DenseClip m_DenseClip;
        public ConstantClip m_ConstantClip;
        public ValueArrayConstant m_Binding;

        public Clip() { }

        public Clip(ObjectReader reader)
        {
            var version = reader.version;
            m_StreamedClip = new StreamedClip(reader);
            m_DenseClip = new DenseClip(reader);
            if (version >= (4, 3)) //4.3 and up
            {
                m_ConstantClip = new ConstantClip(reader);
            }
            if (version < (2018, 3)) //2018.3 down
            {
                m_Binding = new ValueArrayConstant(reader);
            }
        }

        public AnimationClipBindingConstant ConvertValueArrayToGenericBinding()
        {
            var bindings = new AnimationClipBindingConstant();
            var genericBindings = new List<GenericBinding>();
            var values = m_Binding;
            for (int i = 0; i < values.m_ValueArray.Length;)
            {
                var curveID = values.m_ValueArray[i].m_ID;
                var curveTypeID = values.m_ValueArray[i].m_TypeID;
                var binding = new GenericBinding();
                genericBindings.Add(binding);
                if (curveTypeID == 4174552735) //CRC(PositionX))
                {
                    binding.path = curveID;
                    binding.attribute = 1; //kBindTransformPosition
                    binding.typeID = ClassIDType.Transform;
                    i += 3;
                }
                else if (curveTypeID == 2211994246) //CRC(QuaternionX))
                {
                    binding.path = curveID;
                    binding.attribute = 2; //kBindTransformRotation
                    binding.typeID = ClassIDType.Transform;
                    i += 4;
                }
                else if (curveTypeID == 1512518241) //CRC(ScaleX))
                {
                    binding.path = curveID;
                    binding.attribute = 3; //kBindTransformScale
                    binding.typeID = ClassIDType.Transform;
                    i += 3;
                }
                else
                {
                    binding.typeID = ClassIDType.Animator;
                    binding.path = 0;
                    binding.attribute = curveID;
                    i++;
                }
            }
            bindings.genericBindings = genericBindings.ToArray();
            return bindings;
        }
    }

    public class ValueDelta
    {
        public float m_Start;
        public float m_Stop;

        public ValueDelta() { }

        public ValueDelta(ObjectReader reader)
        {
            m_Start = reader.ReadSingle();
            m_Stop = reader.ReadSingle();
        }
    }

    public class ClipMuscleConstant
    {
        public HumanPose m_DeltaPose;
        public xform m_StartX;
        public xform m_StopX;
        public xform m_LeftFootStartX;
        public xform m_RightFootStartX;
        public xform m_MotionStartX;
        public xform m_MotionStopX;
        public Vector3 m_AverageSpeed;
        public OffsetPtr m_Clip;
        public float m_StartTime;
        public float m_StopTime;
        public float m_OrientationOffsetY;
        public float m_Level;
        public float m_CycleOffset;
        public float m_AverageAngularSpeed;
        public int[] m_IndexArray;
        public ValueDelta[] m_ValueArrayDelta;
        public float[] m_ValueArrayReferencePose;
        public bool m_Mirror;
        public bool m_LoopTime;
        public bool m_LoopBlend;
        public bool m_LoopBlendOrientation;
        public bool m_LoopBlendPositionY;
        public bool m_LoopBlendPositionXZ;
        public bool m_StartAtOrigin;
        public bool m_KeepOriginalOrientation;
        public bool m_KeepOriginalPositionY;
        public bool m_KeepOriginalPositionXZ;
        public bool m_HeightFromFeet;

        public ClipMuscleConstant() { }

        public ClipMuscleConstant(ObjectReader reader)
        {
            var version = reader.version;
            m_DeltaPose = new HumanPose(reader);
            m_StartX = new xform(reader);
            if (version >= (5, 5)) //5.5 and up
            {
                m_StopX = new xform(reader);
            }
            m_LeftFootStartX = new xform(reader);
            m_RightFootStartX = new xform(reader);
            if (version < 5)//5.0 down
            {
                m_MotionStartX = new xform(reader);
                m_MotionStopX = new xform(reader);
            }
            m_AverageSpeed = version >= (5, 4) ? reader.ReadVector3() : (Vector3)reader.ReadVector4();//5.4 and up
            m_Clip = new OffsetPtr(reader);
            m_StartTime = reader.ReadSingle();
            m_StopTime = reader.ReadSingle();
            m_OrientationOffsetY = reader.ReadSingle();
            m_Level = reader.ReadSingle();
            m_CycleOffset = reader.ReadSingle();
            m_AverageAngularSpeed = reader.ReadSingle();

            m_IndexArray = reader.ReadInt32Array();
            if (version < (4, 3)) //4.3 down
            {
                var m_AdditionalCurveIndexArray = reader.ReadInt32Array();
            }
            int numDeltas = reader.ReadInt32();
            m_ValueArrayDelta = new ValueDelta[numDeltas];
            for (int i = 0; i < numDeltas; i++)
            {
                m_ValueArrayDelta[i] = new ValueDelta(reader);
            }
            if (version >= (5, 3))//5.3 and up
            {
                m_ValueArrayReferencePose = reader.ReadSingleArray();
            }

            m_Mirror = reader.ReadBoolean();
            if (version >= (4, 3)) //4.3 and up
            {
                m_LoopTime = reader.ReadBoolean();
            }
            m_LoopBlend = reader.ReadBoolean();
            m_LoopBlendOrientation = reader.ReadBoolean();
            m_LoopBlendPositionY = reader.ReadBoolean();
            m_LoopBlendPositionXZ = reader.ReadBoolean();
            if (version >= (5, 5))//5.5 and up
            {
                m_StartAtOrigin = reader.ReadBoolean();
            }
            m_KeepOriginalOrientation = reader.ReadBoolean();
            m_KeepOriginalPositionY = reader.ReadBoolean();
            m_KeepOriginalPositionXZ = reader.ReadBoolean();
            m_HeightFromFeet = reader.ReadBoolean();
            reader.AlignStream();
        }
    }

    public class GenericBinding
    {
        public uint path;
        public uint attribute;
        public PPtr<Object> script;
        public ClassIDType typeID;
        public byte customType;
        public byte isPPtrCurve;
        public byte isIntCurve;
        public byte isSerializeReferenceCurve;

        public GenericBinding() { }

        public GenericBinding(ObjectReader reader)
        {
            var version = reader.version;
            path = reader.ReadUInt32();
            attribute = reader.ReadUInt32();
            script = new PPtr<Object>(reader);
            if (version >= (5, 6)) //5.6 and up
            {
                typeID = (ClassIDType)reader.ReadInt32();
            }
            else
            {
                typeID = (ClassIDType)reader.ReadUInt16();
            }
            customType = reader.ReadByte();
            isPPtrCurve = reader.ReadByte();
            if (version >= (2022, 1)) //2022.1 and up
            {
                isIntCurve = reader.ReadByte();
            }
            if (version >= (2022, 2)) //2022.2 and up
            {
                isSerializeReferenceCurve = reader.ReadByte();
            }
            reader.AlignStream();
        }
    }

    public class AnimationClipBindingConstant
    {
        public GenericBinding[] genericBindings;
        public PPtr<Object>[] pptrCurveMapping;

        public AnimationClipBindingConstant() { }

        public AnimationClipBindingConstant(ObjectReader reader)
        {
            int numBindings = reader.ReadInt32();
            genericBindings = new GenericBinding[numBindings];
            for (int i = 0; i < numBindings; i++)
            {
                genericBindings[i] = new GenericBinding(reader);
            }

            int numMappings = reader.ReadInt32();
            pptrCurveMapping = new PPtr<Object>[numMappings];
            for (int i = 0; i < numMappings; i++)
            {
                pptrCurveMapping[i] = new PPtr<Object>(reader);
            }
        }

        public GenericBinding FindBinding(int index)
        {
            int curves = 0;
            foreach (var b in genericBindings)
            {
                if (b.typeID == ClassIDType.Transform)
                {
                    switch (b.attribute)
                    {
                        case 1: //kBindTransformPosition
                        case 3: //kBindTransformScale
                        case 4: //kBindTransformEuler
                            curves += 3;
                            break;
                        case 2: //kBindTransformRotation
                            curves += 4;
                            break;
                        default:
                            curves += 1;
                            break;
                    }
                }
                else
                {
                    curves += 1;
                }
                if (curves > index)
                {
                    return b;
                }
            }

            return null;
        }
    }

    public class AnimationEvent
    {
        public float time;
        public string functionName;
        public string data;
        public PPtr<Object> objectReferenceParameter;
        public float floatParameter;
        public int intParameter;
        public int messageOptions;

        public AnimationEvent() { }

        public AnimationEvent(ObjectReader reader)
        {
            var version = reader.version;
            time = reader.ReadSingle();
            functionName = reader.ReadAlignedString();
            data = reader.ReadAlignedString();
            if (version >= (2, 6)) //2.6 and up
            {
                objectReferenceParameter = new PPtr<Object>(reader);
                floatParameter = reader.ReadSingle();
                if (version >= 3) //3 and up
                {
                    intParameter = reader.ReadInt32();
                }
            }
            messageOptions = reader.ReadInt32();
        }
    }

    public enum AnimationType
    {
        Legacy = 1,
        Generic = 2,
        Humanoid = 3
    };

    public sealed class AnimationClip : NamedObject
    {
        public AnimationType m_AnimationType;
        public bool m_Legacy;
        public bool m_Compressed;
        public bool m_UseHighQualityCurve;
        public QuaternionCurve[] m_RotationCurves;
        public CompressedAnimationCurve[] m_CompressedRotationCurves;
        public Vector3Curve[] m_EulerCurves;
        public Vector3Curve[] m_PositionCurves;
        public Vector3Curve[] m_ScaleCurves;
        public FloatCurve[] m_FloatCurves;
        public PPtrCurve[] m_PPtrCurves;
        public float m_SampleRate;
        public int m_WrapMode;
        public AABB m_Bounds;
        public uint m_MuscleClipSize;
        public ClipMuscleConstant m_MuscleClip;
        public AnimationClipBindingConstant m_ClipBindingConstant;
        public AnimationEvent[] m_Events;
        public byte[] m_AnimData;
        public StreamingInfo m_StreamingInfo;

        public AnimationClip() { }

        public AnimationClip(ObjectReader reader, byte[] type, JsonSerializerOptions jsonOptions, ObjectInfo objInfo) : base(reader)
        {
            var parsedAnimClip = JsonSerializer.Deserialize<AnimationClip>(type, jsonOptions);
            m_AnimationType = parsedAnimClip.m_AnimationType;
            if (version >= 5)//5.0 and up
            {
                m_Legacy = parsedAnimClip.m_Legacy;
            }
            else if (version >= 4)//4.0 and up
            {
                m_Legacy = m_AnimationType == AnimationType.Legacy;
            }
            else
            {
                m_Legacy = true;
            }
            m_Compressed = parsedAnimClip.m_Compressed;
            m_UseHighQualityCurve = parsedAnimClip.m_UseHighQualityCurve;
            m_RotationCurves = parsedAnimClip.m_RotationCurves;
            m_CompressedRotationCurves = parsedAnimClip.m_CompressedRotationCurves;
            m_EulerCurves = parsedAnimClip.m_EulerCurves;
            m_PositionCurves = parsedAnimClip.m_PositionCurves;
            m_ScaleCurves = parsedAnimClip.m_ScaleCurves;
            m_FloatCurves = parsedAnimClip.m_FloatCurves;
            m_PPtrCurves = parsedAnimClip.m_PPtrCurves;
            m_SampleRate = parsedAnimClip.m_SampleRate;
            m_WrapMode = parsedAnimClip.m_WrapMode;
            m_Bounds = parsedAnimClip.m_Bounds;
            m_MuscleClipSize = parsedAnimClip.m_MuscleClipSize;
            m_MuscleClip = parsedAnimClip.m_MuscleClip;
            m_ClipBindingConstant = parsedAnimClip.m_ClipBindingConstant;
            m_Events = parsedAnimClip.m_Events;
            if (!reader.version.IsTuanjie) 
                return;
            m_AnimData = parsedAnimClip.m_AnimData;
            m_StreamingInfo = parsedAnimClip.m_StreamingInfo;
            if (!(m_AnimData?.Length > 0)) 
                return;
            m_MuscleClipSize = (uint)m_AnimData.Length;
            using (var muscleStream = new MemoryStream(m_AnimData))
            {
                using (var muscleReader = new EndianBinaryReader(muscleStream, EndianType.LittleEndian))
                {
                    _ = muscleReader.ReadUInt32();
                    var objReader = new ObjectReader(muscleReader, assetsFile, objInfo);
                    m_MuscleClip = new ClipMuscleConstant(objReader);
                }
            }
        }

        public AnimationClip(ObjectReader reader) : base(reader)
        {
            if (version >= 5)//5.0 and up
            {
                m_Legacy = reader.ReadBoolean();
            }
            else if (version >= 4)//4.0 and up
            {
                m_AnimationType = (AnimationType)reader.ReadInt32();
                m_Legacy = m_AnimationType == AnimationType.Legacy;
            }
            else
            {
                m_Legacy = true;
            }
            if (version >= (2, 6)) //2.6 and up
            {
                m_Compressed = reader.ReadBoolean();
            }
            if (version >= (4, 3))//4.3 and up
            {
                m_UseHighQualityCurve = reader.ReadBoolean();
            }
            reader.AlignStream();
            int numRCurves = reader.ReadInt32();
            m_RotationCurves = new QuaternionCurve[numRCurves];
            for (int i = 0; i < numRCurves; i++)
            {
                m_RotationCurves[i] = new QuaternionCurve(reader);
            }

            if (version >= (2, 6)) //2.6 and up
            {
                int numCRCurves = reader.ReadInt32();
                m_CompressedRotationCurves = new CompressedAnimationCurve[numCRCurves];
                for (int i = 0; i < numCRCurves; i++)
                {
                    m_CompressedRotationCurves[i] = new CompressedAnimationCurve(reader);
                }
            }

            if (!version.IsTuanjie)
            {
                if (version >= (5, 3)) //5.3 and up
                {
                    int numEulerCurves = reader.ReadInt32();
                    m_EulerCurves = new Vector3Curve[numEulerCurves];
                    for (int i = 0; i < numEulerCurves; i++)
                    {
                        m_EulerCurves[i] = new Vector3Curve(reader);
                    }
                }

                int numPCurves = reader.ReadInt32();
                m_PositionCurves = new Vector3Curve[numPCurves];
                for (int i = 0; i < numPCurves; i++)
                {
                    m_PositionCurves[i] = new Vector3Curve(reader);
                }

                int numSCurves = reader.ReadInt32();
                m_ScaleCurves = new Vector3Curve[numSCurves];
                for (int i = 0; i < numSCurves; i++)
                {
                    m_ScaleCurves[i] = new Vector3Curve(reader);
                }
            }

            int numFCurves = reader.ReadInt32();
            m_FloatCurves = new FloatCurve[numFCurves];
            for (int i = 0; i < numFCurves; i++)
            {
                m_FloatCurves[i] = new FloatCurve(reader);
            }

            if (version >= (4, 3)) //4.3 and up
            {
                int numPtrCurves = reader.ReadInt32();
                m_PPtrCurves = new PPtrCurve[numPtrCurves];
                for (int i = 0; i < numPtrCurves; i++)
                {
                    m_PPtrCurves[i] = new PPtrCurve(reader);
                }
            }

            m_SampleRate = reader.ReadSingle();
            if (version >= (2, 6)) //2.6 and up
            {
                m_WrapMode = reader.ReadInt32();
            }
            if (version >= (3, 4)) //3.4 and up
            {
                m_Bounds = new AABB(reader);
            }
            if (version >= 4)//4.0 and up
            {
                m_MuscleClipSize = reader.ReadUInt32();
                if (!version.IsTuanjie)
                {
                    m_MuscleClip = new ClipMuscleConstant(reader);
                }
                else if (m_MuscleClipSize > 0)
                {
                    _ = reader.ReadInt32();
                    m_MuscleClip = new ClipMuscleConstant(reader); //m_AnimData (Tuanjie)
                    m_StreamingInfo = new StreamingInfo(reader);
                }
            }
            if (version >= (4, 3)) //4.3 and up
            {
                m_ClipBindingConstant = new AnimationClipBindingConstant(reader);
            }
            if (version >= (2018, 3)) //2018.3 and up
            {
                var m_HasGenericRootTransform = reader.ReadBoolean();
                var m_HasMotionFloatCurves = reader.ReadBoolean();
                reader.AlignStream();
            }
            int numEvents = reader.ReadInt32();
            m_Events = new AnimationEvent[numEvents];
            for (int i = 0; i < numEvents; i++)
            {
                m_Events[i] = new AnimationEvent(reader);
            }
            if (version >= 2017) //2017 and up
            {
                reader.AlignStream();
            }
        }

        public class EqComparer : IEqualityComparer<AnimationClip>
        {
            public bool Equals(AnimationClip clipA, AnimationClip clipB)
            {
                return clipA?.m_PathID == clipB?.m_PathID 
                       && clipA?.byteSize == clipB?.byteSize;
            }

            public int GetHashCode(AnimationClip obj)
            {
                var result = obj.m_PathID * 31;
                result = result * 31 + obj.byteSize;
                return result.GetHashCode();
            }
        }
    }
}
