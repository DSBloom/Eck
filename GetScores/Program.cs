using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLStats;
using XMLStats.Entities;
using XMLStats.Helpers;

namespace GetScores
{
    class Program
    {
        static void Main(string[] args)
        {
            //Global vars
            EventsRequest er;
            EventList eList;

            //Set the date we should start looking for MLB games
            DateTime eventDate = new DateTime(2015, 4, 5);
            Console.Out.WriteLine("The date we will begin looking for games is " + eventDate.ToString("yyyyMMdd"));

            //Look for games until it is one day after today
            while (eventDate < DateTime.Now.AddDays(1))
            {
                try
                {
                    Console.Out.WriteLine(">>>> Making the request for the list of games on " + eventDate.ToString("yyyyMMdd"));
                    er = new EventsRequest("a36e424b-8c00-48c1-8b2d-d112f8ce61b0", "nadcraker@gmail.com");
                    eList = er.get("mlb", eventDate);
                    Console.Out.WriteLine("          Found " + eList.@event.Count + " events for " + eventDate.Date.ToString());
                }
                catch(Exception ex)
                {
                    Console.Error.WriteLine("          There was an error getting the list of events on " + eventDate);
                    Console.Error.WriteLine(ex.Message);
                    continue;
                }

                //Foreach game on that day...
                if (eList != null)
                {
                    foreach (Event singleEvent in eList.@event)
                    {
                        DateTime tempDate;
                        if (DateTime.TryParse(singleEvent.start_date_time, out tempDate))
                        {
                            try
                            {
                                //Check if that game is already in the database with a COMPLETED status
                                Console.Out.WriteLine("          Get the status of " + singleEvent.event_id);
                                GameStatus gs = DataAccess.GameStatus(singleEvent);
                                if (gs.IsGameInDb == false)
                                {
                                    //Insert that game into the database
                                    Console.Out.WriteLine("          " + singleEvent.event_id + " was not found in the database.");
                                    Console.Out.WriteLine("          Inserting the game - " + singleEvent.event_id + " into the database...");
                                    SqlReturnObject sro = DataAccess.InsertEvent(singleEvent);

                                    if (sro.Success == true)
                                    {
                                        Console.Out.WriteLine("          " + singleEvent.event_id + " was inserted successfully.");
                                    }
                                    else
                                    {
                                        if (sro.ErrorMessage != null)
                                        {
                                            Console.Error.WriteLine("          " + sro.ErrorMessage);
                                        }
                                        else
                                        {
                                            Console.Error.WriteLine("          Error inserting " + singleEvent.event_id + " into the database.");
                                        }
                                    }
                                    Console.Error.WriteLine("          Sleeping for 15 seconds so we don't exceed 6 requests per minute...");
                                    System.Threading.Thread.Sleep(15000);
                                }
                                //It's already in the database. Do we need to update it?
                                else
                                {
                                    if(gs.Status == singleEvent.event_status)
                                    {
                                        //Do nuffin. We up to date
                                        Console.Out.WriteLine("          " + singleEvent.event_id + " was already in the database and has the same status as what came from XMLStats.");
                                        Console.Out.WriteLine("          So don't do nuffin.");
                                        Console.Out.WriteLine("");
                                    }
                                    else
                                    {
                                        Console.Out.WriteLine("          " + singleEvent.event_id + " was already in the database, but its status doesn't match the current status from XMLStats");

                                        SqlReturnObject sro = DataAccess.UpdateEvent(singleEvent);
                                        if (sro.Success == true)
                                        {
                                            Console.Out.WriteLine("          " + singleEvent.event_id + " was updated successfully.");
                                            Console.Out.WriteLine("");
                                        }
                                        else
                                        {
                                            if (sro.ErrorMessage != null)
                                            {
                                                Console.Error.WriteLine("          " + sro.ErrorMessage);
                                            }
                                            else
                                            {
                                                Console.Error.WriteLine("          Error updating " + singleEvent.event_id);
                                            }
                                        }
                                        Console.Error.WriteLine("          Sleeping for 15 seconds so we don't exceed 6 requests per minute...");
                                        Console.Out.WriteLine("");
                                        System.Threading.Thread.Sleep(15000);
                                    }
                                }
                            }
                            catch(Exception ex)
                            {
                                Console.Error.WriteLine("          Oh no! There was trouble dealing with eventID " + singleEvent.event_id);
                                Console.Error.WriteLine(ex.Message);
                                Console.Out.WriteLine("");
                                continue;
                            }
                        }
                        else
                        {
                            //We weren't able to parse the event date in SingleEvent.events_date
                            Console.Error.WriteLine("          We weren't able to parse the event date in SingleEvent.events_date");
                            Console.Out.WriteLine("");
                        }
                    }
                    Console.Out.WriteLine("          Incrementing the day...");
                    eventDate = eventDate.AddDays(1);
                    Console.Out.WriteLine("          eventDate is now equal to " + eventDate);
                }
                Console.Error.WriteLine("          Sleeping for 15 seconds so we don't exceed 6 requests per minute...");
                Console.Out.WriteLine("");
                System.Threading.Thread.Sleep(15000);
            }
        }
    }
}