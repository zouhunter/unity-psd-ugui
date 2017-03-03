# FullPSD2UGUI
full support to unity, from psd to ugui
NormalLayer	控件类型	"参数
（以:分隔）"	"子artlayer名
（命名前缀）"	子对象命名	【完成状态】	"备注： 
1、信息记录类型的对象@Size必须放置在最后一层"				
	父级		 @Size(若伸展可不写)	 	V					
	面板@Panel		b_		V					
	按扭@Button		"n_,p_,h_,d_"		V					
	双选@Toggle		"b_,m_"		V					
	滑动区@ScrollView	"v,h,vh"	"v_,b_, @Size（有背景可不写）"	"c_,vb_,hb"	V					
	手动条@Scrollbar	"l,r,t,b;0~1"	"b_,h_"		V					
	滑动条@Slider	"l,r,t,b"	"b_,f_,h_"		V					
	下拉框@Dropdown		"b1_,b2_,b3,l1_,l2,m_"	vb_	V					
	输入框@InputField		"b_,t_,h_"		V					
	组@Group	"v,h;0-n"			V					
	格子@Grid	"c,r;1-n"	 @Size (小格子的尺寸)		V					
										
										
										
										
ArtLayer	图片类型	唯一	公用	全局		"备注： 
1、solidfill（色块）类型的图片，将不会导出图，只记录信息，并生成方块"				
	image	"img,img#C"	img#N	img#G	V					
	slicedImage	img#C:S	img#N:S	img#G:S	V					
	texture	img#C:T	img#N:T	img#G:T	V					
										
										
										
										
	文字				V					
										
										
PSDName.xml	带Globle名的xml，会将图片导入到指定文件夹中									
