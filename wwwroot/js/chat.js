"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/ChatHub").build();
document.getElementById("sendButton").disabled = true;

function CreateTitledMessage(title, description) {
    var divNew = document.createElement("div");
    divNew.innerHTML = `
                    <div class="card">
                        <div class="card-body">
                            <h5 class="card-title">${title}</h5>
                            <p class="card-text">${description}</p>
                        </div>
                    </div>
    `;
    document.getElementById("messagesList").appendChild(divNew);
}

function AddConnectedUser(user) {
    if (document.getElementById(user) == null && user != null) {
        var divNew = document.createElement("div");
        divNew.classList.add("card");
        divNew.id = user;
        divNew.innerHTML = `
							<div class="card-body">
								<p class="card-text">${user}</p>
							</div>
    `;
        var UserContainer = document.getElementById("usersList");
        if (UserContainer != null) {
            UserContainer.appendChild(divNew);
        } 
    }
}

function RemoveConnectedUser(user) {
    var userElement = document.getElementById(user);
    if (userElement) {
        userElement.remove();
    }
}

function CreateMessage(user, message) {
    var divNew = document.createElement("div");
    divNew.innerHTML = `
                    <div class="card">
                        <div class="card-body">
                            <p class="card-text"><b>${user}</b>: ${message}</p>
                        </div>
                    </div>
    `;
    document.getElementById("messagesList").appendChild(divNew);
}


connection.on("ReceiveMessage", function (user, message) {
    CreateMessage(user, message);
});

connection.on("ConnectMessage", function (alert, user) {
    if (alert == "Connected") {
        AddConnectedUser(user);
    } else {
        RemoveConnectedUser(user);
    }
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});
 
document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = document.getElementById("messageInput").value;
    if (message != null && message != "") {
        connection.invoke("SendMessage", message).catch(function (err) {
            return console.error(err.toString());
        });
        document.getElementById("messageInput").value = "";
    } 
    event.preventDefault();
});

document.addEventListener('DOMContentLoaded', function () {
    fetch('api/Messages/Recent')
        .then(response => response.json())
        .then(data => {
            var messages = data;
            if (Array.isArray(messages) && messages.length >= 1) {           
                messages.reverse().forEach(msg => {
                    CreateMessage(msg.user, msg.text);
                });
            }
        })
        .catch(error => console.error('Error:', error));
    fetch('api/Messages/Connected')
        .then(response => response.json())
        .then(data => {
            var users = data;
            if (Array.isArray(users) && users.length >= 1) {
                users.forEach(user => {
                    AddConnectedUser(user);
                });
            }
        })
        .catch(error => console.error('Error:', error));
});

