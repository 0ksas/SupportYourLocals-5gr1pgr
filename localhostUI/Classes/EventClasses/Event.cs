﻿using System;
using System.Collections.Generic;
using System.Text;

namespace localhostUI.EventClasses
{
    class Event
    {
        private string name;
        private DateTime date;
        private string sport;
        private float price;
        private string description;
        private string adress;
        private List<Team> team;
        
        public Event(string name, DateTime date, string sport, string description, string adress, float price = 0)
        {
            this.name = name;
            this.date = date;
            this.sport = sport;
            this.description = description;
            this.adress = adress;
            this.price = price;
        }
        
        public override string ToString()
        {
            /*return $"{name} {date} {sport} {price}";*/
            return this.name + " " + this.date.ToString() + " " + sport + " " + price;
        }
        /*
         * More to come:
         * -location
         * -author
         * -external links
         * -event type
         */
    }
}