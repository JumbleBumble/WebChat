"use strict";

var currentID = null;

var connection = null;

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

function CreateMessage(user, message) {
    var divNew = document.createElement("div");
    divNew.classList.add("card")
    divNew.classList.add("mt-2")
    divNew.innerHTML = `
                    <div class="card-body">
                        <p class="card-text"><b>${user}</b>: ${message}</p>
                    </div>
    `;
    document.getElementById("messagesList").appendChild(divNew);
}

document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = document.getElementById("messageInput").value;
    if (message != null && message != "" && currentID != null) {
        connection.invoke("SendMessageToGroup", currentID, message).catch(function (err) {
            return console.error(err.toString());
        });
    }
    document.getElementById("messageInput").value = "";
    event.preventDefault();
});

function joinGroup(groupName) {
    connection.invoke("AddToGroup", groupName).catch(function (err) {
        return console.error(err.toString());
    });
}

document.addEventListener('DOMContentLoaded', function () {
    currentID = document.getElementById('CurrentID').value;
    connection = new signalR.HubConnectionBuilder().withUrl("/GroupHub").build();
    connection.on("ReceiveMessage", function (user, message) {
        console.log(user+message)
        CreateMessage(user, message);
    });

    connection.start().then(function () {
        document.getElementById("sendButton").disabled = false;
        joinGroup(currentID);
        console.log('Joined group?'+currentID.toString())
    }).catch(function (err) {
        return console.error(err.toString());
    });
    
});

