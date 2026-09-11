Add-Type -AssemblyName System.Drawing
Add-Type -ReferencedAssemblies System.Drawing -TypeDefinition @'
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
public static class NativeSpriteExport {
 public static void Run(string src,string dst) {
  Directory.CreateDirectory(dst);
  string[] names={"knight-front","knight-side","knight-back","helmet","breastplate","sword","bow","arrow","shield","mace","lantern","urn","shrine","broken-column","banner","grass"};
  int[] xs={0,314,628,942,1254}, ys={0,373,669,949,1254};
  using(var input=new Bitmap(src)) using(var sheet=new Bitmap(256,256,PixelFormat.Format32bppArgb)) {
   var report=new List<string>();
   for(int i=0;i<16;i++) {
    int col=i%4,row=i/4,w=xs[col+1]-xs[col],h=ys[row+1]-ys[row];
    using(var cut=new Bitmap(w,h,PixelFormat.Format32bppArgb)) {
     for(int y=0;y<h;y++) for(int x=0;x<w;x++) cut.SetPixel(x,y,input.GetPixel(xs[col]+x,ys[row]+y));
     // Remove only bright neutral checkerboard connected to each cell's border.
     // Dark outlines protect the opaque sprite interior; this is a rough matte.
     bool[] visited=new bool[w*h]; var queue=new Queue<int>();
     for(int x=0;x<w;x++){queue.Enqueue(x);queue.Enqueue((h-1)*w+x);}
     for(int y=0;y<h;y++){queue.Enqueue(y*w);queue.Enqueue(y*w+w-1);}
     if(i==6)queue.Enqueue(155*w+150); // Empty space enclosed by the bow and string.
     if(i==10)queue.Enqueue(49*w+161); // Empty center of the lantern hanging ring.
     while(queue.Count>0){int n=queue.Dequeue();if(visited[n])continue;visited[n]=true;int x=n%w,y=n/w;Color c=cut.GetPixel(x,y);int lo=Math.Min(c.R,Math.Min(c.G,c.B)),hi=Math.Max(c.R,Math.Max(c.G,c.B));if(lo<160||hi-lo>26)continue;cut.SetPixel(x,y,Color.Transparent);for(int dy=-1;dy<=1;dy++)for(int dx=-1;dx<=1;dx++){int nx=x+dx,ny=y+dy;if(nx>=0&&nx<w&&ny>=0&&ny<h&&!visited[ny*w+nx])queue.Enqueue(ny*w+nx);}}
     int left=w,top=h,right=-1,bottom=-1;
     for(int y=0;y<h;y++)for(int x=0;x<w;x++)if(cut.GetPixel(x,y).A!=0){left=Math.Min(left,x);right=Math.Max(right,x);top=Math.Min(top,y);bottom=Math.Max(bottom,y);}
     int bw=right-left+1,bh=bottom-top+1;double scale=56.0/Math.Max(bw,bh);int tw=Math.Max(1,(int)Math.Round(bw*scale)),th=Math.Max(1,(int)Math.Round(bh*scale));
     using(var sprite=new Bitmap(64,64,PixelFormat.Format32bppArgb)) {
      int ox=(64-tw)/2,oy=60-th;
      for(int y=0;y<th;y++)for(int x=0;x<tw;x++){int sx=Math.Min(right,left+(int)((x+.5)*bw/tw)),sy=Math.Min(bottom,top+(int)((y+.5)*bh/th));sprite.SetPixel(ox+x,oy+y,cut.GetPixel(sx,sy));}
      sprite.Save(Path.Combine(dst,names[i]+".png"),ImageFormat.Png);
      int transparent=0;for(int y=0;y<64;y++)for(int x=0;x<64;x++){Color c=sprite.GetPixel(x,y);sheet.SetPixel(col*64+x,row*64+y,c);if(c.A==0)transparent++;}
      report.Add(names[i]+".png: 64x64 RGBA, transparent pixels="+transparent);
     }
    }
   }
   sheet.Save(Path.Combine(dst,"atlas-256.png"),ImageFormat.Png);
   using(var preview=new Bitmap(1024,1024,PixelFormat.Format32bppArgb)){
    for(int y=0;y<1024;y++)for(int x=0;x<1024;x++){Color c=sheet.GetPixel(x/4,y/4);preview.SetPixel(x,y,c.A==0?Color.FromArgb(35,42,48):c);}preview.Save(Path.Combine(dst,"preview-4x.png"),ImageFormat.Png);
   }
   File.WriteAllLines(Path.Combine(dst,"dimensions-check.txt"),report);
  }
 }
}
'@
[NativeSpriteExport]::Run('C:/Users/kasra/.codex/generated_images/01a07753-9e09-7493-939b-081153ef94fe/exec-d22dddd7-adbe-44d4-9568-f98d6950e94f.png','C:/Unity/crossguard/output/ashen-sprites-64')
Get-Content -LiteralPath 'C:/Unity/crossguard/output/ashen-sprites-64/dimensions-check.txt'
