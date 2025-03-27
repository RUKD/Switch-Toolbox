using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bfres.Structs;
using Toolbox.Library.Forms;
using BrightIdeasSoftware;
using OpenTK;

namespace FirstPlugin.Forms
{
    public partial class BoneAnimEditor : UserControl
    {
        public BoneAnimEditor()
        {
            InitializeComponent();

            listViewCustom2.ShowGroups = false;
            listViewCustom2.View = View.Details;

            listViewCustom2.BackColor = FormThemes.BaseTheme.FormBackColor;
            listViewCustom2.ForeColor = FormThemes.BaseTheme.FormForeColor;

            InitializeListView();
        }

        public List<KeyFrame> keyFrames = new List<KeyFrame>();

        public void LoadBoneAnim(FSKA.BoneAnimNode boneAnim)
        {
            listViewCustom2.BeginUpdate();
            listViewCustom2.Items.Clear();

            FSKA fska = (FSKA)boneAnim.Parent;

            if (boneAnim.BoneAnimU != null)
                stPropertyGrid1.LoadProperty(boneAnim.BoneAnimU, onPropertyChanged);
            else
                stPropertyGrid1.LoadProperty(boneAnim.BoneAnim, onPropertyChanged);

            frameCountLbl.Text = $" / {fska.FrameCount}";
            currentFrameUD.Maximum = fska.FrameCount;

            fska.SetFrame(0);
            for (int frame = 0; frame < fska.FrameCount; frame++)
            {
                bool IsKeyed = boneAnim.HasKeyedFrames(frame);
                if (IsKeyed)
                {
                    KeyFrame key = new KeyFrame();
                    key.Frame = frame;
                    keyFrames.Add(key);

                    Vector3 pos = boneAnim.GetPosition(frame);
                    var rot = boneAnim.GetRotation(frame);
                    Vector3 sca = boneAnim.GetScale(frame);

                    key.HasPosX = boneAnim.XPOS.HasAnimation();
                    key.HasPosY = boneAnim.YPOS.HasAnimation();
                    key.HasPosZ = boneAnim.ZPOS.HasAnimation();

                    key.HasRot = boneAnim.XROT.HasAnimation() || boneAnim.YROT.HasAnimation() || boneAnim.ZROT.HasAnimation() || boneAnim.WROT.HasAnimation();

                    key.HasScaleX = boneAnim.XSCA.HasAnimation();
                    key.HasScaleY = boneAnim.YSCA.HasAnimation();
                    key.HasScaleZ = boneAnim.ZSCA.HasAnimation();
                    

                    key.PosX = pos.X;
                    key.PosY = pos.Y;
                    key.PosZ = pos.Z;

                    key.RotX = rot.X;
                    key.RotY = rot.Y;
                    key.RotZ = rot.Z;
                    key.RotW = rot.W;

                    key.ScaX = sca.X;
                    key.ScaY = sca.Y;
                    key.ScaZ = sca.Z;

                    listViewCustom2.Items.Add(key.Frame.ToString()).SubItems.AddRange(
                        new ListViewItem.ListViewSubItem[]
                        {
                             new ListViewItem.ListViewSubItem() { Text = key.HasScaleX ? key.ScaX.ToString() : "None" },
                             new ListViewItem.ListViewSubItem() { Text = key.HasScaleY ? key.ScaY.ToString() : "None" },
                             new ListViewItem.ListViewSubItem() { Text = key.HasScaleZ ? key.ScaZ.ToString() : "None" },
                             
                             new ListViewItem.ListViewSubItem() { Text = key.HasRot ? key.RotX.ToString() : "None" },
                             new ListViewItem.ListViewSubItem() { Text = key.HasRot ? key.RotY.ToString() : "None" },
                             new ListViewItem.ListViewSubItem() { Text = key.HasRot ? key.RotZ.ToString() : "None" },
                             new ListViewItem.ListViewSubItem() { Text = key.HasRot ? key.RotW.ToString() : "None" },
                             
                             new ListViewItem.ListViewSubItem() { Text = key.HasPosX ? key.PosX.ToString() : "None" },
                             new ListViewItem.ListViewSubItem() { Text = key.HasPosY ? key.PosY.ToString() : "None" },
                             new ListViewItem.ListViewSubItem() { Text = key.HasPosZ ? key.PosZ.ToString() : "None" },
                        });
                }
            }
            listViewCustom2.EndUpdate();

        }

        public class KeyFrame
        {

            public int Frame { get; set; }

            public bool HasPosX { get; set; }
            public bool HasPosY { get; set; }
            public bool HasPosZ { get; set; }

            public bool HasRot { get; set; }

            public bool HasScaleX { get; set; }
            public bool HasScaleY { get; set; }
            public bool HasScaleZ { get; set; }

            public float PosX { get; set; }
            public float PosY { get; set; }
            public float PosZ { get; set; }

            public float RotX { get; set; }
            public float RotY { get; set; }
            public float RotZ { get; set; }
            public float RotW { get; set; } = 1;

            public float ScaX { get; set; } = 1;
            public float ScaY { get; set; } = 1;
            public float ScaZ { get; set; } = 1;

            const int size = 4;
            const int padding = 8;

            public override string ToString()
            {
                return String.Format("[{0}][{1}],[{2}],[{3}],[{4}],[{5}],[{6}],[{7}],[{8}],[{9}]", (Frame + 1).ToString().PadLeft(5),
                  PosX.ToString().ToFixedString(size, ' ').PadRight(padding),
                  PosY.ToString().ToFixedString(size, ' ').PadRight(padding),
                  PosZ.ToString().ToFixedString(size, ' ').PadRight(padding),
                  RotX.ToString().ToFixedString(size, ' ').PadRight(padding),
                  RotY.ToString().ToFixedString(size, ' ').PadRight(padding),
                  RotZ.ToString().ToFixedString(size, ' ').PadRight(padding),
                  RotW.ToString().ToFixedString(size, ' ').PadRight(padding),
                  ScaX.ToString().ToFixedString(size, ' ').PadRight(padding),
                  ScaY.ToString().ToFixedString(size, ' ').PadRight(padding),
                  ScaZ.ToString().ToFixedString(size, ' ').PadRight(padding));
            }
        }


        public void onPropertyChanged() { }

        private void stPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void listViewCustom2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCustom2.SelectedIndices.Count > 0)
            {
                var keyframe = keyFrames[listViewCustom2.SelectedIndices[0]];
                LoadData(keyframe);
            }
        }

        private void LoadData(KeyFrame keyFrame)
        {

        }

        private void InitializeListView()
        {
            listViewCustom2.View = View.Details;
            
            listViewCustom2.Columns.Clear();
            
            listViewCustom2.Columns.Add("Frame", 60);
            
            listViewCustom2.Columns.Add("Scale X", 80);
            listViewCustom2.Columns.Add("Scale Y", 80);
            listViewCustom2.Columns.Add("Scale Z", 80);
            
            listViewCustom2.Columns.Add("Rot X", 80);
            listViewCustom2.Columns.Add("Rot Y", 80);
            listViewCustom2.Columns.Add("Rot Z", 80);
            listViewCustom2.Columns.Add("Rot W", 80);
            
            listViewCustom2.Columns.Add("Pos X", 80);
            listViewCustom2.Columns.Add("Pos Y", 80);
            listViewCustom2.Columns.Add("Pos Z", 80);
        }
    }
}
