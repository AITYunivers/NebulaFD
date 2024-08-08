using Nebula;
using Nebula.Core.Data.Chunks.BankChunks.Shaders;
using Nebula.Core.Data.Chunks.ObjectChunks;
using Nebula.Core.Utilities;
using Nebula.Tools.GameDumper;
using Spectre.Console;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace GameDumper.AssetDumpers
{
    public class ShaderDumper
    {
        public static void Execute()
        {
            ProgressTask? task = AssetDump.ProgressContext!.AddTask($"[{NebulaCore.ColorRules[4]}]Dumping Shaders[/]", false);
            string path = "Dumps\\" + Utilities.ClearName(NebulaCore.PackageData.AppName) + "\\Shaders\\";
            ShaderBank shdBnk = NebulaCore.PackageData.ShaderBank;
            DX9ShaderBank dx9ShdBnk = NebulaCore.PackageData.DX9ShaderBank;
            task.MaxValue = shdBnk.Shaders.Count + dx9ShdBnk.Shaders.Count;
            Directory.CreateDirectory(path);

            Shader[] shdrs = shdBnk.Shaders.Values.ToArray();
            int[] keys = shdBnk.Shaders.Keys.ToArray();
            for (int i = 0; i < shdrs.Length; i++)
            {
                string filePath = path + Path.GetFileNameWithoutExtension(shdrs[i].Name) + (shdrs[i].Compiled ? ".fxc" : ".fx");
                File.WriteAllBytes(filePath, shdrs[i].FXData);
                filePath = path + Path.GetFileNameWithoutExtension(shdrs[i].Name) + ".xml";

                string fxData = shdBnk[keys[i]].GetFXData();
                if (dx9ShdBnk.Shaders.ContainsKey(keys[i]))
                    fxData = dx9ShdBnk[keys[i]].GetFXData();
                File.WriteAllText(filePath, GenerateXML(shdrs[i], fxData));

                task.Value++;
            }

            shdrs = dx9ShdBnk.Shaders.Values.ToArray();
            keys = dx9ShdBnk.Shaders.Keys.ToArray();
            for (int i = 0; i < shdrs.Length; i++)
            {
                string filePath = path + "\\" + shdrs[i].Name;
                File.WriteAllBytes(filePath, shdrs[i].FXData);
                filePath = path + Path.GetFileNameWithoutExtension(shdrs[i].Name) + ".xml";

                string fxData = dx9ShdBnk[keys[i]].GetFXData();
                File.WriteAllText(filePath, GenerateXML(shdrs[i], fxData));

                task.Value++;
            }
            task.StopTask();
        }

        public static string GenerateXML(Shader shdr, string fxData)
        {
            fxData = Regex.Replace(fxData, @"\s+", "");
            StringBuilder sb = new StringBuilder();
            XmlWriter w = XmlWriter.Create(sb, new XmlWriterSettings()
            {
                Indent = true,
                Encoding = Encoding.Unicode,
                OmitXmlDeclaration = true
            });
            w.WriteStartDocument();

            w.WriteStartElement("effect");
            List<string> shdName = Path.GetFileNameWithoutExtension(shdr.Name).Replace('_', ' ').Replace('-', ' ').Split(' ').ToList();
            shdName.RemoveAll(x => x.Length == 0);
            for (int i = 0; i < shdName.Count; i++)
                shdName[i] = shdName[i].Substring(0, 1).ToUpper() + shdName[i].Substring(1);
            w.WriteElementString("name", string.Join(' ', shdName));
            w.WriteElementString("name", Path.GetFileNameWithoutExtension(shdr.Name));
            w.WriteElementString("description", "Decompiled using Nebula by Yunivers");
            w.WriteElementString("author", "Unknown");

            int texParamCnt = shdr.Parameters.Where(x => x.Type == 3).ToArray().Length;
            if (fxData.Contains($"register(s{texParamCnt + 1})"))
                w.WriteElementString("BackgroundTexture", "1");
            else if (texParamCnt > 0)
                w.WriteElementString("BackgroundTexture", "0");

            if (NebulaCore.D3D == 8)
                w.WriteElementString("dx8", "1");

            int sampler = 0;
            while (true)
            {
                if (fxData.Contains($"register(s{sampler})"))
                {
                    if (fxData.Contains($"register(s{sampler})=sampler_state{{MinFilter=linear;") ||
                        fxData.Contains($"register(s{sampler})=sampler_state{{MagFilter=linear;"))
                    {
                        w.WriteStartElement("sampler");
                        w.WriteElementString("index", sampler.ToString());
                        w.WriteElementString("filter", "LINEAR");
                        w.WriteEndElement();
                    }
                }
                else break;
                sampler++;
            }

            int[] shdrParams = new int[shdr.Parameters.Length];
            foreach (ObjectInfo obj in NebulaCore.PackageData.FrameItems.Items.Values)
                if (obj.Shader.ShaderHandle == shdr.Handle)
                {
                    shdrParams = obj.Shader.ShaderParameters;
                    break;
                }

            for (int i = 0; i < shdr.Parameters.Length; i++)
            {
                ShaderParameter param = shdr.Parameters[i];
                w.WriteStartElement("parameter");
                List<string> name = param.Name.Replace('_', ' ').Replace('-', ' ').Split(' ').ToList();
                name.RemoveAll(x => x.Length == 0);
                for (int ii = 0; ii < name.Count; ii++)
                    name[ii] = name[ii].Substring(0, 1).ToUpper() + name[ii].Substring(1);
                w.WriteElementString("name", string.Join(' ', name));
                w.WriteElementString("code", param.Name);
                int value = shdrParams[i];
                switch (param.Type)
                {
                    case 0:
                        w.WriteElementString("type", "int");
                        if (fxData.Contains("bool" + param.Name + ";") ||
                            fxData.Contains("bool" + param.Value + "="))
                            w.WriteElementString("property", "checkbox");
                        else
                            w.WriteElementString("property", "edit");
                        w.WriteElementString("value", value.ToString());
                        break;
                    case 1:
                        w.WriteElementString("type", "float");
                        w.WriteElementString("property", "edit");
                        w.WriteElementString("value", value == 0 ? value.ToString() : BitConverter.Int32BitsToSingle(value).ToString());
                        break;
                    case 2:
                        w.WriteElementString("type", "INT_FLOAT4");
                        w.WriteElementString("property", "COLOR");
                        w.WriteElementString("value", value.ToString());
                        break;
                    case 3:
                        w.WriteElementString("type", "IMAGE");
                        w.WriteElementString("property", "image");
                        break;
                }
                w.WriteEndElement();
            }

            w.WriteEndElement();
            w.Flush();
            return sb.ToString();
        }
    }
}
