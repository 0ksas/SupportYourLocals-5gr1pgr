﻿using Common;
using localhostUI.EventClasses;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace localhostUI.Classes.EventClasses
{
    class EventBrief
    {
        private string name;
        private List<string> sports;
        private List<Team> teams;
        private double latitude;
        private double longitude;
        private DateTime startDate;
        private DateTime endDate;
        private decimal price;
        private string thumbnail; // Links to images (first one always the thumbnail)

        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public string Thumbnail { get; set; }
        public List<string> GetSports()
        {
            return sports;
        }
        public List<Team> GetTeams()
        {
            return teams;
        }

        private void Init()
        {
            sports = new List<string>();
            teams = new List<Team>();

            name = "";
            latitude = 0.0;
            longitude = 0.0;
            startDate = new DateTime(0L);
            endDate = new DateTime(0L);
            price = 0;
            thumbnail = "";
        }

        public EventBrief()
        {
            Init();
        }

        public EventBrief(DataList data)
        {
            Init();

            try
            {
                // Basic conversions
                object nameObj = data.Get("name");
                if (nameObj != null)
                {
                    name = (string)nameObj;
                }
                object coordinatesObj = data.Get("coordinates");
                if (coordinatesObj != null)
                {
                    object latitudeObj = ((DataList)coordinatesObj).Get(0);
                    object longitudeObj = ((DataList)coordinatesObj).Get(1);
                    if (latitudeObj != null) latitude = (double)latitudeObj;
                    if (longitudeObj != null) longitude = (double)longitudeObj;
                }
                object startDateObj = data.Get("start_date");
                if (startDateObj != null)
                {
                    startDate = DateTime.ParseExact((string)startDateObj, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                }
                object endDateObj = data.Get("end_date");
                if (startDateObj != null)
                {
                    endDate = DateTime.ParseExact((string)startDateObj, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                }
                object priceObj = data.Get("price");
                if (priceObj != null)
                {
                    price = (decimal)priceObj;
                }

                // Complex conversions
                object sportsObj = data.Get("sports");
                if (sportsObj != null)
                {
                    foreach (ListItem sport in (DataList)sportsObj)
                    {
                        sports.Add((string)sport.item);
                    }
                }
                object imagesObj = data.Get("images");
                if (imagesObj != null)
                {
                    foreach (ListItem image in (DataList)imagesObj)
                    {
                        thumbnail = (string)image.item;
                        break; // Select only the first image
                    }
                }
                object teamsObj = data.Get("teams");
                if (teamsObj != null)
                {
                    foreach (ListItem team in (DataList)teamsObj)
                    {
                        string name = team.name;
                        List<Player> players = new List<Player>();

                        object playersObj = team.item;
                        if (playersObj != null)
                        {
                            foreach (ListItem player in (DataList)playersObj)
                            {
                                players.Add(new Player((string)player.item));
                            }
                        }

                        teams.Add(new Team(name, players));
                    }
                }
            }
            catch (InvalidCastException)
            {
                // TODO: log incident
            }
        }

        public static DataList ToDataList(EventBrief ev)
        {
            DataList data = new DataList();

            // Simple
            data.Add(ev.name, "name");
            data.Add(ev.startDate.ToString("O"), "start_date");
            data.Add(ev.endDate.ToString("O"), "end_date");
            data.Add(ev.price, "price");

            // Complex
            DataList sportsDl = new DataList();
            foreach (string sport in ev.sports)
            {
                sportsDl.Add(sport);
            }
            DataList imagesDl = new DataList();
            imagesDl.Add(ev.thumbnail);
            DataList teamsDl = new DataList();
            foreach (Team team in ev.teams)
            {
                DataList playersDl = new DataList();
                foreach (Player player in team.GetPlayers())
                {
                    playersDl.Add(player.Name);
                }
                teamsDl.Add(playersDl, team.Name);
            }

            data.Add(sportsDl, "sports");
            data.Add(imagesDl, "images");
            data.Add(teamsDl, "teams");

            return data;
        }
    }
}
