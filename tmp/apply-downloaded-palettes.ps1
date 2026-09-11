$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
Add-Type -ReferencedAssemblies System.Drawing -TypeDefinition @'
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Globalization;
public static class DownloadedPaletteExport {
 static double Linear(double s){s/=255;return s<=0.04045?s/12.92:Math.Pow((s+0.055)/1.055,2.4);}
 static double F(double t){return t>0.008856?Math.Pow(t,1.0/3):7.787*t+16.0/116;}
 static double[] Lab(Color c){double r=Linear(c.R),g=Linear(c.G),b=Linear(c.B);double x=F((r*.4124564+g*.3575761+b*.1804375)/.95047),y=F(r*.2126729+g*.7151522+b*.0721750),z=F((r*.0193339+g*.1191920+b*.9503041)/1.08883);return new[]{116*y-16,500*(x-y),200*(y-z)};}
 public static void Run(string source,string hexFile,string dst){
  Directory.CreateDirectory(dst);var colors=new List<Color>();var labs=new List<double[]>();var allowed=new HashSet<int>();
  foreach(string raw in File.ReadAllLines(hexFile)){string h=raw.Trim().TrimStart('#');if(h.Length==0)continue;if(h.Length!=6)throw new Exception("Invalid HEX line: "+raw);int rgb=int.Parse(h,NumberStyles.HexNumber,CultureInfo.InvariantCulture);Color c=Color.FromArgb(255,(rgb>>16)&255,(rgb>>8)&255,rgb&255);colors.Add(c);labs.Add(Lab(c));allowed.Add(c.ToArgb());}
  if(colors.Count!=64)throw new Exception("Expected 64 colors in "+hexFile);
  File.Copy(hexFile,Path.Combine(dst,Path.GetFileName(hexFile)),false);
  string[] names={"knight-front","knight-side","knight-back","helmet","breastplate","sword","bow","arrow","shield","mace","lantern","urn","shrine","broken-column","banner","grass"};
  var cache=new Dictionary<int,Color>();var log=new List<string>();
  using(var atlas=new Bitmap(256,256,PixelFormat.Format32bppArgb)){
   for(int i=0;i<names.Length;i++)using(var input=new Bitmap(Path.Combine(source,names[i]+".png")))using(var output=new Bitmap(64,64,PixelFormat.Format32bppArgb)){
    if(input.Width!=64||input.Height!=64)throw new Exception("Wrong input size");
    for(int y=0;y<64;y++)for(int x=0;x<64;x++){
     Color c=input.GetPixel(x,y),mapped=Color.Transparent;
     if(c.A!=0){int key=c.ToArgb();if(!cache.TryGetValue(key,out mapped)){double[] lab=Lab(c);double best=double.MaxValue;int selected=0;for(int k=0;k<labs.Count;k++){double dl=lab[0]-labs[k][0],da=lab[1]-labs[k][1],db=lab[2]-labs[k][2],d=dl*dl+da*da+db*db;if(d<best){best=d;selected=k;}}mapped=colors[selected];cache[key]=mapped;}mapped=Color.FromArgb(c.A,mapped.R,mapped.G,mapped.B);}
     output.SetPixel(x,y,mapped);atlas.SetPixel((i%4)*64+x,(i/4)*64+y,mapped);
    }
    string target=Path.Combine(dst,names[i]+".png");output.Save(target,ImageFormat.Png);
    using(var verify=new Bitmap(target)){var used=new HashSet<int>();for(int y=0;y<64;y++)for(int x=0;x<64;x++){Color v=verify.GetPixel(x,y);if(v.A!=input.GetPixel(x,y).A)throw new Exception("Alpha changed");if(v.A>0){int rgb=Color.FromArgb(255,v.R,v.G,v.B).ToArgb();if(!allowed.Contains(rgb))throw new Exception("Off palette pixel");used.Add(rgb);}}log.Add(names[i]+".png: 64x64, "+used.Count+" palette colors, alpha unchanged, PASS");}
   }
   atlas.Save(Path.Combine(dst,"atlas-256.png"),ImageFormat.Png);
   using(var preview=new Bitmap(1024,1024,PixelFormat.Format32bppArgb)){for(int y=0;y<1024;y++)for(int x=0;x<1024;x++){Color c=atlas.GetPixel(x/4,y/4);preview.SetPixel(x,y,c.A==0?Color.FromArgb(35,42,48):c);}preview.Save(Path.Combine(dst,"preview-4x.png"),ImageFormat.Png);}
  }
  using(var swatches=new Bitmap(128,128)){for(int y=0;y<128;y++)for(int x=0;x<128;x++)swatches.SetPixel(x,y,colors[(y/16)*8+x/16]);swatches.Save(Path.Combine(dst,"palette.png"),ImageFormat.Png);}
  var gpl=new List<string>{"GIMP Palette","Name: "+Path.GetFileNameWithoutExtension(hexFile),"Columns: 8","# Converted from user's downloaded HEX file; original order preserved"};
  for(int i=0;i<colors.Count;i++)gpl.Add(colors[i].R+" "+colors[i].G+" "+colors[i].B+" Color-"+(i+1));
  File.WriteAllLines(Path.Combine(dst,"palette.gpl"),gpl);File.WriteAllLines(Path.Combine(dst,"validation.txt"),log);
 }
}
'@
$paletteSourceDir = 'C:/Unity/crossguard/output/ashen-sprites-64'
$paletteDestination = 'C:/Unity/crossguard/output/ashen-sprites-paletted'
foreach ($paletteName in @('resurrect-64','endesga-64')) {
 [DownloadedPaletteExport]::Run($paletteSourceDir,('E:/Aseprite/palettes/' + $paletteName + '.hex'),($paletteDestination + '/' + $paletteName))
 Get-Content -LiteralPath ($paletteDestination + '/' + $paletteName + '/validation.txt')
}
