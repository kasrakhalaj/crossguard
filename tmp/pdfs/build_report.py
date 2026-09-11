from pathlib import Path
import re, html, json
from reportlab.pdfgen import canvas
from reportlab.platypus import SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle, PageBreak, KeepTogether
from reportlab.lib.styles import ParagraphStyle
from reportlab.lib import colors
from reportlab.lib.enums import TA_LEFT
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from pypdf import PdfReader

ROOT=Path('C:/Unity/crossguard')
SRC=ROOT/'tmp/pdfs/report-source.md'
OUT=ROOT/'output/pdf/crossguard-project-and-steam-assessment.pdf'
OUT.parent.mkdir(parents=True,exist_ok=True)
pdfmetrics.registerFont(TTFont('Report','C:/Windows/Fonts/calibri.ttf'))
pdfmetrics.registerFont(TTFont('ReportBold','C:/Windows/Fonts/calibrib.ttf'))
pdfmetrics.registerFontFamily('Report',normal='Report',bold='ReportBold',italic='Report',boldItalic='ReportBold')
ink=colors.HexColor('#183136'); teal=colors.HexColor('#087B80'); muted=colors.HexColor('#53676B')
styles={
 'body':ParagraphStyle('body',fontName='Report',fontSize=10.7,leading=14.2,textColor=ink,spaceAfter=9),
 'title':ParagraphStyle('title',fontName='ReportBold',fontSize=28,leading=31,textColor=ink,spaceAfter=12),
 'h2':ParagraphStyle('h2',fontName='ReportBold',fontSize=20,leading=24,textColor=ink,spaceAfter=15),
 'meta':ParagraphStyle('meta',fontName='Report',fontSize=10,leading=13,textColor=muted,spaceAfter=10),
 'cell':ParagraphStyle('cell',fontName='Report',fontSize=9.4,leading=12,textColor=ink,spaceAfter=0),
 'headcell':ParagraphStyle('headcell',fontName='ReportBold',fontSize=9.4,leading=12,textColor=colors.white),
 'bullet':ParagraphStyle('bullet',fontName='Report',fontSize=10.2,leading=13.6,textColor=ink,leftIndent=10,firstLineIndent=-8,spaceAfter=7),
}

def fmt(s):
    links=[]
    def link(m):
        links.append(f'<a href="{html.escape(m.group(2),quote=True)}" color="#087B80">{html.escape(m.group(1))}</a>')
        return f'ZZLINK{len(links)-1}ZZ'
    s=re.sub(r'\[([^\]]+)\]\(([^)]+)\)',link,s)
    s=html.escape(s)
    s=re.sub(r'\*\*(.+?)\*\*',r'<b>\1</b>',s)
    for i,v in enumerate(links):s=s.replace(f'ZZLINK{i}ZZ',v)
    return s

def para(s,style='body'):return Paragraph(fmt(s),styles[style])

def page_elements(text):
    result=[]; lines=text.strip().splitlines(); i=0
    while i<len(lines):
        line=lines[i].strip()
        if not line:i+=1;continue
        if line.startswith('|'):
            rows=[]
            while i<len(lines) and lines[i].strip().startswith('|'):
                vals=[v.strip() for v in lines[i].strip().strip('|').split('|')]
                if not all(re.fullmatch(r':?-+:?',v) for v in vals):rows.append(vals)
                i+=1
            n=len(rows[0]); width=499.27
            widths=([112,205,width-317] if n==3 and rows[0][0]=='Area' else
                    [85,218,width-303] if n==3 else
                    [245,75,90,width-410] if n==4 else [width/n]*n)
            data=[[para(v,'headcell' if r==0 else 'cell') for v in row] for r,row in enumerate(rows)]
            t=Table(data,colWidths=widths,repeatRows=1,hAlign='LEFT')
            t.setStyle(TableStyle([('BACKGROUND',(0,0),(-1,0),ink),('VALIGN',(0,0),(-1,-1),'TOP'),('LEFTPADDING',(0,0),(-1,-1),8),('RIGHTPADDING',(0,0),(-1,-1),8),('TOPPADDING',(0,0),(-1,-1),8),('BOTTOMPADDING',(0,0),(-1,-1),8),('ROWBACKGROUNDS',(0,1),(-1,-1),[colors.HexColor('#EDF5F4'),colors.HexColor('#F7F9F9')]),('LINEBELOW',(0,0),(-1,0),0.7,teal)]))
            result.extend([t,Spacer(1,13)]);continue
        if line.startswith('# '):result.append(para(line[2:],'title'));i+=1;continue
        if line.startswith('## '):result.append(para(line[3:],'h2'));i+=1;continue
        if line.startswith('- '):result.append(para('- '+line[2:],'bullet'));i+=1;continue
        chunk=[line];i+=1
        while i<len(lines) and lines[i].strip() and not lines[i].startswith(('#','|','- ')):
            chunk.append(lines[i].strip());i+=1
        s=' '.join(chunk)
        result.append(para(s,'meta' if s.startswith(('Project, design','Prepared for Kasra')) else 'body'))
    return result

class NumberedCanvas(canvas.Canvas):
    def __init__(self,*a,**kw):super().__init__(*a,**kw);self.saved=[]
    def showPage(self):self.saved.append(dict(self.__dict__));self._startPage()
    def save(self):
        total=len(self.saved)
        for state in self.saved:
            self.__dict__.update(state); self.decorate(total); super().showPage()
        super().save()
    def decorate(self,total):
        self.setFillColor(teal);self.rect(48,803,25,3,fill=1,stroke=0)
        self.setFont('ReportBold',8.7);self.setFillColor(muted)
        self.drawString(82,801,'CROSSGUARD / PROJECT & MARKET ASSESSMENT')
        self.setStrokeColor(colors.HexColor('#D5E3E2'));self.line(48,39,547,39)
        self.setFont('Report',8.3);self.drawString(48,26,'Prepared for Kasra | 6 September 2026')
        self.drawRightString(547,26,f'{self._pageNumber} / {total}')

text=SRC.read_text(encoding='utf-8');pages=text.split('<!-- PAGE -->');story=[]
for n,page in enumerate(pages):
    if n:story.append(PageBreak())
    story.extend(page_elements(page))
doc=SimpleDocTemplate(str(OUT),pagesize=(595.27,841.89),rightMargin=48,leftMargin=48,topMargin=60,bottomMargin=53,title='Crossguard: Project and Steam Market Assessment',author='Prepared for Kasra',pageCompression=1)
doc.build(story,canvasmaker=NumberedCanvas)
reader=PdfReader(str(OUT)); audit={'pages':len(reader.pages),'intended_pages':len(pages),'bytes':OUT.stat().st_size,'links':sum(len(p.get('/Annots',[])) for p in reader.pages),'pages_detail':[]}
for i,p in enumerate(reader.pages):
    txt=p.extract_text() or ''
    audit['pages_detail'].append({'page':i+1,'words':len(txt.split()),'start':txt[:110],'end':txt[-180:]})
(ROOT/'tmp/pdfs/pdf-qa.json').write_text(json.dumps(audit,indent=2),encoding='utf-8')
print(json.dumps(audit,indent=2))
