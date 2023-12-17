using Caves_of_Chaos.CreatureScripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Caves_of_Chaos.ItemScripts
{
    public static class ItemManager
    {
        public static List<ItemTemplate> templates = new List<ItemTemplate>();

        public static void Init()
        {
            // Load item templates:
            String[] raws = Directory.GetFiles(Directory.GetCurrentDirectory() + "/Items");
            for (int i = 0; i < raws.Length; i++)
            {
                String text = File.ReadAllText(raws[i]);
                ItemTemplate? template = JsonSerializer.Deserialize<ItemTemplate>(text);
                if (template == null)
                {
                    System.Diagnostics.Debug.WriteLine("Invalid creature template");
                    continue;
                }
                templates.Add(template);
            }
        }

        public static ItemTemplate? getTemplate(String name)
        {
            for (int i = 0; i < templates.Count; i++)
            {
                if (templates[i].name == name)
                {
                    return templates[i];
                }
            }

            return null;
        }
    }
}
