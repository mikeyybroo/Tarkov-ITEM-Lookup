using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Tarkov_ITEM_Lookup
{
    public partial class Form1 : Form
    {
        private Dictionary<string, string> itemIdToShortName; // Dictionary with item IDs as keys and short names as values
        private Dictionary<string, string> shortNameToItemId; // Dictionary with short names as keys and item IDs as values

        public Form1()
        {
            InitializeComponent();

            // Load JSON data and populate dictionaries
            string jsonFilePath = "items.json"; // Update file path accordingly
            string json = File.ReadAllText(jsonFilePath);
            PopulateItemDictionaries(json);
            listBox1.SelectedIndexChanged += ListBox1_SelectedIndexChanged;
            textBox1.KeyDown += TextBox1_KeyDown;
        }
        public class ItemData
        {
            public string _id { get; set; }
            public string _name { get; set; }
            public ItemProps _props { get; set; }
        }

        public class ItemProps
        {
            public string ShortName { get; set; }
        }
        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if the Enter key is pressed
            if (e.KeyCode == Keys.Enter)
            {
                // Call the search functionality
                SearchButton(sender, e);
            }
        }
        private void PopulateItemDictionaries(string json)
        {
            itemIdToShortName = new Dictionary<string, string>();
            shortNameToItemId = new Dictionary<string, string>();

            // Deserialize JSON data into a dictionary of item data
            var items = JsonConvert.DeserializeObject<Dictionary<string, ItemData>>(json);

            // Populate item dictionaries
            foreach (var item in items)
            {
                string itemId = item.Value._id;

                // Check if the item has a ShortName property
                if (item.Value._props != null && !string.IsNullOrEmpty(item.Value._props.ShortName))
                {
                    string shortName = item.Value._props.ShortName;

                    // Add to dictionaries
                    itemIdToShortName[itemId] = shortName;
                    shortNameToItemId[shortName] = itemId;
                }
            }
        }

        private void SearchButton(object sender, EventArgs e)
        {
            string searchTerm = textBox1.Text.Trim(); // Get the search term from the TextBox

            // Perform bidirectional search
            List<string> results = SearchItem(searchTerm);
            if (results != null && results.Count > 0)
            {
                // Clear the ListBox before adding new items
                listBox1.Items.Clear();

                // Fill out the ListBox with the corresponding items
                foreach (var result in results)
                {
                    listBox1.Items.Add(result);
                }

                // If there's only one result, populate the text fields
                if (results.Count == 1)
                {
                    string result = results[0];
                    textBox2.Text = itemIdToShortName.ContainsKey(result) ? itemIdToShortName[result] : result; // ShortName
                    textBox3.Text = shortNameToItemId.ContainsKey(result) ? shortNameToItemId[result] : result; // Item ID
                }
            }
            else
            {
                MessageBox.Show("Item not found.", "Item Lookup", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check if an item is selected
            if (listBox1.SelectedItem != null)
            {
                string selectedItem = listBox1.SelectedItem.ToString();

                // Find the corresponding item ID for the selected short name
                string itemId = shortNameToItemId.ContainsKey(selectedItem) ? shortNameToItemId[selectedItem] : selectedItem;

                // Populate the text boxes with the selected item's details
                textBox2.Text = selectedItem; // Short name
                textBox3.Text = itemId; // Item ID
            }
        }
        private List<string> SearchItem(string searchTerm)
        {
            // Convert the search term to lowercase for case-insensitive comparison
            searchTerm = searchTerm.ToLower();

            List<string> results = new List<string>();

            // Search for items matching the search term
            foreach (var item in itemIdToShortName)
            {
                // Check if the item ID contains the search term
                if (item.Key.ToLower().Contains(searchTerm))
                {
                    results.Add(item.Key);
                }
                // Check if the short name contains the search term
                if (item.Value.ToLower().Contains(searchTerm))
                {
                    results.Add(item.Value);
                }
            }

            return results;
        }
    }
}
