using System;
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

    public class STGenericShaderAssign
    {
        public string ShaderModel = "";
        public string ShaderArchive = "";

        public Dictionary<string, string> options = new Dictionary<string, string>();
        public Dictionary<string, string> samplers = new Dictionary<string, string>();
        public Dictionary<string, string> attributes = new Dictionary<string, string>();
    }

    public class STGenericMaterialParams
    {
        public string Name { get; set;}
        public Dictionary<string,string> parameters = new Dictionary<string, string>();
        public Dictionary<string,string> textures = new Dictionary<string, string>();
    }
    public class STGenericMaterial : TreeNodeCustom
    {
        public List<STGenericMatTexture> TextureMaps = new List<STGenericMatTexture>();

        //render info
        virtual public List<STGenericRenderInfo> GetRenderInfo()
        {
            return new List<STGenericRenderInfo>();
        }

        virtual public STGenericShaderAssign GetShaderAssign()
        {
            return null;
        }

        virtual public STGenericMaterialParams GetMaterialParams()
        {
            return null;
        }
        public override void OnClick(TreeView treeView)
        {

        }
    }
}
