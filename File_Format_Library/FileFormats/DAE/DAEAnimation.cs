using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;

public class DAEAnimation 
{
    // Method 1: Save to file directly
    public static void Save(FSKA animation, STSkeleton skeleton, string fileName) 
    {
        using (XmlTextWriter writer = new XmlTextWriter(fileName, Encoding.UTF8)) 
        {
            writer.Formatting = Formatting.Indented;
            WriteAnimationToWriter(animation, skeleton, writer);
        }
    }

    // Method 2: Write to existing XmlTextWriter
    public static void Write(FSKA animation, STSkeleton skeleton, XmlTextWriter writer) 
    {
        WriteAnimationToWriter(animation, skeleton, writer);
    }

    // Common writing logic
    private static void WriteAnimationToWriter(FSKA animation, STSkeleton skeleton, XmlTextWriter writer)
    {
        // For direct file save, we need the full DAE structure
        bool isNewFile = writer.WriteState == WriteState.Start;
        
        if (isNewFile)
        {
            // Write DAE header
            writer.WriteStartDocument();
            writer.WriteStartElement("COLLADA");
            writer.WriteAttributeString("xmlns", "http://www.collada.org/2005/11/COLLADASchema");
            writer.WriteAttributeString("version", "1.4.1");

            // Write asset info
            WriteAssetInfo(writer);
        }

        // Write library_animations
        writer.WriteStartElement("library_animations");
        
        // Get animation data
        var tracks = animation.SkeletalAnimU != null ? 
                    animation.SkeletalAnimU.Tracks : 
                    animation.SkeletalAnim.Tracks;

        var frameCount = animation.SkeletalAnimU != null ? 
                        animation.SkeletalAnimU.FrameCount : 
                        animation.SkeletalAnim.FrameCount;

        // Write each bone's animation
        foreach (var track in tracks) 
        {
            WriteAnimationTrack(writer, track, frameCount);
        }
        
        writer.WriteEndElement(); // library_animations

        // Write library_visual_scenes with skeleton
        if (skeleton != null)
        {
            WriteVisualScene(writer, skeleton);
        }

        if (isNewFile)
        {
            writer.WriteEndElement(); // COLLADA
            writer.WriteEndDocument();
        }
    }

    private static void WriteAnimationTrack(XmlTextWriter writer, AnimationTrack track, int frameCount) 
    {
        writer.WriteStartElement("animation");
        writer.WriteAttributeString("id", $"Anim_{track.BoneName}");
        
        // Write input array (time)
        writer.WriteStartElement("source");
        writer.WriteAttributeString("id", $"{track.BoneName}_input");
        writer.WriteStartElement("float_array");
        writer.WriteAttributeString("count", frameCount.ToString());
        
        // Write time values
        for (int frame = 0; frame < frameCount; frame++) 
        {
            writer.WriteString($"{frame / 30.0f} "); // Assuming 30fps
        }
        
        writer.WriteEndElement(); // float_array
        writer.WriteEndElement(); // source

        // Write output array (transforms)
        writer.WriteStartElement("source");
        writer.WriteAttributeString("id", $"{track.BoneName}_output");
        writer.WriteStartElement("float_array");
        writer.WriteAttributeString("count", (frameCount * 16).ToString());
        
        // Write transform matrices
        for (int frame = 0; frame < frameCount; frame++) 
        {
            Matrix4x4 transform = GetTransformMatrix(track, frame);
            WriteMatrix(writer, transform);
        }
        
        writer.WriteEndElement(); // float_array
        writer.WriteEndElement(); // source

        // Write sampler
        writer.WriteStartElement("sampler");
        writer.WriteAttributeString("id", $"{track.BoneName}_sampler");
        
        WriteInput(writer, "INPUT", $"{track.BoneName}_input");
        WriteInput(writer, "OUTPUT", $"{track.BoneName}_output");
        WriteInput(writer, "INTERPOLATION", "LINEAR");
        
        writer.WriteEndElement(); // sampler

        // Write channel
        writer.WriteStartElement("channel");
        writer.WriteAttributeString("source", $"#{track.BoneName}_sampler");
        writer.WriteAttributeString("target", $"{track.BoneName}/transform");
        writer.WriteEndElement(); // channel

        writer.WriteEndElement(); // animation
    }

    private static Matrix4x4 GetTransformMatrix(AnimationTrack track, int frame) 
    {
        Vector3 translation = track.GetTranslation(frame);
        Quaternion rotation = track.GetRotation(frame);
        Vector3 scale = track.GetScale(frame);

        return Matrix4x4.CreateScale(scale) *
               Matrix4x4.CreateFromQuaternion(rotation) *
               Matrix4x4.CreateTranslation(translation);
    }

    private static void WriteMatrix(XmlTextWriter writer, Matrix4x4 matrix) 
    {
        writer.WriteString($"{matrix.M11} {matrix.M12} {matrix.M13} {matrix.M14} ");
        writer.WriteString($"{matrix.M21} {matrix.M22} {matrix.M23} {matrix.M24} ");
        writer.WriteString($"{matrix.M31} {matrix.M32} {matrix.M33} {matrix.M34} ");
        writer.WriteString($"{matrix.M41} {matrix.M42} {matrix.M43} {matrix.M44} ");
    }

    private static void WriteInput(XmlTextWriter writer, string semantic, string source) 
    {
        writer.WriteStartElement("input");
        writer.WriteAttributeString("semantic", semantic);
        writer.WriteAttributeString("source", $"#{source}");
        writer.WriteEndElement();
    }

    private static void WriteVisualScene(XmlTextWriter writer, STSkeleton skeleton) 
    {
        writer.WriteStartElement("library_visual_scenes");
        writer.WriteStartElement("visual_scene");
        writer.WriteAttributeString("id", "Scene");
        
        // Write skeleton hierarchy
        foreach (var bone in skeleton.bones) 
        {
            if (bone.ParentIndex == -1) // Root bone
            {
                WriteBoneNode(writer, bone, skeleton);
            }
        }
        
        writer.WriteEndElement(); // visual_scene
        writer.WriteEndElement(); // library_visual_scenes
    }

    private static void WriteBoneNode(XmlTextWriter writer, STBone bone, STSkeleton skeleton) 
    {
        writer.WriteStartElement("node");
        writer.WriteAttributeString("id", bone.Text);
        writer.WriteAttributeString("name", bone.Text);
        writer.WriteAttributeString("type", "JOINT");
        
        // Write transform
        writer.WriteStartElement("matrix");
        writer.WriteAttributeString("sid", "transform");
        WriteMatrix(writer, bone.WorldMatrix);
        writer.WriteEndElement(); // matrix
        
        // Write child bones
        foreach (var childBone in skeleton.bones.Where(b => b.ParentIndex == bone.Index)) 
        {
            WriteBoneNode(writer, childBone, skeleton);
        }
        
        writer.WriteEndElement(); // node
    }

    private static void WriteAssetInfo(XmlTextWriter writer) 
    {
        writer.WriteStartElement("asset");
        
        writer.WriteStartElement("contributor");
        writer.WriteElementString("authoring_tool", "Switch Toolbox");
        writer.WriteEndElement();
        
        writer.WriteStartElement("created");
        writer.WriteString(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        writer.WriteEndElement();
        
        writer.WriteStartElement("modified");
        writer.WriteString(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        writer.WriteEndElement();
        
        writer.WriteStartElement("unit");
        writer.WriteAttributeString("meter", "1");
        writer.WriteAttributeString("name", "meter");
        writer.WriteEndElement();
        
        writer.WriteStartElement("up_axis");
        writer.WriteString("Y_UP");
        writer.WriteEndElement();
        
        writer.WriteEndElement(); // asset
    }
} 