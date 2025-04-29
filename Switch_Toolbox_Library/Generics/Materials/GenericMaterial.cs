﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library
{
    public class STGenericRenderInfo
    {
        public string Name { get; set;}
        public string Value{get;set;}
    }
    public class STGenericMaterial : TreeNodeCustom
    {
        public List<STGenericMatTexture> TextureMaps = new List<STGenericMatTexture>();

        //render info
        virtual public List<STGenericRenderInfo> GetRenderInfo()
        {
            return new List<STGenericRenderInfo>();
        }
        public override void OnClick(TreeView treeView)
        {

        }
    }
}
