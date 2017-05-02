// **************************************************
// This file created by Brett Bibby (c) 2010-2013
// You may freely use and modify this file as you see fit
// You may not sell it
//**************************************************
// hidden object game exporter
//$.writeln("=== Starting Debugging Session ===");
//@include "stdlib.jsx"
// enable double clicking from the Macintosh Finder or the Windows Explorer
#target photoshop

// debug level: 0-2 (0:disable, 1:break on error, 2:break at beginning)
// $.level = 0;
// debugger; // launch debugger on next line

var sceneData;
var sourcePsd;
var duppedPsd;
var destinationFolder;
var uuid;
var sourcePsdName;
var exportSolid;
main();

function main(){
    //选择是否导出色框
    PreSelect();
    // got a valid document?
    if (app.documents.length <= 0)
    {
        if (app.playbackDisplayDialogs != DialogModes.NO)
        {
            alert("You must have a document open to export!");
        }
        // quit, returning 'cancel' makes the actions palette not record our script
        return 'cancel';
    }

    // ask for where the exported files should go
    destinationFolder = Folder.selectDialog("Choose the destination for export.");
    if (!destinationFolder)
    {
        return;
    }

    // cache useful variables
    uuid = 1;
    sourcePsdName = app.activeDocument.name;
    var layerCount = app.documents[sourcePsdName].layers.length;
    var layerSetsCount = app.documents[sourcePsdName].layerSets.length;

    if ((layerCount <= 1) && (layerSetsCount <= 0))
    {
        if (app.playbackDisplayDialogs != DialogModes.NO)
        {
            alert("You need a document with multiple layers to export!");
            // quit, returning 'cancel' makes the actions palette not record our script
            return 'cancel';
        }
    }

    // setup the units in case it isn't pixels
    var savedRulerUnits = app.preferences.rulerUnits;
    var savedTypeUnits = app.preferences.typeUnits;
    app.preferences.rulerUnits = Units.PIXELS;
    app.preferences.typeUnits = TypeUnits.PIXELS;

    // duplicate document so we can extract everythng we need
    duppedPsd = app.activeDocument.duplicate();
    duppedPsd.activeLayer = duppedPsd.layers[duppedPsd.layers.length - 1];

    hideAllLayers(duppedPsd);

    // export layers
    sceneData = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
    sceneData += "<PSDUI>";

    sceneData += "<psdSize>";
    sceneData += "<width>" + duppedPsd.width.value + "</width>";
    sceneData += "<height>" + duppedPsd.height.value+ "</height>";
    sceneData += "</psdSize>";

    exportLayerSet(app.activeDocument);

    sceneData += "</PSDUI>";
    $.writeln(sceneData);

    duppedPsd.close(SaveOptions.DONOTSAVECHANGES);

    // create export
    var sceneFile = new File(destinationFolder + "/" + destinationFolder.name + ".xml");
    sceneFile.open('w');
    sceneFile.writeln(sceneData);
    sceneFile.close();

    app.preferences.rulerUnits = savedRulerUnits;
    app.preferences.typeUnits = savedTypeUnits;
}

function exportLayerSet(obj)
{
    sceneData += "<layers>";
    for (var i = obj.layers.length - 1; 0 <= i; i--)
    {
        if (obj.layers[i].typename == "LayerSet")//空节点（组）
        {
            if (obj.layers[i].name.toUpperCase().search("@SCROLLVIEW") >= 0)
            {
                exportScrollView(obj.layers[i]);
            }
            else if (obj.layers[i].name.toUpperCase().search("@GRID") >= 0)
            {
                exportGrid(obj.layers[i]);
            }
            else if (obj.layers[i].name.toUpperCase().search("@GROUP")>=0) {
                exportGroup(obj.layers[i]);
            }
            else if (obj.layers[i].name.toUpperCase().search("@DROPDOWN")>=0) {
                exportDropdown(obj.layers[i]);
            }
            else if (obj.layers[i].name.toUpperCase().search("@BUTTON") >= 0)
            {
                exportButton(obj.layers[i]);
            }
            else if (obj.layers[i].name.toUpperCase().search("@TOGGLE") >= 0)
            {
                exportToggle(obj.layers[i]);
            }
            else if (obj.layers[i].name.toUpperCase().search("@SLIDER")>=0) {
                exportSlider(obj.layers[i]);
            }
           
            else if (obj.layers[i].name.toUpperCase().search("@INPUTFIELD") >=0) {
                exportInputField(obj.layers[i]);
            }
            else if (obj.layers[i].name.toUpperCase().search("@SCROLLBAR") >=0) {
                exportScrollBar(obj.layers[i]);
            }
            else
            {
                exportPanel(obj.layers[i]);
            }
        }
    }
    sceneData += "</layers>";
}
function exportScrollView(obj)
{
    var itemName = obj.name.substring(0, obj.name.search("@"));
    sceneData += ("<Layer>\n<type>ScrollView</type>\n<name>" + itemName + "</name>\n");

    exportLayerSet(obj);

    var params = obj.name.split(":");

    if (params.length > 2)
    {
        alert(obj.name + "-------Layer's name is illegal------------");
    }

    var index = recordPositionAndSize(obj);

    sceneData += "<images>";
    for (var j = 0; j < obj.artLayers.length; j++)
    {
        if(j != index)
            exportArtLayer(obj.artLayers[j]);
    }
    sceneData += "</images>";

    sceneData += "<arguments>";
    sceneData += "<string>" + params[1] + "</string>";     //滑动方向
    sceneData += "</arguments>";

    sceneData += "</Layer>";
}

function exportGrid(obj)
{
    var itemName = obj.name.substring(0, obj.name.search("@"));
    sceneData += ("<Layer>\n<type>Grid</type>\n<name>" + itemName + "</name>\n");
    
    exportLayerSet(obj);

    var params = obj.name.split(":");

    if (params.length < 3)
    {
        alert(obj.name + "-------Layer's name not equals 2------------");
    }


    var index = recordPositionAndSize(obj);

    sceneData += "<images>";
    for (var j = 0; j < obj.artLayers.length; j++)
    {
        if(j != index)
            exportArtLayer(obj.artLayers[j]);
    }
    sceneData += "</images>";
  
    sceneData += "<arguments>";
    sceneData += "<string>" + params[1] + "</string>";   
    sceneData += "<string>" + params[2] + "</string>";   
    sceneData += "</arguments>";

    sceneData += "</Layer>";
}

function exportGroup(obj)
{
    var itemName = obj.name.substring(0, obj.name.search("@"));
    sceneData += ("<Layer>\n<type>Group</type>\n<name>" + itemName + "</name>\n");

    exportLayerSet(obj);

    var params = obj.name.split(":");

    if (params.length < 3)
    {
        var ipt = ""; 
        while (!(ipt.toLowerCase() == "v" || ipt.toLowerCase() == "h")) {
            ipt = prompt("方向选择：请输入以下方向中的一个:\nh:水平;v:垂直");
        }
        params[1] = ipt;
        while (IsNumber(ipt)) {
            ipt = prompt("间距离选择：请输入一像素值");
        }
        params[2] = ipt;

    }

    var index = recordPositionAndSize(obj);

    sceneData += "<images>";
    for (var j = 0; j < obj.artLayers.length; j++)
    {
        if(index != j)
            exportArtLayer(obj.artLayers[j]);
    }
    sceneData += "</images>";
  
    sceneData += "<arguments>";
    sceneData += "<string>" + params[1] + "</string>";   
    sceneData += "<string>" + params[2] + "</string>";   
    sceneData += "</arguments>";

    sceneData += "</Layer>";
}

function IsNumber(object){
    var nub = Number(object);
    return isNaN(nub);
}

function exportDropdown(obj)
{
    var itemName = obj.name.substring(0, obj.name.search("@"));
    sceneData += ("<Layer>\n<type>Dropdown</type>\n<name>" + itemName + "</name>\n");

    exportLayerSet(obj);

    sceneData += "<images>\n";

    for (var j = obj.artLayers.length - 1; 0 <= j; j--)
    {
        exportArtLayer(obj.artLayers[j]);
    }

    sceneData += "\n</images>\n</Layer>";
}

function exportInputField(obj)
{
    var itemName = obj.name.substring(0, obj.name.search("@"));
    sceneData += ("<Layer>\n<type>InputField</type>\n<name>" + itemName + "</name>\n");

    sceneData += "<images>\n";

    for (var i = obj.layers.length - 1; 0 <= i; i--)
    {
        exportArtLayer(obj.layers[i]);
    }

    sceneData += "\n</images>\n</Layer>";
}

function exportButton(obj)
{
    var itemName = obj.name.substring(0, obj.name.search("@"));
    sceneData += ("<Layer>\n<type>Button</type>\n<name>" + itemName + "</name>\n");

    sceneData += "<images>\n";

    for (var i = obj.layers.length - 1; 0 <= i; i--)
    {
        exportArtLayer(obj.layers[i]);
    }

    sceneData += "\n</images>\n</Layer>";
}

function exportToggle(obj)
{
    var itemName = obj.name.substring(0, obj.name.search("@"));
    sceneData += ("<Layer>\n<type>Toggle</type>\n<name>" + itemName + "</name>\n");

    sceneData += "<images>\n";

    for (var i = obj.layers.length - 1; 0 <= i; i--)
    {
        exportArtLayer(obj.layers[i]);
    }

    sceneData += "\n</images>\n</Layer>";
}

function exportSlider(obj)
{
    var itemName = obj.name.substring(0, obj.name.search("@"));
    sceneData += ("<Layer>\n<type>Slider</type>\n<name>" + itemName + "</name>\n");

    var params = obj.name.split(":");

    if (params.length < 2)
    {
        var ipt = ""; 
        while (!(ipt.toLowerCase() == "l" || ipt.toLowerCase()== "r"||ipt.toLowerCase()=="b")||ipt.toLowerCase() == "t") {
            ipt = prompt("请选择"+ obj.name + "的方向：\nl:从左到右;r:从右到左;b:从下到上;t:从上到下");
        }
        params[1] = ipt;
    }

    sceneData += "<arguments>";
    sceneData += "<string>" + params[1] + "</string>"; //滑动方向
    sceneData += "</arguments>";


    sceneData += "<images>\n";

    for (var i = obj.layers.length - 1; 0 <= i; i--)
    {
        exportArtLayer(obj.layers[i]);
    }

    sceneData += "\n</images>\n</Layer>";
}

function exportScrollBar(obj)
{
    var itemName =  obj.name.substring(0, obj.name.search("@"));
    sceneData += ("<Layer>\n<type>ScrollBar</type>\n<name>" + itemName + "</name>\n");

    var params = obj.name.split(":");

    if (params.length < 2)
    {
        var ipt = ""; 
        while (!(ipt.toLowerCase() == "l" || ipt.toLowerCase()== "r"||ipt.toLowerCase()=="b")||ipt.toLowerCase() == "t") {
            ipt = prompt("请选择"+ obj.name + "的方向：\nl:从左到右;r:从右到左;b:从下到上;t:从上到下");
        }
        params[1] = ipt;
    }

    sceneData += "<arguments>";
    sceneData += "<string>" + params[1] + "</string>"; //滑动方向
    sceneData += "</arguments>";

    sceneData += "<images>\n";

    for (var i = obj.layers.length - 1; 0 <= i; i--)
    {
        exportArtLayer(obj.layers[i]);
    }

    sceneData += "\n</images>\n</Layer>";
}

function exportPanel(obj)
{
    var itemName = obj.name;
    sceneData += ("<Layer>\n<type>Panel</type>\n<name>" + itemName + "</name>\n");

    exportLayerSet(obj);
    
    var index = recordPositionAndSize(obj);
	
    sceneData += "<images>";
    for (var j = 0; j < obj.artLayers.length; j++)
    {
        if(j!= index)
            exportArtLayer(obj.artLayers[j]);
    }

    sceneData += "\n</images>\n</Layer>";
}

function exportArtLayer(obj)
{
    sceneData += "<Image>\n";
	
    if (LayerKind.TEXT == obj.kind)
    {
        exportLabel(obj);
    }
    else
    {
        exportImage(obj);
    }
    sceneData += "</Image>\n";
}

function exportLabel(obj)
{
    sceneData += "<imageType>" + "Label" + "</imageType>\n";
    var validFileName = makeValidFileName(obj.name);
    sceneData += "<name>" + validFileName + "</name>\n";
    obj.visible = true;
    saveScenePng(duppedPsd.duplicate(), validFileName, false);
    obj.visible = false;
    sceneData += "<arguments>";
    sceneData += "<string>" + "#" +obj.textItem.color.rgb.hexValue + (Math.floor(obj.opacity * 2.55)).toString(16) + "</string>";
    sceneData += "<string>" + obj.textItem.font + "</string>";
    try{
        sceneData += "<string>" + obj.textItem.size.value + "</string>";
    }
    catch(err){
        alert(err);
        sceneData += "<string>" + 22 + "</string>";
    }
    sceneData += "<string>" + obj.textItem.contents + "</string>";
    sceneData += "</arguments>";
}

function exportImage(obj)
{
    var validFileName = obj.name;
    if (obj.name.match("#")) {
        validFileName = obj.name.substring (0,obj.name.indexOf ("#"));//截取#之前的字符串作为图片的名称。
    }
    if (!obj.name.match("#") || obj.name.match("#C")) {//私用图片自动为图片增加后缀
        validFileName += "_" + uuid++;
    }
    validFileName = makeValidFileName(validFileName);
    sceneData += "<name>" + validFileName + "</name>\n";

    if(exportSolid && obj.kind == LayerKind.SOLIDFILL)
    {
        sceneData += "<arguments>";
        sceneData += "<string>" + "#" + getLayerColor(obj) + (Math.floor(obj.opacity * 2.55)).toString(16) + "</string>";
        sceneData += "</arguments>";
		
        var recSize = getLayerRec(duppedPsd.duplicate());
        sceneData += "<position>";
        sceneData += "<x>" + recSize.x + "</x>";
        sceneData += "<y>" + recSize.y + "</y>";
        sceneData += "</position>";

        sceneData += "<size>";
        sceneData += "<width>" + recSize.width + "</width>";
        sceneData += "<height>" + recSize.height + "</height>";
        sceneData += "</size>";
		
        obj.visible = false;
    }
    else{
        obj.visible = true;
        saveScenePng(duppedPsd.duplicate(), validFileName, true);
        obj.visible = false;
   
        if(obj.name.match("#G"))
        {
            sceneData += "<imageSource>" + "Globle" + "</imageSource>\n";
        }
        else  if (obj.name.match("#N"))
        {
            sceneData += "<imageSource>" + "Normal" + "</imageSource>\n";
        }
        else
        {
            sceneData += "<imageSource>" + "Custom" + "</imageSource>\n";
        }
    }

    if (obj.name.match("#T")) {
        sceneData += "<imageType>" + "Texture" + "</imageType>\n";
    }
    else if(obj.name.match("#S"))
    {
        sceneData += "<imageType>" + "SliceImage" + "</imageType>\n";
    }
    else{
        sceneData += "<imageType>" + "Image" + "</imageType>\n";
    }
}

//记录最后一层有@Size的layer尺寸和坐标
function recordPositionAndSize(obj)
{
    var index = -1;
    for (var i = 0; i < obj.artLayers.length; i++) {
        if(obj.artLayers[i].name.toUpperCase().search("@SIZE") == 0)
        {
            index = i;
        }
    }
	

    if(index != -1)
    {
        var recSize;
        obj.artLayers[index].visible = true;

        recSize = getLayerRec(duppedPsd.duplicate());

        sceneData += "<position>";
        sceneData += "<x>" + recSize.x + "</x>";
        sceneData += "<y>" + recSize.y + "</y>";
        sceneData += "</position>";

        sceneData += "<size>";
        sceneData += "<width>" + recSize.width + "</width>";
        sceneData += "<height>" + recSize.height + "</height>";
        sceneData += "</size>";

        obj.artLayers[index].visible = false;
    }
    
    return index;
}

//获取层级颜色
function getLayerColor(obj){
    var desc = Stdlib.getLayerDescriptor(app.activeDocument, obj);
    var adjs = desc.getList(cTID('Adjs'));
    var clrDesc = adjs.getObjectValue(0);
    var color= clrDesc.getObjectValue(cTID('Clr '));
    var red = Math.round(color.getDouble(cTID('Rd  ')));
    var green = Math.round(color.getDouble(cTID('Grn ')));
    var blue = Math.round(color.getDouble(cTID('Bl  ')));
    return Stdlib.createRGBColor(red, green, blue).rgb.hexValue;
}

function hideAllLayers(obj)
{
    hideLayerSets(obj);
}

function hideLayerSets(obj)
{
    for (var i = obj.layers.length - 1; 0 <= i; i--)
    {
        if (obj.layers[i].typename == "LayerSet")
        {
            hideLayerSets(obj.layers[i]);
        }
        else
        {
            obj.layers[i].visible = false;
        }
    }
}

function getLayerRec(psd)
{
    // we should now have a single art layer if all went well
    psd.mergeVisibleLayers();
    // figure out where the top-left corner is so it can be exported into the scene file for placement in game
    // capture current size
    var height = psd.height.value;
    var width = psd.width.value;
    var top = psd.height.value;
    var left = psd.width.value;
    // trim off the top and left
    psd.trim(TrimType.TRANSPARENT, true, true, false, false);
    // the difference between original and trimmed is the amount of offset
    top -= psd.height.value;
    left -= psd.width.value;
    // trim the right and bottom
    psd.trim(TrimType.TRANSPARENT);
    // find center
    top += (psd.height.value / 2)
    left += (psd.width.value / 2)
    // unity needs center of image, not top left
    top = -(top - (height / 2));
    left -= (width / 2);

    height = psd.height.value;
    width = psd.width.value;

    psd.close(SaveOptions.DONOTSAVECHANGES);

    return {
        y: top,
        x: left,
        width: width,
        height: height
    };
}

function saveScenePng(psd, fileName, writeToDisk)
{
    // we should now have a single art layer if all went well
    psd.mergeVisibleLayers();
    // figure out where the top-left corner is so it can be exported into the scene file for placement in game
    // capture current size
    var height = psd.height.value;
    var width = psd.width.value;
    var top = psd.height.value;
    var left = psd.width.value;
    // trim off the top and left
    psd.trim(TrimType.TRANSPARENT, true, true, false, false);
    // the difference between original and trimmed is the amount of offset
    top -= psd.height.value;
    left -= psd.width.value;
    // trim the right and bottom
    psd.trim(TrimType.TRANSPARENT);
    // find center
    top += (psd.height.value / 2)
    left += (psd.width.value / 2)
    // unity needs center of image, not top left
    top = -(top - (height / 2));
    left -= (width / 2);

    height = psd.height.value;
    width = psd.width.value;


    if (writeToDisk)
    {
        // save the image
        var pngFile = new File(destinationFolder + "/" + fileName + ".png");
        var pngSaveOptions = new PNGSaveOptions();
        psd.saveAs(pngFile, pngSaveOptions, true, Extension.LOWERCASE);
    }
    psd.close(SaveOptions.DONOTSAVECHANGES);

    var rec = {
        y: top,
        x: left,
        width: width,
        height: height
    };
    // save the scene data
    sceneData += "<position>";
    sceneData += "<x>" + rec.x + "</x>";
    sceneData += "<y>" + rec.y + "</y>";
    sceneData += "</position>";

    sceneData += "<size>";
    sceneData += "<width>" + rec.width + "</width>";
    sceneData += "<height>" + rec.height + "</height>";
    sceneData += "</size>";
}

function makeValidFileName(fileName)
{
    var validName = fileName.replace(/^\s+|\s+$/gm, ''); // trim spaces
    validName = validName.replace(/[\\\*\/\?:"\|<>]/g, ''); // remove characters not allowed in a file name
    validName = validName.replace(/[ ]/g, '_'); // replace spaces with underscores, since some programs still may have troubles with them
    $.writeln(validName);
    return validName;
}

function PreSelect()
{
    exportSolid =confirm("不导出色块为图片？");
    //var ipt=prompt("请输入您的名字","KING视界")
}