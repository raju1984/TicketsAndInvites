using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Owin;
using EventManager1.Models;

namespace EventManager1.Hubs
{
    public class ChatHub : Hub
    {

        public void Send(string user, string eventID, string message, string username)
        {
            if (username == "1")
            {
                var i = HandleChat.UpdateLike(Convert.ToInt32(eventID), Convert.ToInt32(user), Convert.ToInt32(message));
                Clients.All.updateLikeCount(i.Totallike, i.Totaldislike, i.Totalviewer);
            }
            else
            {
                var i = HandleChat.insertMessage(user, eventID, message);
                Clients.All.addNewMessageToPage(username, message);
            }

        }
        //public void SendLike(string user, string eventID, string value)
        //{

        //}
    }
}