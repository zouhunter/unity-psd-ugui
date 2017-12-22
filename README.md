## about this project:
   as a programmer,sometime I have to make ui in unity ,and program later. but it just not my work at all,
   and the ui desiner dont think it is his job neither. last year we extend the Open Source project of PSD2UGUI
   to support my work,it`s excellent and useful.but the question is I have to Open PS to Exprot Sprite and 
   Import to Unity,that is so disappointed. however in github we fond some other Solution to use psd : 
   read *.psd file by c# replace ps place;

## 1.what this project can help you ?

*  load psd from file,preview it in unity;
*  create atlas、sprite or Textures in unity;
*  generate UGUI gameobject automaticly;

## 2.very simple to use.
 * "PsdUnity/ConfigWindow" show preview psd in unity
 ![previewindow](PSDDemo/imagecreater.png)
 *  in this window "创建模板" will show you how to make sprites and GameObjects quickly
 ![imagecreater](PSDDemo/psdpreviewWindow.png)
 * you can define some roule about pslayer name
 ![rouledefine](PSDDemo/rouleobject.png)

## 3.what next update?
 * script attachd prefab,when you change pictures,this link will continue exist,but rect and position will update

## 4.contact me.
* this project path:[PsdUnity](https://github.com/zouhunter/PsdUnity)
* QQ:1063627025 [when you have any question,or find some bugs,thank you to made me know.]

## 5Thanks for this project
in this projects,I learned so much,mybe you need it also

* [Psd2UnityImporter](https://github.com/SubjectNerd-Unity/Psd2UnityImporter) 
* [psd-parser](https://github.com/NtreevSoft/psd-parser)
* [UnityPSDLayoutTool](https://github.com/GlitchEnzo/UnityPSDLayoutTool)
* [Unity Psd Importer](https://github.com/Banbury/UnityPsdImporter)

### about deep seriation
until now i can`t find a good way to Serialize class of GroupNode,but find an option way below...;if you have good way ,thank you to warning me;
```

    public abstract class GroupNode<T> :GroupNode where T:GroupNode,new()
    {
        public List<T> _groups = new List<T>();
        public List<ImgNode> _images = new List<ImgNode>();
        public override List<ImgNode> images
        {
            get
            {
                return _images;
            }

            set
            {
                _images = value;
            }
        }
        public override List<GroupNode> groups
        {
            get
            {
                if (_groups == null) _groups = new List<T>();
                return _groups.ConvertAll<GroupNode>(x => x);
            }
            set
            {
                _groups = value.ConvertAll<T>(x => (T)x);
            }
        }
        public override GroupNode InsertChild(Rect rect)
        {
            T node = new T();
            node.rect = rect;
            _groups.Add(node);
            return node;
        }
    }

    [System.Serializable]
    public class GroupNode1 : GroupNode<GroupNode2> { }
    [System.Serializable]
    public class GroupNode2 : GroupNode<GroupNode3> { }
    [System.Serializable]
    public class GroupNode3 : GroupNode<GroupNode4> { }
    [System.Serializable]
    public class GroupNode4 : GroupNode<GroupNode5> { }
    [System.Serializable]
    public class GroupNode5 : GroupNode<GroupNode6> { }
    [System.Serializable]
    public class GroupNode6 : GroupNode<GroupNode7> { }
    [System.Serializable]
    public class GroupNode7 : GroupNode
    {
        public override List<ImgNode> images { get { return null; } set { } }
        public override List<GroupNode> groups { get; set; }
        public override GroupNode InsertChild(Rect rect)
        {
            Debug.Log("cant Insert");
            return null;
        }
    }

```
