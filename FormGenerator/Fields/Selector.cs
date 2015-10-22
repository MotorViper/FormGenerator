﻿using TextParser;

namespace FormGenerator.Fields
{
    public class Selector : ComboBox
    {
        public Selector(Field parent, TokenTree data, int level) : base(parent, data, level)
        {
        }

        protected override void AddProperties(TokenTree parameters)
        {
            base.AddProperties(parameters);
            AddProperty("SelectedValue", "{Binding Selected}");
            AddProperty("SelectedIndex", 0);
            AddProperty("ItemsSource", "{Binding Keys}");
        }
    }
}
