using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using System.Collections.Generic;
namespace PSDUnity
{
    public class RuleObject : ScriptableObject
    {
        public class RuleItem
        {
            public ControlType itemname;
            public string ruleKey;
            public RuleItem(ControlType itemname, string ruleKey)
            {
                this.itemname = itemname;
                this.ruleKey = ruleKey;
            }
        }
        public List<RuleItem> items = new List<RuleItem>();
        public RuleObject()
        {
            items.Add(new RuleItem(ControlType.Button, "@button"));
            items.Add(new RuleItem(ControlType.Toggle, "@toggle"));
            items.Add(new RuleItem(ControlType.Grid, "@grid"));
            items.Add(new RuleItem(ControlType.ScrollView, "@scrollview"));
            items.Add(new RuleItem(ControlType.Slider, "@slider"));
            items.Add(new RuleItem(ControlType.ScrollBar, "@scrollbar"));
            items.Add(new RuleItem(ControlType.Group, "@group"));
            items.Add(new RuleItem(ControlType.InputField, "@inputfield"));
            items.Add(new RuleItem(ControlType.Dropdown, "@dropdown"));
        }

        public string GetClampdName(string fullname)
        {
            var index = fullname.IndexOf("@");
            return fullname.Remove(index);
        }

        public ControlType GetGroupType(string fullname)
        {
            var controlType = ControlType.Panel;
            var ruleitem = items.Find(x => fullname.ToLower().Contains(x.ruleKey));
            if (ruleitem != null)
            {
                controlType = ruleitem.itemname;
            }
            return controlType;
        }
    }
}
